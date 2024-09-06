using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;

public class UIManager : SingletonMonobehaviour<UIManager>
{
    // クイズ画像の表示
    [Header("Display Quiz")]
    [SerializeField] public Image QuizDisplayImage;
    [SerializeField] Slider ProgressBarSlider;

    // プログレスバーが上昇中かどうか
    private bool isUpdatingProgressBar = false;

    // 現在読み込んでいるCardID
    [SerializeField] CardID? currentDisplayCardID = null;

    [Header("Timer")]
    [SerializeField] TextMeshProUGUI limitTimeText;

    public async UniTask DisplayQuestionImageWithProgressBarUniTask(CardID cardID)
    {
        // 読み込み演出の途中で新たに読み込み演出をしないようにする
        if(isUpdatingProgressBar)
        {
            return;
        }
        isUpdatingProgressBar = true;

        // ImageをOFFにする
        DisplayImageSetActive(false);

        // 今読んだカードが現在読み込んでいる問題の答えの場合
        if( currentDisplayCardID != null 
            && GameManager.Instance.GetAnswerQuizCardIDsImagepair().IsExistQuestionAnswerCardIDPair((CardID)currentDisplayCardID, (CardID)cardID) )
        {
            // 新たな問題を表示する
            GameManager.Instance.GetAnswerQuizCardIDsImagepair().DisplayQuestionImage((CardID)currentDisplayCardID, (CardID)cardID);
        }
        else
        {
            // 答えではないので，単体で読み込んだときの処理を行う
            GameManager.Instance.GetCardIDImagePair().DisplayQuestionImage((CardID)cardID);
        }

        // 現在表示しているカードIDを更新する
        currentDisplayCardID = cardID;

        // イージングを繋ぐタイミングをややランダムにする
        float addSeconds_01 = UnityEngine.Random.Range(-0.1f, 0.1f);
        float addSeconds_02 = UnityEngine.Random.Range(-0.1f, 0.1f);

        // イージングの種類をランダムにする(弾性や振動を除く)
        int enumLength = Enum.GetNames(typeof(Easing.Ease)).Length - 9;
        int easeRandIdx_01 = UnityEngine.Random.Range(0, enumLength);
        int easeRandIdx_02 = UnityEngine.Random.Range(0, enumLength);
        int easeRandIdx_03 = UnityEngine.Random.Range(0, enumLength);

        // プログレスバーを3段階に分けて上昇させる
        UpdateProgressText("組成を分析中……");
        await EasingSecondsFromTo(1.5f, 0.0f,                  0.25f + addSeconds_01, (Easing.Ease)easeRandIdx_01);

        UpdateProgressText("物質を構成中……");
        await EasingSecondsFromTo(1.0f, 0.25f + addSeconds_01, 0.55f + addSeconds_02, (Easing.Ease)easeRandIdx_02);
        await UniTask.Delay(500);

        UpdateProgressText("生成物を検証中……");
        await EasingSecondsFromTo(1.5f, 0.55f + addSeconds_02, 1.0f,                  (Easing.Ease)easeRandIdx_03);

        // ImageをONにする
        DisplayImageSetActive(true);

        // リセット処理
        ProgressBarSlider.value = 0;

        isUpdatingProgressBar = false;
    }

    // N秒でaからbまでイージングを行う関数
    private async UniTask EasingSecondsFromTo(float seconds, float fromValue, float toValue, Easing.Ease easing)
    {
        Func<float, float> easingMethod = Easing.GetEasingMethod(easing);
        float rate = 0;
        float sub = toValue - fromValue;
        while (rate < 1.0f)
        {
            await UniTask.Yield();
            rate += Time.deltaTime / seconds;
            if (rate >= 1.0f) rate = 1.0f;
            ProgressBarSlider.value = fromValue + easingMethod(rate) * sub;
        }
    }

    private void UpdateProgressText(string text)
    {
        // プログレスバーに記載されている文字を変更する
    }

    // Imageのオンオフを行う
    public void DisplayImageSetActive(bool active)
    {
        QuizDisplayImage.enabled = active;
    }

    // 時刻の更新
    public void SetLimitTimeText(string text)
    {
        limitTimeText.text = text;
    }
}
