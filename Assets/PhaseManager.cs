using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Phase
{
    Phase1,
    Phase2,
    Phase3,
    Finish
}

public class PhaseManager : SingletonMonobehaviour<PhaseManager>
{
    [SerializeField]
    private Phase phase;    
    public Phase Phase
    {
        get{ return phase; } 
        private set{ phase = value; }
    }

    [SerializeField]
    private int firstFloorRemainQuiz;
    public int FirstFloorRemainQuiz 
    { 
        get { return firstFloorRemainQuiz; } 
        private set { firstFloorRemainQuiz = value; }
    }

    [SerializeField]
    private int secondFloorRemainQuiz;
    public int SecondFloorRemainQuiz
    { 
        get { return secondFloorRemainQuiz; } 
        private set { secondFloorRemainQuiz = value; }
    }

    // 鍵のアイコンのUI画像
    [SerializeField]
    List<Image> firstFloorLockImage;
    [SerializeField]
    List<Image> secondFloorLockImage;

    [SerializeField]
    Sprite lockedSpriteRed;
    [SerializeField]
    Sprite lockedSpriteBlue;

    void Start()
    {
        // クイズの残り数の初期化とUIの更新
        firstFloorRemainQuiz = 1;
        secondFloorRemainQuiz = 1;
        UpdateLockedQuizUI();
    }

    public void QuizClear(CardID cardID)
    {
        // どのクライアントが担当しているクイズかどうかを判別して、残りクイズ数を減少させる
        if(DataBase.Instance.IsExistQuiz(cardID, Client.FirstFloor))
        {
            firstFloorRemainQuiz -= 1;
        }
        if(DataBase.Instance.IsExistQuiz(cardID, Client.SecondFloor))
        {
            secondFloorRemainQuiz -= 1;
        }
        // UIの更新
        UpdateLockedQuizUI();

        // 残りクイズ数がどのクライアントも0になった場合はフェーズクリア
        if(firstFloorRemainQuiz == 0 && SecondFloorRemainQuiz == 0)
        {
            PhaseClear();
        }
    }

    private void PhaseClear()
    {
        switch(phase)
        {
            case Phase.Phase1:
                phase = Phase.Phase2;
                firstFloorRemainQuiz = 2;
                secondFloorRemainQuiz = 2;
                break;

            case Phase.Phase2:
                phase = Phase.Phase3;
                
                break;
            case Phase.Phase3:
                phase = Phase.Finish;
                
                break;
        }
    }

    private void UpdateLockedQuizUI()
    {
        // クイズの残り数に応じて表示を変える
        foreach(var image in firstFloorLockImage)
        {
            image.enabled = false;
        }
        foreach(var image in secondFloorLockImage)
        {
            image.enabled = false;
        }
        for(int i=0; i<firstFloorRemainQuiz; i++)
        {
            firstFloorLockImage[i].enabled = true;
        }
        for(int i=0; i<secondFloorRemainQuiz; i++)
        {
            secondFloorLockImage[i].enabled = true;
        }
    }
}
