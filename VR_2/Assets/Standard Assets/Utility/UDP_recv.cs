using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;//for UDP
using System.Net.Sockets; //for UDP
using System.Threading;
using System.Diagnostics;//for Interlocked

namespace ForUDP
{
    public class UDP_recv
    {
        private UdpClient udpForReceive; //受信用クライアント

        private System.Threading.Thread rcvThread; //受信用スレッド

        public float[] rcv_float_arr = new float[10];

        public UDP_recv()
        {
        }

        public bool init(int port_rcv) //UDP設定（受信用スレッドを生成）
        {
            try
            {
                udpForReceive = new UdpClient(port_rcv); //受信用ポート
                rcvThread = new System.Threading.Thread(new System.Threading.ThreadStart(receive_xyz)); //受信スレッド生成
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void receive_xyz() //受信スレッドで実行される関数
        {
            IPEndPoint remoteEP = null;//任意の送信元からのデータを受信
            while (true)
            {
                try
                {
                    byte[] rcvBytes = udpForReceive.Receive(ref remoteEP);
                    //UnityEngine.Debug.Log(remoteEP.Address);
                    //UnityEngine.Debug.Log(remoteEP.Port);
                    for (int i = 0; i < rcvBytes.Length; i++)
                    {
                        var a = BitConverter.ToSingle(rcvBytes, i * 4);
                        rcv_float_arr[i] = a;
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
                rcvThread.Abort();
            }
            catch { }
        }
    }
}
