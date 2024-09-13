using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(DebugInput))] // 拡張するクラスを指定する
public class DebugInputEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 元のInspector部分を表示
        base.OnInspectorGUI();

        // targetを変換して拡張対象クラスを取得
        DebugInput debugInput = target as DebugInput;

        GUILayout.Label("Buttons");
        // ボタンを表示する
        if(GUILayout.Button("Read Test Card"))
        {
            debugInput.Read();
        }
        if(GUILayout.Button("Send Test Card"))
        {
            debugInput.SendReadedCard();
        }
        if(GUILayout.Button("Recieve Test Card"))
        {
            debugInput.RecieveReadedCard();
        }

        GUILayout.Label("Clear Debug");
        if(GUILayout.Button("ClearQuiz of Test Card"))
        {
            debugInput.ClearQuiz();
        }
    }
}
#endif