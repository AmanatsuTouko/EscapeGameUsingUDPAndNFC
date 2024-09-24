using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public static class ExtensionMethods
{
    // For Image
    public static async UniTask FadeIn(this UnityEngine.UI.Image image, float duration, Easing.Ease ease)
    {
        Func<float, float> easingMethod = Easing.GetEasingMethod(ease);

        float rate = 0;
        while (rate < 1.0f)
        {
            await UniTask.Yield();
            rate += Time.deltaTime / duration;
            if (rate >= 1.0f) rate = 1.0f;

            Color color = image.color;
            color.a = easingMethod(rate);
            image.color = color;
        }
    }
    public static async UniTask FadeOut(this UnityEngine.UI.Image image, float duration, Easing.Ease ease)
    {
        Func<float, float> easingMethod = Easing.GetEasingMethod(ease);

        float rate = 0;
        while (rate < 1.0f)
        {
            await UniTask.Yield();
            rate += Time.deltaTime / duration;
            if (rate >= 1.0f) rate = 1.0f;

            Color color = image.color;
            color.a = easingMethod(1.0f - rate);
            image.color = color;
        }
    }
    // アルファ値の変更
    public static void SetAlpha(this UnityEngine.UI.Image image, float alpha)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
    }

    // For Text
    public static async UniTask FadeIn(this TMPro.TextMeshProUGUI text, float duration, Easing.Ease ease)
    {
        Func<float, float> easingMethod = Easing.GetEasingMethod(ease);

        float rate = 0;
        while (rate < 1.0f)
        {
            await UniTask.Yield();
            rate += Time.deltaTime / duration;
            if (rate >= 1.0f) rate = 1.0f;

            Color color = text.color;
            color.a = easingMethod(rate);
            text.color = color;
        }
    }
    public static async UniTask FadeOut(this TMPro.TextMeshProUGUI text, float duration, Easing.Ease ease)
    {
        Func<float, float> easingMethod = Easing.GetEasingMethod(ease);

        float rate = 0;
        while (rate < 1.0f)
        {
            await UniTask.Yield();
            rate += Time.deltaTime / duration;
            if (rate >= 1.0f) rate = 1.0f;

            Color color = text.color;
            color.a = easingMethod(1.0f - rate);
            text.color = color;
        }
    }

    // For Slider
    public static async UniTask EasingSecondsFromTo(
        this UnityEngine.UI.Slider slider, 
        float seconds, 
        float fromValue, 
        float toValue, 
        Easing.Ease easing, 
        System.Threading.CancellationTokenSource cancellationTokenSource
        )
    {
        Func<float, float> easingMethod = Easing.GetEasingMethod(easing);
        float rate = 0;
        float sub = toValue - fromValue;
        while (rate < 1.0f)
        {
            await UniTask.Yield(cancellationTokenSource.Token);            
            rate += Time.deltaTime / seconds;
            if (rate >= 1.0f) rate = 1.0f;
            slider.value = fromValue + easingMethod(rate) * sub;
        }
    }
}
