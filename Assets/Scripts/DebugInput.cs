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

    void Start()
    {
        CardUUIDs = GameManager.Instance.GetUuidToCardIdDictScriptableObject().UuidCard;
    }

    void Update()
    {
        // NFCカードを読み込んだ後のUDP送信をデバッグする
        if(Input.GetKeyDown(KeyCode.P))
        {
            GameManager.Instance.DisplayImageOnRemoteClientFromUUID(CardUUIDs[(int)TestCardID].Uuid);
        }

        // 別クライアントから送信されたきた時をデバッグする
        if (Input.GetKeyDown(KeyCode.M))
        {
            RPCStaticMethods.DisplayQuestionImage(CardUUIDs[(int)TestCardID].Uuid);
        }

        // Imageの画像をオフにする
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            UIManager.Instance.DisplayImageSetActive(false);
        }
    }
}
