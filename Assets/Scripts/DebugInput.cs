using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1台だけでテストできるようにする
public class DebugInput : MonoBehaviour
{
    // テスト用に送受信するUUIDのインデックス
    public CardID TestCardID;

#if UNITY_EDITOR
    void Update()
    {
        // NFCカードを読み込んだ際の挙動をデバッグする(R:Readの略)
        if(Input.GetKeyDown(KeyCode.R))
        {
            GameManager.Instance.OnRead(GetUUIDFromCardID(TestCardID));
        }

        // NFCカードを読み込んだ後のUDP送信をデバッグする(S:Sendの略)
        if(Input.GetKeyDown(KeyCode.S))
        {
            GameManager.Instance.DisplayImageOnRemoteClientFromUUID(GetUUIDFromCardID(TestCardID));
        }

        // 別クライアントから送信されたきた時をデバッグする(M:Messageの略)
        if (Input.GetKeyDown(KeyCode.M))
        {
            RPCStaticMethods.DisplayQuestionImage(GetUUIDFromCardID(TestCardID));
        }
    }

    private string GetUUIDFromCardID(CardID cardID)
    {
        string? uuid = GameManager.Instance.GetUuidToCardIdDictScriptableObject().GetUUIDFromCardID(TestCardID);
        if(uuid == null)
        {
            Debug.LogError($"CardID{TestCardID}に対応するUUIDが存在しません。");
            return "";
        }
        return (string)uuid;
    }

#endif
}
