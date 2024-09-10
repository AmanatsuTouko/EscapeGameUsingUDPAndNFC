using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using System.Threading;
using UnityEngine.Rendering.PostProcessing;

public class UIManager : SingletonMonobehaviour<UIManager>
{
    // PostProcess
    [Header("Post Process")]
    [SerializeField] PostProcessVolume postProcessVolume;

    // メイン画面
    [Header("Main Panel")]
    [SerializeField] GameObject mainPanel;
    [SerializeField] TextMeshProUGUI limitTimeText;

    // クイズ画像の表示
    [Header("Quiz Panel")]
    [SerializeField] GameObject QuizPanel;
    [SerializeField] public Image QuizDisplayImage;
    [SerializeField] Slider ProgressBarSlider;

    // プログレスバーが上昇中かどうか
    public bool IsUpdatingProgressBar { get; private set; } = false;

    // 現在読み込んでいるCardID
    [SerializeField] CardID? currentDisplayCardID = null;

    // 読み込み中の文言
    [SerializeField] TextMeshProUGUI progressText;

    // UniTaskのキャンセル用トークン
    private CancellationTokenSource cancellationTokenSource;

    // 現在読み込んでいる問題カード
    public CardID currentDisplayQuestionCard;
    // 現在読み込んでいるヒントカード
    public CardID currentDisplayHintCard;

    private void Start()
    {
        progressText.enabled = false;
    }

    // 読み込んだカードが現在読み込んでいる問題の答えかどうか
    public bool IsCorrectForCurrentQuestion(CardID readCardID)
    {
        // 修正 ヒントカード + 問題カード + 解答カードの組み合わせで答えなければならない
        // return currentDisplayCardID != null 
        //     && GameManager.Instance.GetAnswerQuizCardIDsImagepair().IsExistQuestionAnswerCardIDPair((CardID)currentDisplayCardID, (CardID)cardID);
        Debug.LogError("未実装の関数がコールされました。");
        return false;
    }

    public void CorrectPerformance(string uuid)
    {
        Debug.LogError("未実装の関数がコールされました。");
    }

    public void DisplayQuestionImageWithProgressBar(CardID cardID)
    {
        // トークンソースのリソース解放
        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Dispose();
        }
        // CancellationTokenSource を初期化
        cancellationTokenSource = new CancellationTokenSource();
        // UniTask を実行
        DisplayQuestionImageWithProgressBarUniTask(cardID, cancellationTokenSource.Token).Forget();
    }

    public async UniTask DisplayQuestionImageWithProgressBarUniTask(CardID cardID, CancellationToken token)
    {
        // 読み込み演出の途中で新たに読み込み演出をしないようにする
        if (IsUpdatingProgressBar)
        {
            return;
        }
        IsUpdatingProgressBar = true;

        // リセット処理
        ProgressBarSlider.value = 0;
        postProcessVolume.enabled = true;

        QuizPanelSetActive(true);
        QuizPanelComponentSetActive(false);

        // クイズが他の問題の答えかどうか
        bool isQuizHasAnswer = false;

        // 今読んだカードが現在読み込んでいる問題の答えの場合
        if( currentDisplayCardID != null 
            && GameManager.Instance.GetAnswerQuizCardIDsImagepair().IsExistQuestionAnswerCardIDPair((CardID)currentDisplayCardID, (CardID)cardID) )
        {
            // 新たな問題を表示する
            GameManager.Instance.GetAnswerQuizCardIDsImagepair().DisplayQuestionImage((CardID)currentDisplayCardID, (CardID)cardID);
            isQuizHasAnswer = true;
        }
        else
        {
            // 答えではないので，単体で読み込んだときの処理を行う
            GameManager.Instance.GetCardIDImagePair().DisplayQuestionImage((CardID)cardID);
            isQuizHasAnswer = false;
        }

        // 現在表示しているカードIDを更新する
        currentDisplayCardID = cardID;

        // イージングを繋ぐタイミングをややランダムにする
        float addSeconds_12 = UnityEngine.Random.Range(-0.1f, 0.1f);
        float addSeconds_23 = UnityEngine.Random.Range(-0.1f, 0.1f);
        float addSeconds_34 = UnityEngine.Random.Range(-0.1f, 0.1f);

        // イージングの種類をランダムにする(弾性や振動を除く)
        int enumLength = Enum.GetNames(typeof(Easing.Ease)).Length - 9;
        int easeRandIdx_01 = UnityEngine.Random.Range(0, enumLength);
        int easeRandIdx_02 = UnityEngine.Random.Range(0, enumLength);
        int easeRandIdx_03 = UnityEngine.Random.Range(0, enumLength);
        int easeRandIdx_04 = UnityEngine.Random.Range(0, enumLength);

        // プログレスバーの上の文字を表示する
        progressText.enabled = true;

        // プログレスバーを4段階に分けて上昇させる
        UpdateProgressText("機器を初期化中......");
        await EasingSecondsFromTo(1.2f, 0.0f,                  0.20f + addSeconds_12, (Easing.Ease)easeRandIdx_01, token);

        UpdateProgressText(isQuizHasAnswer ? "正当性を確認中......" : "原子を生成中......");
        await EasingSecondsFromTo(1.0f, 0.20f + addSeconds_12, 0.45f + addSeconds_23, (Easing.Ease)easeRandIdx_02, token);
        await UniTask.Delay(100);

        UpdateProgressText(isQuizHasAnswer ? "権限を確認中......" : "電気を充填中......");
        await EasingSecondsFromTo(1.2f, 0.45f + addSeconds_23, 0.65f + addSeconds_34, (Easing.Ease)easeRandIdx_03, token);

        UpdateProgressText(isQuizHasAnswer ? "電子錠を開錠中......" : "物体を転送中......");
        await EasingSecondsFromTo(1.0f, 0.65f + addSeconds_34, 1.00f,                 (Easing.Ease)easeRandIdx_04, token);
        await UniTask.Delay(500);

        // PostProcessをOFFにする
        postProcessVolume.enabled = false;

        // ImageをONにする
        QuizPanelComponentSetActive(true);

        // プログレスバーの上の文字を非表示にして文字を消す
        UpdateProgressText("");
        progressText.enabled = false;

        IsUpdatingProgressBar = false;
    }

    // N秒でaからbまでイージングを行う関数
    private async UniTask EasingSecondsFromTo(float seconds, float fromValue, float toValue, Easing.Ease easing, CancellationToken token)
    {
        Func<float, float> easingMethod = Easing.GetEasingMethod(easing);
        float rate = 0;
        float sub = toValue - fromValue;
        while (rate < 1.0f)
        {
            await UniTask.Yield(token);
            rate += Time.deltaTime / seconds;
            if (rate >= 1.0f) rate = 1.0f;
            ProgressBarSlider.value = fromValue + easingMethod(rate) * sub;
        }
    }

    private void UpdateProgressText(string text)
    {
        // プログレスバーに記載されている文字を変更する
        progressText.text = text;
    }

    // UI要素のON・OFF
    public void QuizPanelSetActive(bool active)
    {
        QuizPanel.SetActive(active);

        // パネルをOFFにする時に，プログレスバーが進行中であればUniTaskのキャンセル処理を行う
        if(active == false)
        {
            cancellationTokenSource.Cancel();
            IsUpdatingProgressBar = false;
            // PostProcessの再開
            postProcessVolume.enabled = true;
        }
    }
    public void QuizPanelComponentSetActive(bool quizImageActive)
    {
        QuizDisplayImage.enabled = quizImageActive;
    }

    // 時刻の更新
    public void SetLimitTimeText(string text)
    {
        limitTimeText.text = text;
    }
}
