using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AnswerPairScriptableObject", order = 4)]
public class AnswerPairScriptableObject : ScriptableObject
{
    public List<AnswerPair> AnswerPairs;

    public bool IsExistPair(CardID quiz, CardID hint, CardID answer)
    {
        foreach(var pair in AnswerPairs)
        {
            if(pair.QuizCardID == quiz && pair.HintCardID == hint && pair.AnswerCardID == answer)
            {
                return true;
            }
        }
        return false;
    }

    public CardID? GetCardIDFromAnswer(CardID answer)
    {
        foreach(var pair in AnswerPairs)
        {
            if(pair.AnswerCardID == answer)
            {
                return pair.QuizCardID;
            }
        }
        return null;
    }
}

// 問題カードとヒントカードに対応する解答カードと表示する画像のペア
[System.Serializable]
public class AnswerPair
{
    public CardID QuizCardID;
    public CardID HintCardID;
    public CardID AnswerCardID;
}