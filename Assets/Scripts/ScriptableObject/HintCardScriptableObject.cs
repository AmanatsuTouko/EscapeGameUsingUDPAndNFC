using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

// CardIDと表示する画像のPair
[System.Serializable]
public class QuizHintImagePair
{
    public CardID QuizCardID;
    public CardID HintCardID;
    public Sprite Sprite;
    public UnityEvent UnityEvent; // Hintカード読み込み時に実行する関数を登録できるようにする
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/HintCardScriptableObject", order = 3)]
public class HintCardScriptableObject : ScriptableObject
{
    public List<QuizHintImagePair> QuizHintImagePairs;
   
    public bool IsExistQuestionAnswerCardIDPair(CardID questionCardID, CardID hintCardD)
    {
        foreach(QuizHintImagePair pair in QuizHintImagePairs)
        {
            if(pair.QuizCardID == questionCardID && pair.HintCardID == hintCardD)
            {
                return true;
            }
        }
        return false;
    }

    public Sprite GetSpriteFromCardID(CardID questionCardID, CardID hintCardD)
    {
        foreach(QuizHintImagePair pair in QuizHintImagePairs)
        {
            if(pair.QuizCardID == questionCardID && pair.HintCardID == hintCardD)
            {
                return pair.Sprite;
            }
        }
        return null;
    }

    public bool InvokeUniqueMethodIfPossible(CardID quizCardID, CardID answerCardD)
    {
         foreach(QuizHintImagePair pair in QuizHintImagePairs)
        {
            if(pair.QuizCardID == quizCardID && pair.HintCardID == answerCardD)
            {
                if(pair.UnityEvent != null)
                {
                    pair.UnityEvent.Invoke();
                    return true;
                }
            }
        }
        return false;
    }

    // 固有メソッドを呼び出すラッパー
    // 実態はStaticMethodsOnHintReadに記述する
    public void OnReadHintForSnowQuiz()
    {
        Debug.Log("OnReadHintForSnowQuiz");
        StaticMethodsOnHintRead.OnReadHintForSnowQuizUniTask().Forget();
    }

    public void OnReadHintBugForBugQuiz()
    {
        Debug.Log("OnReadHintBugForBugQuiz");
        StaticMethodsOnHintRead.OnReadHintBugForBugQuizUniTask().Forget();
    }

    public void OnReadHintSpeakerForTrafficJamQuiz()
    {
        Debug.Log("OnReadHintSpeakerForTrafficJamQuiz");
        StaticMethodsOnHintRead.OnReadHintSpeakerForTrafficJamQuizUniTask().Forget();
    }

    public void OnReadHintSirenForTrafficJamQuiz()
    {
        Debug.Log("OnReadHintSirenForTrafficJamQuiz");
        StaticMethodsOnHintRead.OnReadHintSirenForTrafficJamQuizUniTask().Forget();
    }
}

