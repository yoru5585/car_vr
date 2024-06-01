using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UnityStandardAssets.Vehicles.Car {
    [RequireComponent(typeof(CarController))]
    //[RequireComponent(typeof(GM))]
    public class CarUserControl : MonoBehaviour {
        private CarController m_Car;
        // the car controller we want to use

        //public CarController gm;
        //public CollisionWithMyCar coll;
        //public GM gg;

        public string debugtext = "";
        public bool debugmode = true;
        private Text debugText;
        private Text gearText;
        //
        public string ststeer = "axis1";
        public string stapedal = "axis3";
        public string stbpedal = "axis4";
        public string stDLR = "axis5";
        public string stDDU = "axis6";
        /* 0 ×
1 □
2 ○
3 △*/
        public string stbtnBATSU = "joystick button 0";
        public string stbtnSHIKAKU = "joystick button 1";
        public string stbtnMARU = "joystick button 2";
        public string stbtnSANKAKU = "joystick button 3";
        public string stbtnTEMAE =      "joystick button 12"; //+
        public string stbtnMUKOU =    "joystick button 13"; //-
        public string stbtnR2 =    "joystick button 6"; //
        public string stbtnL2 =    "joystick button 7"; //
         public string stbtnGT =    "joystick button 19";        
//        6 R2
//        7 L2

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



        public enum ATGear {
            P = 0,
            R = 1,
            N = 2,
            D = 3,
        }
        public ATGear atgear = ATGear.D;

        public GameObject camera;

        private Renderer brakeLampON;
        private Renderer brakeLampOFF;

        void Start() {
            
            this.debugText = GameObject.Find("Debug").GetComponent<Text>(); 
            this.gearText = GameObject.Find("GearText").GetComponent<Text>(); 
            camera = GameObject.Find("Main Camera");
            maincamera = GameObject.Find("Main Camera");

            m_Rigidbody = GetComponent<Rigidbody>();

            var children = GetComponentsInChildren<Transform>(true);
            foreach ( var trans in children )
            {
                if ( trans.name == "BLON" )
                {
                    brakeLampON = trans.gameObject.GetComponent<Renderer>();
                }
                if ( trans.name == "BLOFF" )
                {
                    brakeLampOFF = trans.gameObject.GetComponent<Renderer>();
                }
            }
        }






        private void Awake() {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }

//        private void Kubi(){
//            //debugtext = ""+Input.GetKey(stbtnR2);
//
//            if (Input.GetKey(stbtnL2)) {
//                r -= 200f*Time.deltaTime;
//                if (r <= -90f) {
//                    r = -90f;
//                }
//                debugtext = "H";
//            } else if (Input.GetKey(stbtnR2)) {
//                r += 200f*Time.deltaTime;
//                if (r  >= 90f) {
//                    r = 90f;
//                }
//                debugtext = "M";
//            } else {
//                if ( r > 10f) {
//                    r -= 400f*Time.deltaTime;
//                } else if ( r < -10f) {
//                    r += 400f*Time.deltaTime;
//                } else {
//                    r = 0;
//                }
//            }
//
//            debugtext += " "+r;
////                        string s = "";
////                        for(int i = 0; i < 20; i++) {
////                            s += i+" "+Input.GetKey("joystick button "+i)+" ";
//////                            if ((i-1)%5==0) {
//////                                s += "\n";
//////                            }
////                        }
////            Debug.Log(s);
//            camera.transform.localRotation = Quaternion.Euler(new Vector3(0,r,0));
//
//        }

        private void Gear(){
            bMukouPrev = bMukou;
            bMukou = Input.GetKey(stbtnMUKOU);
            bTemaePrev = bTemae;
            bTemae = Input.GetKey(stbtnTEMAE);

            if (bMukou && !bMukouPrev && (atgear!=ATGear.P)) {
                atgear--;
            }
            if (bTemae && !bTemaePrev && (atgear!=ATGear.D)) {
                atgear++;
            }

            if (Input.GetKey(KeyCode.P)) {
                atgear =ATGear.P;
            }
            if (Input.GetKey(KeyCode.R)) {
                atgear =ATGear.R;
            }
            if (Input.GetKey(KeyCode.N)) {
                atgear =ATGear.N;
            }
            if (Input.GetKey(KeyCode.D)) {
                atgear = ATGear.D;
            }

            if (atgear==ATGear.P) { //P
                m_Car.reverse = false;
                m_Car.neutral = true;
                gearText.text = "P";
            } else if (atgear==ATGear.R) { //R
                m_Car.reverse = true;
                m_Car.neutral = false;                  
                gearText.text = "R";
            } else if (atgear==ATGear.N) { //N
                m_Car.reverse = false;
                m_Car.neutral = true;                  
                gearText.text = "N";
            } else if (atgear==ATGear.D) { //D
                m_Car.reverse = false;
                m_Car.neutral = false;                  
                gearText.text = "D";
            }
            //gearText.text +=" "+atgear;
        }

        void ChangeInputDevice(){
            if (Input.GetKey(KeyCode.Alpha0)) {            
                inputmode = 0;
            } else if (Input.GetKey(KeyCode.Alpha1)) {            
                inputmode = 1;
            } else if (Input.GetKey(KeyCode.Alpha2)) {
                inputmode = 2;
            } else if (Input.GetKey(KeyCode.Alpha3)) {
                inputmode = 3;
            } else if (Input.GetKey(KeyCode.Alpha4)) {
                inputmode = 4;
            }
            SetInputDevice();
        }
        public void SetInputDevice(){
            if (inputmode==0) {            
                ststeer = "axis1";
                stapedal = "axis3";
                stbpedal = "axis4";                
                stDLR = "axis5";
                stDDU = "axis6";
            } else if (inputmode==1) {            
                ststeer = "axis1";
                stapedal = "axis3";
                stbpedal = "axis4";                
                stDLR = "axis5";
                stDDU = "axis6";
            } else if (inputmode==2) {      
                ststeer = "axis1";
                stapedal = "axis3";
                stbpedal = "axis6";                
                stDLR = "axis5";
                stDDU = "axis7";
            } else if (inputmode==3) {      
                ststeer = "axis1";
                stapedal = "axis2";
                stbpedal = "axis2";                
                stDLR = "axis5";
                stDDU = "axis7";
            } else if (inputmode==4) {      
                ststeer = "axis1";
                stapedal = "axis5";
                stbpedal = "axis5";                  
                stDLR = "axis6";
                stDDU = "axis7";
            }            
        }

        private void FixedUpdate() {
            bool brakelamp = false;
//            Kubi();
            ChangeInputDevice();

                prevDLR = dLR;
                steer = Input.GetAxis(ststeer);


                if (inputmode == 0) { // keyboard
                    if (Input.GetKey(KeyCode.UpArrow)) {
                        apedal = 0.9f;
                    } else {
                        apedal = 0;
                    }
                    if (Input.GetKey(KeyCode.DownArrow)) {
                        bpedal = 0.9f;
                    } else {
                        bpedal = 0;
                    }

                } else {
                    if (inputmode == 1 || inputmode == 2) {
                        apedal = Input.GetAxis(stapedal);
                        bpedal = Input.GetAxis(stbpedal);
                        dLR = (int)Input.GetAxis(stDLR);
                        apedal = (1 - apedal) / 2;
                        bpedal = (1 - bpedal) / 2;
                        if (bpedal < 0.04f && apedal < 0.04f) {
                            apedal = 0.04f; // クリープ
                        }                    
                    } else if(inputmode==3 || inputmode == 4) { // 1軸でアクセルブレーキ
                        cpedal = Input.GetAxis(stapedal);
                        if (cpedal <-0.01) {
                            apedal = -cpedal;
                            bpedal = 0;
                        } else if (cpedal > 0.01) {
                            apedal = 0;
                            bpedal = cpedal;
                        } else {
                            apedal = 0;
                            bpedal = 0;
                        }
                        dLR = (int)Input.GetAxis(stDLR);

                    }
                    if (Input.GetKey(KeyCode.UpArrow)) {
                        apedal = 0.9f;
                        bpedal = 0;
                    } 
                    if (Input.GetKey(KeyCode.DownArrow)) {
                        apedal = 0;
                        bpedal = 0.9f;
                    }                 
                }


            if (!active) {
                gearText.text = "P";
                transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            } else { 
                Gear();

                // else {
                //     if (Input.GetKey(KeyCode.UpArrow)) {
                //         apedal = 0.9f;
                //     } else {
                //         apedal = 0;
                //     }
                //     if (Input.GetKey(KeyCode.DownArrow)) {
                //         bpedal = 0.9f;
                //     } else {
                //         bpedal = 0;
                //     }
                //}
                //debugText.text = apedal + " " + bpedal;
                // pass the input to the car!
                // float h = CrossPlatformInputManager.GetAxis("Horizontal");
                // float v = CrossPlatformInputManager.GetAxis("Vertical");
                float handbrake = CrossPlatformInputManager.GetAxis("Jump");

                m_Car.Move(steer, apedal, -bpedal, 0);
                if (bpedal > 0.01f) {
                    brakelamp = true;
                }
                brakeLampON.enabled = brakelamp;
                brakeLampOFF.enabled = ! brakelamp;

                if (turboBoostEnabled) TurboBoost();
                if (pursuitEnabled) Pursuit();
            }

        }

        void TurboBoost(){
            if (Input.GetKey(KeyCode.Space)) {
                RaycastHit hit;
                float radius = 0.1f;
                float distance = 0.1f;
                if (Physics.SphereCast (transform.position+Vector3.up*0.1f,radius,Vector3.down, out hit, distance)) { 
                    // Debug.DrawRay(transform.position,Vector3.down*hit.distance, Color.red,0,false);
                    m_Rigidbody.AddForce(transform.up * ThrustTB);
                    sound.PlayJump();
                }  else {
                    // Debug.DrawRay(transform.position,Vector3.down*distance, Color.blue,0,false);
                }                    
            }
        }

        void Pursuit(){
            pursuitSound -= Time.deltaTime;
            if (Input.GetKey(KeyCode.B)) {
                m_Rigidbody.AddForce(transform.forward * ThrustP);
                if (pursuitSound < 0) {
                    sound.PlayDash();
                    pursuitSound = 10;
                }
            }
        }


        void OnCollisionEnter(Collision collision)
        {
            // Debug.Log(collision.gameObject.name); // ぶつかった相手の名前を取得
            // if(collision.gameObject.name== "CarWaypointBased"|| collision.gameObject.name == "CaraypointBasedOncomingCar")
            // {
            //     SceneManager.LoadScene("input"+(GM.instance.sceneCount-1)+"Screen");
            //     GM.instance.sceneCount = GM.instance.sceneCount - 1;
            // }
        }


        private void Kubi(){
            //debugtext = ""+Input.GetKey(stbtnR2);
            int dLR = (int)Input.GetAxis(stDLR);
            int dDU = (int)Input.GetAxis(stDDU);
            if (dLR==-1) {
                r -= 200f*Time.deltaTime;
                if (r <= -90f) {
                    r = -90f;
                }
                lookForward = false;
                debugtext = "H";
            } else if (dLR==1) {
                r += 200f*Time.deltaTime;
                if (r  >= 90f) {
                    r = 90f;
                }
                lookForward = false;
                debugtext = "M";
            } else if (dDU == 1) {
                lookForward = true;
            }

            if (lookForward) {
                if ( r > 10f) {
                    r -= 400f*Time.deltaTime;
                } else if ( r < -10f) {
                    r += 400f*Time.deltaTime;
                } else {
                    r = 0;
                }
            }

            debugtext += " "+r;
            //                        string s = "";
            //                        for(int i = 0; i < 20; i++) {
            //                            s += i+" "+Input.GetKey("joystick button "+i)+" ";
            ////                            if ((i-1)%5==0) {
            ////                                s += "\n";
            ////                            }
            //                        }
            //            Debug.Log(s);
            maincamera.transform.localRotation = Quaternion.Euler(new Vector3(0,r,0));

        }

    }
}
