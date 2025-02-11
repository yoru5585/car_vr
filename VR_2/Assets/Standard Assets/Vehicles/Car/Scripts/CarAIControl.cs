using System;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityStandardAssets.Utility; // 追加 KG
using UnityEngine.UI;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    //[RequireComponent(typeof(GM))]
    public class CarAIControl : MonoBehaviour
    {
        public enum BrakeCondition
        {
            NeverBrake,                 // the car simply accelerates at full throttle all the time.
            TargetDirectionDifference,  // the car will brake according to the upcoming change in direction of the target. Useful for route-based AI, slowing for corners.
            TargetDistance,             // the car will brake as it approaches its target, regardless of the target's direction. Useful if you want the car to
                                        // head for a stationary target and come to rest when it arrives there.
        }

        // This script provides input to the car controller in the same way that the user control script does.
        // As such, it is really 'driving' the car, with no special physics or animation tricks to make the car behave properly.

        // "wandering" is used to give the cars a more human, less robotic feel. They can waver slightly
        // in speed and direction while driving towards their target.

        [SerializeField] [Range(0, 1)] private float m_CautiousSpeedFactor = 0.05f;               // percentage of max speed to use when being maximally cautious
        [SerializeField] [Range(0, 180)] private float m_CautiousMaxAngle = 50f;                  // angle of approaching corner to treat as warranting maximum caution
        [SerializeField] private float m_CautiousMaxDistance = 100f;                              // distance at which distance-based cautiousness begins
        [SerializeField] private float m_CautiousAngularVelocityFactor = 30f;                     // how cautious the AI should be when considering its own current angular velocity (i.e. easing off acceleration if spinning!)
        [SerializeField] private float m_SteerSensitivity = 0.05f;                                // how sensitively the AI uses steering input to turn to the desired direction
        [SerializeField] private float m_AccelSensitivity = 0.04f;                                // How sensitively the AI uses the accelerator to reach the current desired speed
        [SerializeField] private float m_BrakeSensitivity = 1f;                                   // How sensitively the AI uses the brake to reach the current desired speed
        [SerializeField] private float m_LateralWanderDistance = 3f;                              // how far the car will wander laterally towards its target
        [SerializeField] private float m_LateralWanderSpeed = 0.1f;                               // how fast the lateral wandering will fluctuate
        [SerializeField] [Range(0, 1)] private float m_AccelWanderAmount = 0.1f;                  // how much the cars acceleration will wander
        [SerializeField] private float m_AccelWanderSpeed = 0.1f;                                 // how fast the cars acceleration wandering will fluctuate
        [SerializeField] private BrakeCondition m_BrakeCondition = BrakeCondition.TargetDistance; // what should the AI consider when accelerating/braking?
        [SerializeField] private bool m_Driving;                                                  // whether the AI is currently actively driving or stopped.
        [SerializeField] private Transform m_Target;                                              // 'target' the target object to aim for.
        [SerializeField] private bool m_StopWhenTargetReached;                                    // should we stop driving when we reach the target?
        [SerializeField] private float m_ReachTargetThreshold = 2;                              // proximity to target to consider we 'reached' it, and stop driving.
        

        private float m_RandomPerlin;             // A random value for the car to base its wander on (so that AI cars don't all wander in the same pattern)
        private CarController m_CarController;    // Reference to actual car controller we are controlling
        private float m_AvoidOtherCarTime;        // time until which to avoid the car we recently collided with
        private float m_AvoidOtherCarSlowdown;    // how much to slow down due to colliding with another car, whilst avoiding
        private float m_AvoidPathOffset;          // direction (-1 or 1) in which to offset path to avoid other car, whilst avoiding
        private Rigidbody m_Rigidbody;

        public Text debugText;
        public bool move;

        //private Vector3 initpos;

        public bool active = true;


        WaypointProgressTracker wpt;// 追加 KG

        public float targetSpeed = 40;
        public float currentTargetSpeed = 40;
        public float followingDistance;
        public float followingDistanceM;
        public float followingDistanceX;        
        public float followingDistanceS; //Signal
        public float steer;
        public float targetAngle;
        public float desiredSpeed;

        public float signalDistance;
        public Vector3 signalPosition;
        public GameObject goSignal;
        public SignalController signal;
        public int signalColor;

        public float adjustrate= 0.5f;

        public GameObject mycar;
        private BlockFinder blockFinder;

        public bool brakelamp = false;
        public  Renderer brakeLampON;
        public Renderer brakeLampOFF;

        private float cull;

        public GameObject go;
        public GameObject go2;
        public Vector3 rayRot;
        public float rayDist;     

        public float wait = 0;

        // 急停止
        public bool quickstop = false;   

        // 前方で待機
        public bool waitStart = false;
        public float waitFrontDistance = 100;
        public float distanceMycar;

        // 前方に離れたら減速
        public bool slowMode = false;


        void Start(){// 追加 KG
            go = new GameObject("LookAt");
            go2 = new GameObject("LookAt2");
            wpt = transform.GetComponent<WaypointProgressTracker>();
            wpt.Init(transform.position,transform.localRotation);
            //initpos = transform.position;
            this.mycar = GameObject.Find("Mycar");
            blockFinder = mycar.GetComponent<BlockFinder>();

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


//           Debug.Log(" brakeLampON "+brakeLampON);
        }

        public void SetActive(){
            wpt.SetActive();
            quickstop = false;
        }

        private void Awake()
        {
            // get the car controller reference
            m_CarController = GetComponent<CarController>();

            // give the random perlin a random value
            m_RandomPerlin = Random.value*100;

            m_Rigidbody = GetComponent<Rigidbody>();
        }
        //int count = 0;
        public void Stop()
        {
            //wpt.active = false;
            wait = 10;            
        }

        private void FixedUpdate()
        {
            brakelamp = false;


            active = wpt.active;
            if (wait> 0){
                wait -= Time.deltaTime;
                active = false;
                if (wait <= 0) {
                    wait = 0;
                }
            }          
            if (waitStart) {
                distanceMycar = Vector3.Distance(mycar.transform.position,transform.position);
                if (distanceMycar > waitFrontDistance) {
                    active = false;
                    wpt.active = false;
                    return;
                } else {
                    waitStart = false;
                    wpt.active = true;
                }


            }  

            if (!active || quickstop) {
//                transform.position = wpt.initpos;
//                transform.localRotation = wpt.initqua;
                transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
                brakelamp = true;
                SetBrakeLamp(-1);
                return;
            }


            if (m_Target == null || !m_Driving)
            {
                // Car should not be moving,
                // use handbrake to stop
                m_CarController.Move(0, 0, -1f, 1f);
            }
            else
            {
                // Vector3 rayPos0 = wpt.circuit.Waypoints[wpt.progressNum].position;
                // Vector3 rayPos1 = wpt.circuit.Waypoints[wpt.progressNum+1].position;
                // Transform tf = go.transform;
                // tf.position = rayPos0;
                // tf.LookAt(rayPos1);
                // Transform tf2 = go2.transform;
                // tf2.position = rayPos1;
                // //Vector3 
                // rayRot = tf.forward;
                // //float 
                // rayDist = Vector3.Distance(rayPos0,rayPos1);
                // Debug.DrawRay(rayPos0,rayRot*rayDist,Color.red,0,false);
                  

                currentTargetSpeed = targetSpeed;
                followingDistance = 127;
                followingDistanceM = 127;
                followingDistanceX = 127;
                followingDistanceS = 127;
                // KG Signal

                if (signalDistance != 0) {
                    signalDistance = Vector3.Distance(transform.position,signalPosition);
                    signalColor = signal.color;
                    if ( signalColor == 1 ) { //&& signalDistance * 2 < m_CarController.currentSpeed) {
                        followingDistanceS = signalDistance;
                    }
                }

                // KG
                RaycastHit hit;
                RaycastHit hit2Right;
                RaycastHit hit2Left;
                Vector3 pdiff = (transform.forward * 1f) + (transform.up * 1.1f);//new Vector3(0f, 1.1f, 1f);
                Vector3 pdiff2Right = pdiff + (transform.right * 1.1f) ;//new Vector3(1.1f, 1.1f, 1f);
                Vector3 pdiff2Left = pdiff + (transform.right * -1.1f) ;//new Vector3(-1.1f, 1.1f, 1f);
                int distance = 20;
                int distance2Right = 5;
                int distanceLeft = 5;
                float radius = 0.5f; 
                //Debug.DrawLine(transform.position,transform.position+(wpt.target.position-transform.position).normalized*distance);
//                Debug.DrawRay(transform.position + pdiff, transform.forward*distance, Color.red,0,false);
                if (  debugText != null) {
                    debugText.text = "";
                }
                //Corgiレイヤーとだけ衝突しない
                //int layerMask = ~(1 << 9);
                //                if (Physics.SphereCast (transform.position + pdiff,radius,transform.forward, out hit, distance)) { 
                // if (Physics.SphereCast (transform.position + pdiff,radius,(wpt.target.position-transform.position).normalized, out hit, distance)) {//半径,長さ
                if (Physics.SphereCast (transform.position + pdiff,radius,transform.forward, out hit, distance)) {//半径,長さ
                    if (gameObject.name != "goalCube")
                    {
                        followingDistanceM = hit.distance;
                    }
                    

                    if (  debugText != null) {
                        debugText.text = hit.collider.tag +" dist:"+hit.distance+"  "+hit.collider.transform.localScale.z;
                            //hit.collider.name;
                    }
                    // Debug.DrawRay(transform.position+ pdiff,(wpt.target.position-transform.position).normalized*hit.distance,Color.red,0,false);
                    Debug.DrawRay(transform.position+ pdiff,transform.forward*hit.distance,Color.red,0,false);
                } else {
                    // Debug.DrawRay(transform.position+ pdiff,(wpt.target.position-transform.position).normalized*distance,Color.blue,0,false);
                    Debug.DrawRay(transform.position+ pdiff,transform.forward*distance,Color.blue,0,false);
                }

                // WayPointに沿って車間距離を計算
                float wptdistance = 0;
                int startIndex = wpt.progressNum;
                // 開始地点を設定
                for(int i = wpt.progressNum+1; i < wpt.circuit.Waypoints.Length; i++) {
                    rayDist = Vector3.Distance(transform.position, wpt.circuit.Waypoints[i].position);
                    if (rayDist > 3) {
                        startIndex = i;
                        wptdistance = rayDist;
                        break;
                    }
                }
                // WayPoint毎に距離計測を繰り返す
                for(int i = startIndex; i < wpt.circuit.Waypoints.Length-1; i++) {
                    Vector3 rayPos0 = wpt.circuit.Waypoints[i].position;
                    Vector3 rayPos1 = wpt.circuit.Waypoints[i+1].position;
                    Transform tf = go.transform;
                    tf.position = rayPos0;
                    tf.LookAt(rayPos1);
                    Transform tf2 = go2.transform;
                    tf2.position = rayPos1;
                    //Vector3 
                    rayRot = tf.forward;
                    //float 
                    rayDist = Vector3.Distance(rayPos0,rayPos1);
                    if (Physics.SphereCast (rayPos0,radius,rayRot, out hit, rayDist)) {//半径,長さ
                        Debug.DrawRay(rayPos0,rayRot*hit.distance,Color.red,0,false);
                        wptdistance += hit.distance;
                        followingDistanceX = wptdistance;
                        break;
                    } else {
                        Debug.DrawRay(rayPos0,rayRot*rayDist,Color.green,0,false);
                        wptdistance += rayDist;
                        if (wptdistance > 80) {
                            break;
                        }
                    }

                }                    
                followingDistance = Mathf.Min(followingDistanceM,followingDistanceX);
                //右のRay
                // if (Physics.SphereCast(transform.position + pdiff2Right, radius, (wpt.target.position - transform.position).normalized, out hit2Right, distance2Right)) {//半径,長さ
                if (Physics.SphereCast(transform.position + pdiff2Right, radius,transform.forward, out hit2Right, distance2Right)) {//半径,長さ
                    followingDistance = Mathf.Min(followingDistance,hit2Right.distance);


                    if (debugText != null)
                    {
                        debugText.text = hit2Right.collider.tag + " dist2Right:" + hit2Right.distance + "  " + hit2Right.collider.transform.localScale.z;
                        //hit.collider.name;
                    }
                    Debug.DrawRay(transform.position + pdiff2Right, transform.forward * hit2Right.distance, Color.red, 0, false);
                } else {
                    Debug.DrawRay(transform.position + pdiff2Right, transform.forward * distance2Right, Color.black, 0, false);

                }
                //左のRay
                // if (Physics.SphereCast(transform.position + pdiff2Left, radius, (wpt.target.position - transform.position).normalized, out hit2Left, distanceLeft))
                if (Physics.SphereCast(transform.position + pdiff2Left, radius,transform.forward, out hit2Left, distanceLeft))
                {//半径,長さ
                    followingDistance = Mathf.Min(followingDistance,hit2Left.distance);

                    if (debugText != null)
                    {
                        debugText.text = hit2Left.collider.tag + " dist2Right:" + hit2Left.distance + "  " + hit2Left.collider.transform.localScale.z;
                        //hit.collider.name;
                    }
                    Debug.DrawRay(transform.position + pdiff2Left, transform.forward * hit2Left.distance, Color.red, 0, false);

                } else {
                    Debug.DrawRay(transform.position + pdiff2Left, transform.forward * distanceLeft, Color.grey, 0, false);

                }


                followingDistance = Mathf.Min(followingDistance,followingDistanceS);


                if (currentTargetSpeed > followingDistance+10) { // 車間距離の2倍より速度がでていたら目標速度を下げる
                    currentTargetSpeed = followingDistance+10;
/////                    brakelamp = true;
                }
                if ( followingDistance < 10.0f) {  // 3m未満なら目標速度 0km/h
                    currentTargetSpeed = 10;
                    brakelamp = true;
                }                
                if ( followingDistance < 3.0f) {  // 3m未満なら目標速度 0km/h
                    currentTargetSpeed = 0;
                    brakelamp = true;
                }

                // 自車から遠い時は減速　10km/h
                if (slowMode){
                    float mycarz = mycar.transform.position.z;//mycarz
                    float z = transform.position.z;//自車z
                    // debug = "mycarz " + (int)mycarz + " z " + (int)z + "  " + (int)(z - mycarz);
                    //if (z - (mycarz - resetdistance) < 0)
                    if (z -mycarz > 80 && currentTargetSpeed > 10)
                    {         
                        currentTargetSpeed = 10;         
                    }
                }
                //                ////////////////////////////////////////////////////////////// 追加 KG  位置の強制移動
                //count ++;
                //if (count % 10 == 0) {
                // circuit.GetRoutePosition(progressDistance)
                //                Vector3 pos = wpt.circuit.GetRoutePosition(wpt.progressDistance);
                //                transform.position = pos*(adjustrate)+transform.position*(1-adjustrate);
                //
                //transform.localRotation = wpt.target.localRotation;
                //}



                Vector3 fwd = transform.forward;
                if (m_Rigidbody.velocity.magnitude > m_CarController.MaxSpeed*0.1f)
                {
                    fwd = m_Rigidbody.velocity;
                }

//                float desiredSpeed = m_CarController.MaxSpeed;
                desiredSpeed = currentTargetSpeed;  // KG

                // now it's time to decide if we should be slowing down...
                switch (m_BrakeCondition)
                {
                    case BrakeCondition.TargetDirectionDifference:
                        {
                            // the car will brake according to the upcoming change in direction of the target. Useful for route-based AI, slowing for corners.

                            // check out the angle of our target compared to the current direction of the car
                            float approachingCornerAngle = Vector3.Angle(m_Target.forward, fwd);

                            // also consider the current amount we're turning, multiplied up and then compared in the same way as an upcoming corner angle
                            float spinningAngle = m_Rigidbody.angularVelocity.magnitude*m_CautiousAngularVelocityFactor;

                            // if it's different to our current angle, we need to be cautious (i.e. slow down) a certain amount
                            float cautiousnessRequired = Mathf.InverseLerp(0, m_CautiousMaxAngle,
                                                                           Mathf.Max(spinningAngle,
                                                                                     approachingCornerAngle));
                            desiredSpeed = Mathf.Lerp(m_CarController.MaxSpeed, m_CarController.MaxSpeed*m_CautiousSpeedFactor,
                                                      cautiousnessRequired);
                            break;
                        }

                    case BrakeCondition.TargetDistance:
                        {
                            // the car will brake as it approaches its target, regardless of the target's direction. Useful if you want the car to
                            // head for a stationary target and come to rest when it arrives there.

                            // check out the distance to target
                            Vector3 delta = m_Target.position - transform.position;
                            float distanceCautiousFactor = Mathf.InverseLerp(m_CautiousMaxDistance, 0, delta.magnitude);

                            // also consider the current amount we're turning, multiplied up and then compared in the same way as an upcoming corner angle
                            float spinningAngle = m_Rigidbody.angularVelocity.magnitude*m_CautiousAngularVelocityFactor;

                            // if it's different to our current angle, we need to be cautious (i.e. slow down) a certain amount
                            float cautiousnessRequired = Mathf.Max(
                                Mathf.InverseLerp(0, m_CautiousMaxAngle, spinningAngle), distanceCautiousFactor);
//                            desiredSpeed = Mathf.Lerp(m_CarController.MaxSpeed, m_CarController.MaxSpeed*m_CautiousSpeedFactor,
                            desiredSpeed = Mathf.Lerp(currentTargetSpeed,currentTargetSpeed*m_CautiousSpeedFactor,
                                                      cautiousnessRequired);
                            break;
                        }

                    case BrakeCondition.NeverBrake:
                        break;
                }

                // Evasive action due to collision with other cars:

                // our target position starts off as the 'real' target position
                Vector3 offsetTargetPos = m_Target.position;

//                // if are we currently taking evasive action to prevent being stuck against another car:
//                if (Time.time < m_AvoidOtherCarTime)
//                {
//                    // slow down if necessary (if we were behind the other car when collision occured)
//                    desiredSpeed *= m_AvoidOtherCarSlowdown;
//
//                    // and veer towards the side of our path-to-target that is away from the other car
//                    offsetTargetPos += m_Target.right*m_AvoidPathOffset;
//                }
//                else
//                {
//                    // no need for evasive action, we can just wander across the path-to-target in a random way,
//                    // which can help prevent AI from seeming too uniform and robotic in their driving
//                    offsetTargetPos += m_Target.right*
//                                       (Mathf.PerlinNoise(Time.time*m_LateralWanderSpeed, m_RandomPerlin)*2 - 1)*
//                                       m_LateralWanderDistance;
//                }

                // use different sensitivity depending on whether accelerating or braking:
                float accelBrakeSensitivity = (desiredSpeed < m_CarController.CurrentSpeed)
                                                  ? m_BrakeSensitivity
                                                  : m_AccelSensitivity;

                // decide the actual amount of accel/brake input to achieve desired speed.
                float accel = Mathf.Clamp((desiredSpeed - m_CarController.CurrentSpeed)*accelBrakeSensitivity, -1, 1);

                // add acceleration 'wander', which also prevents AI from seeming too uniform and robotic in their driving
                // i.e. increasing the accel wander amount can introduce jostling and bumps between AI cars in a race
                accel *= (1 - m_AccelWanderAmount) +
                         (Mathf.PerlinNoise(Time.time*m_AccelWanderSpeed, m_RandomPerlin)*m_AccelWanderAmount);

                // calculate the local-relative position of the target, to steer towards
                Vector3 localTarget = transform.InverseTransformPoint(offsetTargetPos);

                // work out the local angle towards the target
                //float 
                targetAngle = Mathf.Atan2(localTarget.x, localTarget.z)*Mathf.Rad2Deg;

                // get the amount of steering needed to aim the car towards the target
               // float 
                steer = Mathf.Clamp(targetAngle*m_SteerSensitivity, -1, 1)*Mathf.Sign(m_CarController.CurrentSpeed);





                // feed input to the car controller.
                if (move) {
                    if (  debugText != null) {
                        debugText.text = "accel "+ accel;
                    }
                    m_CarController.Move(steer, accel, accel, 0f);
                }
                SetBrakeLamp(accel);
                
                // if appropriate, stop driving when we're close enough to the target.
                if (m_StopWhenTargetReached && localTarget.magnitude < m_ReachTargetThreshold)
                {
                    m_Driving = false;
                }
            }
        }

        void SetBrakeLamp(float accel){
                cull += Time.deltaTime;
                if (cull > 0.25f) {
                    cull -= 0.25f;

                    if (m_CarController.currentSpeed < 0.01f) {
                        brakelamp = true;
                    }
                    if (accel < -0.01f) {
                        brakelamp = true;
                    }
                    brakeLampON.enabled = brakelamp ;
                    brakeLampOFF.enabled = !brakelamp;
                }
        }

        private void OnCollisionStay(Collision col)
        {
            // detect collision against other cars, so that we can take evasive action
            if (col.rigidbody != null)
            {
                var otherAI = col.rigidbody.GetComponent<CarAIControl>();
                if (otherAI != null)
                {
                    // we'll take evasive action for 1 second
                    m_AvoidOtherCarTime = Time.time + 1;

                    // but who's in front?...
                    if (Vector3.Angle(transform.forward, otherAI.transform.position - transform.position) < 90)
                    {
                        // the other ai is in front, so it is only good manners that we ought to brake...
                        m_AvoidOtherCarSlowdown = 0.5f;
                    }
                    else
                    {
                        // we're in front! ain't slowing down for anybody...
                        m_AvoidOtherCarSlowdown = 1;
                    }

                    // both cars should take evasive action by driving along an offset from the path centre,
                    // away from the other car
                    var otherCarLocalDelta = transform.InverseTransformPoint(otherAI.transform.position);
                    float otherCarAngle = Mathf.Atan2(otherCarLocalDelta.x, otherCarLocalDelta.z);
                    m_AvoidPathOffset = m_LateralWanderDistance*-Mathf.Sign(otherCarAngle);
                }
            }
        }


        public void SetTarget(Transform target)
        {
            m_Target = target;
            m_Driving = true;
        }

        // 信号のエリアに入ったら対象の信号を調べて停止位置への距離を計算
        void OnTriggerEnter(Collider col) {
            //Debug.Log(gameObject.name + " : " + "Enter Trigger col  "+col.gameObject.name);
            if (col.gameObject.name == "SignalCube"){
                signalPosition = col.gameObject.transform.parent.transform.position;
                signalDistance = Vector3.Distance(transform.position,signalPosition);
                goSignal = col.gameObject.transform.parent.gameObject.transform.parent.gameObject;
                signal = goSignal.GetComponent<SignalController>();
            }

        }

        // 信号のエリアからでたら対象信号はなしに
        void OnTriggerExit(Collider col) {
            Debug.Log(gameObject.name + " : " + "Exit Trigger col  "+col.gameObject.name);
            if (col.gameObject.name == "SignalCube"){
                signalPosition = new Vector3();
                signalDistance = 0;
                signalColor = 0;
            }
        }

//        void OnTriggerStay(Collider col) {
//            Debug.Log(gameObject.name + " : " + "Stay Trigger col  "+col.gameObject.name);
//        }
    }
}
