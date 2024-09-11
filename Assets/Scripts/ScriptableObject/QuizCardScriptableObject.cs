using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/QuizCardScriptableObject", order = 2)]
public class QuizCardScriptableObject : ScriptableObject
{
    public List<QuizImagePair> QuizImagePairs;

    [System.NonSerialized]
    public Image TargetImage;

    private Sprite? GetSpriteFromCardID(CardID cardID)
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

    public void DisplayQuestionImage(CardID cardID)
    {
        // nullチェック
        if(TargetImage == null)
        {
            Debug.LogError($"Error:Spriteの反映先のImageが登録されていません．");
            return;
        }

        Sprite? sprite = GetSpriteFromCardID(cardID);
        if(sprite == null)
        {
            Debug.LogError($"Error:{cardID}に対応するSpriteがnullか，{cardID}に対応するSpriteが登録されていません．");
        }

        // ImageへSpriteを反映させる
        TargetImage.sprite = sprite;
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
