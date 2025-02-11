using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility; 

namespace UnityStandardAssets.Vehicles.Car {
    public class CarsControl : MonoBehaviour, IRecieveMessage {


        public int number = 2;
        public int currentActiveNumber = 0;
        private Transform [] transformarray;
        private CarAIControl [] carAIControls;
        public string debugText;
        private float cull;



            public void OnReceive (int count)
            {
                Debug.Log("receive"+count);
                number = count;
            }




    	// Use this for initialization
    	void Start () {


            List<Transform> childList = new List<Transform>();
            foreach (Transform child in transform)
            {
                childList.Add(child);
                //Debug.Log(child.name);
            }

            //transformarray = childList.ToArray<Transform>();//gameObject.transform.GetComponentsInChildren<Transform>();
            //Debug.Log(" transformarray.Length "+transformarray.Length);
            carAIControls = new CarAIControl[childList.Count];
            for( int i = 0;i < childList.Count; i++) {
                
                carAIControls[i] = childList[i].gameObject.GetComponent<CarAIControl>();
                //Debug.Log(carAIControls[i].name);
            }

    	
    	}
    	
    	// Update is called once per frame
    	void Update () {

            cull += Time.deltaTime;
            if (cull > 1f) {
                cull -= 1f;
                int num = 0;
//                debugText = "";
                for( int i = 0;i < carAIControls.Length; i++) {
                    num += carAIControls[i].active ? 1 : 0;
//                    debugText += " "+ carAIControls[i].name+" "+ carAIControls[i].active;
                }
                currentActiveNumber = num;

                if (num < number) {
                    int i = Random.Range(0,carAIControls.Length);
                    if (!carAIControls[i].active) {
                        carAIControls[i].SetActive();
                    }

                }
            }
    		
    	}
    }
}
