using UnityEngine;

// RPCで実行される関数(publicでstaticでなければならない)
public class RPCStaticMethods : MonoBehaviour
{
    public static void DisplayQuestionImage(string uuidString)
    {
        GameManager.Instance.DisplayQuestionImage(uuidString);
    }

    public static void DisableQuizPanelIfWhileReadCard()
    {
        GameManager.Instance.DisableQuizPanelIfWhileReadCard();
    }

    public static void QuizClear(CardID cardID)
    {
        PhaseManager.Instance.QuizClear(cardID);
    }
}
