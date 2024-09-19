using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

public class CorrectPanel : MonoBehaviour, IFadeable, IActivable
{
    [SerializeField] Image bgImage;
    [SerializeField] List<TextMeshProUGUI> textMeshProUGUIs;

    public async UniTask FadeIn(float duration, Easing.Ease ease)
    {
        bgImage.FadeIn(duration, ease);
        foreach(var text in textMeshProUGUIs)
        {
            text.FadeIn(duration, ease);
        }
        await UniTask.WaitForSeconds(duration);
    }

    public async UniTask FadeOut(float duration, Easing.Ease ease)
    {
        bgImage.FadeOut(duration, ease);
        foreach (var text in textMeshProUGUIs)
        {
            text.FadeOut(duration, ease);
        }
        await UniTask.WaitForSeconds(duration);
    }

    public void SetActive(bool active)
    {
        bgImage.enabled = active;
        SetActiveTexts(active);
    }

    public void SetActiveTexts(bool active)
    {
        foreach(var text in textMeshProUGUIs)
        {
            text.enabled = active;
        }
    }

    // 表示したときの一覧の流れ
    public async UniTask Action()
    {
        // 2秒待つ
        await UniTask.WaitForSeconds(2.0f);
        // フェードアウトしながらメイン画面に戻る
        SetActiveTexts(false);
        await FadeOut(2.0f, Easing.Ease.InQuad);
    }
}
