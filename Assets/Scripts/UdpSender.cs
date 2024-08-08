using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UdpSender : MonoBehaviour
{
    //public string remoteIpAddress = "192.168.1.2"; // 受信者のIPアドレス
    public int remotePort = 50392; // 受信者のポート番号

    private UdpClient udpClient;

    void Start()
    {
        udpClient = new UdpClient();
        // ブロードキャスト有効化
        udpClient.EnableBroadcast = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendMessage("Hello from Sender");
        }
    }

    void SendMessage(string message)
    {
        

        byte[] data = Encoding.UTF8.GetBytes(message);

        //udpClient.Send(data, data.Length, remoteIpAddress, remotePort);
        // ポート8888にブロードキャスト送信
        udpClient.Send(data, data.Length, new IPEndPoint(IPAddress.Broadcast, remotePort));

        Debug.Log("Message sent: " + message);
    }

    void OnApplicationQuit()
    {
        udpClient.Close();
    }
}