using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System;

public class UdpReceiver : MonoBehaviour
{
    // 受信するポート番号
    public int listenPort = 50400;

    private UdpClient udpClient;
    private Thread receiveThread;

    // 受信した時に実行するメソッド
    public Action<string> ActionRecieveData;

    // UDPを待機しているスレッドからメインスレッドに処理を戻せるようにする
    private SynchronizationContext mainThreadContext;

    void Start()
    {
        udpClient = new UdpClient(listenPort);

        // メインスレッドのSynchronizationContextを取得
        mainThreadContext = SynchronizationContext.Current;
        
        // 別スレッドでUDP受信を待機する
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, listenPort);

        try
        {
            // UDPメッセージの受信を待機する
            while (true)
            {
                // 受信したデータを文字列に変換する
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                string message = Encoding.UTF8.GetString(data);
                Debug.Log("Message received: " + message);

                // メインスレッドに処理を戻して，登録したメソッドを実行する
                // (UnityのUI操作などの関数はメインスレッドからしか実行できないため)
                mainThreadContext.Post(_ =>
                {
                    // 登録したメソッドを実行する
                    if (ActionRecieveData != null)
                    {
                        ActionRecieveData.Invoke(message);
                    }
                }, null);
            }
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"SocketException: {ex.Message}");
        }
        // スレッド終了時に例外が発生するので，udpClientを終了してからスレッドを終了する
        finally
        {
            udpClient.Close();
        }
    }

    void OnApplicationQuit()
    {
        receiveThread.Abort();
    }
}
