using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;

public class StaticMethodsOnHintRead : MonoBehaviour
{
    public static async UniTask OnReadQuizSnowHintUniTask()
    {
        Debug.Log("OnReadQuizHintUniTask");

        UIManager.Instance.IsUpdatingProgressBarFromExternal = true;
        Image image = UIManager.Instance.SnowFadeImage;
        image.enabled = true;

        // 上に重ねたUI画像の透明度を少しずつ下げてフェードアウトさせる
        
        Color color = image.color;
        Func<float, float> easingMethod = Easing.GetEasingMethod(Easing.Ease.InCubic);

        float rate = 0;
        float seconds = 4.0f;
        while (rate < 1.0f)
        {
            await UniTask.Yield();
            rate += Time.deltaTime / seconds;
            if (rate >= 1.0f) rate = 1.0f;

            color.a = easingMethod(1.0f - rate);
            image.color = color;
        }

        image.enabled = false;
        UIManager.Instance.IsUpdatingProgressBarFromExternal = false;
    }

    public static async UniTask OnReadQuizBugHintUniTask()
    {
        Debug.Log("OnReadQuizBugHintUniTask");
        UIManager.Instance.IsUpdatingProgressBarFromExternal = true;
        
        // 毛虫, 殺虫剤の画像の表示
        Image bug = UIManager.Instance.BugImage;
        Image spray = UIManager.Instance.KillBugSprayImage;
        bug.enabled = true;
        spray.enabled = true;
        // アルファ値のリセット
        ResetAlphaImage(bug);
        ResetAlphaImage(spray);

        // 殺虫剤画像のフェードイン
        await FadeIn(spray, 2.0f, Easing.Ease.InCirc);
        await UniTask.WaitForSeconds(1.0f);

        // 毛虫画像をN秒かけてフェードアウト
        // (音を鳴らす)
        await FadeOut(bug, 2.0f, Easing.Ease.InCubic);

        await UniTask.WaitForSeconds(1.0f);
        // 殺虫剤をN秒かけてフェードアウト
        await FadeOut(spray, 1.5f, Easing.Ease.OutCubic);

        bug.enabled = false;
        spray.enabled = false;
        UIManager.Instance.IsUpdatingProgressBarFromExternal = false;
    }

    // N秒かけてフェードアウトさせる
    private static async UniTask FadeOut(Image image, float seconds, Easing.Ease ease)
    {
        Color color = image.color;
        Func<float, float> easingMethod = Easing.GetEasingMethod(ease);

        float rate = 0;
        while (rate < 1.0f)
        {
            await UniTask.Yield();
            rate += Time.deltaTime / seconds;
            if (rate >= 1.0f) rate = 1.0f;

            color.a = easingMethod(1.0f - rate);
            image.color = color;
        }
    }

    // N秒かけてフェードインさせる
    private static async UniTask FadeIn(Image image, float seconds, Easing.Ease ease)
    {
        Color color = image.color;
        Func<float, float> easingMethod = Easing.GetEasingMethod(ease);

        float rate = 0;
        while (rate < 1.0f)
        {
            await UniTask.Yield();
            rate += Time.deltaTime / seconds;
            if (rate >= 1.0f) rate = 1.0f;

            color.a = easingMethod(rate);
            image.color = color;
        }
    }

    private static void ResetAlphaImage(Image image)
    {
        Color color = image.color;
        color.a = 1.0f;
        image.color = color;
    }

    public static async UniTask OnReadQuizTrafficJamHintUniTask()
    {
        Debug.Log("OnReadQuizTrafficJamHintUniTask");
        Debug.Log("マウスクリックでスピーカーの再生を検知したことをクライアントに通知できるようになった！"); 
    }

    public static async UniTask OnReadSirenHintTafficJamUniTask()
    {
        Debug.Log("フェードアウトしながらdogを表示する");

        UIManager.Instance.IsUpdatingProgressBarFromExternal = true;
        Image image = UIManager.Instance.DogFadeImageForQuizTrafficJam;
        image.enabled = true;

        // 上に重ねたUI画像の透明度を少しずつ下げてフェードアウトさせる
        await FadeOut(image, 4.0f, Easing.Ease.InCubic);

        // リセット処理
        image.enabled = false;
        ResetAlphaImage(image);
        UIManager.Instance.IsUpdatingProgressBarFromExternal = false;
    }
}
