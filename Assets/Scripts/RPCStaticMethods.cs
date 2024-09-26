using System;
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

    public static void QuizClear(string quizCardUUIDString)
    {
        CardID? cardID = DataBase.Instance.GetCardIDFromUUID(quizCardUUIDString);
        PhaseManager.Instance.QuizClear((CardID)cardID);
    }

    public static void SynchronizeTimer(DateTime dateTime)
    {
        TimeManager.Instance.SetStartTime(dateTime);
    }
}
