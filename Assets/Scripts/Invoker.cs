using System.ComponentModel;
using System.Linq;
using System.Reflection;

// https://qiita.com/block/items/3773c01003ee12f49761
// C#で型名もメソッド名も引数も文字列で指定して呼び出す

/*
サンプルコード
以下のように使える

namespace SampleNamespace
{
    public class SampleClass
    {
        // SampleMethod(1, 23.4) -> "(1, 23.4)"
        public static string SampleMethod(int a, float b)
        {
            return $"({a}, {b})";
        }
    }
}

var result = Invoker.Invoke("SampleNamespace.SampleClass", "SampleMethod", "42", "1.23");
// result == "(42, 1.23)";
*/

public static class Invoker
{
    /// <summary>
    // 参照しているアセンブリを含めたすべての型のキャッシュ
    /// </summary>
    static readonly TypeInfo[] AllTypes = GetAllTypes();

    /// <summary>
    /// 型名、メソッド名、引数を文字列で指定して呼び込む
    /// </summary>
    /// <param name="typeName">型名。フルパスもしくは型名だけの文字列</param>
    /// <param name="methodName">メソッド名/param>
    /// <param name="args">引数に渡す文字列</param>
    /// <returns>メソッドの実行結果</returns>
    [return: System.Diagnostics.CodeAnalysis.MaybeNull]
    public static object Invoke(string typeName, string methodName, params string[] args)
    {
        var typeInfo = AllTypes.FirstOrDefault(e => e.FullName == typeName) ?? AllTypes.First(e => e.Name == typeName);

        var methods = typeInfo.GetMethods(BindingFlags.Static | BindingFlags.Public);
        var methodInfo = methods.First(e => e.Name == methodName);

        var parameterTypes = methodInfo.GetParameters().Select(e => e.ParameterType).ToArray();
        var parameters = args.Select((arg, i) =>
        {
            var converter = TypeDescriptor.GetConverter(parameterTypes[i]);
            return converter.ConvertFrom(arg);
        }).ToArray();

        return methodInfo.Invoke(null, parameters);
    }

    /// <summary>
    // 参照しているアセンブリを含めたすべての型を取得するメソッド
    /// </summary>
    /// <returns>参照しているアセンブリを含めたすべての型</returns>
    static TypeInfo[] GetAllTypes()
    {
        // 実行中のアセンブリを取得、
        // 参照先のアセンブリもすべて取得して実行中のアセンブリも合わせてコレクションにする
        // すべてのアセンブリのすべての型情報を配列化して返す
        var executingAssembly = Assembly.GetExecutingAssembly();
        var allAssemblies = executingAssembly.GetReferencedAssemblies().Select(e => Assembly.Load(e)).Append(executingAssembly);
        return allAssemblies.SelectMany(assembly => assembly.GetTypes()).Select(e => e.GetTypeInfo()).ToArray();
    }
}