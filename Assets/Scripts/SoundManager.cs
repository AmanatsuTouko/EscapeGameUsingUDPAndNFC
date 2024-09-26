using System.Collections.Generic;
using UnityEngine;

// 一部参考：https://i-school.memo.wiki/d/SoundManager%A4%C7%A5%B2%A1%BC%A5%E0%C6%E2%A4%CE%B2%BB%B8%BB%A4%F2%B4%C9%CD%FD%A4%B9%A4%EB
public class SoundManager : SingletonMonobehaviour<SoundManager>
{
    // AudioClipの種類を記述する
    // 最下に追加していくこと（InspectorのListがズレてしまうため）
    public enum SE
    {
        OnReadCard,
        OnCancelRead,
        ErrorNoHint,
        ClearQuiz,
        ClearPhase,
    }

    [Range(0, 1)]
    public float SEMasterVolume = 1.0f;
    public bool Mute = false;

    public List<SEData> SEDatas;
    private AudioSource[] SE_Sources = new AudioSource[16];

    public override void Awake()
    {
        base.Awake();
        // SE用 AudioSource追加
        for (int i = 0; i < SE_Sources.Length; i++)
        {
            SE_Sources[i] = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update() 
    {
        foreach (AudioSource source in SE_Sources)
        {
            source.volume = SEMasterVolume;
        }
    }
    
    public void PlaySE(SE se)  
    {
        AudioClip audioClip = GetAudioClipFromSE(se);
        if(audioClip == null)
        {
            return;
        }

        // 再生中ではないAudioSourceをつかってSEを鳴らす
        foreach (AudioSource source in SE_Sources) 
        {
            // 再生中の AudioSource の場合には次のループ処理へ移る
            if (source.isPlaying) 
            {
                continue;
            }
            // 再生中でない AudioSource に Clip をセットして SE を鳴らす
            source.clip = audioClip;
            source.Play();
            break;
        }
    }

    public void StopSE() 
    {
        // 全てのSE用のAudioSourceを停止する
        foreach (AudioSource source in SE_Sources) 
        {
            source.Stop();
            source.clip = null;
        }
    }

    private AudioClip GetAudioClipFromSE(SE se)
    {
        foreach(var ses in SEDatas)
        {
            if(ses.SE == se)
            {
                return ses.AudioClip;
            }
        }
        Debug.LogError($"エラー：登録されていないSE：{se}を再生しようとしています。");
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

