using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PictureTaker : MonoBehaviour
{

    public int num = 0;
    private WebCamDevice cameraDevice;
    private WebCamTexture camera;

    public RawImage DefaultBackground;

    public Text myText;


    // Start is called before the first frame update
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.Log("no Cameras found");
            myText.text = "No Cameras found";

        }
        else
            myText.text = "we got some Cameras!  " + devices.Length;

        for (int i = 0; i < devices.Length; i++)
        {

            if (!devices[i].isFrontFacing)
            {
                camera = new WebCamTexture(devices[i].name, Screen.height, Screen.width);
            }

            if(camera == null)
            {
                myText.text = " we could not find back Camera";
            }


          

        }
    }

    // Update is called once per frame
    void Update()
    {

    }


   


    public void takePic()
    {
        num = num +1;   //   num++;
        Debug.Log("You took " + num + "  pic");

        camera.Play();
        DefaultBackground.texture = camera;

    }
        
}
