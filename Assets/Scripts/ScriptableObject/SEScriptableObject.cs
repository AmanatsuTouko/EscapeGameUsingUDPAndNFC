using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SEScriptableObject", order = 6)]
public class SEScriptableObject : ScriptableObject
{
    [SerializeField]
    public List<SEData> SEs;

    public AudioClip GetAudioClipFromSE(SE seType)
    {
        foreach(var se in SEs)
        {
            if(se.SE == seType)
            {
                return se.AudioClip;
            }
        }
        Debug.LogError($"エラー：登録されていないSE：{seType}を再生しようとしています。");
        return null;
    }

    // SEの種類とAudioClip, 音量などを保持するクラス
    [System.Serializable]
    public class SEData
    {
        public SE SE;
        public AudioClip AudioClip;
        public float Volume = 1.0f;
    }
}