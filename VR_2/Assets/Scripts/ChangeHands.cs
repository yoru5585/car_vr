using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChangeHands : MonoBehaviour
{
    GameObject[] hand = new GameObject[3];

    private void Start()
    {
        for (int i = 0; i < 3; i ++)
        {
            hand[i] = transform.GetChild(i).gameObject;
        }
           
    }

    public void Change(int num)
    {
        for (int i = 0; i < hand.Length; i++)
        {
            hand[i].SetActive(false);
        }
        hand[num].SetActive(true);
    }
}
