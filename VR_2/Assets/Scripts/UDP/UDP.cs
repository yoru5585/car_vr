using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net;//for UDP
using System.Net.Sockets; //for UDP
using System.Threading;//for Interlocked
using System;


namespace forudp //namespaceは本体に合わせて要修正
{
    public class UDP
    {
        private UdpClient udpForSend; //送信用クライアント
        private string remoteHost = "localhost";//送信先のIPアドレス
        private int remotePort;//送信先のポート

        private UdpClient udpForReceive; //受信用クライアント
        public string rcvMsg = "ini";//受信メッセージ格納用
        public float rcvMsg_xyz = 0;

        private System.Threading.Thread rcvThread; //受信用スレッド

        //public List<float> rcv_float_arr = new List<float>();
        public float rcv_float;

        public UDP()
        {
        }

        public bool init(int port_snd, int port_to, int port_rcv) //UDP設定（送受信用ポートを開きつつ受信用スレッドを生成）
        {
            try
            {
                udpForSend = new UdpClient(port_snd); //送信用ポート
                remotePort = port_to; //送信先ポート
                udpForReceive = new UdpClient(port_rcv); //受信用ポート
                rcvThread = new System.Threading.Thread(new System.Threading.ThreadStart(receive_xyz)); //受信スレッド生成
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void send_xyz_arr(float[] sendMsg) //float配列を送信用ポートから送信先ポートに送信
        {
            try
            {


                byte[] sendBytes = new byte[sendMsg.Length];
                byte[] rv = new byte[4 * sendBytes.Length];
                for (int i = 0; i < sendMsg.Length; i++)

                {
                    sendBytes = BitConverter.GetBytes(sendMsg[i]);
                    System.Buffer.BlockCopy(sendBytes, 0, rv, i * 4, sendBytes.Length);
                }
                //Debug.Log(sendBytes);
                //Debug.Log(rv);
                udpForSend.Send(rv, rv.Length, remoteHost, remotePort);
                //udpForSend.Send(sendBytes, sendBytes.Length, remoteHost, remotePort);
            }
            catch { }
        }

        public void receive_xyz() //受信スレッドで実行される関数
        {
            IPEndPoint remoteEP = null;//任意の送信元からのデータを受信
            while (true)
            {
                try
                {
                    byte[] rcvBytes = udpForReceive.Receive(ref remoteEP);
                    for (int i = 0; i < rcvBytes.Length; i++)
                    {
                        var a = BitConverter.ToSingle(rcvBytes, i * 4);
                        //rcv_float_arr.Add(a);
                        rcv_float = a;
                    }
                }
                catch { }
            }
        }

        public void start_receive() //受信スレッド開始
        {
            try
            {
                rcvThread.Start();
            }
            catch { }
        }

        public void stop_receive() //受信スレッドを停止
        {
            try
            {
                rcvThread.Interrupt();
            }
            catch { }
        }

        public void end() //送受信用ポートを閉じつつ受信用スレッドも廃止
        {
            try
            {
                udpForReceive.Close();
                udpForSend.Close();
                rcvThread.Abort();
            }
            catch { }
        }
    }
}
