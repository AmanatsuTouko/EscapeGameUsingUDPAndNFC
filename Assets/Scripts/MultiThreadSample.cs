using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

public class MultiThreadSample : MonoBehaviour
{
    ConnectionBroadcast connectionBroadcast;

    // (小ネタ: Start()メソッドはUniTaskVoidにしても動く)
    async UniTaskVoid Start()
    {
        Debug.Log($"実行前のスレッド={Thread.CurrentThread.ManagedThreadId}");

        // スレッドプール上で処理を実行する
        //await UniTask.RunOnThreadPool(SampleMethod);
        await UniTask.RunOnThreadPool(connectionBroadcast.ListenBroadcastMessage);

        Debug.Log($"実行完了後のスレッド={Thread.CurrentThread.ManagedThreadId}");
    }

    private void SampleMethod()
    {
        Debug.Log($"{nameof(SampleMethod)}を実行中:スレッド={Thread.CurrentThread.ManagedThreadId}");
        // ちょっと待つ
        Thread.Sleep(1000);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("あ！");
        }
    }
}
