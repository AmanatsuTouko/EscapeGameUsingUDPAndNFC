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

    void Start()
    {
        udpClient = new UdpClient();
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

        udpClient.Send(data, data.Length, remoteIpAddress, remotePort);
        Debug.Log("Message sent: " + message);
    }

    void OnApplicationQuit()
    {
        udpClient.Close();
    }
}