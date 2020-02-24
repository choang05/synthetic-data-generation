using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Rotator : MonoBehaviour
{

    private float turnSpeed = 15f;

    public bool ui = false;

    public Transform anchor;
    
    
    void Update()
    {
        if (!ui) // for my camer to rotate
        {
            transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime * 4);
            transform.Rotate(Vector3.left, -turnSpeed * Time.deltaTime * 2);
        }
        else
        {
            this.transform.position = anchor.position;
        }
    }
}
