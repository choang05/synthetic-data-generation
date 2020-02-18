using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canvas_Manager : MonoBehaviour
{

    public GameObject welcome;
    public GameObject main;
    public GameObject productD;
    public GameObject processing;
    public GameObject helperS;
    public GameObject archive;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void swapCanvas(string canvasName)
    {
        welcome.gameObject.SetActive(false);
        main.gameObject.SetActive(false);
        productD.gameObject.SetActive(false);
        processing.gameObject.SetActive(false);
        helperS.gameObject.SetActive(false);
        archive.gameObject.SetActive(false);


        switch (canvasName)
        {
            case "welcome":
            welcome.gameObject.SetActive(true);
                break;
            case "main":
                main.gameObject.SetActive(true);
                break;
            case "productD":
                productD.gameObject.SetActive(true);
                break;
            case "processing":
                processing.gameObject.SetActive(true);
                break;
            case "helperS":
                helperS.gameObject.SetActive(true);
                break;
            case "archive":
			archive.gameObject.SetActive(true);
			break;
           

        }
        



    }


}
