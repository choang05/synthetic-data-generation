using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using System;

public class CloudServiceManager : MonoBehaviour
{
    public static CloudServiceManager instance = null;              //Static instance which allows it to be accessed by any other script.

    [Header("API/Service Settings")]
    public string PredictionKey = "c2854719f84840f6b0b456562652db05";
    public string MicrosoftCustomeVisionAIURL = "https://lootbox.cognitiveservices.azure.com/customvision/v3.0/Prediction/a8d69bbf-9fcb-4a44-9f6e-7b3aa1e2fff0/classify/iterations/Iteration3/image";

    public Text debuggerText;
    public string imagePath;


    //  Events
    public delegate void CloudServiceEvent(string path);
    public static CloudServiceEvent OnImageRecognized;

    private void Awake()
    {
        #region Instance
        //Check if instance already exists
        if (instance == null)
        {
            //if not, set instance to this
            instance = this;
        }
        //If instance already exists and it's not this:
        else if (instance != this)
        {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        }

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
        #endregion
    }

    //private void Start()
    //{
    //    StartCoroutine(RecognizeImage(null, null, imagePath));
    //}

    private void OnEnable()
    {
        PictureTaker.OnPictureLoaded += OnPictureLoaded;
    }

    private void OnDisable()
    {
        PictureTaker.OnPictureLoaded -= OnPictureLoaded;
    }

    private void OnPictureLoaded(Texture2D texture2d, RawImage rawImage, string imagePath)
    {
        StopAllCoroutines();

        StartCoroutine(RecognizeImage(texture2d, rawImage, imagePath));
    }

    /// <summary>
    /// Connects to the Custom Vision Service and uploads image
    /// </summary>
    /// <param name="imagePath"></param>
    /// <returns></returns>
    public IEnumerator RecognizeImage(Texture2D texture2d, RawImage rawImage, string imagePath)
    {
        //Set the URL
        UnityWebRequest request = new UnityWebRequest();

        //Set the headers
        request.method = "POST";
        request.url = MicrosoftCustomeVisionAIURL;
        request.SetRequestHeader("Prediction-Key", PredictionKey);
        request.SetRequestHeader("Content-Type", "application/octet-stream");

        //Set the image as the body of the request
        byte[] byteData = GetImageAsByteArray(imagePath);
        //Texture2D rawImageTexture = (Texture2D)rawImage.texture;
        //byte[] byteData = texture2d.GetRawTextureData();

        //Create upload and download handlers
        UploadHandlerRaw upHandler = new UploadHandlerRaw(byteData);
        upHandler.contentType = "application/octet-stream";
        request.uploadHandler = upHandler;
        DownloadHandler downHandler = new DownloadHandlerBuffer();
        request.downloadHandler = downHandler;

        //Make the request
        yield return request.SendWebRequest();

        debuggerText.text = "PROCESSING";

        //Error handling
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);

            debuggerText.text = request.error;
        }
        else
        {
            try
            {
                //  Parse JSON
                JObject parsed = JObject.Parse(request.downloadHandler.text);

                //  get all predictions
                //foreach (JToken myToken in parsed["predictions"])
                //{
                //    string tagName = myToken["tagName"].ToString();
                //    float probability = );

                //    debuggerText.text += (tagName + " : " + probability);
                //    debuggerText.text += "\n";
                //}

                //  fetch highest prediction
                string tagName = (string)parsed["predictions"][0]["tagName"];
                float probability = (float)parsed["predictions"][0]["probability"];
                debuggerText.text += (tagName + " : " + probability);

                //  Broadcast events
                //OnImageRecognized?.Invoke(resultJson);
            }
            catch (Exception e)
            {
                Debug.Log("AAAAUGGH!\n" + e.Message);
            }
        }
    }

    /// <summary>
    /// This gets the file we need to load in.
    /// </summary>
    /// <param name="imageFilePath"></param>
    /// <returns></returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    private static byte[] GetImageAsByteArray(string imageFilePath)
    {
        FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
        BinaryReader binaryReader = new BinaryReader(fileStream);
        return binaryReader.ReadBytes((int)fileStream.Length);
    }
}

