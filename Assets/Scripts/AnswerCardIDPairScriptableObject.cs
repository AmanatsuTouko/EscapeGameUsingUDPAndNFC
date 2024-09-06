using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AnswerCardIDPairScriptableObject", order = 3)]
public class AnswerQuizCardIDsImagepair : ScriptableObject
{
    public List<QuestionAnswerPair> QuestionAnswerPairs;

    [System.NonSerialized]
    public Image TargetImage;
   
    public bool IsExistQuestionAnswerCardIDPair(CardID questionCardID, CardID answerCardD)
    {
        foreach(QuestionAnswerPair pair in QuestionAnswerPairs)
        {
            if(pair.QuestionCardID == questionCardID && pair.AnswerCardID == answerCardD)
            {
                return true;
            }
        }
        return false;
    }

    public void DisplayQuestionImage(CardID questionCardID, CardID answerCardD)
    {
        // nullチェック
        if(TargetImage == null)
        {
            Debug.LogError($"Error:Spriteの反映先のImageが登録されていません．");
            return;
        }

        // 対応する問題と解答のペアに一致するSpriteを取得
        Sprite sprite = null;
        foreach(QuestionAnswerPair pair in QuestionAnswerPairs)
        {
            if(pair.QuestionCardID == questionCardID && pair.AnswerCardID == answerCardD)
            {
                sprite = pair.Sprite;
                break;
            }
        }

        // Spriteのnullチェック
        if (sprite == null)
        {
            Debug.LogError($"Error:{questionCardID}の問題の答えとなる{answerCardD}は設定されていません．");
            return;
        }

        // ImageへSpriteを反映させる
        TargetImage.sprite = sprite;
    }
}

// CardIDと表示する画像のPair
[System.Serializable]
public class QuestionAnswerPair
{
    public CardID QuestionCardID;
    public CardID AnswerCardID;
    public Sprite Sprite;
}