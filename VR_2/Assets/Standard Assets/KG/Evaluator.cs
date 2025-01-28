using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;
namespace UnityStandardAssets.Vehicles.Car
{
    public class Evaluator : MonoBehaviour {
        private Sound sound;


        public bool active = false;
        public int countSpeedOK;
        public int countSpeedNG;
        public int adequateSpeed = 40;
        BlockFinder blockFinder;
        public int index = 0;
        public int prevIndex =0;
        Rigidbody mrigidbody;
        public float speed;

        public Vector3 v3;

        public float distance;
        public int countPosOK;
        public int countPosNG;
        public float adequatedistance = 1.0f;
        public float distanceSum;

        public string colname;
        public string prevcolname;
        public string colnameExit;
        public int colCountCar;
        public int colCountEtc;


        public float signalDistance;
        public Vector3 signalPosition;
        public GameObject goSignal;
        public SignalController signal;
        public int signalColor;
        public int countIgnoreRed;
        public int police_countIgnoreRed = 3;
        public string prevSigName;

        public float followingDistance;
        public float stoppingDistance;
        public float cpi;
        public int countUnsafe;
        public int countALL;
        public float unsafeRatio;

        public string dobject;
        public GameObject goFrontCar;
        public GameObject goCollision;
        public bool frontCarStop = false;
        public bool useBlock = false;
        CarAIControl carAIc;
        CarUserControl carUC;

        public int oppositeLaneTime = 0;
        public bool inOppositeLane = false;

        public int countRightTurnLane = 0;
        public float prevPosZ = 0;


        public float prevTime = 0;

        public bool resultGoal = false;
        public bool resultCollision = false;
        public bool resultFrontCar = false;
        public bool resultArrested = false;
        public bool resultGetJump = false;

        public int overSpeedThreshold = 55;
        public int overSpeedTime = 0;
        public bool isOverSpeed = false;

        public bool policeActive = true;
        bool policeStart = false;
        public float policeTime = 0;
        public float policeOppositeLaneTime = 1;//対向車線　注意元の値は10
        public int policeCountRightTurnLane = 2;
        public float policeOverSpeedTime = 10;

        //kumagai追加
        [SerializeField] private GameObject police_car;//GameObject型の変数aを宣言　好きなゲームオブジェクトをアタッチ
        [SerializeField] private GameObject police_car2;
        [SerializeField] private GameObject police_car3;
        public Rigidbody rigidy;
        public  float policeMoveTime = 0;


        // Use this for initialization

        void Start () {
            carUC =  GameObject.Find("Mycar").GetComponent<CarUserControl>();
            sound = GameObject.Find("GameController").GetComponent<Sound>();
            blockFinder = transform.GetComponent<BlockFinder>();
            mrigidbody = transform.GetComponent<Rigidbody>();

            active = true;

            countSpeedOK = 0;
            countSpeedNG = 0;
            countPosOK = 0;
            countPosNG = 0;

            countIgnoreRed = 0;

            countALL = 0;
            countUnsafe = 0;
            
        }
        
