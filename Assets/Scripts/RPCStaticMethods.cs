using UnityEngine;

// RPCで実行される関数(publicでstaticでなければならない)
public class RPCStaticMethods : MonoBehaviour
{
    public static void DisplayQuestionImage(string uuidString)
    {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.DisplayQuestionImage(uuidString);
    }
}
