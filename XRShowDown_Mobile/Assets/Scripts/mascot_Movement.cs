using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mascot_Movement : MonoBehaviour
{

    public int rotSpeed = 3;
    public Transform model;
    Vector3 tempPos = new Vector3();


    private void Start()
    {
        tempPos = model.position;

    }
    void OnMouseDrag()
    {

            float rotX = Input.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad;
            float rotY = Input.GetAxis("Mouse Y") * rotSpeed * Mathf.Deg2Rad;

            model.RotateAround(Vector3.up, -rotX);
            model.RotateAround(Vector3.right, rotY);

    }

   
    void Update()
    {
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI) * .25f;

        model.position = tempPos;

    }
}