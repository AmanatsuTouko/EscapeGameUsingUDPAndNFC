using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class RotationCard : MonoBehaviour
{
    [Header("Rotation Details")]
    [SerializeField] float secondForOneRotate = 5.0f;
    [SerializeField] Easing.Ease ease = Easing.Ease.InOutSine;

    private float ratio = 0;

    void Update()
    {
        SimpleRotation();   
    }

    void Start()
    {
        // RotateSequenceUnitask().Forget();
    }

    private void SimpleRotation()
    {
        // 回転割合の増加
        ratio += Time.deltaTime / secondForOneRotate;
        if(ratio >= 1)
        {
            ratio -= 1;
        }

        // イージングを適用した角度を取得して、回転させる
        var easingMethod = Easing.GetEasingMethod(ease);
        float angle = easingMethod(ratio) * 360;
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    private async UniTask RotateSequenceUnitask(CancellationToken cancellationToken = default)
    {
        while(!cancellationToken.IsCancellationRequested)
        {
            await RotateUnitask(0.5f, 90f, Easing.Ease.Linear, cancellationToken);
            await RotateUnitask(1.5f, 540f, ease, cancellationToken);
        }
    }
    
    private async UniTask RotateUnitask(float duration = 1.0f, float angle = 360f, Easing.Ease ease = Easing.Ease.Linear, CancellationToken cancellationToken = default)
    {
        float ratio = 0;
        float initAngle = transform.rotation.eulerAngles.y;
        var easingMethod = Easing.GetEasingMethod(ease);
        
        // 指定したイージングと秒数で回転せる
        while(true)
        {
            await UniTask.Yield(cancellationToken);
            ratio += Time.deltaTime / duration;
            float nowAngle = initAngle + easingMethod(ratio) * angle;
            transform.rotation = Quaternion.Euler(0, nowAngle, 0);

            if(ratio >= 1)
            {
                transform.rotation = Quaternion.Euler(0, initAngle + angle, 0);
                break;
            }
        }
    }
}