        // Update is called once per frame
        void Update () {
            // if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown("joystick button 0")) {
            //     SceneManager.LoadScene("Start");
            // }            
            if (Time.time > prevTime + 1) {
                if (inOppositeLane) {
                    oppositeLaneTime ++;
                }
                inOppositeLane = false;
                if (isOverSpeed) {
                    overSpeedTime++;
                }
                isOverSpeed = false;
                prevTime = Time.time;
            }     
            if (active ){

                if (speed > overSpeedThreshold) {
                    isOverSpeed = true;
                    

                    Debug.Log(overSpeedTime + "ンゴ");
                    //Debug.Log(policeOppositeLaneTime + "でないはず");
                    Debug.Log(oppositeLaneTime + "対向車線時間");
                    //Debug.Log(prevTime + "数値確認");
                    Debug.Log(countIgnoreRed + "信号無視");


                }

                ////////////////////////////////////////////////////////
                if (policeActive && !policeStart && (oppositeLaneTime >= policeOppositeLaneTime || countRightTurnLane >= policeCountRightTurnLane || overSpeedTime >= policeOverSpeedTime || countIgnoreRed >= police_countIgnoreRed))
                {
                    policeStart = true;
                    sound.PlayPolice();

                    // パトカー出現開始処理をここに
                            police_car.transform.localPosition -= new Vector3(-2, 0, 10);
                            police_car2.transform.localPosition -= new Vector3(10, 0, 0);// * Time.deltaTime;
                            police_car3.transform.localPosition -= new Vector3(-2, 0, 10);// * Time.deltaTime;
                            police_car.transform.localPosition -= new Vector3(0, 0, 7);// * Time.deltaTime;
                            police_car2.transform.localPosition -= new Vector3(7, 0, 0);// * Time.deltaTime;
                            police_car3.transform.localPosition -= new Vector3(0, 0, 7);// * Time.deltaTime;

                        this.rigidy = this.GetComponent<Rigidbody>();
                        this.rigidy.constraints = RigidbodyConstraints.FreezePosition;
                        police_car2.SetActive(true);
                        if (oppositeLaneTime >= policeOppositeLaneTime)
                        {
                            police_car3.SetActive(true);
                        }
                        else {
                            police_car.SetActive(true);
                        }
                        police_car2.SetActive(true);

                        GameObject.Find("OtherCars").SetActive(false);
                }

                if (policeStart) {
                    policeTime += Time.deltaTime;
                    if (true)  { //if (policeTime > 3) { //  ?秒後逮捕



                        policeMoveTime += Time.deltaTime;
                        // //Debug.Log("経過時間" + policeMoveTime);
                        // police_car.transform.position += new Vector3(-2, 0, 10) * Time.deltaTime;
                        // police_car2.transform.position += new Vector3(10, 0, 0) * Time.deltaTime;
                        // police_car3.transform.position += new Vector3(-2, 0, 10) * Time.deltaTime;
                        // resultArrested = true;

                        // if (policeMoveTime >= 1 && policeMoveTime < 2)
                        // {
                        //     police_car.transform.position -= new Vector3(0, 0, 3) * Time.deltaTime;
                        //     police_car2.transform.position -= new Vector3(3, 0, 0) * Time.deltaTime;
                        //     police_car3.transform.position -= new Vector3(0, 0, 3) * Time.deltaTime;
                        // }

                        // else if (policeMoveTime >= 2) 
                        // {
                        //     police_car.transform.position -= new Vector3(-2, 0, 10) * Time.deltaTime;
                        //     police_car2.transform.position -= new Vector3(10, 0, 0) * Time.deltaTime;
                        //     police_car3.transform.position -= new Vector3(-2, 0, 10) * Time.deltaTime;
                        // }
                        //Debug.Log("経過時間" + policeMoveTime);
                        if (policeMoveTime >= 0 && policeMoveTime < 1) {
                            police_car.transform.localPosition += new Vector3(-2, 0, 10) * Time.deltaTime;
                            police_car2.transform.localPosition += new Vector3(10, 0, 0) * Time.deltaTime;
                            police_car3.transform.localPosition += new Vector3(-2, 0, 10) * Time.deltaTime;
                        } else if (policeMoveTime >= 1 && policeMoveTime < 2)
                        {
                            police_car.transform.localPosition += new Vector3(0, 0, 7) * Time.deltaTime;
                            police_car2.transform.localPosition += new Vector3(7, 0, 0) * Time.deltaTime;
                            police_car3.transform.localPosition += new Vector3(0, 0, 7) * Time.deltaTime;
                        }
                        else if (policeMoveTime >= 2) 
                        {
                            resultArrested = true;
                        }
                    }

                }


                prevIndex = index;
                index = blockFinder.index;
                speed = mrigidbody.velocity.magnitude*3.6f;

                // KG
                RaycastHit hit; 
                Vector3 pdiff = new Vector3(0f, 1.1f, 0f); 
                int sdistance = 80; 
                float radius = 0.1f;// 0.5f; 
                followingDistance = 127f;
                dobject ="";
                //Debug.DrawLine(transform.position,transform.position+(wpt.target.position-transform.position).normalized*distance);
                //Debug.DrawRay(transform.position,(wpt.target.position-transform.position).normalized*distance,Color.blue,0,false);
                //                Debug.DrawRay(transform.position + pdiff, transform.forward*distance, Color.red,0,false);
    //            if (  debugText != null) {
    //                debugText.text = "";
    //            }
                //                if (Physics.SphereCast (transform.position + pdiff,radius,transform.forward, out hit, distance)) { 
                sdistance = 40; 
                if (frontCarStop) { // 前方車両急停止
                    if (Physics.SphereCast (transform.position + pdiff,radius,transform.forward, out hit, sdistance)) { 
                        followingDistance = hit.distance;
                        dobject = hit.transform.name;

                        stoppingDistance = (1.5f * speed * 1000/3600)+ (speed*1000/3600)* (speed*1000/3600) / (2*9.8f*0.65f);
                        cpi = stoppingDistance / followingDistance;
                        if (dobject.Contains("CarWay")) {
                            GameObject gc = hit.collider.gameObject;
                            float forntspeed = gc.transform.parent.transform.parent.GetComponent<Rigidbody>().velocity.magnitude*60*60/1000;
                            // if (cpi > 1.5 && speed > 40 && forntspeed > 35 && transform.position.z > 1430) {
                            if (cpi > 1.5 && speed > 40 && forntspeed > 35 && followingDistance < 10 && carUC.bpedal < 0.5f
                                && transform.position.z > 1430 ) {
                                goFrontCar = gc.transform.parent.transform.parent.gameObject;
                                CarAIControl cc = goFrontCar.GetComponent<CarAIControl>(); //gc.transform.parent.transform.parent.GetComponent<CarAIControl>();
                                cc.Stop();
                            }
                        }
                    }
                }
                sdistance = 80; 
                if (Physics.SphereCast (transform.position + pdiff,radius,transform.forward, out hit, sdistance)) { 
                    followingDistance = hit.distance;
                    dobject = hit.transform.name;

                    stoppingDistance = (1.5f * speed * 1000/3600)+ (speed*1000/3600)* (speed*1000/3600) / (2*9.8f*0.65f);
                    cpi = stoppingDistance / followingDistance;
                    if (cpi > 1 && speed > 1 && dobject.Contains("Car")) {
                        countUnsafe++;
                    }

//
//                if (  debugText != null) {
//                    debugText.text = hit.collider.tag +" dist:"+hit.distance+"  "+hit.collider.transform.localScale.z;
//                    //hit.collider.name;
//                }
                }
                if (speed > 1)  {
                    countALL ++;
                }
                if (countALL > 0) {
                    unsafeRatio = (1.0f*countUnsafe/countALL)*100;
                } else {
                    unsafeRatio = 0;
                }

                //   信号無視
    //            if (blockFinder.types[index] == 2) { 
    //                signalColor = signal.color;
    //                //                    if (0.01f <= speed && ) {
    //                //                        countRedOK ++;
    //                //                    } else {
    //                //                        countRedNG ++;
    //                //                    }
    //
    //            } else {
    //                signalColor = 0;
    //            }

                if (useBlock) {
                    if (prevIndex != index) {

                        // 速度
                        if (blockFinder.types[index] == 0) { 
                            
                            if (adequateSpeed - 10 <= speed && speed < adequateSpeed + 10) {
                                countSpeedOK ++;
                            } else {
                                countSpeedNG ++;
                            }

                        }

                        // ズレ/ふらつき
                        v3 = blockFinder.transforms[index+1].InverseTransformPoint(transform.position);
                        distance = v3.x;
                        if (-adequatedistance < distance && distance < adequatedistance) {
                            countPosOK ++;
                        } else {
                            countPosNG ++;
                        }
                        distanceSum += Mathf.Abs(distance);
                    }
                }



            }
                

            
        }

