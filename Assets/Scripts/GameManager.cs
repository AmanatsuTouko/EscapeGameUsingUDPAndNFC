using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
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
        // UDP受信時に実行する関数を登録する
        udpReceiver.ActionRecieveData += DisplayQuestionImage;
        // UUID取得時に実行する関数を登録する
        nfcReader.ActionOnReadCard += SendUUID;
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

    // 受信した時に実行する関数
    void DisplayQuestionImage(string cardIDString)
    {
        // TODO :
        // CardIDをUUIDに変更する
        // 辞書を用いて管理できるようにする

        // cardNameをstringからenumへ変換する
        CardID cardID;
        if (!Enum.TryParse(cardIDString, out cardID))
        {
            Debug.LogError($"Error:{cardIDString}はCardIDに存在しません");
            return;
        }

        // cardIDに応じた処理を行う
        clientScriptableObject.DisplayQuestionImage(cardID);
    }

    // NFCカードの識別番号と対応するクイズ画像のデータを更新する
    void UpdateClientScriptableObject(ClientScriptableObject clientScriptableObject)
    {
        this.clientScriptableObject = clientScriptableObject;
        clientScriptableObject.image = QuizDisplayImage;
    }

    // NFCカードから取得したUUIDをもとにメッセージをUDPで送信する
    void SendUUID(string uuid)
    {
        udpSender.SendMessage(uuid);
    }
}
