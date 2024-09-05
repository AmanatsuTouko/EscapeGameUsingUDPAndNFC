using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ClientScriptableObject", order = 1)]
public class ClientScriptableObject : ScriptableObject
{
    public List<CardImagePair> cardImagePair;

    [System.NonSerialized]
    public Image image;

    public void DisplayQuestionImage(CardID cardID)
    {
        // nullチェック
        if(image == null)
        {
            Debug.LogError($"Error:Spriteの反映先のImageが登録されていません．");
            return;
        }

        // cardIDに対応するSpriteでImageを更新する
        int cardIdx = (int)cardID;

        // Spriteのnullチェック
        if (cardImagePair[cardIdx].Sprite == null)
        {
            Debug.LogError($"Error:{cardID}に対応するSpriteがnullです．");
            return;
        }
        if (cardImagePair.Count <= cardIdx)
        {
            Debug.LogError($"Error:{cardID}に対応するSpriteが登録されていません．");
            return;
        }

        // ImageへSpriteを反映させる
        image.sprite = cardImagePair[cardIdx].Sprite;
    }
}

// CardIDと表示する画像のPair
[System.Serializable]
public class CardImagePair
{
    public CardID cardID;
    public Sprite Sprite;
}
