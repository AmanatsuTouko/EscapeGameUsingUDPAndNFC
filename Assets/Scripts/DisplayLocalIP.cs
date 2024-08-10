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
        IPAddress[] adrList = GetLocalIPAdresses();
        foreach (IPAddress address in adrList)
        {
            Debug.Log(address.ToString());
        }
    }

    public IPAddress[] GetLocalIPAdresses()
    {
        return Dns.GetHostAddresses(Dns.GetHostName());
    }
}
