using System;
using System.Diagnostics;
using UnityEngine;

public class ShowDotNetVersion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 現在の .NET のバージョンを表示
        UnityEngine.Debug.Log($".NET Version: {Environment.Version}");

        // dotnet --version コマンドを実行する
        ProcessStartInfo startInfo = new ProcessStartInfo("/usr/local/share/dotnet/dotnet", "--version")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = new Process())
        {
            process.StartInfo = startInfo;
            process.Start();

            // 結果を読み取る
            string sdkVersion = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // SDKのバージョンを表示
            UnityEngine.Debug.Log($".NET SDK Version: {sdkVersion}");
        }
    }
}
