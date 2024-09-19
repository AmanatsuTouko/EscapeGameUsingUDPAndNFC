using System;
using UnityEngine;

public enum Client
{
    FirstFloor,
    SecondFloor,
}

public class DataBase : SingletonMonobehaviour<DataBase>
{
    [SerializeField]
    private Client client;
    
    public Client Client
    {
        get
        {
            return client;
        } 
        private set
        {
            client = value;
        }
    }

    // データ
    [Header("FirstFloor")]
    [SerializeField] QuizCardScriptableObject firstFloorQuiz;
    [SerializeField] HintCardScriptableObject firstFloorHint;
    [SerializeField] AnswerPairScriptableObject firstFloorAnswer;

    [Header("SecondFloor")]
    [SerializeField] QuizCardScriptableObject secondFloorQuiz;
    [SerializeField] HintCardScriptableObject secondFloorHint;
    [SerializeField] AnswerPairScriptableObject secondFloorAnswer;
    
    [Header("CommonData")]
    [SerializeField] UUIDToCardIDScriptableObject cardUUID;
    [SerializeField] HintDefaultSpriteScriptableObject hintDefaultSprite;

    // 内部的Getter クライアントによって処理を変えられるようにする
    private QuizCardScriptableObject GetQuiz()
    {
        switch(client)
        {
            case Client.FirstFloor:
                return firstFloorQuiz;
            case Client.SecondFloor:
                return secondFloorQuiz;
            default:
                return null;
        }
    }
    private HintCardScriptableObject GetHint()
    {
        switch(client)
        {
            case Client.FirstFloor:
                return firstFloorHint;
            case Client.SecondFloor:
                return secondFloorHint;
            default:
                return null;
        }
    }
    private AnswerPairScriptableObject GetAnswer()
    {
        switch(client)
        {
            case Client.FirstFloor:
                return firstFloorAnswer;
            case Client.SecondFloor:
                return secondFloorAnswer;
            default:
                return null;
        }
    }

    public CardID? GetCardIDFromUUID(string uuid)
    {
        return cardUUID.GetCardIDFromUUID(uuid);
    }
    public string? GetUUIDFromCardID(CardID cardID)
    {
        return cardUUID.GetUUIDFromCardID(cardID);
    }
    public CardType? GetCardTypeFromCardID(CardID cardID)
    {
        return cardUUID.GetCardTypeFromCardID(cardID);
    }
    public CardID? GetCardIDFromAnswer(CardID answer)
    {
        return GetAnswer().GetCardIDFromAnswer(answer);
    }

    public void GetHintDefaultPosSize(CardID quiz, out Vector2 pos, out Vector2 size)
    {
        GetQuiz().GetHintDefaultPosSize(quiz, out pos, out size);
    }

    public bool IsCorrectHint(CardID quiz, CardID hint)
    {
        return GetHint().IsExistQuestionAnswerCardIDPair(quiz, hint);
    }
    public bool IsCorrectAnswer(CardID quiz, CardID hint, CardID answer)
    {
        return GetAnswer().IsExistPair(quiz, hint, answer);
    }

    public Sprite GetQuizSprite(CardID cardID)
    {
        return GetQuiz().GetSpriteFromCardID(cardID);
    }
    public Sprite GetQuizAddedHintSprite(CardID quiz, CardID hint)
    {
        return GetHint().GetSpriteFromCardID(quiz, hint);
    }
    public Sprite GetQuizWrongHintSprite(CardID wrongHint)
    {
        return hintDefaultSprite.GetSpriteFromCardID( wrongHint);
    }

    public void InvokeMethodOnHintDisplay(CardID quiz, CardID hint)
    {
        GetHint().InvokeUniqueMethodIfPossible(quiz, hint);
    }

    public bool IsExistQuiz(CardID quiz, Client targetClient)
    {
        switch(targetClient)
        {
            case Client.FirstFloor:
                return firstFloorQuiz.IsExistQuiz(quiz);
            case Client.SecondFloor:
                return secondFloorQuiz.IsExistQuiz(quiz);
            default:
                return false;
        }
    }
}
