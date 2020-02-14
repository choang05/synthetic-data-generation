using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;

public class CSVManager : MonoBehaviour
{
    public static CSVManager instance = null;              //Static instance which allows it to be accessed by any other script.

    public string datasetFileName = "dataset.csv";
    public string autoMLBucketName = "xr-showdown-2020";
    public string prefixImageDirectoryName = "images";

    public enum AutoMLSets
    {
        TRAIN,
        VALIDATE,
        TEST,
        UNASSIGNED
    }

    public List<string> rowsData = new List<string>();

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

    }

    public void AppendRowData(string rowData)
    {
        rowsData.Add(rowData);
    }

    public string GenerateRowString(AutoMLSets set, string imgName, string imageClass, float x1, float y1, float x2, float y2)
    {
        imgName = Path.GetFileName(imgName);

        string imageBucketPath = Path.Combine(string.Format("gs://{0}/{1}/{2}", autoMLBucketName, prefixImageDirectoryName, imgName));

        string csvRow = string.Format("{0},{1},{2},{3},{4},,,{5},{6},,", set.ToString(), imageBucketPath, imageClass, x1,y1,x2,y2);

        return csvRow;
    }

    [ContextMenu(nameof(CreateCSVFromRows))]
    public string CreateCSVFromRows(string filepath)
    {
        filepath = string.IsNullOrEmpty(filepath) ? getPath() : filepath;

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < rowsData.Count; i++)
        {
            sb.AppendLine(rowsData[i]);
        }

        StreamWriter outStream = System.IO.File.CreateText(filepath);
        outStream.WriteLine(sb);
        outStream.Close();

        return filepath;
    }

    void Save()
    {

        //// Creating First row of titles manually..
        //string[] rowDataTemp = new string[3];
        //rowDataTemp[0] = "Name";
        //rowDataTemp[1] = "ID";
        //rowDataTemp[2] = "Income";
        //rowData.Add(rowDataTemp);

        // You can add up the values in as many cells as you want.
        //for (int i = 0; i < 10; i++)
        //{
        //    rowDataTemp = new string[3];
        //    rowDataTemp[0] = "Sushanta" + i; // name
        //    rowDataTemp[1] = "" + i; // ID
        //    rowDataTemp[2] = "$" + UnityEngine.Random.Range(5000, 10000); // Income
        //    rowData.Add(rowDataTemp);
        //}

        //string[][] output = new string[rowData.Count][];

        //for (int i = 0; i < output.Length; i++)
        //{
        //    output[i] = rowData[i];
        //}

        //int length = output.GetLength(0);
        //string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        //for (int index = 0; index < length; index++)
        //    sb.AppendLine(string.Join(delimiter, output[index]));


        string filePath = getPath();

        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();
    }

    // Following method is used to retrive the relative path as device platform
    private string getPath()
    {
        return Path.Combine(CaptureManager.instance.datasetDirPath, datasetFileName);

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