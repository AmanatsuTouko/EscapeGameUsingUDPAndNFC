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


    [Header("Quiz Image Corresponding to Card ID")]

    // CardIDとクイズ画像の組み合わせのデータ
    [SerializeField] CardIDImagePair cardIDImagePair;

    // クイズ画像における問題と答えの組み合わせのデータ
    [SerializeField] AnswerQuizCardIDsImagepair answerQuizCardIDsImagePair;

    [Header("UUID / Card ID Pair")]
    // NECカードのUUIDとCardIDの対応付けを定義したScriptableObject
    [SerializeField] UUIDToCardIDScriptableObject uuidToCardIdDictScriptableObject;

    public CardIDImagePair GetCardIDImagePair()
    {
        return cardIDImagePair;
    }

    public AnswerQuizCardIDsImagepair GetAnswerQuizCardIDsImagepair()
    {
        return answerQuizCardIDsImagePair;
    }

    private void Start()
    {
        // Imageの反映先を自身の持つImageクラスにする
        RegisterImageToScriptableObjejcts(UIManager.Instance.QuizDisplayImage);

        // カード読み取り時に実行する関数を登録する
        nfcReader.ActionOnReadCard += DisplayImageOnRemoteClientFromUUID;

        // 交通系IC読み取り時に実行する関数を登録する
        nfcReader.ActionOnReadTranspotationICCard += ActionOnReadTransportationICCard;

        // UDP受信時に実行する関数を登録する
        udpReceiver.ActionRecieveData += OnRecieveMessage;

        // カードを離したタイミングでクイズ画像を非表示にする関数を登録する
        nfcReader.ActionOnReleaseCard += DisableQuizPanel;
    }

    private void Update()
    {
        // スペースキー入力でクイズ画像を消せるようにする
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DisableQuizPanel();
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
        // 特殊なクイズ画像の表示
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

    // ScriptableObjejctとImageの紐づけ
    public void RegisterImageToScriptableObjejcts(Image image)
    {
        cardIDImagePair.TargetImage = image;
        answerQuizCardIDsImagePair.TargetImage = image;
    }

    // 受信した時に実行する関数
    public void DisplayQuestionImage(string uuidString)
    {
        // 文字列のUUIDからCardIDに変換する
        CardID? cardID = uuidToCardIdDictScriptableObject.GetCardIDFromUUID(uuidString);
        if (cardID == null)
        {
            Debug.LogError($"{uuidString}をCardIDに変換できないため，処理を停止します．");
            return;
        }
        // クイズ画像の表示
        UIManager.Instance.DisplayQuestionImageWithProgressBar((CardID)cardID);
    }

    // クイズ画像の非表示
    private void DisableQuizPanel()
    {
        UIManager.Instance.QuizPanelSetActive(false);
    }

    public UUIDToCardIDScriptableObject GetUuidToCardIdDictScriptableObject()
    {
        return uuidToCardIdDictScriptableObject;
    }
}