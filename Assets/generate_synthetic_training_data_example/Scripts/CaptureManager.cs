﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CaptureManager : MonoBehaviour
{
    public static CaptureManager instance = null;              //Static instance which allows it to be accessed by any other script.

    [Header("Capture Parameters")]
    public bool capturePerScene = true;
    public Material[] skyboxesToCaptureIn;
    public GameObject[] objsToScan;
    public Camera camera;
    public int screenshots = 300;
    public int datasetImageSize = 512;
    public float distance = 5;
    public float distanceRandomOffset;
    public Vector3 cameraViewRandomOffsetRange;

    [Header("IO")]
    public string datasetDirPath = "TRAINING_DATA";
    public string datasetJsonName = "ml_dataset";
    [HideInInspector] public string classFolderName;

    //  Holds our data that maps image filenames and region coordinates
    //public Dictionary<string, double[]> imageNameToRegions = new Dictionary<string, double[]>();

    [Header("Debug")]
    public GameObject debugPrefab;
    public bool debugPoints = false;
    public float debugSize = .1f;

    private ScreenshotManager screenshotManager;
    private BoundsManager boundsManager;
    private CSVManager csvManager;

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

        screenshotManager = FindObjectOfType<ScreenshotManager>();
        boundsManager = FindObjectOfType<BoundsManager>();
        csvManager = FindObjectOfType<CSVManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        datasetDirPath = Path.Combine(Application.dataPath, "../", datasetDirPath);
        datasetDirPath = Path.GetFullPath(datasetDirPath);

        Screen.SetResolution(datasetImageSize, datasetImageSize, false);

        if (screenshotManager)
        {
            screenshotManager.captureHeight = datasetImageSize;
            screenshotManager.captureWidth = datasetImageSize;
        }
    }

    private void Update()
    {
        Rect rect = boundsManager.Get3dTo2dRect(objsToScan[0]);

        //print(string.Format("{0},{1},{2},{3}", rect.x, rect.y, rect.width+rect.x, rect.height+rect.y));

        //  correct bounding box coordinates to our dataset image size
        float xNorm = (datasetImageSize / Screen.width) * rect.x;
        float yNorm = (datasetImageSize / Screen.height) * rect.y;
        float x2Norm = (datasetImageSize / Screen.width) * rect.xMax;
        float y2Norm = (datasetImageSize / Screen.height) * rect.yMax;
        xNorm = Mathf.Clamp(xNorm / datasetImageSize, 0, 1);
        yNorm = Mathf.Clamp(yNorm / datasetImageSize, 0, 1);
        x2Norm = Mathf.Clamp(x2Norm / datasetImageSize, 0, 1);
        y2Norm = Mathf.Clamp(y2Norm / datasetImageSize, 0, 1);
        //print(string.Format("{0}, {1}, {2}, {3}", xNorm, yNorm, x2Norm, y2Norm));

    }

    [ContextMenu(nameof(CaptureObject))]
    public void CaptureObject()
    {
        Vector3[] points = GetSpherePoints(screenshots, distance);

        //  Delete previous dataset folder
        for (int i = 0; i < objsToScan.Length; i++)
        {
            string deletePath = Path.Combine(datasetDirPath, objsToScan[i].name);
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

            if (skyboxesToCaptureIn.Length > 0 && capturePerScene)
            {
                for (int i = 0; i < skyboxesToCaptureIn.Length; i++)
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

        string csvPath = csvManager.CreateCSVFromRows(null);
        
        Debug.Log(string.Format("Captured {0} images! Results in {1}", screenshotCount, datasetDirPath));
        Debug.Log(string.Format("Dataset csv created at {0}", csvPath));

        //DebugPoints();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="points"></param>
    /// <param name="objToScan"></param>
    private void CaptureImages(Vector3[] points, GameObject objToScan)
    {
        if (!camera || !objToScan || !screenshotManager || points.Length <= 0)
            return;

        //  Get bounds used to check if its visable within the camera. Get collider if avaliable, add one if not
        BoxCollider col = /*objToScan.GetComponent<Collider>()? objToScan.GetComponent<Collider>() :*/ objToScan.AddComponent<BoxCollider>();
        col.size *= .5f;
        Bounds visable_bounds = col.bounds;

        //Dictionary<string, double[]> imageNameToRegions = new Dictionary<string, double[]>();
        for (int i = 0; i < points.Length; i++)
        {
            camera.transform.position = points[i];
            camera.transform.LookAt(objToScan.transform);

            //  keep randomizing view until its within the shot
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            do
            {
                camera.transform.LookAt(objToScan.transform);

                //  random distance offset
                camera.transform.localPosition += new Vector3(0, 0, UnityEngine.Random.Range(-distanceRandomOffset, distanceRandomOffset));

                //  Add randomness to the view to assist training quality
                float ranX = UnityEngine.Random.Range(cameraViewRandomOffsetRange.x, -cameraViewRandomOffsetRange.x);
                float ranY = UnityEngine.Random.Range(cameraViewRandomOffsetRange.y, -cameraViewRandomOffsetRange.y);
                float ranZ = UnityEngine.Random.Range(cameraViewRandomOffsetRange.z, -cameraViewRandomOffsetRange.z);
                camera.transform.eulerAngles += new Vector3(ranX, ranY, ranZ);

                planes = GeometryUtility.CalculateFrustumPlanes(camera);

            } while (!GeometryUtility.TestPlanesAABB(planes, col.bounds));

            // folder name of the dataset object key
            //if (string.IsNullOrEmpty(classFolderName))
                classFolderName = objToScan.name;

            //  Take screenshot, calculate bounding box regions, and cache results
            string filename = screenshotManager.TakeScreenshot(datasetDirPath);
            Rect rect = boundsManager.Get3dTo2dRect(objToScan);

            //print(string.Format("{0},{1},{2},{3}", rect.x, rect.y, rect.width+rect.x, rect.height+rect.y));

            //  correct bounding box coordinates to our dataset image size
            float xNorm = (datasetImageSize / Screen.width) * rect.x;
            float yNorm = (datasetImageSize / Screen.height) * rect.y;
            float x2Norm = (datasetImageSize / Screen.width) * rect.xMax;
            float y2Norm = (datasetImageSize / Screen.height) * rect.yMax;
            xNorm = Mathf.Clamp(xNorm / datasetImageSize, 0, 1);
            yNorm = Mathf.Clamp(yNorm / datasetImageSize, 0, 1);
            x2Norm = Mathf.Clamp(x2Norm / datasetImageSize, 0, 1);
            y2Norm = Mathf.Clamp(y2Norm / datasetImageSize, 0, 1);

            //print("Screen: " + Screen.width + ", " + Screen.height);
            print(string.Format("{0}, {1}, {2}, {3}", xNorm, yNorm, x2Norm, y2Norm));

            //imageNameToRegions.Add(filename, new double[] {xNorm, yNorm, wNorm, hNorm});
            //print(string.Format("{0}, {1}", filename, new double[] { xNorm, yNorm, wNorm, hNorm }));

            //  cache csv rowdata
            string row = csvManager.GenerateRowString(CSVManager.AutoMLSets.UNASSIGNED, filename, objToScan.name, xNorm, yNorm, x2Norm, y2Norm);
            csvManager.AppendRowData(row);
        }

        //  dataset dict to json
        //string json = JsonConvert.SerializeObject(imageNameToRegions, Formatting.Indented);
        //string datasetFilepath = Path.Combine(datasetDirPath, datasetJsonName);
        //File.WriteAllText(datasetFilepath, json); 
    }

    [ContextMenu(nameof(DebugPoints))]
    public void DebugPoints()
    {
        Vector3[] points = GetSpherePoints(screenshots, distance);

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

    public Vector3[] GetSpherePoints(int numDirections, float distance)
    {
        Vector3[] directions = new Vector3[numDirections];

        float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        float angleIncrement = Mathf.PI * 2 * goldenRatio;

        for (int i = 0; i < numDirections; i++)
        {
            float t = (float)i / numDirections;
            float inclination = Mathf.Acos(1 - 2 * t);
            float azimuth = angleIncrement * i;

            float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth) * distance;
            float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth) * distance;
            float z = Mathf.Cos(inclination) * distance;
            directions[i] = new Vector3(x, y, z);
        }

        return directions;
    }
}