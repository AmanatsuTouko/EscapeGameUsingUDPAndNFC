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

    // 1階もしくは2階のデータ
    [Header("Quiz Image Corresponding to Card ID")]
    [SerializeField] ClientScriptableObject clientScriptableObject;

    // NECカードのUUIDとCardIDの対応付けを定義したScriptableObject
    [SerializeField] UUIDToCardIDScriptableObject uuidToCardIdDictScriptableObject;

    // クイズ画像表示用UI
    [Header("UI")]
    [SerializeField] Image QuizDisplayImage;

    void Start()
    {
        // Imageの反映先を自身の持つImageクラスにする
        clientScriptableObject.image = QuizDisplayImage;

        // カード読み取り時に実行する関数を登録する
        nfcReader.ActionOnReadCard += DisplayImageOnRemoteClientFromUUID;

        // 交通系IC読み取り時に実行する関数を登録する
        nfcReader.ActionOnReadTranspotationICCard += ActionOnReadTransportationICCard;

        // UDP受信時に実行する関数を登録する
        udpReceiver.ActionRecieveData += OnRecieveMessage;
    }

    void Update()
    {
        // キー入力でUDPを送信する
        // NFCカードの読み取りがあれば，読み込んだCardIDを送信する
        if (Input.GetKeyDown(KeyCode.Space))
        {
            udpSender.SendMessage("Hello from MacBook!");
        }
    }

    // 別クライアントでカード読み取り時にNFCカードから取得したUUIDを引数にして画像を表示する関数を起動するメッセージをUDPで送信する
    void DisplayImageOnRemoteClientFromUUID(string uuid)
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
        RPCManager.InvokeFromJson(receivedJson);
    }

    // NFCカードの識別番号と対応するクイズ画像のデータを更新する
    void UpdateClientScriptableObject(ClientScriptableObject clientScriptableObject)
    {
        this.clientScriptableObject = clientScriptableObject;
        clientScriptableObject.image = QuizDisplayImage;
    }

    // TODO：RPCするので，staticにしなければならない
    // 受信した時に実行する関数
    public void DisplayQuestionImage(string uuidString)
    {
        // 文字列のUUIDからCardIDに変換する
        CardID? cardID = uuidToCardIdDictScriptableObject.GetCardIDFromUUID(uuidString);
        if(cardID == null)
        {
            Debug.LogError($"{uuidString}をCardIDに変換できないため，処理を停止します．");
            return;
        }
        // cardIDに応じた処理を行う
        clientScriptableObject.DisplayQuestionImage((CardID)cardID);
    }
}