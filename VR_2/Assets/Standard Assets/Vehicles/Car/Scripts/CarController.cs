using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityStandardAssets.Utility;
using static UnityStandardAssets.Utility.WaypointCircuit;
//(Vector3、座標)https://tech.pjin.jp/blog/2021/12/23/unity-script-transform/
//(元の座標に値をセットする、座標)https://ekulabo.com/cant-change-position#outline__1

namespace UnityStandardAssets.Vehicles.Car
{

    internal enum CarDriveType
    {
        FrontWheelDrive,
        RearWheelDrive,
        FourWheelDrive
    }

    internal enum SpeedType
    {
        MPH,
        KPH
    }
    //[RequireComponent(typeof(GM))]
    public class CarController : MonoBehaviour
    {

        //public GM gm;
        public static CarController instance = null;
        [SerializeField] private CarDriveType m_CarDriveType = CarDriveType.FourWheelDrive;
        [SerializeField] public WheelCollider[] m_WheelColliders = new WheelCollider[4];
        [SerializeField] public GameObject[] m_WheelMeshes = new GameObject[4];
        [SerializeField] private WheelEffects[] m_WheelEffects = new WheelEffects[4];
        [SerializeField] private Vector3 m_CentreOfMassOffset;
        [SerializeField] private float m_MaximumSteerAngle;
        [Range(0, 1)] [SerializeField] private float m_SteerHelper; // 0 is raw physics , 1 the car will grip in the direction it is facing
        [Range(0, 1)] [SerializeField] private float m_TractionControl; // 0 is no traction control, 1 is full interference
        [SerializeField] private float m_FullTorqueOverAllWheels;
        [SerializeField] private float m_ReverseTorque;
        [SerializeField] private float m_MaxHandbrakeTorque;
        [SerializeField] private float m_Downforce = 100f;
        [SerializeField] private SpeedType m_SpeedType;
        [SerializeField] private float m_Topspeed = 200;

        [SerializeField] private static int NoOfGears = 5;
        [SerializeField] private float m_RevRangeBoundary = 1f;
        [SerializeField] private float m_SlipLimit;
        [SerializeField] private float m_BrakeTorque;
        //[SerializeField] private float m_z;
        //[SerializeField] private float m_zAI;
        [SerializeField] private Vector3 m_zAI;
        [SerializeField] private float m_zInFrontOfAI;
        //[SerializeField] private float m_zmy;
        [SerializeField] private Vector3 m_zmy;


        private Quaternion[] m_WheelMeshLocalRotations;
        private Vector3 m_Prevpos, m_Pos;
        private float m_SteerAngle;
        private int m_GearNum;
        private float m_GearFactor;
        private float m_OldRotation;
        private float m_CurrentTorque;
        private Rigidbody m_Rigidbody;
        private const float k_ReversingThreshold = 0.01f;

        public bool Skidding { get; private set; }
        public float BrakeInput { get; private set; }
        public float CurrentSteerAngle { get { return m_SteerAngle; } }
        //        public float CurrentSpeed{ get { return m_Rigidbody.velocity.magnitude*2.23693629f; }}
        public float CurrentSpeed { get { return m_Rigidbody.velocity.magnitude * 3.6f; } }
        public float MaxSpeed { get { return m_Topspeed; } }
        public float Revs { get; private set; }
        public float AccelInput { get; private set; }

        public float currentSpeed; // 追加 KG
        public float before;
        public GameObject bloff;

        public bool reverse = false;
        public bool neutral = false;
        public bool parking = false;
        public float revs;
        public string debugtext;
        public GameObject myCar;
        public GameObject oncomingCar;
        public GameObject infrontofCar;
        public GameObject m_gmcar;
        //public GM gm;
        Vector3 pos;
        Transform carWaypointBasedTransform;

        GameObject parent;
        GameObject child;
        //public int zmyzAICount=390;


        //[SerializeField] private Transform waypoint;
        //List<string> test = new List<string>();
        //public int test;
        Transform test;
        float carWaypointBasedX;


        // Use this for initialization
        private void Start()
        {
            //bloff.SetActive(true);
            m_WheelMeshLocalRotations = new Quaternion[4];
            for (int i = 0; i < 4; i++)
            {
                m_WheelMeshLocalRotations[i] = m_WheelMeshes[i].transform.localRotation;
            }
            m_WheelColliders[0].attachedRigidbody.centerOfMass = m_CentreOfMassOffset;

            m_MaxHandbrakeTorque = float.MaxValue;

            m_Rigidbody = GetComponent<Rigidbody>();
            m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl * m_FullTorqueOverAllWheels);

