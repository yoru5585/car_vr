using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSetting : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.parent.name == "Colliders_p")
        {
            //loading...������
            Debug.Log("delete loading");
            GameObject.Find("ExampleCanvas").transform.GetChild(3).gameObject.SetActive(false);
            //������ʂ��o��
            GameObject.Find("ExampleCanvas").transform.GetChild(2).gameObject.SetActive(true);
        }
    }
}
