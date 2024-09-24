using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugRegisterCard : MonoBehaviour
{
    // NFCカードリーダー
    [SerializeField] NFCReader nfcReader;
    
    // 登録対象のScriptableObject
    [SerializeField] UUIDToCardIDScriptableObject registerTarget;

    // 現在登録中のCardIDのインデックス
    [SerializeField] CardID nextRegister;

    // QuestionCardの枚数
    [SerializeField] int questionCardNum;

    void Start()
    {
        // カード読み取り時に実行する関数を登録する
        nfcReader.ActionOnReadCard += OnRead;
    }

    private void OnRead(string uuid)
    {
        // UUIDを登録する
        registerTarget.UuidCard.Add(
            new UUIDCardID(nextRegister, 
            (int)nextRegister < questionCardNum ? CardType.Question : CardType.Hint, 
            uuid)
        );
        // 登録対象のインデックスを加算
        nextRegister = (CardID)((int)nextRegister + 1);
    }
}