            oncomingCar = GameObject.Find("CarWaypointBasedOncomingCar");
            myCar = GameObject.Find("Mycar");
            infrontofCar = GameObject.Find("CarWaypointBased");
            m_gmcar = GameObject.Find("GM");

            //infrontofCar =this.gameObject;
            // transformを取得
            //carWaypointBasedTransform = infrontofCar.transform;

            //carWaypointBasedTransform = oncomingCar.transform;

            // 座標を取得
            //pos = carWaypointBasedTransform.position;

            //m_zAI = oncomingCar.transform.position.z;
            //m_zmy = myCar.transform.position.z;

            //m_zAI = oncomingCar.transform.position;
            //m_zmy = myCar.transform.position;

            //parent = transform.Find("Waypoins6").gameObject;
            //pparent = transform.Find("signal6/Waypoins6").gameObject;

            //test=WaypointCircuit.instance.numPoints;
            //test = WaypointList.instance.iiii;
            //test = WaypointList.instance.items[0];
            //Debug.Log(test);

            //float carWaypointBasedX = infrontofCar.transform.position.x;
        }

        private void GearChanging()
        {
            float f = Mathf.Abs(CurrentSpeed / MaxSpeed);
            float upgearlimit = (1 / (float)NoOfGears) * (m_GearNum + 1);
            float downgearlimit = (1 / (float)NoOfGears) * m_GearNum;

            if (m_GearNum > 0 && f < downgearlimit)
            {
                m_GearNum--;
            }

            if (f > upgearlimit && (m_GearNum < (NoOfGears - 1)))
            {
                m_GearNum++;
            }

        }


        // simple function to add a curved bias towards 1 for a value in the 0-1 range
        private static float CurveFactor(float factor)
        {
            return 1 - (1 - factor) * (1 - factor);
        }


        // unclamped version of Lerp, to allow value to exceed the from-to range
        private static float ULerp(float from, float to, float value)
        {
            return (1.0f - value) * from + value * to;
        }


        private void CalculateGearFactor()
        {
            float f = (1 / (float)NoOfGears);
            // gear factor is a normalised representation of the current speed within the current gear's range of speeds.
            // We smooth towards the 'target' gear factor, so that revs don't instantly snap up or down when changing gear.
            var targetGearFactor = Mathf.InverseLerp(f * m_GearNum, f * (m_GearNum + 1), Mathf.Abs(CurrentSpeed / MaxSpeed));
            m_GearFactor = Mathf.Lerp(m_GearFactor, targetGearFactor, Time.deltaTime * 5f);
        }


        private void CalculateRevs()
        {
            // calculate engine revs (for display / sound)
            // (this is done in retrospect - revs are not used in force/power calculations)
            if (!neutral)
            {
                CalculateGearFactor();
                var gearNumFactor = m_GearNum / (float)NoOfGears;
                var revsRangeMin = ULerp(0f, m_RevRangeBoundary, CurveFactor(gearNumFactor));
                var revsRangeMax = ULerp(m_RevRangeBoundary, 1f, gearNumFactor);
                Revs = ULerp(revsRangeMin, revsRangeMax, m_GearFactor);
            }
            else
            {
                Revs = AccelInput;
            }
            revs = Revs;
        }


