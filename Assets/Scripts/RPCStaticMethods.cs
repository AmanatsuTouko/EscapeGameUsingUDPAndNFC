using UnityEngine;

// RPCで実行される関数(publicでstaticでなければならない)
public class RPCStaticMethods : MonoBehaviour
{
    public static void DisplayQuestionImage(string uuidString)
    {
        GameManager.Instance.DisplayQuestionImage(uuidString);
    }
}
