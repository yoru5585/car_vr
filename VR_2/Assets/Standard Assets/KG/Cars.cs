using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility; 

public class Cars : MonoBehaviour {
    public WaypointCircuit circuit;
    public string roadname = "Road";
    public string waypointname = "WaypointDOWN";




	// Use this for initialization
	void Start () {



        if (circuit == null) {
            GameObject rgo = GameObject.Find(roadname).gameObject;
            //Debug.Log("Cars");
            //Debug.Log(rgo.name);
            GameObject wgo = rgo.transform.Find("Waypoint").gameObject;
            GameObject wpgo = wgo.transform.Find(waypointname).gameObject;
            circuit = wpgo.GetComponent<WaypointCircuit>();

        }
		
	}
	
	// Update is called once per frame
	void Update () {


		
	}
}