        public void Move(float steering, float accel, float footbrake, float handbrake)
        {
            if (GM.instance.sceneCount == 1)
            {
                m_Topspeed = 100;
            }
            else if (GM.instance.sceneCount == 2)
            {
                m_Topspeed = 60;
            }
            else if (GM.instance.sceneCount == 3)
            {
                m_Topspeed = 40;
            }
            else
            {
                //m_gmcar.;
                //myTransform.position = pos;  // 座標を設定
                
            }
            /*
            m_zInFrontOfAI = infrontofCar.transform.position.z;
            if (m_zInFrontOfAI > 3007.53589)
            {
                //infrontofCar.SetActive(false);

                carWaypointBasedX += 100f;
            }
            */
            /*m_zAI = oncomingCar.transform.position;
            m_zmy = myCar.transform.position;

            if (m_zmy.z - m_zAI.z > 20f)
            {
                //pos.z = m_zAI + 1000f;
                //m_zAI = m_zmy + 1000f;
                //transform.position = m_zAI; // ローカル変数を代入
                m_zAI.z = m_zAI.z+30f;
                //waypointをとってくる
                //子オブジェクトの順番で取得。最初が0で二番目が1となる。つまり↓は最初の子オブジェクト
                //child = parent.transform.GetChild(zmyzAICount).gameObject;
                //zmyzAICount-=10;
                m_zAI = new Vector3(m_zmy.x+6f, m_zAI.y, m_zAI.z+60f); // (,,)に移動
                oncomingCar.transform.position = m_zAI;
            }*/

            /*if (m_zmy - m_zAI > 20)
            {
                m_zAI = m_zmy + 1000f;
            }*/


            for (int i = 0; i < 4; i++)
            {
                Quaternion quat;
                Vector3 position;
                m_WheelColliders[i].GetWorldPose(out position, out quat);
                m_WheelMeshes[i].transform.position = position;
                m_WheelMeshes[i].transform.rotation = quat;
            }

            //clamp input values
            steering = Mathf.Clamp(steering, -1, 1);
            AccelInput = accel = Mathf.Clamp(accel, 0, 1);
            BrakeInput = footbrake = -1 * Mathf.Clamp(footbrake, -1, 0);
            handbrake = Mathf.Clamp(handbrake, 0, 1);

            //Set the steer on the front wheels.
            //Assuming that wheels 0 and 1 are the front wheels.
            m_SteerAngle = steering * m_MaximumSteerAngle;
            m_WheelColliders[0].steerAngle = m_SteerAngle;
            m_WheelColliders[1].steerAngle = m_SteerAngle;

            SteerHelper();
            ApplyDrive(accel, footbrake);
            CapSpeed();

            if (parking)
            {
                handbrake = 1;
            }

            //Set the handbrake.
            //Assuming that wheels 2 and 3 are the rear wheels.
            if (handbrake > 0f)
            {
                var hbTorque = handbrake * m_MaxHandbrakeTorque;
                m_WheelColliders[2].brakeTorque = hbTorque;
                m_WheelColliders[3].brakeTorque = hbTorque;
            }


            CalculateRevs();
            GearChanging();

            AddDownForce();
            CheckForWheelSpin();
            TractionControl();



        }


        private void CapSpeed()
        {
            float speed = m_Rigidbody.velocity.magnitude;
            switch (m_SpeedType)
            {
                case SpeedType.MPH:

                    speed *= 2.23693629f;
                    if (speed > m_Topspeed)
                        m_Rigidbody.velocity = (m_Topspeed / 2.23693629f) * m_Rigidbody.velocity.normalized;
                    break;

                case SpeedType.KPH:
                    speed *= 3.6f;
                    debugtext = speed + " " + m_Topspeed;
                    if (speed > m_Topspeed)
                    {
                        m_Rigidbody.velocity = (m_Topspeed / 3.6f) * m_Rigidbody.velocity.normalized;
                        debugtext += "limit";
                    }
                    break;
            }
            currentSpeed = speed; //追加KG
            before =currentSpeed;
        }


        private void ApplyDrive(float accel, float footbrake)
        {

            float r = reverse ? -1 : 1;

            float thrustTorque;
            switch (m_CarDriveType)
            {
                case CarDriveType.FourWheelDrive:
                    thrustTorque = neutral ? 0 : accel * (m_CurrentTorque / 4f) * r;
                    for (int i = 0; i < 4; i++)
                    {
                        m_WheelColliders[i].motorTorque = thrustTorque;
                    }
                    break;

                case CarDriveType.FrontWheelDrive:
                    thrustTorque = accel * (m_CurrentTorque / 2f) * 4;
                    m_WheelColliders[0].motorTorque = m_WheelColliders[1].motorTorque = thrustTorque;
                    break;

                case CarDriveType.RearWheelDrive:
                    thrustTorque = accel * (m_CurrentTorque / 2f) * 4;
                    m_WheelColliders[2].motorTorque = m_WheelColliders[3].motorTorque = thrustTorque;
                    break;

            }
            // if (CurrentSpeed > before)
            // {
            //     bloff.SetActive(false);
            // }
            for (int i = 0; i < 4; i++)
            {
                //                if (CurrentSpeed > 5 && Vector3.Angle(transform.forward, m_Rigidbody.velocity) < 50f)
                //                {
                //                    m_WheelColliders[i].brakeTorque = m_BrakeTorque*footbrake;
                //                }
                //                else if (footbrake > 0)
                //                {
                //                    m_WheelColliders[i].brakeTorque = 0f;
                //                    m_WheelColliders[i].motorTorque = -m_ReverseTorque*footbrake;
                //                }
                if (CurrentSpeed > 0)
                {
                    m_WheelColliders[i].brakeTorque = m_BrakeTorque * footbrake;
                }
                //                else if (footbrake > 0)
                //                {
                //                    m_WheelColliders[i].brakeTorque = 0f;
                //                    m_WheelColliders[i].motorTorque = -m_ReverseTorque*footbrake;
                //                }
            }
        }


