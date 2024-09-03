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

    void Start()
    {
        udpClient = new UdpClient(listenPort);
        
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
                this.Invoke(new Action(() =>
                {
                    // 登録したメソッドを実行する
                    if(ActionRecieveData != null)
                    {
                        ActionRecieveData.Invoke(message);
                    }
                }).Method.Name, 0f);   
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
