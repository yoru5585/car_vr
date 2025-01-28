using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    //[RequireComponent(typeof(GM))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car;
        [SerializeField] private RecvManager m_RecvManager;
        public float steer;
        public float apedal;
        public float bpedal;
        public float cpedal;

        public int dLR = 0;
        public int prevDLR = 0;

        public bool active = false;
        public int inputmode = 0;

        public bool bMukou;
        public bool bTemae;
        public bool bMukouPrev;
        public bool bTemaePrev;

        public float r;

        public bool turboBoostEnabled;
        public bool pursuitEnabled;
        Rigidbody m_Rigidbody;
        float ThrustTB = 800000;
        public float ThrustP = 20000;
        public float pursuitSound = 0;
        public Sound sound;

        private GameObject maincamera;

        bool lookForward = true;



        public enum ATGear
        {
            P = 0,
            R = 1,
            N = 2,
            D = 3,
        }
        public ATGear atgear = ATGear.D;

        private Renderer brakeLampON;
        private Renderer brakeLampOFF;

        void Start()
        {
            maincamera = GameObject.Find("Main Camera");

            m_Rigidbody = GetComponent<Rigidbody>();

            var children = GetComponentsInChildren<Transform>(true);
            foreach (var trans in children)
            {
                if (trans.name == "BLON")
                {
                    brakeLampON = trans.gameObject.GetComponent<Renderer>();
                }
                if (trans.name == "BLOFF")
                {
                    brakeLampOFF = trans.gameObject.GetComponent<Renderer>();
                }
            }
        }

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }

        private void Gear()
        {
            bMukouPrev = bMukou;
            //bMukou = Input.GetKey(stbtnMUKOU);
            bTemaePrev = bTemae;
            //bTemae = Input.GetKey(stbtnTEMAE);

            if (bMukou && !bMukouPrev && (atgear != ATGear.P))
            {
                atgear--;
            }
            if (bTemae && !bTemaePrev && (atgear != ATGear.D))
            {
                atgear++;
            }

            if (Input.GetKey(KeyCode.P))
            {
                atgear = ATGear.P;
            }
            if (Input.GetKey(KeyCode.R))
            {
                atgear = ATGear.R;
            }
            if (Input.GetKey(KeyCode.N))
            {
                atgear = ATGear.N;
            }
            if (Input.GetKey(KeyCode.D))
            {
                atgear = ATGear.D;
            }

            if (atgear == ATGear.P)
            { //P
                m_Car.reverse = false;
                m_Car.neutral = true;
                //gearText.text = "P";
            }
            else if (atgear == ATGear.R)
            { //R
                m_Car.reverse = true;
                m_Car.neutral = false;
                //gearText.text = "R";
            }
            else if (atgear == ATGear.N)
            { //N
                m_Car.reverse = false;
                m_Car.neutral = true;
                //gearText.text = "N";
            }
            else if (atgear == ATGear.D)
            { //D
                m_Car.reverse = false;
                m_Car.neutral = false;
                //gearText.text = "D";
            }
            //gearText.text +=" "+atgear;
        }

        private void FixedUpdate()
        {
            bool brakelamp = false;
            prevDLR = dLR;

            //ハンドルの入力軸を取得
            //steer = Input.GetAxis(ststeer);
            steer = m_RecvManager.GetStsteerAxis();
            //アクセル
            //apedal = Input.GetAxis(stapedal);
            apedal = m_RecvManager.GetStapedalAxis();
            //ブレーキ
            //bpedal = Input.GetAxis(stbpedal);
            bpedal = m_RecvManager.GetStbpedalAxis();
            apedal = (1 - apedal) / 2;
            bpedal = (1 - bpedal) / 2;
            if (bpedal < 0.04f && apedal < 0.04f)
            {
                apedal = 0.04f; // クリープ
            }

            float handbrake = CrossPlatformInputManager.GetAxis("Jump");

            Debug.Log(apedal);
            m_Car.Move(steer, apedal, -bpedal, 0);
            if (bpedal > 0.01f)
            {
                brakelamp = true;
            }
            brakeLampON.enabled = brakelamp;
            brakeLampOFF.enabled = !brakelamp;

            if (turboBoostEnabled) TurboBoost();
            if (pursuitEnabled) Pursuit();


            //if (!active)
            //{
            //    //gearText.text = "P";
            //    transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            //}
            //else
            //{
            //    Gear();
                
            //}

        }

        void TurboBoost()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                RaycastHit hit;
                float radius = 0.1f;
                float distance = 0.1f;
                if (Physics.SphereCast(transform.position + Vector3.up * 0.1f, radius, Vector3.down, out hit, distance))
                {
                    // Debug.DrawRay(transform.position,Vector3.down*hit.distance, Color.red,0,false);
                    m_Rigidbody.AddForce(transform.up * ThrustTB);
                    sound.PlayJump();
                }
                else
                {
                    // Debug.DrawRay(transform.position,Vector3.down*distance, Color.blue,0,false);
                }
            }
        }

        void Pursuit()
        {
            pursuitSound -= Time.deltaTime;
            if (Input.GetKey(KeyCode.B))
            {
                m_Rigidbody.AddForce(transform.forward * ThrustP);
                if (pursuitSound < 0)
                {
                    sound.PlayDash();
                    pursuitSound = 10;
                }
            }
        }
    }
}
