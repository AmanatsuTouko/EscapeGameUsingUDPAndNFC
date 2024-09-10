using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/HintCardScriptableObject", order = 3)]
public class HintCardScriptableObject : ScriptableObject
{
    public List<QuizHintImagePair> QuizHintImagePairs;

    [System.NonSerialized]
    public Image TargetImage;
   
    public bool IsExistQuestionAnswerCardIDPair(CardID questionCardID, CardID answerCardD)
    {
        foreach(QuizHintImagePair pair in QuizHintImagePairs)
        {
            if(pair.QuizCardID == questionCardID && pair.HintCardID == answerCardD)
            {
                return true;
            }
        }
        return false;
    }

    public void DisplayQuestionImage(CardID questionCardID, CardID hintCardD)
    {
        // nullチェック
        if(TargetImage == null)
        {
            Debug.LogError($"Error:Spriteの反映先のImageが登録されていません．");
            return;
        }

        // 対応するクイズとヒントのペアに一致するSpriteを取得
        Sprite sprite = null;
        foreach(QuizHintImagePair pair in QuizHintImagePairs)
        {
            if(pair.QuizCardID == questionCardID && pair.HintCardID == hintCardD)
            {
                sprite = pair.Sprite;
                break;
            }
        }

        // Spriteのnullチェック
        if (sprite == null)
        {
            Debug.LogError($"Error:{questionCardID}の問題のヒントとなる{hintCardD}は設定されていません．");
            return;
        }

        // ImageへSpriteを反映させる
        TargetImage.sprite = sprite;
    }
}

// CardIDと表示する画像のPair
[System.Serializable]
public class QuizHintImagePair
{
    public CardID QuizCardID;
    public CardID HintCardID;
    public Sprite Sprite;
}