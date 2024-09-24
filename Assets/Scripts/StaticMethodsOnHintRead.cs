using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;

public class StaticMethodsOnHintRead : MonoBehaviour
{
    public static async UniTask OnReadHintForSnowQuizUniTask()
    {
        Debug.Log("OnReadQuizHintUniTask");

        QuizPanel quizPanel = FindObjectOfType<QuizPanel>();
        if(quizPanel)
        {
            await quizPanel.MeltSnowUniTask();
        }
        else
        {
            Debug.LogError($"エラー：QuizPanelが存在しない状態で，雪が溶ける演出を実行しようとしています．");
        }
    }

    public static async UniTask OnReadHintBugForBugQuizUniTask()
    {
        Debug.Log("OnReadQuizBugHintUniTask");

        QuizPanel quizPanel = FindObjectOfType<QuizPanel>();
        if (quizPanel)
        {
            await quizPanel.EraseBugUniTask();
        }
        else
        {
            Debug.LogError($"エラー：QuizPanelが存在しない状態で，虫をスプレーで消す演出を実行しようとしています．");
        }
    }

    public static async UniTask OnReadHintSpeakerForTrafficJamQuizUniTask()
    {
        Debug.Log("OnReadQuizTrafficJamHintUniTask");
        Debug.Log("マウスクリックでスピーカーの再生を検知したことをクライアントに通知できるようになった！"); 
        await UniTask.Yield();
    }

    public static async UniTask OnReadHintSirenForTrafficJamQuizUniTask()
    {
        Debug.Log("フェードアウトしながらdogを表示する");

        QuizPanel quizPanel = FindObjectOfType<QuizPanel>();
        if (quizPanel)
        {
            await quizPanel.DisplayDogUnitask();
        }
        else
        {
            Debug.LogError($"エラー：QuizPanelが存在しない状態で，渋滞を遷移する演出を実行しようとしています．");
        }
    }
}
