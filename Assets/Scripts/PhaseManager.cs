using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

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

    public bool[] IsClearedQuiz = new bool[7];

    // フェーズ演出をしているかどうか
    public bool IsPhaseProcessing { get; private set; } = false;

    void Start()
    {
        // クイズの残り数の初期化とUIの更新
        firstFloorRemainQuiz = 1;
        secondFloorRemainQuiz = 1;
        UpdateLockIcon();
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
        // このクライアントが担当しているクイズかどうか
        bool isOwnQuizCorrected = false;

        // どのクライアントが担当しているクイズかどうかを判別して、残りクイズ数を減少させる
        if(DataBase.Instance.IsExistQuiz(cardID, Client.FirstFloor))
        {
            if(IsClearQuizFromCardID(cardID) == false)
            {
                firstFloorRemainQuiz -= 1;
                ClearQuiz(cardID);
            }
            else
            {
                Debug.Log("既に正解したクイズを再度正解しています．");
            }
            if (DataBase.Instance.Client == Client.FirstFloor)
            {
                isOwnQuizCorrected = true;
            }
        }
        else if(DataBase.Instance.IsExistQuiz(cardID, Client.SecondFloor))
        {
            if(IsClearQuizFromCardID(cardID) == false)
            {
                secondFloorRemainQuiz -= 1;
                ClearQuiz(cardID);
            }
            else
            {
                Debug.Log("既に正解したクイズを再度正解しています．");
            }
            if (DataBase.Instance.Client == Client.SecondFloor)
            {
                isOwnQuizCorrected = true;
            }
        }
        else
        {
            Debug.LogError($"エラー：クイズとして登録されていないCardID{cardID}のクリア通知を受信しました。");
            return;
        }

        // 画面演出を行う
        QuizClearProcessUniTask(isOwnQuizCorrected).Forget();
    }

    // クイズをクリアした時の画面演出操作
    private async UniTask QuizClearProcessUniTask(bool isOwnQuizCorrected)
    {
        if(IsPhaseProcessing) { return; }
        IsPhaseProcessing = true;

        if (isOwnQuizCorrected)
        {
            Debug.Log("このクライアントでクイズがクリアされました．");
        }
        else
        {
            Debug.Log("別のクライアントでクイズがクリアされました．");
        }

        // プログレスバーを動的に変化させる
        await UIManager.Instance.DisplayProgressBarUniTask(true);
        // 正解！のUIを表示してメイン画面に戻る
        await UIManager.Instance.DisplayCorrectAndBackMainUniTask();

        // UIの更新（鍵のアイコンが減少）
        UpdateLockIcon();

        // 残りクイズ数がどのクライアントも0になった場合はフェーズクリア
        if (firstFloorRemainQuiz == 0 && SecondFloorRemainQuiz == 0)
        {
            await PhaseClearUniTask();
        }

        // UIの更新
        UpdateLockIcon();

        IsPhaseProcessing = false;
    }

    // フェーズクリア時の動作
    private async UniTask PhaseClearUniTask()
    {
        switch(phase)
        {
            case Phase.Phase1:
                phase = Phase.Phase2;
                firstFloorRemainQuiz = 2;
                secondFloorRemainQuiz = 2;
                await UIManager.Instance.PhaseClearProcessUniTask(Phase.Phase1);
                break;

            case Phase.Phase2:
                phase = Phase.Phase3;
                firstFloorRemainQuiz = 1;
                await UIManager.Instance.PhaseClearProcessUniTask(Phase.Phase2);
                break;

            case Phase.Phase3:
                phase = Phase.Finish;
                await UIManager.Instance.PhaseClearProcessUniTask(Phase.Phase3);
                break;
        }
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
            case CardID.Question07_FinalQuestion:
                return 6;
            // それ以外の場合はエラー値として-1を返す
            default:
                return -1;
        }
    }

    // 鍵のアイコンを更新する
    private void UpdateLockIcon()
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

    private bool IsClearQuizFromCardID(CardID cardID)
    {
        return IsClearedQuiz[GetQuizIndex(cardID)];
    }
    private void ClearQuiz(CardID cardID)
    {
        IsClearedQuiz[GetQuizIndex(cardID)] = true;
    }
}
