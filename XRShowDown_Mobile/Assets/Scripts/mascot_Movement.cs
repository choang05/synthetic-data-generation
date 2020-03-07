using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mascot_Movement : MonoBehaviour
{

    public int rotSpeed = 3;
    public Transform model;
    Vector3 tempPos = new Vector3();
    Vector3 ogPos = new Vector3();

    private bool init = false;

    private void Start()
    {
        tempPos = model.position;
        ogPos = tempPos;
        init = true;


    }
    void OnMouseDrag()
    {

            float rotX = Input.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad;
            float rotY = Input.GetAxis("Mouse Y") * rotSpeed * Mathf.Deg2Rad;

            model.RotateAround(Vector3.up, -rotX);
            model.RotateAround(Vector3.right, rotY);

    }
    void OnEnable()
    {
        if (init)
        {
            model.position = ogPos;
            tempPos = model.position;
        }
    }

    void Update()
    {
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI) * .25f;

        model.position = tempPos;

    }
}