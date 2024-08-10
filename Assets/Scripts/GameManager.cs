using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // UDP通信
    [SerializeField] UdpSender udpSender;
    [SerializeField] UdpReceiver udpReceiver;
    [SerializeField] DisplayLocalIP displayLocalIP;

    // 1階もしくは2階のデータ
    [SerializeField] ClientScriptableObject clientScriptableObject;

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
}
