using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Signal : MonoBehaviour {
    GameObject goRed;
    GameObject goBlue;
    GameObject goYellow;
    public string singnalName;
    public int color;

	// Use this for initialization
	void Start () {
        goRed = transform.Find(singnalName+"/red").gameObject;
        goBlue = transform.Find(singnalName+"/blue").gameObject;
        goYellow = transform.Find(singnalName+"/yellow").gameObject;

//        goRed.SetActive(true);
//        goBlue.SetActive(false);
//        goYellow.SetActive(false);

		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setColor(int c) {
        if (goRed == null || goBlue == null || goYellow == null) {
            return;
        }
        
        color = c;
        if (c == 0) {
            goRed.SetActive(false);
            goBlue.SetActive(false);
            goYellow.SetActive(false);
        } else if (c == 1) {
            goRed.SetActive(true);
            goBlue.SetActive(false);
            goYellow.SetActive(false);
        } else if (c == 2) {
            goRed.SetActive(false);
            goBlue.SetActive(false);
            goYellow.SetActive(true);
        } else if (c == 3) {
            goRed.SetActive(false);
            goBlue.SetActive(true);
            goYellow.SetActive(false);
        }


    }
}

