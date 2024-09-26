using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 設定した音を鳴らせるようにする
public class DebugSound : MonoBehaviour
{
    [SerializeField] SE testSE;

    public void PlaySE()
    {
        SoundManager.Instance.PlaySE(testSE);
    }
}
