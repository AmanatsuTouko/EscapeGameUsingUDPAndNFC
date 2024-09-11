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

    // Imageの参照の設定
    private void Start()
    {
        firstFloorQuiz.TargetImage = UIManager.Instance.QuizDisplayImage;
        firstFloorHint.TargetImage = UIManager.Instance.QuizDisplayImage;
        firstFloorAnswer.TargetImage = UIManager.Instance.QuizDisplayImage;

        secondFloorQuiz.TargetImage = UIManager.Instance.QuizDisplayImage;
        secondFloorHint.TargetImage = UIManager.Instance.QuizDisplayImage;
        secondFloorAnswer.TargetImage = UIManager.Instance.QuizDisplayImage;

        hintDefaultSprite.TargetImage = UIManager.Instance.DefaultHintImage;
    }

    // 他のクラスから呼び出す関数

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

    public void DisplayQuiz(CardID cardID)
    {
        GetQuiz().DisplayQuestionImage(cardID);
    }
    public void DisplayQuizAddedHint(CardID quiz, CardID hint)
    {
        GetHint().DisplayQuestionImage(quiz, hint);
    }
    public void DisplayQuizWrongHint(CardID quiz, CardID wrongHint)
    {
        hintDefaultSprite.DisplayIncorrectHintImage(quiz, wrongHint);
    }

    public void InvokeMethodOnHintDisplay(CardID quiz, CardID hint)
    {
        GetHint().InvokeUniqueMethodIfPossible(quiz, hint);
    }
}
