using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindCamera : MonoBehaviour
{
    
    void Start()
    {
        Camera cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        this.gameObject.GetComponent<Canvas>().worldCamera = cam;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
