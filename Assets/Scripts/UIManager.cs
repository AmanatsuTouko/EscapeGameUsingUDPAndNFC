using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using System.Threading;
using UnityEngine.Rendering.PostProcessing;

public class UIManager : SingletonMonobehaviour<UIManager>
{
    // メイン画面
    [Header("Main Panel")]
    [SerializeField] GameObject mainPanel;
    [SerializeField] TextMeshProUGUI limitTimeText;

    [Header("Canvas & Panels")]
    [SerializeField] GameObject Canvas;
    [SerializeField] GameObject ProgressBarPanel;
    [SerializeField] GameObject QuizPanelPrefab;
    [SerializeField] GameObject CorrectPanelPrefab;
    [SerializeField] GameObject NonHintPanelPrefab;
    [SerializeField] GameObject PhaseClearPanelPrefab;

     // PostProcess
    [Header("Post Process")]
    [SerializeField] PostProcessVolume postProcessVolume;
    
    // UniTaskのキャンセル用トークン
    public CancellationTokenSource cancellationTokenSource { get; private set; }

    // 現在読み込んでいる問題カード
    CardID? currentDisplayQuestionCard = null;
    // 現在読み込んでいるヒントカード
    CardID? currentDisplayHintCard = null;
    
    private void Start()
    {
        currentDisplayQuestionCard = null;
        currentDisplayHintCard = null;

        // デバッグ時にTokenSourceがnullとなるので初期化だけしておく
        // （カードをスキャンせずに，クリア演出をしようとするとなる）
        InitializeCancellationTokenSource();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log($"現在の問題:{currentDisplayQuestionCard}, 現在のヒント:{currentDisplayHintCard}");
        }
    }

    public async UniTask DisplayQuestionImageWithProgressBarUniTask(CardID cardID)
    {
        // 読み込み演出の途中で新たに読み込み演出をしないようにする
        if (IsDisplayProgressBar())
        {
            return;
        }
        // 既にQuizPanelがある場合は削除する
        if(FindObjectOfType<QuizPanel>() is QuizPanel deleteQuizPanel)
        {
            Destroy(deleteQuizPanel.gameObject);
        }

        // トークンソースの初期化
        InitializeCancellationTokenSource();

        // プログレスバーを動的に変化させる
        // TODO:
        // dogを出現させるときはプログレスバーを表示しない
        await DisplayProgressBarUniTask(false);

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
                        // 再度Quiz画像を表示する
                        Sprite nowQuizSprite = DataBase.Instance.GetQuizSprite((CardID)currentDisplayQuestionCard);
                        quizPanel.DisplayQuiz(nowQuizSprite);

                        // 現在表示している画像の指定の位置にヒント画像を乗せて表示する
                        Sprite sprite = DataBase.Instance.GetQuizWrongHintSprite(cardID);
                        quizPanel.DisplayHint((CardID)currentDisplayQuestionCard, sprite);
                    }
                    // 現在表示しているヒントの変数を更新する
                    currentDisplayHintCard = cardID;
                }
                break;            
        }
        
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
            Debug.Log($"問題：{currentDisplayQuestionCard}の時にヒント：{currentDisplayQuestionCard}を読み込んだ際の関数が登録されていないので、実行しません。");
        }
    }

    // プログレスバーを表示する
    public async UniTask DisplayProgressBarUniTask(bool isCorrectQuiz)
    {
        // PostProcessをONにする
        postProcessVolume.enabled = true;

        // プログレスバーの生成と動作の実行
        GameObject progressBar = Instantiate(ProgressBarPanel, Canvas.transform);
        ProgressBarPanel progressBarPanel = progressBar.GetComponent<ProgressBarPanel>();
        if(isCorrectQuiz)
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
    }

    // クイズが表示されていないときの演出
    private async UniTask DisplayNonHintError()
    {
        // クイズパネルを消してメイン画面に戻る
        DeleteQuizPanel();
        
        // ERROR!クイズが読み込まれていませんを表示する
        GameObject nonHintPanel = Instantiate(NonHintPanelPrefab, Canvas.transform);
        await nonHintPanel.GetComponent<NonHintError>().Action();
        
        // リセット処理として消去する
        Destroy(nonHintPanel);
    }

    // クイズ画面を削除してメイン画面に戻す
    public void DeleteQuizPanel()
    {
        // QuizPanelを削除する
        QuizPanel quizPanel = FindObjectOfType<QuizPanel>();
        if(quizPanel)
        {
            Destroy(quizPanel.gameObject);
        }
        else
        {
            Debug.LogError("注意：QuizPanelが表示されていないときに、DeleteQuizPanel()を実行しています。");
        }
        // PostProcessの再開
        postProcessVolume.enabled = true;
        // 現在読み込んでいる問題・ヒントをリセットする
        currentDisplayQuestionCard = null;
        currentDisplayHintCard = null;   
    }

    // プログレスバーの停止と削除
    public void DeleteProgressBar()
    {
        // QuizPanelを削除する
        ProgressBarPanel progressBarPanel = FindObjectOfType<ProgressBarPanel>();
        if(progressBarPanel)
        {
            Destroy(progressBarPanel.gameObject);
        }
        else
        {
            Debug.LogError("注意：ProgressBarが表示されていないときに、DeleteProgressBar()を実行しています。");
        }
    }

    // 正解を表示して，暫く経ったらメイン画面に戻る
    public async UniTask DisplayCorrectAndBackMainUniTask()
    {
        Debug.Log("正解を表示して，メイン画面に戻る");

        // クイズ画面を非表示にする
        DeleteQuizPanel();
        // Correct!の表示をする
        GameObject correctPanel = Instantiate(CorrectPanelPrefab, Canvas.transform);
        await correctPanel.GetComponent<CorrectPanel>().Action();
        // 演出終了時にオブジェクトを破棄する
        Destroy(correctPanel);
    }

    // フェーズクリア時の演出をする
    public async UniTask PhaseClearProcessUniTask(Phase clearedPhase)
    {
        Debug.Log("フェーズクリア!");

        // Panelを生成して，演出を行う
        GameObject phaseClearPanelObj = Instantiate(PhaseClearPanelPrefab, Canvas.transform);
        PhaseClearPanel phaseClearPanel = phaseClearPanelObj.GetComponent<PhaseClearPanel>();
        await phaseClearPanel.Action(clearedPhase);
        // 演出終了時にオブジェクトを破棄する
        Destroy(phaseClearPanelObj);
    }

    // 渋滞問題が表示されていてかつ、スピーカーがヒントとして読み込まれているとき
    // dogの画像に変える
    public void OnActionSirenWhenDisplayTrafficJam()
    {
        if(currentDisplayQuestionCard == CardID.Question06_TrafficJam && currentDisplayHintCard == CardID.Hint06_Speaker)
        {
            // 特殊なヒントカード Hint06_Sirenを読み込んだこととして
            // 固有メソッドでフェードアウトしながらdogに変更する
            DisplayQuestionImageWithProgressBarUniTask(CardID.Hint06_Siren).Forget();
        }
        else
        {
            Debug.LogError($"問題{CardID.Question06_TrafficJam}にて、正答のサイレンが流された時の挙動がリクエストされましたが、"
                        + "\n正常な問題とヒントが表示されていないため処理を中断します。");
            return;
        }
    }

    // 読み込んだカードが現在読み込んでいる問題の答えかどうか
    public bool IsCorrectForCurrentQuestion(CardID readCardID)
    {
        if(IsValidCurrentQuiz() && IsValidCurrentHint())
        {
            return DataBase.Instance.IsCorrectAnswer((CardID)currentDisplayQuestionCard, (CardID)currentDisplayHintCard, readCardID);
        }
        Debug.LogError($"読み込んだカード{readCardID}は現在読み込んでいる問題の答えではありません。");
        return false;
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

    public void CancelReadingProgressBar()
    {
        cancellationTokenSource.Cancel();
    }

    // 時刻の更新
    public void SetLimitTimeText(string text)
    {
        limitTimeText.text = text;
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

    // プログレスバーが表示中かどうか
    public bool IsDisplayProgressBar()
    {
        return FindObjectOfType<ProgressBarPanel>() != null;
    }

    public bool IsCurrentQuizSnow()
    {
        return currentDisplayQuestionCard == CardID.Question03_Snow;
    }

    public bool IsDisplayQuizAndHintForTransportation()
    {
        // 現在、問題4(Loop)が表示されていてかつ、ヒント4(Arrow)が読み込まれた後の画像かどうか
        return currentDisplayQuestionCard == CardID.Question04_Loop && currentDisplayHintCard == CardID.Hint04_Arrow;
    }
}
