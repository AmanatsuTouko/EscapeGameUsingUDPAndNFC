using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(DebugSound))] // 拡張するクラスを指定する
public class DebugSoundEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 元のInspector部分を表示
        base.OnInspectorGUI();

        // targetを変換して拡張対象クラスを取得
        DebugSound debugSound = target as DebugSound;

        GUILayout.Label("SE");
        // ボタンを表示する
        if(GUILayout.Button("Sound Test SE"))
        {
            debugSound.PlaySE();
        }
    }
}
#endif