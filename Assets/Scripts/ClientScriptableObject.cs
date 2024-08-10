using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ClientScriptableObject", order = 1)]
public class ClientScriptableObject : ScriptableObject
{
    public List<CardImagePair> cardImagePair;

    public void DisplayQuestionImage(CardID cardID)
    {
        switch (cardID)
        {
            case CardID.ID01:
                break;
            case CardID.ID02:
                break;
            case CardID.ID03:
                break;
            default:
                Debug.LogError($"Error:{cardID}に有効な実装がありません．");
                break;
        }
    }
}

// CardIDと表示する画像のPair
[System.Serializable]
public class CardImagePair
{
    public CardID cardID;
    public Image Image;
}
