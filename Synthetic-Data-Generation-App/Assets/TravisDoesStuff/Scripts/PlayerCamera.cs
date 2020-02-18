using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject cam;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.A)){
            cam.GetComponent<Rigidbody>().AddForce(-1,0,0);
        }
        if(Input.GetKey(KeyCode.S)){
            cam.GetComponent<Rigidbody>().AddForce(0,0,-1);
        }
        if(Input.GetKey(KeyCode.D)){
            cam.GetComponent<Rigidbody>().AddForce(1,0,0);
        }
        if(Input.GetKey(KeyCode.W)){
            cam.GetComponent<Rigidbody>().AddForce(0,0,1);
        }
        if(Input.GetKey(KeyCode.Space)){
            cam.GetComponent<Rigidbody>().AddForce(0,1,0);
        }
        if(Input.GetKey(KeyCode.LeftShift)){
            cam.GetComponent<Rigidbody>().AddForce(0,-1,0);
        }
    }
}
