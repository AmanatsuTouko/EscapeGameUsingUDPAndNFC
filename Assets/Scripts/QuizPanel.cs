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

    private void Start()
    {
        // 特定の問題の場合，雪を降らせる
        if(UIManager.Instance.IsCurrentQuizSnow())
        {
            StartSnowParticle();
        }
    }

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
        // Spriteの設定
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

        // Hint画像の有効化
        HintImage.enabled = true;
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
    public async UniTask MeltSnowUniTask()
    {
        // 雪が溶ける前の画像を有効化する
        SnowFadeImage.enabled = true;
        // 上に重ねたUI画像の透明度を少しずつ下げてフェードアウトさせる
        await SnowFadeImage.FadeOut(4.0f, Easing.Ease.InCubic);
        // 画像を無効化する
        SnowFadeImage.enabled = false;
    }

    // 虫をスプレーで撃退する演出
    public async UniTask EraseBugUniTask()
    {
        // 毛虫の有効化
        BugImage.enabled = true;
        KillBugSprayImage.enabled = true;

        // 殺虫剤のみ透明にしておく
        BugImage.SetAlpha(255);
        KillBugSprayImage.SetAlpha(0);

        // 殺虫剤画像のフェードイン
        await KillBugSprayImage.FadeIn(2.0f, Easing.Ease.InCirc);
        await UniTask.WaitForSeconds(1.0f);

        // 毛虫画像をN秒かけてフェードアウト
        await BugImage.FadeOut(2.0f, Easing.Ease.InCubic);
        await UniTask.WaitForSeconds(1.0f);

        // 殺虫剤をN秒かけてフェードアウト
        await KillBugSprayImage.FadeOut(1.5f, Easing.Ease.OutCubic);

        // 毛虫, 殺虫剤の画像の無効化
        BugImage.enabled = false;
        KillBugSprayImage.enabled = false;
    }
}
