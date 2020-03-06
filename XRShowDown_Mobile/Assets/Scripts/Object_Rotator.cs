using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Object_Rotator : MonoBehaviour
{

    public int rotSpeed = 3;

    public float minScale = 1;
    public float maxScale = 2;

    Vector3 initialScale;
    public Transform model;
    float initialFingerDistance;

    void OnMouseDrag()
    {
        if (Input.touchCount == 1)
        {
            float rotX = Input.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad;
            float rotY = Input.GetAxis("Mouse Y") * rotSpeed * Mathf.Deg2Rad;

            model.RotateAround(Vector3.up, -rotX);
            model.RotateAround(Vector3.right, rotY);
 
        }
        else if (Input.touchCount == 2)
        {
            scale();
        }
        else
        {
            float rotX = Input.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad;
            float rotY = Input.GetAxis("Mouse Y") * rotSpeed * Mathf.Deg2Rad;

            model.RotateAround(Vector3.up, -rotX);
            model.RotateAround(Vector3.right, rotY);
        }

    }

    void scale()
    {
        if (model.localScale.x > maxScale || model.localScale.x < minScale)
            model.localScale = new Vector3(1f, 1f, 1f);
        if (Input.touches.Length == 2)
        {
            Touch touch1 = Input.touches[0];
            Touch touch2 = Input.touches[1];

            if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
            {
                initialFingerDistance = Vector2.Distance(touch1.position, touch2.position);
                initialScale = model.localScale;
            }
            else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
            {
                float currentFingerDistance = Vector2.Distance(touch1.position, touch2.position);
                var scaleFactor = currentFingerDistance / initialFingerDistance;
                Vector3 scale = initialScale * scaleFactor;
                Debug.Log(scale.x + "  " + scale.y + "  " + scale.z);
                // Debug.Log(scale);

                scale.x = Mathf.Clamp(scale.x, minScale, maxScale);
                scale.y = Mathf.Clamp(scale.y, minScale, maxScale);
                scale.z = Mathf.Clamp(scale.z, minScale, maxScale);
                model.localScale = scale;

                this.gameObject.GetComponent<SphereCollider>().radius = scale.x * 1.75f;
            }
        }
    }
}