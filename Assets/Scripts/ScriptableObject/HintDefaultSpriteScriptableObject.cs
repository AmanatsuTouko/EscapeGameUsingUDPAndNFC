using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/HintDefaultSpriteScriptableObject", order = 5)]
public class HintDefaultSpriteScriptableObject : ScriptableObject
{
    public List<HintImagePair> HintImagePairs;

    [System.NonSerialized]
    public Image TargetImage;

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

    public void DisplayIncorrectHintImage(CardID quizCard, CardID hintCard)
    {
        // 画像を表示する座標とWidthHeightを取得する
        Vector2 size = Vector2.zero;
        Vector2 pos = Vector2.zero;
        DataBase.Instance.GetHintDefaultPosSize(quizCard, out pos, out size);

        if(size == Vector2.zero)
        {
            Debug.LogError($"Error:問題カード{quizCard}に，表示するヒント画像のサイズが設定されていません．");
        }
        if(pos == Vector2.zero)
        {
            Debug.LogError($"Error:問題カード{quizCard}に，表示するヒント画像の位置が設定されていません．");
        }

        // ImageのSpriteを取得
        Sprite sprite = GetSpriteFromCardID(hintCard);
        if(sprite == null)
        {
            Debug.LogError($"Error:問題カード{quizCard}に，表示するヒント画像{hintCard}の画像が設定されていません．");
        }

        // ImageのSprite,位置,座標を変更する
        TargetImage.sprite = sprite;
        TargetImage.rectTransform.anchoredPosition = pos;
        TargetImage.SetNativeSize();

        // 横幅に合わせて縦横比を維持する
        Vector2 nativeSize = TargetImage.rectTransform.sizeDelta;
        float shrinkRate = size.x / nativeSize.x;
        TargetImage.rectTransform.sizeDelta *= shrinkRate;
    }
}

// CardIDと表示する画像のPair
[System.Serializable]
public class HintImagePair
{
    public CardID HintCardID;
    public Sprite Sprite;
}
