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
    public Image DefaultHintImage;

    // プログレスバーが上昇中かどうか
    public bool IsUpdatingProgressBar { get; private set; } = false;
    // 読み込み中の文言
    [SerializeField] TextMeshProUGUI progressText;

    // UniTaskのキャンセル用トークン
    public CancellationTokenSource cancellationTokenSource { get; private set; }

    // 現在読み込んでいる問題カード
    CardID? currentDisplayQuestionCard = null;
    // 現在読み込んでいるヒントカード
    CardID? currentDisplayHintCard = null;

    [Header("For Unique Method")]
    public Image SnowFadeImage;

    private void Start()
    {
        progressText.enabled = false;
        currentDisplayQuestionCard = null;
        currentDisplayHintCard = null;
    }

    // 読み込んだカードが現在読み込んでいる問題の答えかどうか
    public bool IsCorrectForCurrentQuestion(CardID readCardID)
    {
        // 修正 ヒントカード + 問題カード + 解答カードの組み合わせで答えなければならない
        if(IsValidCurrentQuiz() && IsValidCurrentHint())
        {
            return DataBase.Instance.IsCorrectAnswer((CardID)currentDisplayQuestionCard, (CardID)currentDisplayHintCard, readCardID);
        }
        Debug.LogError($"読み込んだカード{readCardID}は現在読み込んでいる問題の答えではありません。");
        return false;
    }

    public void CorrectPerformance(string uuid)
    {
        Debug.LogError("未実装の関数がコールされました。");

        CardID? cardID = DataBase.Instance.GetCardIDFromUUID(uuid);
        if(cardID == null)
        {
            Debug.LogError($"正答したカードのUUID:{uuid}は，登録されていないUUIDのため，正答処理を中断します．");
        }

        // 正解！を表示する

        // CardIDやPhaseに応じて，正答処理を行う
    }

    // 交通系ICを読み込んだ時に正答だったら処理を行う
    public void CorrectPerformanceOfTransportationICCard()
    {
        // 現在、問題4(Loop)が表示されていてかつ、ヒント4(Arrow)が読み込まれた後の画像かどうか
        if (currentDisplayQuestionCard == CardID.Question04_Loop && currentDisplayHintCard == CardID.Hint04_Arrow)
        {
            // 処理を行う
            Debug.LogError("Suicaを読みこんで正答した際の処理が未実装です。");
        }
    }

    // 問題とヒントのNullチェック
    private bool IsValidCurrentQuiz()
    {
        if(currentDisplayQuestionCard == null)
        {
            Debug.LogError("エラー：問題が表示されていません。");
            return false;
        }
        return true;
    }
    private bool IsValidCurrentHint()
    {
        if(currentDisplayHintCard == null)
        {
            Debug.LogError("エラー：ヒントが表示されていません。");
            return false;
        }
        return true;
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
        DisplayQuestionImageWithProgressBarUniTask(cardID).Forget();
    }

    public void InitializeCancellationTokenSource()
    {
        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Dispose();
        }
        // CancellationTokenSource を初期化
        cancellationTokenSource = new CancellationTokenSource();
    }

    public async UniTask DisplayQuestionImageWithProgressBarUniTask(CardID cardID)
    {
        // 読み込み演出の途中で新たに読み込み演出をしないようにする
        if (IsUpdatingProgressBar)
        {
            return;
        }
        IsUpdatingProgressBar = true;

        bool isDefaultHint = false;

        QuizPanelSetActive(true);
        QuizPanelComponentSetActive(false, false);

        // カードの種類を取得する
        CardType? cardType = DataBase.Instance.GetCardTypeFromCardID(cardID);
        if(cardType == null)
        {
            Debug.Log($"存在しないCardID:{cardID}のCardTypeを取得できないため終了します。");
            return;
        }
        else if(cardType == CardType.Question)
        {
            // 問題カードなので問題を表示する
            DataBase.Instance.DisplayQuiz(cardID);

            // 現在表示している問題の変数を更新する
            currentDisplayQuestionCard = cardID;
        }
        else if(cardType == CardType.Hint)
        {
            // currentDisplayQuestionCardのnullチェック
            if (IsValidCurrentQuiz())
            {
                // ヒントが正答かどうか
                if(DataBase.Instance.IsCorrectHint((CardID)currentDisplayQuestionCard, cardID))
                {
                    // 問題にヒントを加えた画像に差し替える
                    DataBase.Instance.DisplayQuizAddedHint((CardID)currentDisplayQuestionCard, cardID);
                }
                else
                {
                    // 現在表示しているクイズ画像を再度表示する
                    // （正答したヒント画像に，上から別のヒントが重なって表示されないようにする）
                    DataBase.Instance.DisplayQuiz((CardID)currentDisplayQuestionCard);

                    // 現在表示している画像の指定の位置にヒント画像を乗せて表示する
                    DataBase.Instance.DisplayQuizWrongHint((CardID)currentDisplayQuestionCard, cardID);
                    isDefaultHint = true;
                }
                // 現在表示しているヒントの変数を更新する
                currentDisplayHintCard = cardID;
            }
        }
        else
        {
            Debug.LogError($"CardType:{cardType}に対応する処理が割り当てられていません。");
        }

        // プログレスバーを動的に変化させる
        await InCreaseProgressBarUniTask(false, isDefaultHint);

        // 特殊処理が設定されていた場合は追加で実行する
        if(IsValidCurrentQuiz() && IsValidCurrentHint())
        {
            DataBase.Instance.InvokeMethodOnHintDisplay((CardID)currentDisplayQuestionCard, (CardID)currentDisplayHintCard);
        }
        else
        {
            Debug.LogError($"問題：{currentDisplayQuestionCard}の時にヒント：{currentDisplayQuestionCard}を読み込んだ際の関数が登録されていないので、実行しません。");
        }

        IsUpdatingProgressBar = false;
    }

    // プログレスバーを表示する
    private async UniTask InCreaseProgressBarUniTask(bool isCorrectQuiz, bool isDefaultHint)
    {
        // クイズの正答かどうか
        bool isAnswer = false;

        // リセット処理
        ProgressBarSlider.value = 0;
        postProcessVolume.enabled = true;

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
        UpdateProgressText("機器を初期化中...");
        await EasingSecondsFromTo(1.2f, 0.0f,                  0.20f + addSeconds_12, (Easing.Ease)easeRandIdx_01);

        UpdateProgressText(isAnswer ? "正当性を確認中..." : "原子を生成中...");
        await EasingSecondsFromTo(1.0f, 0.20f + addSeconds_12, 0.45f + addSeconds_23, (Easing.Ease)easeRandIdx_02);
        await UniTask.Delay(100);

        UpdateProgressText(isAnswer ? "権限を確認中..." : "電気を充填中...");
        await EasingSecondsFromTo(1.2f, 0.45f + addSeconds_23, 0.65f + addSeconds_34, (Easing.Ease)easeRandIdx_03);

        UpdateProgressText(isAnswer ? "電子錠を開錠中..." : "物体を転送中...");
        await EasingSecondsFromTo(1.0f, 0.65f + addSeconds_34, 1.00f,                 (Easing.Ease)easeRandIdx_04);
        await UniTask.Delay(500);

        // PostProcessをOFFにする
        postProcessVolume.enabled = false;

        // ImageをONにする
        QuizPanelComponentSetActive(true, isDefaultHint);

        // プログレスバーの上の文字を非表示にして文字を消す
        UpdateProgressText("");
        progressText.enabled = false;
    }

    // N秒でaからbまでイージングを行う関数
    private async UniTask EasingSecondsFromTo(float seconds, float fromValue, float toValue, Easing.Ease easing)
    {
        Func<float, float> easingMethod = Easing.GetEasingMethod(easing);
        float rate = 0;
        float sub = toValue - fromValue;
        while (rate < 1.0f)
        {
            await UniTask.Yield(cancellationTokenSource.Token);            
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
    public void QuizPanelComponentSetActive(bool quizImageActive, bool hintImageActive)
    {
        QuizDisplayImage.enabled = quizImageActive;
        DefaultHintImage.enabled = hintImageActive;
    }

    // 時刻の更新
    public void SetLimitTimeText(string text)
    {
        limitTimeText.text = text;
    }

    // 固有メソッドからプログレスバーのUpdate状態を編集できるようにする
    // TODO:
    // ReadOnlyなのを壊してしまうので、他の解決策を模索中
    // UniTaskの関数をInspectorに登録できれば万事解決なのだけれども
    // ScriptableObjectに関数を登録するという考えが良くない？
    public bool IsUpdatingProgressBarFromExternal 
    {
        get
        {
            return IsUpdatingProgressBar;
        } 
        set
        {
            IsUpdatingProgressBar = value;
        }
    }
}
