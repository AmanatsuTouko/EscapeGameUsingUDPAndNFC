using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/HintDefaultSpriteScriptableObject", order = 5)]
public class HintDefaultSpriteScriptableObject : ScriptableObject
{
    public List<HintImagePair> HintImagePairs;

    private Sprite? GetSpriteFromCardID(CardID cardID)
    {
        foreach (HintImagePair pair in HintImagePairs)
        {
            if (pair.HintCardID == cardID)
            {
                return pair.Sprite;
            }
        }
        return null;
    }
}

// CardIDと表示する画像のPair
[System.Serializable]
public class HintImagePair
{
    public CardID HintCardID;
    public Sprite Sprite;
}
