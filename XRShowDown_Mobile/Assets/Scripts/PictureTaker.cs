using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class PictureTaker : MonoBehaviour
{
    [Header("Camera Settings")]
    public RawImage background;
    public AspectRatioFitter aspectFitter;
    public GameObject mainCameraPanel;
    public bool useDesktopWebcam = false;
    [HideInInspector] public Texture2D currentPicture;
    [HideInInspector] public string currentPicturePath;

    private bool isCameraAvaliable;
    private WebCamTexture deviceCamera;

    [Header("Debug Settings")]
    public Text debugText;

    //  Events
    public delegate void CameraEvents(Texture2D texture2d);
    public static CameraEvents OnPictureTaken;

    // Start is called before the first frame update
    void Start()
    {
        TryGetAndActivateCamera();

        //  Correct aspect fitter parameters
        if (aspectFitter)
            aspectFitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
    }


    // Update is called once per frame
    void Update()
    {
        if (mainCameraPanel && mainCameraPanel.activeSelf)
        {
            OrientCameraScreen();
        }
    }

    /// <summary>
    /// Method to try to get and turn on the back camera of the device its running on.
    /// </summary>
    void TryGetAndActivateCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            print("no webcam ");
            isCameraAvaliable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing || useDesktopWebcam)
            {
                deviceCamera = new WebCamTexture(devices[i].name);
            }
            deviceCamera = new WebCamTexture(devices[i].name, deviceCamera.width, deviceCamera.height);
        }

        if (deviceCamera == null)
        {
            print("No backcam");
            return;
        }

        deviceCamera.Play();
        background.texture = deviceCamera;

        isCameraAvaliable = true;
    }

    /// <summary>
    /// Correct orientation of camera screen (portrait/landscape) given a camera is activated
    /// </summary>
    void OrientCameraScreen()
    {
        if (!isCameraAvaliable)
        {
            TryGetAndActivateCamera();
            return;
        }

        float ratio = (float)deviceCamera.width / (float)deviceCamera.height;
        aspectFitter.aspectRatio = ratio;

        float scaleY = deviceCamera.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        int orient = -deviceCamera.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
    }

    /// <summary>
    /// Method to capture image from camera (android, webcam, etc.) given the WebCamTexture (camera texture) and saves it to given path
    /// </summary>
    /// <param name="webcamTexture"></param>
    /// <param name="directoryPath"></param>
    /// <returns></returns>
    public void ButtonTakePhoto()
    {
        StartCoroutine(TakePhoto(null));
    }
    public IEnumerator TakePhoto(string directoryPath)
    {
        if (!isCameraAvaliable)
            yield break;

        // NOTE - you almost certainly have to do this here:
        yield return new WaitForEndOfFrame();

        OrientCameraScreen();

        // it's a rare case where the Unity doco is pretty clear,
        // http://docs.unity3d.com/ScriptReference/WaitForEndOfFrame.html
        // be sure to scroll down to the SECOND long example on that doco page 

        Texture2D photo = new Texture2D(deviceCamera.width, deviceCamera.height);
        photo.SetPixels(deviceCamera.GetPixels());
        photo.Apply();

        //Encode to a PNG
        currentPicture = photo;
        byte[] bytes = photo.EncodeToPNG();

        if (string.IsNullOrEmpty(directoryPath))
        {
            directoryPath = Application.persistentDataPath;
        }

        currentPicturePath = Path.Combine(directoryPath + "lootbox_photo.png");

        //Write out the PNG.
        File.WriteAllBytes(currentPicturePath, bytes);

        //  Broadcast events
        OnPictureTaken?.Invoke(currentPicture);
        UpdateDebugInfo();
    }

    public void UpdateDebugInfo()
    {
        if (debugText && currentPicture)
        {
            debugText.text = currentPicturePath;
        }
    }
}