        void OnCollisionEnter (Collision col)
        {
            if (active ){
                Debug.Log(col.gameObject.name); // ぶつかった相手の名前を取得
                if(col.gameObject.name== "CarWaypointBased"|| col.gameObject.name == "CaraypointBasedOncomingCar")
                {
                    sound.PlayCrush();
                    resultCollision = true;
                    goCollision = col.gameObject;
                    if (col.gameObject == goFrontCar) {
                        resultFrontCar = true;
                    }

                }

                prevcolname = colname;
                colname = col.gameObject.name;
                colnameExit = col.gameObject.name ;
                if (prevcolname != colname) {
                    if (colname.IndexOf("Car")>-1) {
                        colCountCar++;
                    } else {
                        colCountEtc++;
                    }


                }
            }
    //        if(col.gameObject.name == "prop_powerCube")
    //        {
    //            Destroy(col.gameObject);
    //        }
        }

        void OnCollisionExit (Collision col)
        {
            if (active) {
              
        // colname = col.gameObject.name + " " +Time.time;
                //colname = "";
            }
            //        if(col.gameObject.name == "prop_powerCube")
            //        {
            //            Destroy(col.gameObject);
            //        }
        }


        void OnTriggerEnter(Collider col) {
            Debug.Log(gameObject.name + " : " + "Enter Trigger col  "+col.gameObject.name);

            if (col.gameObject.name == "SignalIgnoreCube"){
                // signalPosition = col.gameObject.transform.parent.transform.position;
                // signalDistance = Vector3.Distance(transform.position,signalPosition);
                goSignal = col.gameObject.transform.parent.gameObject;
                signal = goSignal.GetComponent<SignalController>();
                signalColor = signal.color;
                if (signalColor == 1 && prevSigName != goSignal.name) {
                    countIgnoreRed ++;
                    prevSigName = goSignal.name;
                }

            }
            if (col.gameObject.name == "RCube"){
                float rcubez = col.gameObject.transform.position.z;
                if (rcubez != prevPosZ) {
                    countRightTurnLane++;
                    prevPosZ = rcubez;
                }
            }
           
            if (col.gameObject.name=="jumpCube") 
            {
                resultGetJump = true;

                // if (Manager.selectMenu == 2 && Manager.unLockJump == false) {
                //     Manager.unLockJump = true;
                //     SceneManager.LoadScene("Start");
                // }      
            }        
            if (col.gameObject.name=="goalCube") 
            
            {
                resultGoal = true;
                // sound.PlayGoal();
                // if (Manager.selectMenu == 1 && Manager.unLockFreeRun == false) {
                //     Manager.unLockFreeRun = true;
                //     SceneManager.LoadScene("Start");
                // }
            }
        }

        void OnTriggerExit(Collider col) {
            Debug.Log(gameObject.name + " : " + "Exit Trigger col  "+col.gameObject.name);
            if (col.gameObject.name == "SignalIgnoreCube"){
                signalPosition = new Vector3();
                signalDistance = 0;
                //signalColor = 0;
            }
         
        }

        void OnTriggerStay(Collider col) {
            if (col.gameObject.name == "OCube"){
                inOppositeLane = true;
            } 

        }        


    }
}