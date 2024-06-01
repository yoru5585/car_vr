using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net;//for UDP
using System.Net.Sockets; //for UDP
using System.Threading;//for Interlocked
using System;


namespace forudp //namespace�͖{�̂ɍ��킹�ėv�C��
{
    public class UDP
    {
        private UdpClient udpForSend; //���M�p�N���C�A���g
        private string remoteHost = "localhost";//���M���IP�A�h���X
        private int remotePort;//���M��̃|�[�g

        private UdpClient udpForReceive; //��M�p�N���C�A���g
        public string rcvMsg = "ini";//��M���b�Z�[�W�i�[�p
        public float rcvMsg_xyz = 0;

        private System.Threading.Thread rcvThread; //��M�p�X���b�h

        //public List<float> rcv_float_arr = new List<float>();
        public float rcv_float;

        public UDP()
        {
        }

        public bool init(int port_snd, int port_to, int port_rcv) //UDP�ݒ�i����M�p�|�[�g���J����M�p�X���b�h�𐶐��j
        {
            try
            {
                udpForSend = new UdpClient(port_snd); //���M�p�|�[�g
                remotePort = port_to; //���M��|�[�g
                udpForReceive = new UdpClient(port_rcv); //��M�p�|�[�g
                rcvThread = new System.Threading.Thread(new System.Threading.ThreadStart(receive_xyz)); //��M�X���b�h����
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void send_xyz_arr(float[] sendMsg) //float�z��𑗐M�p�|�[�g���瑗�M��|�[�g�ɑ��M
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

        public void receive_xyz() //��M�X���b�h�Ŏ��s�����֐�
        {
            IPEndPoint remoteEP = null;//�C�ӂ̑��M������̃f�[�^����M
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

        public void start_receive() //��M�X���b�h�J�n
        {
            try
            {
                rcvThread.Start();
            }
            catch { }
        }

        public void stop_receive() //��M�X���b�h���~
        {
            try
            {
                rcvThread.Interrupt();
            }
            catch { }
        }

        public void end() //����M�p�|�[�g�����M�p�X���b�h���p�~
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
