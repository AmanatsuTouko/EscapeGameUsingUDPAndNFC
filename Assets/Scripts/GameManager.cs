using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    // UDP通信
    [Header("UDP Connection")]
    [SerializeField] UdpSender udpSender;
    [SerializeField] UdpReceiver udpReceiver;
    [SerializeField] DisplayLocalIP displayLocalIP;

    // NFCカードの読み込み
    [Header("Read NFC Card")]
    [SerializeField] NFCReader nfcReader;

    private void Start()
    {
        // カード読み取り時に実行する関数を登録する
        nfcReader.ActionOnReadCard += OnRead;

        // 交通系IC読み取り時に実行する関数を登録する
        nfcReader.ActionOnReadTranspotationICCard += ActionOnReadTransportationICCard;

        // UDP受信時に実行する関数を登録する
        udpReceiver.ActionRecieveData += OnRecieveMessage;

        // カード読み込み中にカードを離した場合は，読み込み処理を中断する
        nfcReader.ActionOnReleaseCard += OnReleaseCard;
    }

    private void Update()
    {
        // スペースキー入力でクイズ画像を消せるようにする
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // クイズ画像を消す
            DisableQuizAndProgressBar();
        }

        // マウスホイールクリックでスピーカーが起動した処理を行う
        if(Input.GetMouseButtonDown(2))
        {
            UIManager.Instance.OnActionSirenWhenDisplayTrafficJam();
        }

        // 最後の問題のクリアを判定する
        if(Input.GetMouseButton(0) && Input.GetMouseButton(1) && Input.GetMouseButtonDown(2))
        {
            if(PhaseManager.Instance.Phase != Phase.Phase3)
            {
                return;
            }
            
            // 最後の問題をクリアしたことを送信する
            PhaseManager.Instance.QuizClear(CardID.Question07_FinalQuestion);
            QuizClearOnRemoteClient(CardID.Question07_FinalQuestion);
            
            // タイマーストップ
            TimeManager.Instance.Timer.Stop();
        }
    }

    public void OnRead(string uuid)
    {
        // 読み込み音を鳴らす
        SoundManager.Instance.PlaySE(SE.OnReadCard);

        CardID? cardID = DataBase.Instance.GetCardIDFromUUID(uuid);
        if(cardID == null)
        {
            Debug.LogError($"エラー：登録されていないカードのUUID:{uuid}が読み込まれました。");
            return;
        }

        // 今読んだカードが表示している問題の正答かどうかを判定する
        bool isCorrectQuestion = UIManager.Instance.IsCorrectForCurrentQuestion((CardID)cardID);
        // 正答の場合
        if(isCorrectQuestion)
        {
            // クリアフラグをONにして、演出を追加する、その後メイン画面に戻る
            CorrectPerformance(uuid);
        }
        // そうではない場合は、問題カードorヒントカードなので、他クライアントに送信する
        else
        {
            // 送信中のプログレスバーを表示する
            UIManager.Instance.DisplaySendingBarPanelUniTask().Forget();
            // 問題カードかヒントカードかどうかは他クライアントが判別して処理を行う
            DisplayImageOnRemoteClientFromUUID(uuid);
        }
    }

    // 別クライアントでカード読み取り時にNFCカードから取得したUUIDを引数にして画像を表示する関数を起動するメッセージをUDPで送信する
    public void DisplayImageOnRemoteClientFromUUID(string uuid)
    {
        // UUIDを引数に，画像を表示する関数を別クライアントで実行する
        string jsonMethod = RPCManager.GetJsonFromMethodArgs(nameof(RPCStaticMethods), nameof(RPCStaticMethods.DisplayQuestionImage), new string[]{uuid});
        udpSender.SendMessage(jsonMethod);
    }

    // 交通系IC読み取り時に実行する関数
    void ActionOnReadTransportationICCard()
    {
        Debug.Log("交通系ICが読み込まれた");
        if (UIManager.Instance.IsDisplayQuizAndHintForTransportation())
        {
            // 処理を行う
            Debug.LogError("Suicaを読みこんで正答した際の処理");

            PhaseManager.Instance.QuizClear(CardID.Question04_Loop);
            QuizClearOnRemoteClient(CardID.Question04_Loop);
        }
    }

    // 受信した文字列に応じて関数を実行する
    void OnRecieveMessage(string receivedJson)
    {
        try
        {
            RPCManager.InvokeFromJson(receivedJson);
        }
        catch
        {
            Debug.LogError("関数として無効な文字列を受信したため，動作を終了しました．");
        }
    }

    // カード読み込み中にカードを離した場合の処理
    private void OnReleaseCard()
    {
        // キャンセル音を鳴らす
        SoundManager.Instance.PlaySE(SE.OnCancelRead);
        // 別クライアントでの読み込みを中止する
        DisableQuizPanelIfWhileReadCardOnRemoteClient();
        // 自クライアントでの送信中...の表示を中止する
        UIManager.Instance.DeleteSendingBarPanel();
    }

    // 受信した時に実行する関数
    public void DisplayQuestionImage(string uuidString)
    {
        // 文字列のUUIDからCardIDに変換する
        CardID? cardID = DataBase.Instance.GetCardIDFromUUID(uuidString);
        if (cardID == null)
        {
            Debug.LogError($"{uuidString}をCardIDに変換できないため，処理を停止します．");
            return;
        }
        // クイズ画像の表示
        UIManager.Instance.DisplayQuestionImageWithProgressBarUniTask((CardID)cardID).Forget();
    }

    // 別クライアントでクイズ画像を非表示にする
    private void DisableQuizPanelIfWhileReadCardOnRemoteClient()
    {
        // UUIDを引数に，画像を表示する関数を別クライアントで実行する
        string jsonMethod = RPCManager.GetJsonFromMethodArgs(nameof(RPCStaticMethods), nameof(RPCStaticMethods.DisableQuizPanelIfWhileReadCard), new string[]{});
        udpSender.SendMessage(jsonMethod);
    }

    // 別クライアントでクイズのクリア処理を行う
    public void QuizClearOnRemoteClient(CardID quizCardID)
    {
        string uuidString = DataBase.Instance.GetUUIDFromCardID(quizCardID);
        string jsonMethod = RPCManager.GetJsonFromMethodArgs(nameof(RPCStaticMethods), nameof(RPCStaticMethods.QuizClear), new string[]{uuidString});
        udpSender.SendMessage(jsonMethod);
    }

    // カード読み込み中だった場合には，クイズ画像を非表示にする
    public void DisableQuizPanelIfWhileReadCard()
    {
        if (UIManager.Instance.IsDisplayProgressBar())
        {
            DisableQuizAndProgressBar();
        }
    }
    
    // クイズ画像、プログレスバーの非表示
    private void DisableQuizAndProgressBar()
    {
        // Phase処理中だった場合は何もしない
        if(PhaseManager.Instance.IsPhaseProcessing)
        {
            return;
        }
        // カード読み込みをキャンセルして、クイズ画像とプログレスバーを非表示にする
        UIManager.Instance.CancelReadingProgressBar();
        UIManager.Instance.DeleteProgressBar();
        UIManager.Instance.DeleteQuizPanel();
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

        // クイズのクリアを伝える
        PhaseManager.Instance.QuizClear((CardID)quiz);
        Instance.QuizClearOnRemoteClient((CardID)quiz);
    }

    // Timerの時刻同期メソッド
    // 別クライアントでTimerの時刻同期を行う
    public void SynchronizeTimerOnRemoteClient()
    {
        // UUIDを引数に，画像を表示する関数を別クライアントで実行する
        string jsonMethod = RPCManager.GetJsonFromMethodArgs(
            nameof(RPCStaticMethods), 
            nameof(RPCStaticMethods.SynchronizeTimer), 
            new string[]{TimeManager.Instance.GetStartTime().ToString()}
        );
        udpSender.SendMessage(jsonMethod);
    }
}