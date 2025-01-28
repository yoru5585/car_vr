using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR;

public class check : MonoBehaviour
{
    [SerializeField] Toggle toggleA;
    [SerializeField] Toggle toggleB;
    [SerializeField] Toggle toggleX;
    [SerializeField] Toggle toggleY;
    [SerializeField] TextMeshProUGUI secondaryStick;
    [SerializeField] TextMeshProUGUI primaryStick;
    [SerializeField] TextMeshProUGUI secondaryTrigger;
    [SerializeField] TextMeshProUGUI primaryTrigger;

    

    Vector2 axis2D = Vector2.zero;
    float axis1D = 0;
    private void Start()
    {
        
    }

    private void Update()
    {
        if (OVRInput.Get(OVRInput.RawButton.A))
        {
            toggleA.isOn = true;
        }
        else
        {
            toggleA.isOn = false;
        }

        if (OVRInput.Get(OVRInput.RawButton.B))
        {
            toggleB.isOn = true;
        }
        else
        {
            toggleB.isOn = false;
        }

        if (OVRInput.Get(OVRInput.RawButton.X))
        {
            toggleX.isOn = true;
        }
        else
        {
            toggleX.isOn = false;
        }

        if (OVRInput.Get(OVRInput.RawButton.Y))
        {
            toggleY.isOn = true;
        }
        else
        {
            toggleY.isOn = false;
        }

        axis2D = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        primaryStick.text = "(" + axis2D.x + "," + axis2D.y + ")";
        axis2D = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        secondaryStick.text = "(" + axis2D.x + "," + axis2D.y + ")";

        axis1D = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        primaryTrigger.text = "(" + axis1D + ")";
        axis1D = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
        secondaryTrigger.text = "(" + axis1D + ")";
    }

    
}
