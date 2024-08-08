using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UdpReceiver : MonoBehaviour
{
    public int listenPort = 50392; // 受信するポート番号

    private UdpClient udpClient;
    private Thread receiveThread;

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
        }
    }

    void OnApplicationQuit()
    {
        receiveThread.Abort();
        udpClient.Close();
    }
}
