using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour
{
    public float xAngle, yAngle, zAngle;
    public float speed;

 

    void Awake()
    {


      //  this.transform.position = new Vector3(-0.75f, 0.0f, 0.0f);
      //  this.transform.Rotate(90.0f, 0.0f, 0.0f, Space.World);
       // this.GetComponent<Renderer>().material.color = Color.green;
      //  this.name = "World";
    }

    void Update()
    {

        this.transform.Rotate(xAngle, yAngle, zAngle, Space.World);
    }
}
