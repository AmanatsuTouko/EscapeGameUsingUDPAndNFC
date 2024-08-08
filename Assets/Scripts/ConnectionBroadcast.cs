using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager;
using UnityEngine;

public class ConnectionBroadcast : MonoBehaviour
{
    // 送受信に利用するポート番号
    private int portNum = 50000;

    UdpClient listenClient;

    // 1度だけ実行する
    public void InitUdpclients()
    {
        // UdpClientを生成
        listenClient = new UdpClient(8888);
    }

    public void SendBroadcastMessage(string data)
    {
        // UdpClient作成（ポート番号は適当に割当）
        var client = new UdpClient();
        var requestData = Encoding.ASCII.GetBytes(data);
        // サーバ（通信相手）のエンドポイントserverEndpoint作成（IP/Port未指定）
        var serverEndpoint = new IPEndPoint(IPAddress.Any, 0);

        // ブロードキャスト有効化
        client.EnableBroadcast = true;
        // ポート8888にブロードキャスト送信
        client.Send(requestData, requestData.Length, new IPEndPoint(IPAddress.Broadcast, portNum));

        // 送信データを受信した相手は、自分（クライアント）のエンドポイント情報を知ったはずなので、
        // そこに対してパケットを送信してくれるのを待つ

        // サーバからのパケット受信、serverEndpointにサーバのエンドポイント情報が入る
        var ServerResponseData = client.Receive(ref serverEndpoint);
        var ServerResponse = Encoding.ASCII.GetString(ServerResponseData);

        // serverEndpoint.Address / serverEndpoint.Port でサーバのIP/ポート番号を取得
        Console.WriteLine("Recived {0} from {1}:{2}", ServerResponse, serverEndpoint.Address.ToString(), serverEndpoint.Port.ToString());

        client.Close();

        //// 送信データ
        //var buffer = Encoding.UTF8.GetBytes(data);
        //// UpdClientを作成
        //UdpClient sendClient = new UdpClient(portNum);

        //// ブロードキャスト送信
        //sendClient.EnableBroadcast = true;
        //sendClient.Connect(new IPEndPoint(IPAddress.Broadcast, portNum));
        //sendClient.Send(buffer, buffer.Length);
        //sendClient.Close();
    }

    public void ListenBroadcastMessage()
    {
        var Server = new UdpClient(portNum);                                       // 待ち受けポートを指定してUdpClient生成
        // var ResponseData = Encoding.ASCII.GetBytes("SomeResponseData");         // 適当なレスポンスデータ

        var clientEndpoint = new IPEndPoint(IPAddress.Any, 0);                    // クライアント（通信相手）のエンドポイントclientEndpoint作成（IP/Port未指定）
        var clientRequestData = Server.Receive(ref clientEndpoint);               // クライアントからのパケット受信、clientEndpointにクライアントのエンドポイント情報が入る
        var clientRequest = Encoding.UTF8.GetString(clientRequestData);
        // 無効なデータの場合は終了する
        if (clientRequest == null || clientRequest.Length == 0)
            return;

        Console.WriteLine("Recived {0} from {1}, sending response", clientRequest, clientEndpoint.Address.ToString());    // clientEndpoint.Address：クライアントIP
        // Server.Send(ResponseData, ResponseData.Length, clientEndpoint);           // クライアント情報の入ったclientEndpointに対してパケット送信

        // 受信イベントを実行
        this.OnReceive(clientRequest);

        //while (true)
        //{
        //    var clientEndpoint = new IPEndPoint(IPAddress.Any, 0);                    // クライアント（通信相手）のエンドポイントclientEndpoint作成（IP/Port未指定）
        //    var clientRequestData = Server.Receive(ref clientEndpoint);               // クライアントからのパケット受信、clientEndpointにクライアントのエンドポイント情報が入る
        //    var clientRequest = Encoding.UTF8.GetString(clientRequestData);
        //    // 無効なデータの場合は終了する
        //    if (clientRequest == null || clientRequest.Length == 0)
        //        return;

        //    Console.WriteLine("Recived {0} from {1}, sending response", clientRequest, clientEndpoint.Address.ToString());    // clientEndpoint.Address：クライアントIP
        //    // Server.Send(ResponseData, ResponseData.Length, clientEndpoint);           // クライアント情報の入ったclientEndpointに対してパケット送信

        //    // 受信イベントを実行
        //    this.OnReceive(clientRequest);
        //}

        //// ブロードキャストを監視するエンドポイント
        //var remote = new IPEndPoint(IPAddress.Any, portNum);

        //// データ受信を待機（同期処理なので受信完了まで処理が止まる）
        //// 受信した際は、 remote にどの IPアドレス から受信したかが上書きされる
        //var buffer = listenClient.Receive(ref remote);
        //// 受信データを変換
        //var data = Encoding.UTF8.GetString(buffer);
        //// 無効なデータの場合は終了する
        //if (data == null || data.Length == 0)
        //    return;
        //// 受信イベントを実行
        //this.OnReceive(data);
    }

    public void ListenClose()
    {
        listenClient.Close();
    }

    private void OnReceive(string data)
    {
        // 受信処理...
        Debug.Log($"{data}を受信！！！");
    }
}
