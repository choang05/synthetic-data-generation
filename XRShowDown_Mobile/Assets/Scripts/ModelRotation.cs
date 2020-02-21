using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelRotation : MonoBehaviour
{
    public GameObject model;

    public int minScale;
    public int maxScale;
    
    float initialFingerDistance;
    Vector3 initialScale;
    float firstPoint;
    float secondPoint;

    //int increment = 0;

    
    public float rotationSpeed = 0.05f;
    void OnMouseDrag(){
        if(Input.touchCount == 1){
            float XaxisRotation = Input.GetAxis("Mouse X")*rotationSpeed;
            float YaxisRotation = Input.GetAxis("Mouse Y")*rotationSpeed;
            transform.RotateAround(Vector3.down, XaxisRotation);
            transform.RotateAround(Vector3.right, YaxisRotation);
        }
        else if(Input.touchCount == 2){
            scale();
        }
    }

    /*void Update()
    {
        if(Input.touchCount == 0){
            increment = 0;
        }
        else if(model==null){

        }
        else if(Input.touchCount == 1){
            float rotateSpeed = 0.9f;
            Touch touchZero = Input.GetTouch(0);
            Vector3 localAngle = model.transform.localEulerAngles;
            localAngle.y -= rotateSpeed*touchZero.deltaPosition.x;
            localAngle.x = (localAngle.x + rotateSpeed*touchZero.deltaPosition.y)%359;
            model.transform.localEulerAngles = localAngle;
        }
        else if(Input.touches.Length == 2){
            scale();
        }
    }

    void rotate(bool right){
        if(right){
            model.transform.Rotate(Vector3.up*Time.deltaTime*200f);
        }
        else{
             model.transform.Rotate(Vector3.down*Time.deltaTime*200f);
        }
    }*/
    
    void scale(){
        if(Input.touches.Length == 2){
            Touch touch1 = Input.touches[0];
            Touch touch2 = Input.touches[1];

            if(touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began){
                initialFingerDistance = Vector2.Distance(touch1.position, touch2.position);
                initialScale = model.transform.localScale;
            }
            else if(touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved){
                float currentFingerDistance = Vector2.Distance(touch1.position, touch2.position);
                var scaleFactor = currentFingerDistance/initialFingerDistance;
                Vector3 scale = initialScale*scaleFactor;
                scale.x = Mathf.Clamp(scale.x, minScale, maxScale);
                scale.y = Mathf.Clamp(scale.y, minScale, maxScale);
                scale.z = Mathf.Clamp(scale.z, minScale, maxScale);
                model.transform.localScale = scale;
            }
        }
    }
}