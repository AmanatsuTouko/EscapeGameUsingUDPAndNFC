using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public static class ExtensionMethods
{
    // FadeIn
    public static async UniTask FadeIn(Color color, float duration, Easing.Ease ease)
    {
        Func<float, float> easingMethod = Easing.GetEasingMethod(ease);

        float rate = 0;
        while (rate < 1.0f)
        {
            await UniTask.Yield();
            rate += Time.deltaTime / duration;
            if (rate >= 1.0f) rate = 1.0f;
            color.a = easingMethod(rate);
        }
    }
    // FadeOut
    public static async UniTask FadeOut(Color color, float duration, Easing.Ease ease)
    {
        Func<float, float> easingMethod = Easing.GetEasingMethod(ease);

        float rate = 0;
        while (rate < 1.0f)
        {
            await UniTask.Yield();
            rate += Time.deltaTime / duration;
            if (rate >= 1.0f) rate = 1.0f;

            color.a = easingMethod(1.0f - rate);
        }
    }

    // For Image
    public static async UniTask FadeIn(this UnityEngine.UI.Image image, float duration, Easing.Ease ease)
    {
        await FadeIn(image.color, duration, ease);
    }
    public static async UniTask FadeOut(this UnityEngine.UI.Image image, float duration, Easing.Ease ease)
    {
        await FadeOut(image.color, duration, ease);
    }

    // For Text
    public static async UniTask FadeIn(this TMPro.TextMeshProUGUI text, float duration, Easing.Ease ease)
    {
        await FadeIn(text.color, duration, ease);
    }
    public static async UniTask FadeOut(this TMPro.TextMeshProUGUI text, float duration, Easing.Ease ease)
    {
        await FadeOut(text.color, duration, ease);
    }
}
