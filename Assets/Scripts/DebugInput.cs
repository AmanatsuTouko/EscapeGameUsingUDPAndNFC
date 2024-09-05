using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1台だけでテストできるようにする
public class DebugInput : MonoBehaviour
{
    // テスト用に送受信するUUIDのインデックス
    public int TestCardID = 0;

    // CardのUUID(GameManagerが所持しているScriptableObjectを参照する)
    public List<UUIDCardID> CardUUIDs;    

    void Start()
    {
        CardUUIDs = GameManager.Instance.GetUuidToCardIdDictScriptableObject().UuidCard;
    }

    // Update is called once per frame
    void Update()
    {
        // NFCカードを読み込んだ後のUDP送信をデバッグする
        if(Input.GetKeyDown(KeyCode.P))
        {
            GameManager.Instance.DisplayImageOnRemoteClientFromUUID(CardUUIDs[TestCardID].Uuid);
        }

        // 別クライアントから送信されたきた時をデバッグする
        if (Input.GetKeyDown(KeyCode.M))
        {
            RPCStaticMethods.DisplayQuestionImage(CardUUIDs[TestCardID].Uuid);
        }

        // Imageの画像をオフにする
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            GameManager.Instance.DisplayImageSetActive(false);
        }
    }
}
