using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using System.Threading;

public class UIManager : SingletonMonobehaviour<UIManager>
{
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
    private bool isUpdatingProgressBar = false;

    // 現在読み込んでいるCardID
    [SerializeField] CardID? currentDisplayCardID = null;

    // 読み込み中の文言
    [SerializeField] TextMeshProUGUI progressText;

    // UniTaskのキャンセル用トークン
    private CancellationTokenSource cancellationTokenSource;

    private void Start()
    {
        progressText.enabled = false;
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
        if (isUpdatingProgressBar)
        {
            return;
        }
        isUpdatingProgressBar = true;

        // リセット処理
        ProgressBarSlider.value = 0;

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
        float addSeconds_01 = UnityEngine.Random.Range(-0.1f, 0.1f);
        float addSeconds_02 = UnityEngine.Random.Range(-0.1f, 0.1f);

        // イージングの種類をランダムにする(弾性や振動を除く)
        int enumLength = Enum.GetNames(typeof(Easing.Ease)).Length - 9;
        int easeRandIdx_01 = UnityEngine.Random.Range(0, enumLength);
        int easeRandIdx_02 = UnityEngine.Random.Range(0, enumLength);
        int easeRandIdx_03 = UnityEngine.Random.Range(0, enumLength);

        // プログレスバーの上の文字を表示する
        progressText.enabled = true;

        // プログレスバーを3段階に分けて上昇させる
        UpdateProgressText(isQuizHasAnswer ? "組成を分析中......" : "組成を分析中......");
        await EasingSecondsFromTo(1.5f, 0.0f,                  0.25f + addSeconds_01, (Easing.Ease)easeRandIdx_01, token);

        UpdateProgressText(isQuizHasAnswer ? "正当性を確認中......" : "物質を構成中......");
        await EasingSecondsFromTo(1.0f, 0.25f + addSeconds_01, 0.55f + addSeconds_02, (Easing.Ease)easeRandIdx_02, token);
        await UniTask.Delay(500);

        UpdateProgressText(isQuizHasAnswer ? "電子錠を開錠中......" : "生成物を検証中......");
        await EasingSecondsFromTo(1.5f, 0.55f + addSeconds_02, 1.0f,                  (Easing.Ease)easeRandIdx_03, token);

        // ImageをONにする
        QuizPanelComponentSetActive(true);

        // プログレスバーの上の文字を非表示にして文字を消す
        UpdateProgressText("");
        progressText.enabled = false;

        isUpdatingProgressBar = false;
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
            isUpdatingProgressBar = false;
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
