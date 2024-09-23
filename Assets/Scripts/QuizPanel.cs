using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class QuizPanel : MonoBehaviour, IActivable
{
    [SerializeField] Image BgPanel;
    [SerializeField] Image QuizImage;

    [SerializeField] GameObject UniqueProcessPanel;
    [SerializeField] Image HintImage;

    [Header("For Unique Method")]
    [SerializeField] GameObject SnowParticlePrefab;
    private GameObject snow;

    public Image SnowFadeImage;
    public Image BugImage;
    public Image KillBugSprayImage;
    public Image DogFadeImageForQuizTrafficJam;

    public void SetActive(bool active)
    {
        BgPanel.enabled = active;
        HintImage.enabled = active;
        UniqueProcessPanel.SetActive(active);
    }

    // クイズ画像の表示
    public void DisplayQuiz(Sprite quiz)
    {
        QuizImage.sprite = quiz;
    }

    // ヒント画像の表示
    public void DisplayHint(CardID quiz, Sprite hint)
    {
        HintImage.sprite = hint;

        // 画像を表示する座標とWidthHeightを取得する
        DataBase.Instance.GetHintDefaultPosSize(quiz, out Vector2 pos, out Vector2 size);

        // ImageのSprite,位置,座標を変更する
        HintImage.sprite = hint;
        HintImage.rectTransform.anchoredPosition = pos;
        HintImage.SetNativeSize();

        // 横幅に合わせて縦横比を維持する
        Vector2 nativeSize = HintImage.rectTransform.sizeDelta;
        float shrinkRate = size.x / nativeSize.x;
        HintImage.rectTransform.sizeDelta *= shrinkRate;
    }

    // 雪を降らせる
    public void StartSnowParticle()
    {
        snow = Instantiate(SnowParticlePrefab);
    }

    // 削除時に雪も削除する
    private void OnDestroy()
    {
        Destroy(snow);
    }

    // 雪が溶けていく様子を演出する
    private async UniTask MeltSnowUniTask()
    {
        await UniTask.Yield();
    }
}
