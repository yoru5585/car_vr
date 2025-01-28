using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gchange : MonoBehaviour
{
    GameObject gc;
    public GameObject goSignal;
    public SignalController signal;

    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.Find("Mycar");

    }

    // Update is called once per frame
    void Update()
    {
    }
    void OnTriggerStay(Collider other){
        if(other.CompareTag("Player")){
            float v = gc.GetComponent<Rigidbody>().velocity.magnitude;
            if(v < 1){
                Debug.Log("ok");

                goSignal = transform.parent.gameObject.transform.parent.gameObject;
                signal = goSignal.GetComponent<SignalController>();
                signal.active = true;
                
            }
        }
    }
}
