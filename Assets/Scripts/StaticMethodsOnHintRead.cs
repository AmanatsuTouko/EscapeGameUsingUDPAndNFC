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
        
        // 殺虫剤の画像の表示
        
        // 音を出しながら毛虫を消す

        // 殺虫剤の画像の削除（フェードアウトさせる）

    }

    public static async UniTask OnReadQuizTrafficJamHintUniTask()
    {
        Debug.Log("OnReadQuizTrafficJamHintUniTask");

        // フェードアウトして"dog"の画像を表示させる
        
    }
}
