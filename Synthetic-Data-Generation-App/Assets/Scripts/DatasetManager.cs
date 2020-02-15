using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;
using Newtonsoft.Json;

public class DatasetManager : MonoBehaviour
{
    public static DatasetManager instance = null;              //Static instance which allows it to be accessed by any other script.

    [Header("IO")]
    public string datasetDirPath = "TRAINING_DATA";
    public string datasetFileName = "dataset_labels";
    [HideInInspector] public string classFolderName;

    [Header("AutoML")]
    public string autoMLBucketName = "xr-showdown-2020";
    public string autoMLPrefixImageDirectoryName = "images";

    public enum AutoMLSets
    {
        TRAIN,
        VALIDATE,
        TEST,
        UNASSIGNED
    }

    //  Holds our data that maps image filenames and region coordinates
    private List<string> autoMLData = new List<string>();
    private Dictionary<string, double[]> customVisionData = new Dictionary<string, double[]>();

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

    // Use this for initialization
    void Start()
    {
        ClearDatasetData();

        datasetDirPath = Path.Combine(Application.dataPath, "../", datasetDirPath);
        datasetDirPath = Path.GetFullPath(datasetDirPath);
    }

    public void ClearDatasetData()
    {
        autoMLData.Clear();
        customVisionData.Clear();
    }

    public void AppendAutoMLRowData(string rowData)
    {
        autoMLData.Add(rowData);
    }

    public void AppendCustomVisionRowData(string filename, double[] region)
    {
        customVisionData.Add(filename, region);
    }

    /// <summary>
    /// Generate and return AutoML-dataset-formatted row
    /// </summary>
    /// <param name="set"></param>
    /// <param name="imgName"></param>
    /// <param name="imageClass"></param>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <returns></returns>
    public string GenerateAutoMLRowData(AutoMLSets set, string imgName, string imageClass, float x1, float y1, float x2, float y2)
    {
        imgName = Path.GetFileName(imgName);

        string imageBucketPath = Path.Combine(string.Format("gs://{0}/{1}/{2}", autoMLBucketName, autoMLPrefixImageDirectoryName, imgName));

        string csvRow = string.Format("{0},{1},{2},{3},{4},,,{5},{6},,", set.ToString(), imageBucketPath, imageClass, x1,y1,x2,y2);

        return csvRow;
    }

    /// <summary>
    /// Creates a CSV file used as input dataset labels for Google's AutoML
    /// </summary>
    /// <param name="filepath"></param>
    /// <returns></returns>
    [ContextMenu(nameof(CreateAutoMLDatasetFromRows))]
    public string CreateAutoMLDatasetFromRows(string filepath)
    {
        filepath = string.IsNullOrEmpty(filepath) ? getPath() : filepath;
        filepath += ".csv";

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < autoMLData.Count; i++)
        {
            sb.AppendLine(autoMLData[i]);
        }

        StreamWriter outStream = System.IO.File.CreateText(filepath);
        outStream.WriteLine(sb);
        outStream.Close();

        return filepath;
    }

    /// <summary>
    /// Creates a JSON file used as input dataset labels for Microsoft's custom vision ai
    /// </summary>
    /// <param name="filepath"></param>
    /// <returns></returns>
    public string CreateCustomVisionDatasetFromRows(string filepath)
    {
        filepath = string.IsNullOrEmpty(filepath) ? getPath() : filepath;
        filepath += ".json";

        string json = JsonConvert.SerializeObject(customVisionData, Formatting.Indented);
        File.WriteAllText(filepath, json); 

        return filepath;
    }

    // Following method is used to retrive the relative path as device platform
    private string getPath()
    {
        return Path.Combine(datasetDirPath, datasetFileName);

#if UNITY_EDITOR
        return Application.dataPath + "/CSV/" + "Saved_data.csv";
#elif UNITY_ANDROID
        return Application.persistentDataPath+"Saved_data.csv";
#elif UNITY_IPHONE
        return Application.persistentDataPath+"/"+"Saved_data.csv";
#else
        return Application.dataPath +"/"+"Saved_data.csv";
#endif
    }
}