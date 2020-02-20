using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CaptureManager : MonoBehaviour
{
    public static CaptureManager instance = null;              //Static instance which allows it to be accessed by any other script.

    [Header("Capture Settings")]
    public bool capturePerScene = true;
    public float captureRadius = 5;
    public int screenshots = 300;
    public int datasetImageSize = 512;
    public GameObject[] objsToScan;
    [HideInInspector] public string currentCaptureObjName;

    [Header("Camera Settings")]
    public Camera camera;
    public float randomDistanceOffset = 3;
    public Vector3 randomCameraViewRotationOffset = new Vector3(10,10,180);

    [Header("Skybox Settings")]
    [Tooltip("Number of skyboxes to capture with. Default of -1 means use all avaliable skyboxes")]
    public int skyboxesToCapture = -1;
    public Material[] skyboxesToCaptureIn;

    [Header("Lights Settings")]
    public bool randomizeLights;
    public uint numScreenshotsPerLight = 1;

    [Header("Debug Settings")]
    public GameObject debugPrefab;
    public bool debugPoints = false;
    public float debugSize = .1f;

    private GameObject[] debugPointsGOs; 

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

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(datasetImageSize, datasetImageSize, false);

        if (ScreenshotManager.instance)
        {
            ScreenshotManager.instance.captureHeight = datasetImageSize;
            ScreenshotManager.instance.captureWidth = datasetImageSize;
        }
    }

    /// <summary>
    /// 1-click pipeline to capture images of obj, upload, train, and build model
    /// </summary>
    [ContextMenu(nameof(CaptureObject))]
    public void CaptureObject()
    {
        Vector3[] points = GetSpherePoints(screenshots, captureRadius);

        DatasetManager.instance.ClearDatasetData();

        //  Delete previous dataset folder
        for (int i = 0; i < objsToScan.Length; i++)
        {
            string deletePath = Path.Combine(DatasetManager.instance.datasetDirPath, objsToScan[i].name);
            FileUtil.DeleteFileOrDirectory(deletePath);
        }

        //  Hide all the objects initially
        for (int i = 0; i < objsToScan.Length; i++)
            objsToScan[i].SetActive(false);

        //  if there is skyboxes, capture screenshots for each skybox... else, just capture once in current scene
        Debug.Log("Starting capture...");
        int screenshotCount = 0;
        for (int j = 0; j < objsToScan.Length; j++)
        {
            objsToScan[j].SetActive(true);
            currentCaptureObjName = objsToScan[j].name;

            if (skyboxesToCaptureIn.Length > 0 && capturePerScene)
            {
                skyboxesToCapture = skyboxesToCapture <= -1 ? skyboxesToCaptureIn.Length : skyboxesToCapture;
                for (int i = 0; i < skyboxesToCapture; i++)
                {
                    Debug.Log(string.Format("Capturing in skybox {0}/{1}: {2}...", i+1, skyboxesToCaptureIn.Length, skyboxesToCaptureIn[i].name));
                    RenderSettings.skybox = skyboxesToCaptureIn[i];
                    CaptureImages(points, objsToScan[j]);
                    screenshotCount++;
                }
            }
            else
            {
                CaptureImages(points, objsToScan[j]);
                screenshotCount++;
            }

            objsToScan[j].SetActive(false);
        }

        //  Create dataset files
        string autoMLDatasetPath = DatasetManager.instance.CreateAutoMLDatasetFromRows(null);
        string customVisionDatasetPath = DatasetManager.instance.CreateCustomVisionDatasetFromRows(null);
        string tensorflowDatasetPath = DatasetManager.instance.CreateTensorflowDatasetFromRows(null);

        Debug.Log(string.Format("Captured {0} images! Results in {1}", screenshotCount, DatasetManager.instance.datasetDirPath));
        Debug.Log(string.Format("AutoML dataset created at {0}", autoMLDatasetPath));
        Debug.Log(string.Format("Tensorflow dataset created at {0}", tensorflowDatasetPath));
        Debug.Log(string.Format("Custom Vision dataset created at {0}", customVisionDatasetPath));

        //DebugPoints();
    }
    
    /// <summary>
    /// Given a list of locations and a gameobject, the camera will iterate through each location and take a screenshot of the gameobject 
    /// with variations.
    /// </summary>
    /// <param name="points"></param>
    /// <param name="objToScan"></param>
    private void CaptureImages(Vector3[] points, GameObject objToScan)
    {
        string label = objToScan.name;

        if (!camera || !objToScan || !ScreenshotManager.instance || points.Length <= 0)
            return;

        //Dictionary<string, double[]> imageNameToRegions = new Dictionary<string, double[]>();
        for (int i = 0; i < points.Length; i++)
        {
            camera.transform.position = points[i];
            camera.transform.LookAt(objToScan.transform);

            //  keep randomizing view until its within the shot
            bool isWithinCaptureScreen;
            do
            {
                camera.transform.LookAt(objToScan.transform);

                //  random distance offset
                camera.transform.localPosition += new Vector3(0, 0, UnityEngine.Random.Range(-randomDistanceOffset, randomDistanceOffset));

                //  Add randomness to the view to assist training quality
                float ranX = UnityEngine.Random.Range(randomCameraViewRotationOffset.x, -randomCameraViewRotationOffset.x);
                float ranY = UnityEngine.Random.Range(randomCameraViewRotationOffset.y, -randomCameraViewRotationOffset.y);
                float ranZ = UnityEngine.Random.Range(randomCameraViewRotationOffset.z, -randomCameraViewRotationOffset.z);
                camera.transform.eulerAngles += new Vector3(ranX, ranY, ranZ);

                //  get bounds and check if region is within capture screen
                (float _x, float _y, float _x2, float _y2) = GetNormalizedScreenRegion(objToScan);
                isWithinCaptureScreen = _x > 0 && _y > 0 && _x2 < 1 && _y2 < 1;
            } while (!isWithinCaptureScreen);

            // folder name of the dataset object key
            //if (string.IsNullOrEmpty(classFolderName))
                DatasetManager.instance.classFolderName = label;

            //  Take screenshot, calculate bounding box regions, and cache results
            string filename = ScreenshotManager.instance.TakeScreenshot(DatasetManager.instance.datasetDirPath);
            (float x, float y, float x2, float y2) = GetNormalizedScreenRegion(objToScan);

            //  generate row data for dataset (AutoML, Tensorflow, CustomVision, etc.)
            string row = DatasetManager.instance.GenerateAutoMLRowData(DatasetManager.AutoMLSets.UNASSIGNED, filename, label, x, y, x2, y2);
            DatasetManager.instance.AppendAutoMLRowData(row);
            string baseFilename = Path.GetFileName(filename);
            DatasetManager.instance.AppendCustomVisionRowData(baseFilename, new double[] { x, y, x2, y2 });
            string rowTF = DatasetManager.instance.GenerateTensorflowRowData(filename, datasetImageSize, datasetImageSize, label, x, y, x2, y2);
            DatasetManager.instance.AppendTensorflowRowData(rowTF);
        }
    }

    /// <summary>
    /// Given gameobject, return the region coordinates of it in screen space.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private (float, float, float, float) GetNormalizedScreenRegion(GameObject obj)
    {
        Rect rect = BoundsManager.instance.Get3dTo2dRect(obj);

        //  correct bounding box coordinates to our dataset image size. Convert screen space to dataset image resolution.
        float xNorm = rect.xMin;
        float yNorm = rect.yMin /*- 475*/;  //  [BUG] The Y position gets offsetted on capture. The -475 is to offset it back for resolution of 512x512.
        float x2Norm = rect.width + xNorm;
        float y2Norm = rect.height + yNorm;
        //print(string.Format("{0}, {1}, {2}, {3}", xNorm, yNorm, x2Norm - rect.x, y2Norm - rect.y));

        xNorm = Mathf.Clamp(xNorm / datasetImageSize, 0, 1);
        yNorm = Mathf.Clamp(yNorm / datasetImageSize, 0, 1);
        x2Norm = Mathf.Clamp(x2Norm / datasetImageSize, 0, 1);
        y2Norm = Mathf.Clamp(y2Norm / datasetImageSize, 0, 1);

        return (xNorm, yNorm, x2Norm, y2Norm);
    }

    /// <summary>
    /// [TODO] Given a light object, randomize its position (if its non-directional) or its rotation (if its directional).
    /// </summary>
    /// <param name="lightGO"></param>
    public void RandomizeLights(GameObject lightGO)
    {
        Light light = lightGO.GetComponent<Light>();
        if (light)
        {
            //  if light is directional light, rotate it randomly instead.
            float ranX = UnityEngine.Random.Range(0, 360);
            float ranY = UnityEngine.Random.Range(0, 360);
            float ranZ = UnityEngine.Random.Range(0, 360);
            light.transform.eulerAngles = new Vector3(ranX, ranY, ranZ);
        }
    }

    /// <summary>
    /// Generates a bunch of spheres to visualize where the points of where the camera will take screenshots at.
    /// </summary>
    [ContextMenu(nameof(GenerateCameraDebugPoints))]
    public void GenerateCameraDebugPoints()
    {
        Vector3[] points = GetSpherePoints(screenshots, captureRadius);

        if (debugPointsGOs != null)
            //  Clean debug objects
            for (int i = 0; i < debugPointsGOs.Length; i++)
                if (debugPointsGOs[i] != null)
                    Destroy(debugPointsGOs[i]);
        debugPointsGOs = new GameObject[points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            //  spawn empty gameobjects if arent debugging, else render the points
            if (!debugPoints)
            {
                GameObject pointGO = new GameObject("point_" + points[i].ToString());
                pointGO.transform.SetParent(transform);
                pointGO.transform.position = points[i];
                debugPointsGOs[i] = pointGO;
            }
            else
            {
                GameObject debugGO;
                if (debugPrefab)
                {
                    debugGO = Instantiate(debugPrefab, points[i], Quaternion.identity, transform);
                    debugGO.transform.localScale = new Vector3(debugSize, debugSize, debugSize);
                    debugPointsGOs[i] = debugGO;
                }
                else
                {
                    debugGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    debugGO.transform.SetParent(transform);
                    debugGO.transform.position = points[i];
                    debugGO.transform.localScale = new Vector3(debugSize, debugSize, debugSize);
                    debugPointsGOs[i] = debugGO;
                }
            }
        }
    }

    /// <summary>
    /// Generate and return a array of uniform points in a sphere
    /// </summary>
    /// <param name="numDirections"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public Vector3[] GetSpherePoints(int numDirections, float radius)
    {
        Vector3[] directions = new Vector3[numDirections];

        float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        float angleIncrement = Mathf.PI * 2 * goldenRatio;

        for (int i = 0; i < numDirections; i++)
        {
            float t = (float)i / numDirections;
            float inclination = Mathf.Acos(1 - 2 * t);
            float azimuth = angleIncrement * i;

            float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth) * radius;
            float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth) * radius;
            float z = Mathf.Cos(inclination) * radius;
            directions[i] = new Vector3(x, y, z);
        }

        return directions;
    }
}
