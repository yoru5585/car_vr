using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility; // 追加 KG
using UnityEngine.UI;

namespace UnityStandardAssets.Vehicles.Car
{
public class TireAdjuster : MonoBehaviour {

    public GameObject wheelHubFrontRight;
    public GameObject wheelHubRearRight;
    public GameObject wheelHubFrontLeft;
    public GameObject wheelHubRearLeft;

    public GameObject tFL;
    public GameObject tFR;
    public GameObject tRL;
    public GameObject tRR;

        public GameObject tFLT;

    public CarController carController ;

	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () {
		
	}

    [ContextMenu("Adjust ")]
    private void Adjust()
    {
        carController = GetComponent<CarController>();


        var children = GetComponentsInChildren<Transform>(true);
        foreach ( var trans in children )
        {
            if ( trans.name == "WheelHubFrontRight" )
            {
                wheelHubFrontRight = trans.gameObject;
            }
            if ( trans.name == "WheelHubRearRight" )
            {
                wheelHubRearRight = trans.gameObject;
            }
            if ( trans.name == "WheelHubFrontLeft" )
            {
                wheelHubFrontLeft = trans.gameObject;
            }
            if ( trans.name == "WheelHubRearLeft" )
            {
                wheelHubRearLeft = trans.gameObject;
            }


            if ( trans.name == "TFL" )
            {
                tFL = trans.gameObject;
            }
            if ( trans.name == "TFR" )
            {
                tFR = trans.gameObject;
            }
            if ( trans.name == "TRL" )
            {
                tRL = trans.gameObject;
            }
            if ( trans.name == "TRR" )
            {
                tRR = trans.gameObject;
            }

            if ( trans.name == "TFLT" )
            {
                tFLT = trans.gameObject;
            }

        }

            Vector3 offset = new Vector3(0,0.2f,0);
            wheelHubFrontLeft.transform.position =  offset+tFL.transform.position;
            wheelHubFrontRight.transform.position = offset+tFR.transform.position;
            wheelHubRearLeft.transform.position =   offset+tRL.transform.position;
            wheelHubRearRight.transform.position =  offset+tRR.transform.position;
//       
       carController.m_WheelMeshes[0] = tFR;
           carController.m_WheelMeshes[1] = tFL;
           carController.m_WheelMeshes[2] = tRR;
           carController.m_WheelMeshes[3] = tRL;

            // wheelHubFrontRight.GetComponent<WheelCollider>().radius = tFL.transform.position.y;
            // wheelHubFrontLeft.GetComponent<WheelCollider>().radius = tFL.transform.position.y;
            // wheelHubRearRight.GetComponent<WheelCollider>().radius = tFL.transform.position.y ;
            // wheelHubRearLeft.GetComponent<WheelCollider>().radius = tFL.transform.position.y ;

            wheelHubFrontRight.GetComponent<WheelCollider>().radius = tFL.transform.localPosition.y;
            wheelHubFrontLeft.GetComponent<WheelCollider>().radius = tFL.transform.localPosition.y;
            wheelHubRearRight.GetComponent<WheelCollider>().radius = tFL.transform.localPosition.y ;
            wheelHubRearLeft.GetComponent<WheelCollider>().radius = tFL.transform.localPosition.y ;

            // wheelHubFrontRight.GetComponent<WheelCollider>().radius = tFL.transform.position.y * tFL.transform.scale.y;
            // wheelHubFrontLeft.GetComponent<WheelCollider>().radius = tFL.transform.position.y * tFL.transform.scale.y;
            // wheelHubRearRight.GetComponent<WheelCollider>().radius = tFL.transform.position.y * tFL.transform.scale.y;
            // wheelHubRearLeft.GetComponent<WheelCollider>().radius = tFL.transform.position.y * tFL.transform.scale.y;

         

    }
}

}