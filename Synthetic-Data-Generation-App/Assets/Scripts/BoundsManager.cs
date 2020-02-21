using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoundsManager : MonoBehaviour
{
    public static BoundsManager instance = null;              //Static instance which allows it to be accessed by any other script.

    [Header("Debug Settings")]
    public int frameWidth = 1;
    public Color frameColor;

    //private List<GameObject> boundObjs = new List<GameObject>();
    private List<Rect> rects = new List<Rect>();

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

    /// <summary>
    /// While the game is running... draw the bounding box for each gameobject to capture for debug purposes.
    /// </summary>
    private void LateUpdate()
    {
        //rects.Clear();
        //for (int i = 0; i < CaptureManager.instance.objsToScan.Count; i++)
        //{
        //    //boundObjs = CaptureManager.instance.objsToScan.ToList();

        //    if (!CaptureManager.instance.objsToScan[i].activeSelf)
        //        continue;

        //    Rect rect = Get3dTo2dRect(CaptureManager.instance.objsToScan[i]);
        //    if (rects.Contains(rect))
        //    {
        //        rects[i] = rect;
        //    }
        //    else
        //    {
        //        rects.Add(rect);
        //    }
        //}
    }

    /// <summary>
    /// Given a gameobject, generate the region from its world-space renderer bounding box in screen-space.
    /// </summary>
    /// <param name="go"></param>
    /// <returns>Returns x,y,x2,y2 coordinates of the region in screenspace</returns>
    public Rect Get3dTo2dRect(GameObject go)
    {
        MeshFilter meshFilter = go.GetComponent<MeshFilter>();
        if (!meshFilter)
        {
            Debug.LogError(meshFilter + " is null!");
            return new Rect(-1,-1,-1,-1);
        }

        Vector3[] vertices = meshFilter.mesh.vertices;

        float x1 = float.MaxValue, y1 = float.MaxValue, x2 = 0.0f, y2 = 0.0f;

        foreach (Vector3 vert in vertices)
        {
            Vector2 tmp = WorldToGUIPoint(go.transform.TransformPoint(vert));

            if (tmp.x < x1) x1 = tmp.x;
            if (tmp.x > x2) x2 = tmp.x;
            if (tmp.y < y1) y1 = tmp.y;
            if (tmp.y > y2) y2 = tmp.y;
        }

        Rect bbox = new Rect(x1, y1, x2 - x1, y2 - y1);
        //Debug.Log(bbox);
        return bbox;
    }

    /// <summary>
    /// Converts the world point to screen space point
    /// </summary>
    /// <param name="world"></param>
    /// <returns></returns>
    public static Vector2 WorldToGUIPoint(Vector3 world)
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(world);
        screenPoint.y = (float)Screen.height - screenPoint.y;
        return screenPoint;
    }

    /// <summary>
    /// Required unity method to to draw gui on the screen
    /// </summary>
    void OnGUI()
    {
        for (int i = 0; i < rects.Count; i++)
        {
            DrawRectangle(rects[i], frameWidth, frameColor);
        }
    }

    /// <summary>
    /// Given drawing parameters... draw/print the bounding box to the screen for debug purposes
    /// </summary>
    /// <param name="area"></param>
    /// <param name="frameWidth"></param>
    /// <param name="color"></param>
    void DrawRectangle(Rect area, int frameWidth, Color color)
    {
        //Create a one pixel texture with the right color
        var texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();

        //GUI.DrawTexture(area, texture);   //  solid color
        Rect lineArea = area;
        lineArea.height = frameWidth; //Top line
        GUI.DrawTexture(lineArea, texture);
        lineArea.y = area.yMax - frameWidth; //Bottom
        GUI.DrawTexture(lineArea, texture);
        lineArea = area;
        lineArea.width = frameWidth; //Left
        GUI.DrawTexture(lineArea, texture);
        lineArea.x = area.xMax - frameWidth;//Right
        GUI.DrawTexture(lineArea, texture);

        GUIStyle textStyle = new GUIStyle();
        textStyle.normal.textColor = Color.blue;
        Rect rectNorm = new Rect(area.x/Screen.width, area.y/ Screen.height, area.xMax/Screen.width, area.yMax/Screen.height);
        GUI.Label(area, rectNorm.ToString(), textStyle);
        Rect areaTmp = new Rect(area.x, area.y + 15, area.xMax, area.yMax + 15);
        GUI.Label(areaTmp, area.ToString(), textStyle);
    }
}
