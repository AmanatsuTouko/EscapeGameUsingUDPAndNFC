using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

public class ProgressBarPanel : MonoBehaviour, IActivable
{
    [SerializeField] Image bgImage;
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI textMeshProUGUI;

    public void SetActive(bool active)
    {
        bgImage.enabled = active;
    }

    // カード読み込み時
    public async UniTask ActionOnRead(CancellationTokenSource cancellationTokenSource)
    {
        // イージングを繋ぐタイミングをややランダムにする
        float addSeconds_12 = UnityEngine.Random.Range(-0.1f, 0.1f);
        float addSeconds_23 = UnityEngine.Random.Range(-0.1f, 0.1f);
        float addSeconds_34 = UnityEngine.Random.Range(-0.1f, 0.1f);
        
        // イージングの種類をランダムにする(弾性や振動を除く)
        int enumLength = System.Enum.GetNames(typeof(Easing.Ease)).Length - 9;
        int easeRandIdx_01 = UnityEngine.Random.Range(0, enumLength);
        int easeRandIdx_02 = UnityEngine.Random.Range(0, enumLength);
        int easeRandIdx_03 = UnityEngine.Random.Range(0, enumLength);
        int easeRandIdx_04 = UnityEngine.Random.Range(0, enumLength);

        // プログレスバーを4段階に分けて上昇させる
        textMeshProUGUI.text = "機器を初期化中...";
        await slider.EasingSecondsFromTo(1.2f, 0.0f,                  0.20f + addSeconds_12, (Easing.Ease)easeRandIdx_01, cancellationTokenSource);

        textMeshProUGUI.text = "原子を生成中...";
        await slider.EasingSecondsFromTo(1.0f, 0.20f + addSeconds_12, 0.45f + addSeconds_23, (Easing.Ease)easeRandIdx_02, cancellationTokenSource);
        await UniTask.Delay(100);

        textMeshProUGUI.text = "電気を充填中...";
        await slider.EasingSecondsFromTo(1.2f, 0.45f + addSeconds_23, 0.65f + addSeconds_34, (Easing.Ease)easeRandIdx_03, cancellationTokenSource);

        textMeshProUGUI.text = "物体を転送中...";
        await slider.EasingSecondsFromTo(1.0f, 0.65f + addSeconds_34, 1.00f,                 (Easing.Ease)easeRandIdx_04, cancellationTokenSource);
        await UniTask.Delay(500);
    }

    // 正解カード読み込み時
    public async UniTask ActionCorrect(CancellationTokenSource cancellationTokenSource)
    {
        // イージングを繋ぐタイミングをややランダムにする
        float addSeconds_12 = UnityEngine.Random.Range(-0.1f, 0.1f);
        float addSeconds_23 = UnityEngine.Random.Range(-0.1f, 0.1f);
        float addSeconds_34 = UnityEngine.Random.Range(-0.1f, 0.1f);
        
        // イージングの種類をランダムにする(弾性や振動を除く)
        int enumLength = System.Enum.GetNames(typeof(Easing.Ease)).Length - 9;
        int easeRandIdx_01 = UnityEngine.Random.Range(0, enumLength);
        int easeRandIdx_02 = UnityEngine.Random.Range(0, enumLength);
        int easeRandIdx_03 = UnityEngine.Random.Range(0, enumLength);
        int easeRandIdx_04 = UnityEngine.Random.Range(0, enumLength);

        // プログレスバーを4段階に分けて上昇させる
        textMeshProUGUI.text = "機器を初期化中...";
        await slider.EasingSecondsFromTo(1.2f, 0.0f,                  0.20f + addSeconds_12, (Easing.Ease)easeRandIdx_01, cancellationTokenSource);

        textMeshProUGUI.text = "正当性を確認中...";
        await slider.EasingSecondsFromTo(1.0f, 0.20f + addSeconds_12, 0.45f + addSeconds_23, (Easing.Ease)easeRandIdx_02, cancellationTokenSource);
        await UniTask.Delay(100);

        textMeshProUGUI.text = "権限を確認中...";
        await slider.EasingSecondsFromTo(1.2f, 0.45f + addSeconds_23, 0.65f + addSeconds_34, (Easing.Ease)easeRandIdx_03, cancellationTokenSource);

        textMeshProUGUI.text = "電子錠を開錠中...";
        await slider.EasingSecondsFromTo(1.0f, 0.65f + addSeconds_34, 1.00f,                 (Easing.Ease)easeRandIdx_04, cancellationTokenSource);
        await UniTask.Delay(500);
    }
}
