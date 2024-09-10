using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1台だけでテストできるようにする
public class DebugInput : MonoBehaviour
{
    // テスト用に送受信するUUIDのインデックス
    public CardID TestCardID;

    // CardのUUID(GameManagerが所持しているScriptableObjectを参照する)
    public List<UUIDCardID> CardUUIDs;

#if UNITY_EDITOR
    void Start()
    {
        CardUUIDs = GameManager.Instance.GetUuidToCardIdDictScriptableObject().UuidCard;
    }

    void Update()
    {
        // NFCカードを読み込んだ際の挙動をデバッグする(R:Readの略)
        if(Input.GetKeyDown(KeyCode.R))
        {
            GameManager.Instance.OnRead(CardUUIDs[(int)TestCardID].Uuid.ToString());
        }

        // NFCカードを読み込んだ後のUDP送信をデバッグする(S:Sendの略)
        if(Input.GetKeyDown(KeyCode.S))
        {
            GameManager.Instance.DisplayImageOnRemoteClientFromUUID(CardUUIDs[(int)TestCardID].Uuid);
        }

        // 別クライアントから送信されたきた時をデバッグする(M:Messageの略)
        if (Input.GetKeyDown(KeyCode.M))
        {
            RPCStaticMethods.DisplayQuestionImage(CardUUIDs[(int)TestCardID].Uuid);
        }
    }
#endif
}
