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
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, listenPort);
        while (true)
        {
            byte[] data = udpClient.Receive(ref remoteEndPoint);
            string message = Encoding.UTF8.GetString(data);
            Debug.Log("Message received: " + message);

            // 登録したメソッドを実行する
            ActionRecieveData.Invoke(message);
        }
    }

    void OnApplicationQuit()
    {
        receiveThread.Abort();
        udpClient.Close();
    }
}