        private void SteerHelper()
        {
            for (int i = 0; i < 4; i++)
            {
                WheelHit wheelhit;
                m_WheelColliders[i].GetGroundHit(out wheelhit);
                if (wheelhit.normal == Vector3.zero)
                    return; // wheels arent on the ground so dont realign the rigidbody velocity
            }

            // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
            if (Mathf.Abs(m_OldRotation - transform.eulerAngles.y) < 10f)
            {
                var turnadjust = (transform.eulerAngles.y - m_OldRotation) * m_SteerHelper;
                Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
                m_Rigidbody.velocity = velRotation * m_Rigidbody.velocity;
            }
            m_OldRotation = transform.eulerAngles.y;
        }


        // this is used to add more grip in relation to speed
        private void AddDownForce()
        {
            m_WheelColliders[0].attachedRigidbody.AddForce(-transform.up * m_Downforce *
                                                         m_WheelColliders[0].attachedRigidbody.velocity.magnitude);
        }


        // checks if the wheels are spinning and is so does three things
        // 1) emits particles
        // 2) plays tiure skidding sounds
        // 3) leaves skidmarks on the ground
        // these effects are controlled through the WheelEffects class
        private void CheckForWheelSpin()
        {
            // loop through all wheels
            for (int i = 0; i < 4; i++)
            {
                WheelHit wheelHit;
                m_WheelColliders[i].GetGroundHit(out wheelHit);

                // is the tire slipping above the given threshhold
                if (Mathf.Abs(wheelHit.forwardSlip) >= m_SlipLimit || Mathf.Abs(wheelHit.sidewaysSlip) >= m_SlipLimit)
                {
                    m_WheelEffects[i].EmitTyreSmoke();

                    // avoiding all four tires screeching at the same time
                    // if they do it can lead to some strange audio artefacts
                    if (!AnySkidSoundPlaying())
                    {
                        m_WheelEffects[i].PlayAudio();
                    }
                    continue;
                }

                // if it wasnt slipping stop all the audio
                if (m_WheelEffects[i].PlayingAudio)
                {
                    m_WheelEffects[i].StopAudio();
                }
                // end the trail generation
                m_WheelEffects[i].EndSkidTrail();
            }
        }

        // crude traction control that reduces the power to wheel if the car is wheel spinning too much
        private void TractionControl()
        {
            WheelHit wheelHit;
            switch (m_CarDriveType)
            {
                case CarDriveType.FourWheelDrive:
                    // loop through all wheels
                    for (int i = 0; i < 4; i++)
                    {
                        m_WheelColliders[i].GetGroundHit(out wheelHit);

                        AdjustTorque(wheelHit.forwardSlip);
                    }
                    break;

                case CarDriveType.RearWheelDrive:
                    m_WheelColliders[2].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);

                    m_WheelColliders[3].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);
                    break;

                case CarDriveType.FrontWheelDrive:
                    m_WheelColliders[0].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);

                    m_WheelColliders[1].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);
                    break;
            }
        }


        private void AdjustTorque(float forwardSlip)
        {
            if (forwardSlip >= m_SlipLimit && m_CurrentTorque >= 0)
            {
                m_CurrentTorque -= 10 * m_TractionControl;
            }
            else
            {
                m_CurrentTorque += 10 * m_TractionControl;
                if (m_CurrentTorque > m_FullTorqueOverAllWheels)
                {
                    m_CurrentTorque = m_FullTorqueOverAllWheels;
                }
            }
        }


        private bool AnySkidSoundPlaying()
        {
            for (int i = 0; i < 4; i++)
            {
                if (m_WheelEffects[i].PlayingAudio)
                {
                    return true;
                }
            }
            return false;
        }
        public void Update()
        {
            // if (myCar.transform.position.z - this.transform.position.z <= 5f && myCar.transform.position.z - this.transform.position.z > 0) //&& myCar.transform.position.x+1.1f - this.transform.position.x > 0 )
            // {
            //     m_Topspeed = 0;
            //     //infrontofCar.
            // }
            // else
            // {
            //     m_Topspeed = 60;
            // }
        }
    }
}
