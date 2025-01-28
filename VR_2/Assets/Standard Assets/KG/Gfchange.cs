using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gfchange : MonoBehaviour
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
    void OnTriggerEnter(Collider other){
        if(other.CompareTag("Player")){
            goSignal = transform.parent.gameObject.transform.parent.gameObject;
            signal = goSignal.GetComponent<SignalController>();
            signal.active = true;
        }
    }
                
}
