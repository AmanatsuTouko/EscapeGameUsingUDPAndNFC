using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UDPで受信した文字列をもとに関数を実行できるようにする
public class RPCManager : MonoBehaviour
{
    // サンプルコード

    // 実行する関数は，staticでpublicである必要がある
    // public static void SampleMethod(int a, float b)
    // {
    //     Debug.Log((a, b));
    // }

    // void Start()
    // {
    //     MethodArgs methodArgs = new MethodArgs()
    //     {
    //         NameSpaceAndClassName = GetType().Name,
    //         MethodName = nameof(SampleMethod),
    //         Args = new List<string>() {"1", "23.4"}
    //     };
    //     string json = JsonUtility.ToJson(methodArgs);
        
    //     DoMethodFromJson(json);
    // }

    // 実行する関数の名前と引数のデータ
    [System.Serializable]
    public class MethodArgs
    {
        [SerializeField]
        public string NameSpaceAndClassName;
        [SerializeField]
        public string MethodName;
        [SerializeField]
        public string[] Args;
    }

    public static void InvokeFromJson(string jsonMethodArgs)
    {
        // Jsonをデシリアライズする
        MethodArgs methodArgs = JsonUtility.FromJson<MethodArgs>(jsonMethodArgs);
        // 関数を実行する（staticでpublicな関数限定）
        Invoker.Invoke(methodArgs.NameSpaceAndClassName, methodArgs.MethodName, methodArgs.Args);
    }

    public static string GetJsonFromMethodArgs(string nameSpaceAndClassName, string methodName, string[] args)
    {
        MethodArgs methodArgs = new MethodArgs()
        {
            NameSpaceAndClassName = nameSpaceAndClassName,
            MethodName = methodName,
            Args = args
        };
        return JsonUtility.ToJson(methodArgs);
    }
}
