using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Rotator : MonoBehaviour
{
    public Transform[] anchor;
    public GameObject[] UI;
    void Update()
    {
        for (int i = 0; i < anchor.Length; i++)
        {
            UI[i].transform.position = anchor[i].position;
        }
    }
}
