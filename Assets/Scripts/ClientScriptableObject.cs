using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ClientScriptableObject", order = 1)]
public class CardIDImagePair : ScriptableObject
{
    public List<CardImagePair> cardImagePair;

    [System.NonSerialized]
    public Image TargetImage;

    private Sprite? GetSpriteFromCardID(CardID cardID)
    {
        foreach(CardImagePair pair in cardImagePair)
        {
            if(pair.cardID == cardID)
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
            Debug.LogError($"Error:{cardID}に対応するSpriteがnullか，{cardID}に対応するSpriteが登録されていません．．");
        }

        // ImageへSpriteを反映させる
        TargetImage.sprite = sprite;
    }
}

// CardIDと表示する画像のPair
[System.Serializable]
public class CardImagePair
{
    public CardID cardID;
    public Sprite Sprite;
}
