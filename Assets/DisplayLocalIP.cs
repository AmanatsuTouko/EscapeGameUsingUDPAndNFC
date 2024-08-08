using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class DisplayLocalIP : MonoBehaviour
{
    // 自分のローカルIPアドレスを表示する
    void Start()
    {
        string hostname = Dns.GetHostName();

        IPAddress[] adrList = Dns.GetHostAddresses(hostname);
        foreach (IPAddress address in adrList)
        {
            Debug.Log(address.ToString());
        }
    }
}
