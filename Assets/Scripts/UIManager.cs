using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using System.Threading;
using UnityEngine.Rendering.PostProcessing;
using System.Collections.Generic;

public class UIManager : SingletonMonobehaviour<UIManager>
{
    // PostProcess
    [Header("Post Process")]
    [SerializeField] PostProcessVolume postProcessVolume;

    // メイン画面
    [Header("Main Panel")]
    [SerializeField] GameObject mainPanel;
    [SerializeField] TextMeshProUGUI limitTimeText;

    [Header("Quiz Panel")]
    [SerializeField] GameObject QuizPanelPrefab;
    [SerializeField] GameObject ProgressBarPanel;

    // プログレスバーが上昇中かどうか
    public bool IsUpdatingProgressBar { get; private set; } = false;

    // UniTaskのキャンセル用トークン
    public CancellationTokenSource cancellationTokenSource { get; private set; }

    // 現在読み込んでいる問題カード
    CardID? currentDisplayQuestionCard = null;
    // 現在読み込んでいるヒントカード
    CardID? currentDisplayHintCard = null;

    [Header("Correct Panel")]
    [SerializeField] GameObject CorrectPanelPrefab;

    [Header("Phase Clear")]
    [SerializeField] Image PhaseClearBGPanelImage;
    [SerializeField] TextMeshProUGUI PhaseClearText;
    [SerializeField] TextMeshProUGUI PhaseClearMiniMessage;
    [SerializeField] Color Phase1TextColor;
    [SerializeField] Color Phase2TextColor;
    [SerializeField] Color Phase3TextColor;

    [Header("For Unique Method")]
    [SerializeField] GameObject SnowParticle;
    public Image SnowFadeImage;
    public Image BugImage;
    public Image KillBugSprayImage;
    public Image DogFadeImageForQuizTrafficJam;

    private SynchronizationContext mainThreadContext;

    [Header("Non Hint Error Panel")]
    [SerializeField] GameObject NonHintPanelPrefab;

    [Header("Canvas")]
    [SerializeField] GameObject Canvas;
    
    private void Start()
    {
        currentDisplayQuestionCard = null;
        currentDisplayHintCard = null;

        // デバッグ時にTokenSourceがnullとなるので初期化だけしておく
        // （カードをスキャンせずに，クリア演出をしようとするとなる）
        InitializeCancellationTokenSource();

        // メインスレッドのSynchronizationContextを取得
        mainThreadContext = SynchronizationContext.Current;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log($"現在の問題:{currentDisplayQuestionCard}, 現在のヒント:{currentDisplayHintCard}");
        }
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

    public void CorrectPerformance(string answerUUID)
    {
        Debug.LogError("未実装の関数がコールされました。");

        CardID? answer = DataBase.Instance.GetCardIDFromUUID(answerUUID);
        if(answer == null)
        {
            Debug.LogError($"正答したカードのUUID:{answerUUID}は，登録されていないUUIDのため，正答処理を中断します．");
            return;
        }

        // CardIDやPhaseに応じて，正答処理を行う
        CardID? quiz = DataBase.Instance.GetCardIDFromAnswer((CardID)answer);
        if(quiz == null)
        {
            Debug.LogError($"正答したカード:{answer}の問題が定義されていないため、正答処理を中断します。");
            return;
        }

        // メインスレッドで実行しなければならない
        // メインスレッドに処理を戻して，登録したメソッドを実行する
        // (UnityのUI操作などの関数はメインスレッドからしか実行できないため)
        mainThreadContext.Post(_ =>
        {
            PhaseManager.Instance.QuizClear((CardID)quiz);
        }, null);
        
        GameManager.Instance.QuizClearOnRemoteClient((CardID)quiz);
    }

