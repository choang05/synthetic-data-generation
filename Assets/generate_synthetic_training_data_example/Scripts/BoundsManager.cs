using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsManager : MonoBehaviour
{
    public static BoundsManager instance = null;              //Static instance which allows it to be accessed by any other script.

    public List<GameObject> boundObjs = new List<GameObject>();
    public int frameWidth = 1;
    public Color frameColor;

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

    private void Start()
    {
        //rect = GUI2dRectWithObject(gameObject);
        //Render_Colored_Rectangle((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height, frameColor);
    }

    private void Update()
    {
        rects.Clear();
        for (int i = 0; i < boundObjs.Count; i++)
        {
            Rect rect = Get3dTo2dRect(boundObjs[i]);
            if (rects.Contains(rect))
            {
                rects[i] = rect;
            }
            else
            {
                rects.Add(rect);
            }
        }
    }

    public static Rect GUI3dRectWithObject(GameObject go)
    {
        Vector3 cen = go.GetComponent<Renderer>().bounds.center;
        Vector3 ext = go.GetComponent<Renderer>().bounds.extents;
        Vector2[] extentPoints = new Vector2[8]
        {
            WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z-ext.z)),
            WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z-ext.z)),
            WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z+ext.z)),
            WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z+ext.z)),
            WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z-ext.z)),
            WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z-ext.z)),
            WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z+ext.z)),
            WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z+ext.z))
        };
        Vector2 min = extentPoints[0];
        Vector2 max = extentPoints[0];
        foreach (Vector2 v in extentPoints)
        {
            min = Vector2.Min(min, v);
            max = Vector2.Max(max, v);
        }
        return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
    }

    public Rect Get3dTo2dRect(GameObject go)
    {
        Vector3[] vertices = go.GetComponent<MeshFilter>().mesh.vertices;

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

    public static Vector2 WorldToGUIPoint(Vector3 world)
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(world);
        screenPoint.y = (float)Screen.height - screenPoint.y;
        return screenPoint;
    }

    void OnGUI()
    {
        for (int i = 0; i < rects.Count; i++)
        {
            DrawRectangle(rects[i], frameWidth, frameColor);
        }
    }

    void DrawRectangle(Rect area, int frameWidth, Color color)
    {
        //Create a one pixel texture with the right color
        var texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();

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
    }
}
