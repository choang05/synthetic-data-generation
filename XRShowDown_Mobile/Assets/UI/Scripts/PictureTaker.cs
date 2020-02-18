using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PictureTaker : MonoBehaviour
{

    //Access Camera info from (WebCam/PhoneCam/etc)
    private WebCamTexture camera;

    //Texture/RawImage to render the CameraView
    public RawImage DefaultBackground;

    // For Debugging purposes
    public Text myText;





    void Start()
    {
        // Needs to look for cameras once the user has given us permision to access the cameras
        StartCoroutine("findCamera");
    }

    IEnumerator findCamera()
    {

        //This will trigger the phone to ask you to allow for permission
        WebCamDevice[] devices = WebCamTexture.devices;
        Debug.Log(devices.Length);
       
        if (devices.Length == 0 || devices.Length == 1)
        {
            //if we already have permision we should have more than 0 other wise we dont have cameras
            Debug.Log("no Cameras found");
            myText.text = "No Cameras found";

            yield return new WaitForSeconds(5.0f);
            StartCoroutine("findCamera");
        }
        else
        {
            //We have access to 1 or more cameras
            myText.text = "we got Cameras!  " + devices.Length;

            for (int i = 0; i < devices.Length; i++)
            {
                if (!devices[i].isFrontFacing)
                {
                    camera = new WebCamTexture(devices[i].name, Screen.height, Screen.width);
                }
                if (camera == null)
                {
                    myText.text = " we could not find back Camera";
                }
            }
            camera.Play();
            DefaultBackground.texture = camera;
            yield return 0;
        }
    }

    public void takePic()
    {
        Debug.Log("Taking Pictures");
    }
        
}
