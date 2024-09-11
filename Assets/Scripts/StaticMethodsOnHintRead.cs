using Cysharp.Threading.Tasks;
using UnityEngine;

public class StaticMethodsOnHintRead : MonoBehaviour
{
    public static async UniTask OnReadQuiz03HintUniTask()
    {
        Debug.Log("OnReadQuiz03Hint");
        UIManager.Instance.IsUpdatingProgressBarFromExternal = true;

        // 上に重ねたUI画像の透明度を少しずつ下げてフェードアウトさせる
        

        UIManager.Instance.IsUpdatingProgressBarFromExternal = false;
    }
}
