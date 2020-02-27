using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LoadingAnimation : MonoBehaviour
{
    public Transform[] objectsToRotate;
    public Vector3[] speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        for (int i = 0; i < objectsToRotate.Length; i++)
        {
            objectsToRotate[i].eulerAngles += speed[i];
        }
    }
}
