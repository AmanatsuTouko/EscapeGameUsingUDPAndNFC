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
    [Header("Phase Detail")]
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

    [Header("UI Component")]
    [SerializeField]
    GameObject PhasePanel;
    // 鍵のアイコンのUI画像
    [SerializeField]
    List<Image> firstFloorLockImage;
    [SerializeField]
    List<Image> secondFloorLockImage;

    [SerializeField]
    Sprite lockedSpriteRed;
    [SerializeField]
    Sprite lockedSpriteBlue;

    public bool[] IsClearQuizIndex = new bool[6];

    void Start()
    {
        // クイズの残り数の初期化とUIの更新
        firstFloorRemainQuiz = 1;
        secondFloorRemainQuiz = 1;
        UpdateLockedQuizUI();
        // クイズ画像の初期化
        foreach(var image in firstFloorLockImage)
        {
            image.sprite = lockedSpriteBlue;
        }
        foreach(var image in secondFloorLockImage)
        {
            image.sprite = lockedSpriteRed;
        }
    }

    public void QuizClear(CardID cardID)
    {
        // どのクライアントが担当しているクイズかどうかを判別して、残りクイズ数を減少させる
        if(DataBase.Instance.IsExistQuiz(cardID, Client.FirstFloor))
        {
            if(IsClearQuizIndex[GetQuizIndex(cardID)] == false)
            {
                firstFloorRemainQuiz -= 1;
                IsClearQuizIndex[GetQuizIndex(cardID)] = true;
            }
        }
        else if(DataBase.Instance.IsExistQuiz(cardID, Client.SecondFloor))
        {
            if(IsClearQuizIndex[GetQuizIndex(cardID)] == false)
            {
                secondFloorRemainQuiz -= 1;
                IsClearQuizIndex[GetQuizIndex(cardID)] = true;
            }
        }
        else
        {
            Debug.LogError($"エラー：クイズとして登録されていないCardID{cardID}のクリア通知を受信しました。");
            return;
        }

        // TODO:
        // 正解！のUIを表示してメイン画面に戻る

        
        // 残りクイズ数がどのクライアントも0になった場合はフェーズクリア
        if(firstFloorRemainQuiz == 0 && SecondFloorRemainQuiz == 0)
        {
            PhaseClear();
        }

        // UIの更新
        UpdateLockedQuizUI();
    }

    private int GetQuizIndex(CardID cardID)
    {
        switch(cardID)
        {
            case CardID.Question01_RedBlueArrow:
                return 0;
            case CardID.Question02_Siritori:
                return 1;
            case CardID.Question03_Snow:
                return 2;
            case CardID.Question04_Loop:
                return 3;
            case CardID.Question05_Bug:
                return 4;
            case CardID.Question06_TrafficJam:
                return 5;
            // それ以外の場合はエラー値として-1を返す
            default:
                return -1;
        }
    }

    public void PhasePanelSetActive(bool active)
    {
        PhasePanel.SetActive(active);
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
