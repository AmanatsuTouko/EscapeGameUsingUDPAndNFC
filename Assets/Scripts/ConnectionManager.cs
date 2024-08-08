using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using UnityEditor;

public class ConnectionManager : MonoBehaviour
{
    ConnectionBroadcast connection;

    async UniTaskVoid Start()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

        connection = new ConnectionBroadcast();
        connection.InitUdpclients();

        // 別スレッドでブロードキャストを監視する
        UniTask.RunOnThreadPool(connection.ListenBroadcastMessage);
    }

    void Update()
    {
        // キー入力でメッセージを送信する
        if (Input.GetKeyDown(KeyCode.P))
        {
            string message = $"{Environment.UserName}からMessageSampleを送信";
            Debug.Log($"「{message}」という文字列を送信！");
            connection.SendBroadcastMessage(message);
        }
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // エディタのプレイモードが終了したとき
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            Debug.Log("プレイモードが終了しました。");
            // UdpClientのListen状態をCloseする
            connection.ListenClose();
        }
    }

    private void MyFunction()
    {
        Debug.Log("プレイモードが終了しました。");
        // ここに実行したい関数の内容を書きます。
        connection.ListenClose();
    }
}