    // 交通系ICを読み込んだ時に正答だったら処理を行う
    public void CorrectPerformanceOfTransportationICCard()
    {
        Debug.Log("交通系ICが読み込まれた");
        // 現在、問題4(Loop)が表示されていてかつ、ヒント4(Arrow)が読み込まれた後の画像かどうか
        if (currentDisplayQuestionCard == CardID.Question04_Loop && currentDisplayHintCard == CardID.Hint04_Arrow)
        {
            // 処理を行う
            Debug.LogError("Suicaを読みこんで正答した際の処理");
            PhaseManager.Instance.QuizClear(CardID.Question04_Loop);
            GameManager.Instance.QuizClearOnRemoteClient(CardID.Question04_Loop);
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

    // トークンソースを開放して初期化する
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

        InitializeCancellationTokenSource();

        bool isDefaultHint = false;

        // クイズ画像を表示するためのUIの生成
        GameObject quizPanelObj = Instantiate(QuizPanelPrefab, Canvas.transform);
        QuizPanel quizPanel = quizPanelObj.GetComponent<QuizPanel>();

        // カードの種類を取得する
        CardType? cardType = DataBase.Instance.GetCardTypeFromCardID(cardID);        
        switch(cardType)
        {
            case null:
                Debug.Log($"存在しないCardID:{cardID}のCardTypeを取得できないため終了します。");
                return;
            
            case CardType.Question:
                // 問題カードなので問題を表示する
                Sprite quizSprite = DataBase.Instance.GetQuizSprite(cardID);
                quizPanel.DisplayQuiz(quizSprite);
                // 現在表示している問題の変数を更新する
                currentDisplayQuestionCard = cardID;
                break;
            
            case CardType.Hint:
                // currentDisplayQuestionCardのnullチェック
                if (IsValidCurrentQuiz())
                {
                    // ヒントが正答かどうか
                    if(DataBase.Instance.IsCorrectHint((CardID)currentDisplayQuestionCard, cardID))
                    {
                        // 問題にヒントを加えた画像に差し替える
                        Sprite sprite = DataBase.Instance.GetQuizAddedHintSprite((CardID)currentDisplayQuestionCard, cardID);
                        quizPanel.DisplayQuiz(sprite);
                    }
                    else
                    {
                        // 現在表示している画像の指定の位置にヒント画像を乗せて表示する
                        Sprite sprite = DataBase.Instance.GetQuizWrongHintSprite(cardID);
                        quizPanel.DisplayHint((CardID)currentDisplayQuestionCard, sprite);

                        isDefaultHint = true;
                    }
                    // 現在表示しているヒントの変数を更新する
                    currentDisplayHintCard = cardID;
                }
                break;            
        }

        // プログレスバーを動的に変化させる
        // dogを出現させるときはプログレスバーを表示しない
        await InCreaseProgressBarUniTask(false, isDefaultHint);
        
        // クイズが表示されていないときは、ERROR表示してメイン画面に戻る
        if(currentDisplayQuestionCard == null)
        {
            await DisplayNonHintError();
        }

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
    public async UniTask InCreaseProgressBarUniTask(bool isCorrectQuiz, bool isDefaultHint)
    {
        // クイズの正答かどうか
        bool isAnswer = isCorrectQuiz;

        postProcessVolume.enabled = true;

        GameObject progressBar = Instantiate(ProgressBarPanel, Canvas.transform);
        ProgressBarPanel progressBarPanel = progressBar.GetComponent<ProgressBarPanel>();
        if(isAnswer)
        {
            await progressBarPanel.ActionCorrect(cancellationTokenSource);
        }
        else
        {
            await progressBarPanel.ActionOnRead(cancellationTokenSource);
        }
        Destroy(progressBar);

        // PostProcessをOFFにする
        postProcessVolume.enabled = false;

        // 雪を降らせる
        if(currentDisplayQuestionCard != null)
        {
            DisplaySnowParticleIfCan((CardID)currentDisplayQuestionCard);
        } 
    }


    // クイズが表示されていないときの演出
    private async UniTask DisplayNonHintError()
    {
        // メイン画面に戻る
        QuizPanelSetActive(false);
        
        // ERROR!クイズが読み込まれていませんを表示する
        GameObject nonHintPanel = Instantiate(NonHintPanelPrefab, Canvas.transform);
        await nonHintPanel.GetComponent<NonHintError>().Action();
        
        // リセット処理として消去する
        Destroy(nonHintPanel);
    }

    private static async UniTask FadeOut(Image image, float seconds, Easing.Ease ease)
    {
        Color color = image.color;
        Func<float, float> easingMethod = Easing.GetEasingMethod(ease);

        float rate = 0;
        while (rate < 1.0f)
        {
            await UniTask.Yield();
            rate += Time.deltaTime / seconds;
            if (rate >= 1.0f) rate = 1.0f;

            color.a = easingMethod(1.0f - rate);
            image.color = color;
        }
    }
    private static async UniTask FadeOut(TextMeshProUGUI text, float seconds, Easing.Ease ease)
    {
        Color color = text.color;
        Func<float, float> easingMethod = Easing.GetEasingMethod(ease);

        float rate = 0;
        while (rate < 1.0f)
        {
            await UniTask.Yield();
            rate += Time.deltaTime / seconds;
            if (rate >= 1.0f) rate = 1.0f;

            color.a = easingMethod(1.0f - rate);
            text.color = color;
        }
    }

    private static async UniTask FadeIn(Image image, float seconds, Easing.Ease ease)
    {
        Color color = image.color;
        Func<float, float> easingMethod = Easing.GetEasingMethod(ease);

        float rate = 0;
        while (rate < 1.0f)
        {
            await UniTask.Yield();
            rate += Time.deltaTime / seconds;
            if (rate >= 1.0f) rate = 1.0f;

            color.a = easingMethod(rate);
            image.color = color;
        }
    }

    private static async UniTask FadeIn(TextMeshProUGUI text, float seconds, Easing.Ease ease)
    {
        Color color = text.color;
        Func<float, float> easingMethod = Easing.GetEasingMethod(ease);

        float rate = 0;
        while (rate < 1.0f)
        {
            await UniTask.Yield();
            rate += Time.deltaTime / seconds;
            if (rate >= 1.0f) rate = 1.0f;

            color.a = easingMethod(rate);
            text.color = color;
        }
    }

    // UI要素のON・OFF
    public void QuizPanelSetActive(bool active)
    {
        // パネルをOFFにする時に，プログレスバーが進行中であればUniTaskのキャンセル処理を行う
        if(active == false)
        {
            // QuizPanelを削除する
            GameObject quizPanel = GameObject.FindObjectOfType<QuizPanel>().gameObject;
            Destroy(quizPanel);
            // PostProcessの再開
            postProcessVolume.enabled = true;
            // 雪の日表示
            SnowParticle.SetActive(false);
            // 現在読み込んでいる問題をリセットする
            currentDisplayQuestionCard = null;
        }
    }

    public void CancelReadingProgressBar()
    {
        cancellationTokenSource.Cancel();
        IsUpdatingProgressBar = false;
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

    // 指定したカードならば雪を降らせる
    private void DisplaySnowParticleIfCan(CardID quiz)
    {
        if(quiz == CardID.Question03_Snow)
        {
            SnowParticle.SetActive(true);
        }
        else
        {
            SnowParticle.SetActive(false);
        }
    }

    // 渋滞問題が表示されていてかつ、スピーカーがヒントとして読み込まれているとき
    // dogの画像に変える
    public void OnActionSirenWhenDisplayTrafficJam()
    {
        if( currentDisplayQuestionCard != CardID.Question06_TrafficJam || 
            currentDisplayHintCard != CardID.Hint06_Speaker)
        {
            Debug.LogError($"問題{CardID.Question06_TrafficJam}にて、正答のサイレンが流された時の挙動がリクエストされましたが、"
                        + "\n正常な問題とヒントが表示されていないため処理を中断します。");
            return;
        }

        // 特殊なヒントカード Hint06_Sirenを読み込んだこととして
        // 固有メソッドでフェードアウトしながらdogに変更する
        DisplayQuestionImageWithProgressBarUniTask(CardID.Hint06_Siren).Forget();
    }

    // 正解を表示して，暫く経ったらメイン画面に戻る
    public async UniTask DisplayCorrectAndBackMainUniTask(bool isOwnQuizCorrected)
    {
        Debug.Log("正解を表示して，メイン画面に戻る");
        if (IsUpdatingProgressBar) return;
        IsUpdatingProgressBar = true;

        // 自分のクイズではない時は，クイズ画面を非表示にしない
        // クイズを解いている間は別クライアントでの正解を表示しない
        if(isOwnQuizCorrected)
        {
            // クイズ画面を非表示にする
            QuizPanelSetActive(false);
        }

        postProcessVolume.enabled = true;

        // Correct!の表示をする
        GameObject correctPanel = Instantiate(CorrectPanelPrefab, Canvas.transform);
        await correctPanel.GetComponent<CorrectPanel>().Action();
        Destroy(correctPanel);

        IsUpdatingProgressBar = false;
    }

    // フェーズクリア時の演出をする
    public async UniTask PhaseClearProcessUniTask(Phase clearedPhase)
    {
        Debug.Log("フェーズクリア!");
        if (IsUpdatingProgressBar) return;
        IsUpdatingProgressBar = true;

        // クイズが表示されている場合は非表示にする
        QuizPanelSetActive(false);

        // クリアしたフェーズによって処理を変える
        switch (clearedPhase)
        {
            case Phase.Phase1:
                Debug.Log("フェーズ1クリア!");
                await Phase1ClearUnitask();
                break;

            case Phase.Phase2:
                Debug.Log("フェーズ2クリア!");
                Debug.Log("階段の行き来が可能になる");
                await Phase2ClearUnitask();
                // 
                break;

            case Phase.Phase3:
                Debug.Log("フェーズ3クリア!");
                await Phase3ClearUnitask();
                break;
        }

        //await UniTask.WaitForSeconds(5.0f);

        IsUpdatingProgressBar = false;
    }

    private async UniTask Phase1ClearUnitask()
    {
        // BGPanelを徐々に表示
        await FadeIn(PhaseClearBGPanelImage, 1.0f, Easing.Ease.InSine);

        // 文字色を変更
        PhaseClearText.color = Phase1TextColor;

        // 文字を点滅しながら出現
        await FadeIn(PhaseClearText, 0.2f, Easing.Ease.InSine);
        await FadeOut(PhaseClearText, 0.2f, Easing.Ease.InSine);
        await FadeIn(PhaseClearText, 0.2f, Easing.Ease.InSine);
        await FadeOut(PhaseClearText, 0.2f, Easing.Ease.InSine);
        await FadeIn(PhaseClearText, 0.2f, Easing.Ease.InSine);

        // 横に流れるようにして，行動範囲が拡大されたを表示する
        PhaseClearMiniMessage.text = "";
        await FadeIn(PhaseClearMiniMessage, 0.1f, Easing.Ease.InQuad);
        await AddTextDynamic(PhaseClearMiniMessage, "行動範囲が拡大された");

        await UniTask.WaitForSeconds(5.0f);

        // BGPanelを徐々に非表示
        await FadeOut(PhaseClearText, 0.2f, Easing.Ease.InSine);
        await FadeOut(PhaseClearMiniMessage, 0.2f, Easing.Ease.InSine);
        await FadeOut(PhaseClearBGPanelImage, 1.0f, Easing.Ease.InSine);
    }

    // 流れるようにメッセージを追加する
    private static async UniTask AddTextDynamic(TextMeshProUGUI textMesh, string addText)
    {
        textMesh.text = "";
        int textLen = addText.Length;
        int idx = 0;
        while(idx < textLen)
        {
            await UniTask.Yield(PlayerLoopTiming.Update);
            await UniTask.WaitForSeconds(0.2f);
            textMesh.text += addText[idx];
            idx += 1;
        }
    }

    private async UniTask Phase2ClearUnitask()
    {
        // BGPanelを徐々に表示
        await FadeIn(PhaseClearBGPanelImage, 1.0f, Easing.Ease.InSine);

        // 文字色を変更
        PhaseClearText.color = Phase2TextColor;
        PhaseClearText.text = "PHASE2 クリア!";

        // 文字を点滅しながら出現
        await FadeIn(PhaseClearText, 0.2f, Easing.Ease.InSine);
        await FadeOut(PhaseClearText, 0.2f, Easing.Ease.InSine);
        await FadeIn(PhaseClearText, 0.2f, Easing.Ease.InSine);
        await FadeOut(PhaseClearText, 0.2f, Easing.Ease.InSine);
        await FadeIn(PhaseClearText, 0.2f, Easing.Ease.InSine);

        // 横に流れるようにして，行動範囲が拡大されたを表示する
        PhaseClearMiniMessage.text = "";
        await FadeIn(PhaseClearMiniMessage, 0.1f, Easing.Ease.InQuad);
        await AddTextDynamic(PhaseClearMiniMessage, "階段の行き来が\n可能になった");

        await UniTask.WaitForSeconds(5.0f);

        // BGPanelを徐々に非表示
        await FadeOut(PhaseClearText, 0.2f, Easing.Ease.InSine);
        await FadeOut(PhaseClearMiniMessage, 0.2f, Easing.Ease.InSine);
        await FadeOut(PhaseClearBGPanelImage, 1.0f, Easing.Ease.InSine);

        // 最終問題を出題
        CircleClockQuizManager.Instance.DisplayQuiz();
    }

    private async UniTask Phase3ClearUnitask()
    {
        // BGPanelを徐々に表示
        await FadeIn(PhaseClearBGPanelImage, 1.0f, Easing.Ease.InSine);

        // 文字色を変更
        PhaseClearText.color = Phase3TextColor;
        PhaseClearText.text = "脱出成功";

        // 文字を点滅しながら出現
        await FadeIn(PhaseClearText, 0.2f, Easing.Ease.InSine);
        await FadeOut(PhaseClearText, 0.2f, Easing.Ease.InSine);
        await FadeIn(PhaseClearText, 0.2f, Easing.Ease.InSine);
        await FadeOut(PhaseClearText, 0.2f, Easing.Ease.InSine);
        await FadeIn(PhaseClearText, 0.2f, Easing.Ease.InSine);

        // 横に流れるようにして，行動範囲が拡大されたを表示する
        PhaseClearMiniMessage.text = "";
        await FadeIn(PhaseClearMiniMessage, 0.1f, Easing.Ease.InQuad);
        await AddTextDynamic(PhaseClearMiniMessage, "Congratulations!");

        await UniTask.WaitForSeconds(5.0f);

        // BGPanelを徐々に非表示
        await FadeOut(PhaseClearText, 0.2f, Easing.Ease.InSine);
        await FadeOut(PhaseClearMiniMessage, 0.2f, Easing.Ease.InSine);
        await FadeOut(PhaseClearBGPanelImage, 1.0f, Easing.Ease.InSine);
    }
}
