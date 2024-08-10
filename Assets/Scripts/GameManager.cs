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

    // 1階もしくは2階のデータ
    [Header("Quiz Image Corresponding to NFC ID")]
    [SerializeField] ClientScriptableObject clientScriptableObject;

    // クイズ画像表示用UI
    [Header("UI")]
    [SerializeField] Image QuizDisplayImage;

    void Start()
    {
        // UDP受信時に実行する関数を登録する
        udpReceiver.ActionRecieveData += DisplayQuestionImage;
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
}
