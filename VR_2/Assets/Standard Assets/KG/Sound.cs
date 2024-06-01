using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour {

    public AudioClip startEngine;
    public AudioClip correct; 
    public AudioClip incorrect; 
    public AudioClip goal; 
    public AudioClip roll; 
    public AudioClip rollfinish;
    public AudioClip omedetou;
    public AudioClip blinker;
    public AudioClip horn;   
    public AudioClip crush;   
    public AudioClip police;   
    public AudioClip ambulance;   
    public AudioClip jump;   
    public AudioClip dash;   
    

    private AudioSource audioSource;

	// Use this for initialization
	void Start () {
        audioSource = gameObject.GetComponent<AudioSource>();
        //audioSource.clip = correct;
        //audioSource.Play ();  
    }
	
    // Update is called once per frame
	void Update () {

	}

    public void PlayCorrect(){
        audioSource.clip = correct;
        audioSource.Play ();    
    }

    public void PlayIncorrect() {
        audioSource.clip = incorrect;
        audioSource.Play ();    
    }  

    public void PlayGoal() {
        audioSource.clip = goal;
        audioSource.Play ();    
    } 

    public void PlayRoll() {
        audioSource.clip = roll;
        audioSource.Play ();    
    } 

    public void PlayRollFinish() {
        audioSource.clip = rollfinish;
        audioSource.Play ();    
    } 

    public void PlayOmedetou(){
        
        audioSource.clip = omedetou;
        audioSource.Play ();    
    } 

    public void PlayStart(){

        audioSource.clip = startEngine;
        audioSource.Play ();    
    } 

    public void PlayBlinker(){

        audioSource.clip = blinker;
        audioSource.Play ();    
    } 

    public void PlayHorn(){

        audioSource.clip = horn;
        audioSource.Play ();    
    } 
    public void PlayCrush(){

        audioSource.clip = crush;
        audioSource.Play ();    
    } 
    public void PlayPolice(){
        audioSource.clip = police;
        audioSource.Play ();    
    } 
    public void PlayAmbulance(){
        audioSource.clip = ambulance;
        audioSource.Play ();    
    }

    public void PlayJump(){

        audioSource.clip = jump;
        audioSource.Play ();    
    } 
    public void PlayDash(){

        audioSource.clip = dash;
        audioSource.Play ();    
    } 


}
