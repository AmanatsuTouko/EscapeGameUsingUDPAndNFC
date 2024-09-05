using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AnswerCardIDPairScriptableObject", order = 3)]
public class AnswerCardIDPairScriptableObject : ScriptableObject
{
    public List<QuestionAnswerPair> QuestionAnswerPairs;

    [System.NonSerialized]
    public Image TargetImage;

    // 引数に持つCardIDが持つ答えを取得する
    public CardID? GetAnswerCardID(CardID cardID)
    {
        // 存在する場合はカードIDを返す
        foreach(QuestionAnswerPair pair in QuestionAnswerPairs)
        {
            if(pair.QuestionCardID == cardID)
            {
                return pair.AnswerCardID;
            }
        }
        // 存在しない場合はnullを返す
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

        // cardIDに対応するSpriteでImageを更新する
        int cardIdx = (int)cardID;

        // Spriteのnullチェック
        if (QuestionAnswerPairs[cardIdx].Sprite == null)
        {
            Debug.LogError($"Error:{cardID}に対応するSpriteがnullです．");
            return;
        }
        if (QuestionAnswerPairs.Count <= cardIdx)
        {
            Debug.LogError($"Error:{cardID}に対応するSpriteが登録されていません．");
            return;
        }

        // ImageへSpriteを反映させる
        TargetImage.sprite = QuestionAnswerPairs[cardIdx].Sprite;
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