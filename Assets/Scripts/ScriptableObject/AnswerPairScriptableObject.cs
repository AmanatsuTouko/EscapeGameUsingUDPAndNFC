using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AnswerPairScriptableObject", order = 4)]
public class AnswerPairScriptableObject : ScriptableObject
{
    public List<AnswerPair> AnswerPairs;

    [System.NonSerialized]
    public Image TargetImage;

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

    public void DisplayImage(CardID quiz, CardID hint, CardID answer)
    {
        // nullチェック
        if(TargetImage == null)
        {
            Debug.LogError($"Error:Spriteの反映先のImageが登録されていません．");
            return;
        }

        // 対応するクイズとヒントのペアに一致するSpriteを取得
        Sprite sprite = null;
        foreach(var pair in AnswerPairs)
        {
            if(pair.QuizCardID == quiz && pair.HintCardID == hint && pair.AnswerCardID == answer)
            {
                sprite = pair.Sprite;
                break;
            }
        }

        // Spriteのnullチェック
        if (sprite == null)
        {
            Debug.LogError($"Error:{quiz}の問題のヒント{hint}に対応する解答{answer}は設定されていません．");
            return;
        }

        // ImageへSpriteを反映させる
        TargetImage.sprite = sprite;
    }
}

// 問題カードとヒントカードに対応する解答カードと表示する画像のペア
[System.Serializable]
public class AnswerPair
{
    public CardID QuizCardID;
    public CardID HintCardID;
    public CardID AnswerCardID;
    public Sprite Sprite;
}