using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using forudp;
using TMPro;
using UnityEngine.UI;

public class Receive_udp : MonoBehaviour
{
    private UDP commUDP = new UDP();
    [SerializeField] int port_snd;
    [SerializeField] int port_to;
    [SerializeField] int port_rcv;
    [SerializeField] TextMeshProUGUI logText;
    [SerializeField] Slider rcv_valueSlider;

    // Start is called before the first frame update
    void Start()
    {
        //commUDP.init(int型の送信用ポート番号, int型の送信先ポート番号, int型の受信用ポート番号);
        commUDP.init(port_snd, port_to, port_rcv);
        commUDP.start_receive();
    }

    public void receive()
    {
        //var a = commUDP.rcvMsg_xyz;
        //var b = commUDP.rcv_float_arr;

        //foreach (float i in b)
        //{
        //    logText.text += "\n" + i;
        //    rcv_valueSlider.value = i;
        //}

        var b = commUDP.rcv_float;
        logText.text += "\n" + b;
        rcv_valueSlider.value = b;

    }

    public void logClear()
    {
        logText.text = "";
    }
}
