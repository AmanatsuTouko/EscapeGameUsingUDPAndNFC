using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

public class SendingBarPanel : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI textMeshProUGUI;

    // UniTaskのキャンセル用トークン
    public CancellationTokenSource cancellationTokenSource { get; private set; }

    private void Awake()
    {
        slider.value = 0;
        textMeshProUGUI.text = "";
    }

    public void SetActive(bool active)
    {
        slider.enabled = active;
        textMeshProUGUI.enabled = active;
    }

    // カード読み込み時
    public async UniTask ActionOnSend()
    {
        // キャンセル用トークンソースの初期化
        InitializeCancellationTokenSource();

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
        textMeshProUGUI.text = "送信中...";
        await slider.EasingSecondsFromTo(1.2f, 0.0f, 0.20f + addSeconds_12, (Easing.Ease)easeRandIdx_01, cancellationTokenSource);
        await slider.EasingSecondsFromTo(1.0f, 0.20f + addSeconds_12, 0.45f + addSeconds_23, (Easing.Ease)easeRandIdx_02, cancellationTokenSource);
        await slider.EasingSecondsFromTo(1.2f, 0.45f + addSeconds_23, 0.65f + addSeconds_34, (Easing.Ease)easeRandIdx_03, cancellationTokenSource);
        await slider.EasingSecondsFromTo(1.0f, 0.65f + addSeconds_34, 1.00f, (Easing.Ease)easeRandIdx_04, cancellationTokenSource);
        await UniTask.Delay(500);

        // カードを早めに離さないようにするためにやや余裕を設ける(UDP通信のラグもあるため)
        await UniTask.Delay(500);

        // 送信完了を表示
        textMeshProUGUI.text = "送信完了!";
        await UniTask.Delay(1000);
    }

    // トークンソースを開放して初期化する
    public void InitializeCancellationTokenSource()
    {
        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Dispose();
        }
        // CancellationTokenSource を初期化
        cancellationTokenSource = new CancellationTokenSource();
    }

    public void CancelProgressBar()
    {
        cancellationTokenSource.Cancel();
    }
}
