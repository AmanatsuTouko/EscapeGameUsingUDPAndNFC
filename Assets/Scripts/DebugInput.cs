using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1台だけでテストできるようにする
public class DebugInput : MonoBehaviour
{
    // テスト用に送受信するUUIDのインデックス
    public CardID TestCardID;

    // NFCカードを読み込んだ際の挙動をデバッグする
    public void Read()
    {
        Debug.Log($"デバッグ：NFCカード{TestCardID}を読み込んだ際の挙動");
        GameManager.Instance.OnRead(GetUUIDFromCardID(TestCardID));
    }

    // NFCカードを読み込んだ後のUDP送信をデバッグする
    public void SendReadedCard()
    {
        Debug.Log($"デバッグ：NFCカード{TestCardID}を読み込んで他クライアントに送信する際の挙動");
        GameManager.Instance.DisplayImageOnRemoteClientFromUUID(GetUUIDFromCardID(TestCardID));
    }
    
    // 別クライアントから送信されたきた時をデバッグする
    public void RecieveReadedCard()
    {
        Debug.Log($"デバッグ：NFCカード{TestCardID}が他クライアントから送信されてきた際の挙動");
        RPCStaticMethods.DisplayQuestionImage(GetUUIDFromCardID(TestCardID));
    }

    // クイズカードをクリアした時をデバッグする
    public void ClearQuiz()
    {
        PhaseManager.Instance.QuizClear(TestCardID);
        // GameManager.Instance.QuizClearOnRemoteClient(TestCardID);
    }

    private string GetUUIDFromCardID(CardID cardID)
    {
        string? uuid = DataBase.Instance.GetUUIDFromCardID(TestCardID);
        if(uuid == null)
        {
            Debug.LogError($"CardID{TestCardID}に対応するUUIDが存在しません。");
            return "";
        }
        return (string)uuid;
    }
}
