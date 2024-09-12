using System;
using UnityEngine;
using UnityEngine.UI;

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
        nfcReader.ActionOnReleaseCard += DisableQuizPanelIfWhileReadCardOnRemoteClient;
    }

    private void Update()
    {
        // スペースキー入力でクイズ画像を消せるようにする
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DisableQuizPanel();
        }

        // マウスホイールクリックでスピーカーが起動した処理を行う
        if(Input.GetMouseButtonDown(2))
        {
            UIManager.Instance.OnActionSirenWhenDisplayTrafficJam();
        }
    }

    public void OnRead(string uuid)
    {
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
            UIManager.Instance.CorrectPerformance(uuid);
        }
        // そうではない場合は、問題カードorヒントカードなので、他クライアントに送信する
        else
        {
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
        UIManager.Instance.CorrectPerformanceOfTransportationICCard();
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
        UIManager.Instance.DisplayQuestionImageWithProgressBar((CardID)cardID);
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
        if (UIManager.Instance.IsUpdatingProgressBar)
        {
            DisableQuizPanel();
        }
    }
    // クイズ画像の非表示
    private void DisableQuizPanel()
    {
        UIManager.Instance.QuizPanelSetActive(false);
    }
}