using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForUDP;
using TMPro;

public class RecvManager : MonoBehaviour
{
    private UDP_recv commUDP = new UDP_recv();
    [SerializeField] int port_rcv;
    [SerializeField] TextMeshProUGUI logText;

    //ハンドルの入力値
    //アクセルの入力値
    //ブレーキの入力値

    // Start is called before the first frame update
    void Start()
    {
        //commUDP.init(int型の送信用ポート番号, int型の送信先ポート番号, int型の受信用ポート番号);
        commUDP.init(port_rcv);
        commUDP.start_receive();
    }

    private void Update()
    {
        receive();
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

        commUDP.start_receive();
        var b = commUDP.rcv_float_arr;
        string recvStr = "";

        for (int i = 0; i < b.Length; i++)
        {
            recvStr += $"{i}:{b[i]}\n";
            //valueSlider.value = b[i];
        }

        //logText.text = recvStr;
        //Debug.Log(recvStr);
    }

    public float[] GetRecvValue()
    {
        return commUDP.rcv_float_arr;
    }

    public float GetStsteerAxis()
    {
        return commUDP.rcv_float_arr[0];
    }

    public float GetStapedalAxis()
    {
        return commUDP.rcv_float_arr[1];
    }
    
    public float GetStbpedalAxis()
    {
        return commUDP.rcv_float_arr[2];
    }

    private void OnApplicationQuit()
    {
        commUDP.end();
    }
}
