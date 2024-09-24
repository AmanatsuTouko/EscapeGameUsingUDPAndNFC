using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UdpSender : MonoBehaviour
{
    // 受信者のIPアドレス, ポート番号
    public string remoteIpAddress = "192.168.11.15";
    public int remotePort = 50400;

    private UdpClient udpClient;

    // デバッグモードの指定
    [SerializeField] Mode mode = Mode.Connection;

    public enum Mode
    {
        Connection,
        DebugLoopBack,
    }

    void Start()
    {
        udpClient = new UdpClient();

        // デバッグ時には，自身に送信が返ってくるようにループバックアドレスを用いる
        if(mode == Mode.DebugLoopBack)
        {
            remoteIpAddress = "127.0.0.1";
        }
    }

    public new void SendMessage(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);

        udpClient.Send(data, data.Length, remoteIpAddress, remotePort);
        Debug.Log("Message sent: " + message);
    }

    void OnApplicationQuit()
    {
        udpClient.Close();
    }
}