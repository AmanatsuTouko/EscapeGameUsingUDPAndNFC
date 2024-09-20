using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizPanel : MonoBehaviour, IActivable
{
    [SerializeField] Image BgPanel;
    [SerializeField] GameObject UniqueProcessPanel;
    [SerializeField] Image HintImage;

    [SerializeField] Image QuizImage;

    public void SetActive(bool active)
    {
        BgPanel.enabled = active;
        HintImage.enabled = active;
        UniqueProcessPanel.SetActive(active);
    }

    // クイズ画像の表示
    public void DisplayQuiz(Sprite quiz)
    {
        QuizImage.sprite = quiz;
    }

    // ヒント画像の表示
    public void DisplayHint(CardID quiz, Sprite hint)
    {
        HintImage.sprite = hint;

        // 画像を表示する座標とWidthHeightを取得する
        DataBase.Instance.GetHintDefaultPosSize(quiz, out Vector2 pos, out Vector2 size);

        // ImageのSprite,位置,座標を変更する
        HintImage.sprite = hint;
        HintImage.rectTransform.anchoredPosition = pos;
        HintImage.SetNativeSize();

        // 横幅に合わせて縦横比を維持する
        Vector2 nativeSize = HintImage.rectTransform.sizeDelta;
        float shrinkRate = size.x / nativeSize.x;
        HintImage.rectTransform.sizeDelta *= shrinkRate;
    }
}
