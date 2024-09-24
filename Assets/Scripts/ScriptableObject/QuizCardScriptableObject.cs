using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/QuizCardScriptableObject", order = 2)]
public class QuizCardScriptableObject : ScriptableObject
{
    public List<QuizImagePair> QuizImagePairs;

    public Sprite GetSpriteFromCardID(CardID cardID)
    {
        foreach(QuizImagePair pair in QuizImagePairs)
        {
            if(pair.QuizCardID == cardID)
            {
                return pair.Sprite;
            }
        }
        return null;
    }

    private QuizImagePair GetQuizCard(CardID cardID)
    {
        foreach (QuizImagePair pair in QuizImagePairs)
        {
            if (pair.QuizCardID == cardID)
            {
                return pair;
            }
        }
        return null;
    }

    public void GetHintDefaultPosSize(CardID quizCard, out Vector2 pos, out Vector2 size)
    {
        // クイズカードを取得する
        QuizImagePair quiz = GetQuizCard(quizCard);
        if(quiz == null)
        {
            Debug.LogError($"Error:QuizCardID:{quizCard}が存在しません．");
            pos = Vector2.zero;
            size = Vector2.zero;
        }
        pos = quiz.HintDefaultPos;
        size = quiz.HintDefaultSize;
    }

    public bool IsExistQuiz(CardID quizCard)
    {
        if(GetQuizCard(quizCard) != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

// CardIDと表示する画像のPair
[System.Serializable]
public class QuizImagePair
{
    public CardID QuizCardID;
    public Sprite Sprite;
    public Vector2 HintDefaultPos;  // ヒント画像の中心位置
    public Vector2 HintDefaultSize; // ヒント画像のWidth/Height
}
