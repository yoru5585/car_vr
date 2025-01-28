using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CheckPose : MonoBehaviour
{
    public void Pose(string log)
    {
        GameObject.Find("LogText").GetComponent<TextMeshProUGUI>().text = log;
    }

    public void test_test(int i)
    {
        
        GameObject obj = GameObject.Find("Player2_vis");
        Pose(obj.name);
        obj.gameObject.transform.GetChild(0).gameObject.GetComponent<ChangeHands>().Change(i);
    }
}
