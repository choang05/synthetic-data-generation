using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnFirstPictureTaken : MonoBehaviour
{
    public GameObject objToSetActive;
    public bool setActiveOnFirstPictureLoaded = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        PictureTaker.OnPictureLoaded += OnPictureLoaded;  
    }

    private void OnDisable()
    {
        PictureTaker.OnPictureLoaded -= OnPictureLoaded;
    }

    private void OnPictureLoaded(Texture2D texture2d, RawImage rawImage, string path)
    {
        if (objToSetActive)
        {
            objToSetActive.SetActive(setActiveOnFirstPictureLoaded);
        }
        else
        {
            gameObject.SetActive(setActiveOnFirstPictureLoaded);
        }
    }
}
