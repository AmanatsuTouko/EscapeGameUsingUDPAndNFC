using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

public class PhaseClearPanel : MonoBehaviour, IFadeable, IActivable
{
    [SerializeField] Image BgImage;
    [SerializeField] TextMeshProUGUI MainText;
    [SerializeField] TextMeshProUGUI SubText;

    [SerializeField] Color Phase1TextColor;
    [SerializeField] Color Phase2TextColor;
    [SerializeField] Color Phase3TextColor;

    public async UniTask FadeIn(float duration, Easing.Ease ease)
    {
        BgImage.FadeIn(duration, ease).Forget();
        MainText.FadeIn(duration, ease).Forget();
        SubText.FadeIn(duration, ease).Forget();
        await UniTask.WaitForSeconds(duration);
    }

    public async UniTask FadeOut(float duration, Easing.Ease ease)
    {
        BgImage.FadeOut(duration, ease).Forget();
        MainText.FadeOut(duration, ease).Forget();
        SubText.FadeOut(duration, ease).Forget();
        await UniTask.WaitForSeconds(duration);
    }

    public void SetActive(bool active)
    {
        BgImage.enabled = active;
        SetActiveTexts(active);
    }

    public void SetActiveTexts(bool active)
    {
        MainText.enabled = active;
        SubText.enabled = active;
    }

    // 表示したときの一覧の流れ
    public async UniTask Action(Phase clearedPhase)
    {
        Debug.Log($"フェーズ{clearedPhase}クリア!");

        // クリアしたフェーズによって処理を変える
        switch (clearedPhase)
        {
            case Phase.Phase1:
                await DisplayTextsOnAnimationUniTask("PHASE1 クリア!", "行動範囲が拡大された", Phase1TextColor, 
                    SE.ClearPhase);
                break;

            case Phase.Phase2:
                // 文字を表示して最終問題を出題
                await DisplayTextsOnAnimationUniTask("PHASE2 クリア!", "階段の行き来が\n可能になった", Phase2TextColor, 
                    SE.ClearPhase);
                UIManager.Instance.DisplayCircleClockQuiz();
                break;

            case Phase.Phase3:
                await DisplayTextsOnAnimationUniTask("脱出成功", "Congratulations!", Phase3TextColor, 
                    SE.ExitSuccess);
                break;
        }
    }

    private async UniTask DisplayTextsOnAnimationUniTask(string mainText, string subText, Color mainTextColor, SE se)
    {
        // 文字色を変更
        MainText.color = mainTextColor;
        MainText.text = "";
        SubText.text = "";

        // BGPanelを徐々に表示
        await BgImage.FadeIn(1.0f, Easing.Ease.InSine);
        
        // クリア音を鳴らす
        SoundManager.Instance.PlaySE(se);

        // 文字を点滅しながら出現
        MainText.text = mainText;
        await MainText.FadeIn(0.2f, Easing.Ease.InSine);
        await MainText.FadeOut(0.2f, Easing.Ease.InSine);
        await MainText.FadeIn(0.2f, Easing.Ease.InSine);
        await MainText.FadeOut(0.2f, Easing.Ease.InSine);
        await MainText.FadeIn(0.2f, Easing.Ease.InSine);

        // 横に流れるようにして，下の文字を表示する
        await SubText.FadeIn(0.1f, Easing.Ease.InQuad);
        await AddTextDynamic(SubText, subText);

        await UniTask.WaitForSeconds(5.0f);

        // BGPanelを徐々に非表示
        await MainText.FadeOut(0.2f, Easing.Ease.InSine);
        await SubText.FadeOut(0.2f, Easing.Ease.InSine);
        await BgImage.FadeOut(1.0f, Easing.Ease.InSine);
    }

    // 流れるようにメッセージを追加する
    private static async UniTask AddTextDynamic(TextMeshProUGUI textMesh, string addText)
    {
        textMesh.text = "";
        int textLen = addText.Length;
        int idx = 0;
        while (idx < textLen)
        {
            await UniTask.Yield(PlayerLoopTiming.Update);
            await UniTask.WaitForSeconds(0.2f);
            textMesh.text += addText[idx];
            idx += 1;
        }
    }
}
