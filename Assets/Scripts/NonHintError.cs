using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

public class NonHintError : MonoBehaviour, IFadeable, IActivable
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
        foreach(var text in textMeshProUGUIs)
        {
            text.enabled = active;
        }
    }
}
