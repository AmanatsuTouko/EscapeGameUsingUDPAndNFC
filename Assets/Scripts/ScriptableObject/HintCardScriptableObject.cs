using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

    [System.NonSerialized]
    public Image TargetImage;
   
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

    public void DisplayQuestionImage(CardID questionCardID, CardID hintCardD)
    {
        // nullチェック
        if(TargetImage == null)
        {
            Debug.LogError($"Error:Spriteの反映先のImageが登録されていません．");
            return;
        }

        // 対応するクイズとヒントのペアに一致するSpriteを取得
        Sprite sprite = null;
        foreach(QuizHintImagePair pair in QuizHintImagePairs)
        {
            if(pair.QuizCardID == questionCardID && pair.HintCardID == hintCardD)
            {
                sprite = pair.Sprite;
                break;
            }
        }

        // Spriteのnullチェック
        if (sprite == null)
        {
            Debug.LogError($"Error:{questionCardID}の問題のヒントとなる{hintCardD}は設定されていません．");
            return;
        }

        // ImageへSpriteを反映させる
        TargetImage.sprite = sprite;
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
    public void OnReadQuizSnowHint()
    {
        Debug.Log("OnReadQuizSnowHint");
        StaticMethodsOnHintRead.OnReadQuizSnowHintUniTask().Forget();
    }

    public void OnReadQuizBugHint()
    {
        Debug.Log("OnReadQuizBugHint");
        StaticMethodsOnHintRead.OnReadQuizBugHintUniTask().Forget();
    }

    public void OnReadQuizTrafficJamHint()
    {
        Debug.Log("OnReadQuizTrafficJamHint");
        StaticMethodsOnHintRead.OnReadQuizTrafficJamHintUniTask().Forget();
    }

    public void OnReadSirenHintTrafficJam()
    {
        Debug.Log("OnReadSirenHintTrafficJam");
        StaticMethodsOnHintRead.OnReadSirenHintTafficJamUniTask().Forget();
    }
}

