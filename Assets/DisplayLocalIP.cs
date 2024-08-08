using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class DisplayLocalIP : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string hostname = Dns.GetHostName();

        IPAddress[] adrList = Dns.GetHostAddresses(hostname);
        foreach (IPAddress address in adrList)
        {
            Debug.Log(address.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
