using Cysharp.Threading.Tasks;

interface IFadeable
{
    UniTask FadeIn(float duration, Easing.Ease ease);
    UniTask FadeOut(float duration, Easing.Ease ease);
}
