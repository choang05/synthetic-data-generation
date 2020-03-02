using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class canvas_Manager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject main;
    public GameObject productD;
    public GameObject processing;
    public GameObject helperS;
    public GameObject archive;
    public GameObject settings;
    public GameObject capturedImage;
    public GameObject cameraImageProcessingPanel;

    [Header("UI misc panels")]
    public GameObject imageProcessingAnimationPanel;
    public Transform validImagePanel;
    public Transform invalidImagePanel;
    public Transform capturedImagePlaceHolder;


    void OnEnable()
    {
        CloudServiceManager.OnImageRecognized += OnRecognizedImage;
    }


    void OnDisable()
    {
        CloudServiceManager.OnImageRecognized -= OnRecognizedImage;
    }


    void OnRecognizedImage(string tagName, float probability)
    {
        swapCanvas("productD");
        this.gameObject.GetComponent<ProductDescriptor>().removeObjects();
        this.gameObject.GetComponent<ProductDescriptor>().showProduct(tagName);
    }
    
    public void swapCanvas(string canvasName)
    {
        main.gameObject.SetActive(false);
        productD.gameObject.SetActive(false);
        processing.gameObject.SetActive(false);
        helperS.gameObject.SetActive(false);
        archive.gameObject.SetActive(false);
        settings.gameObject.SetActive(false);

        switch (canvasName)
        {
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
            case "settings":
                settings.gameObject.SetActive(true);
                break;
        }
     }

    public void StartProcessingImage()
    {
        StopAllCoroutines();
        StartCoroutine(ProcessImage());
    }

    private IEnumerator ProcessImage()
    {
        WaitForSeconds delay = new WaitForSeconds(1);
        bool isImageValid = false;
        float maxWaitTime = 15;
        float currentElapsedTime = 0;

        //  Animations
        imageProcessingAnimationPanel.SetActive(true);
        capturedImage.SetActive(true);

        while (!isImageValid)
        {
            currentElapsedTime += 1;

            //  DEBUG (give 3 seconds to process)
            if (currentElapsedTime >= 2)
            {
                isImageValid = true;    //  DEBUG
                break;
            }

            //  Error handling for timeouts
            if (currentElapsedTime >= maxWaitTime)
            {
                Debug.LogError("Time to process image timed out!");
                isImageValid = false;
                break;
            }

            yield return delay; 
        }

        OnImageProcessed(isImageValid);
    }

    /// <summary>
    /// Once image has proessed, if image is valid, turn on image panel, else, give suggestion and return to capture screen
    /// </summary>
    /// <param name="isValid"></param>
    private void OnImageProcessed(bool isValid)
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(imageProcessingAnimationPanel.transform.DOScale(0, .5f))
          .Append(isValid? validImagePanel.DOScale(1, .5f) : invalidImagePanel.DOScale(1, .5f))
          .PrependInterval(3)
          //.Insert(0, transform.DOScale(new Vector3(3, 3, 3), mySequence.Duration()))
          .OnComplete(MyCallback);

        void MyCallback()
        {
            //  Animations
            imageProcessingAnimationPanel.SetActive(false);
            cameraImageProcessingPanel.SetActive(false);

            if (isValid)
            {
                Debug.Log("Image valid!");
                //  Set image to render texture
                PictureTaker pt = FindObjectOfType<PictureTaker>();
                RawImage rawImage = capturedImagePlaceHolder.GetComponent<RawImage>();
            }
            else
            {
                Debug.Log("Image invalid!");
                helperS.gameObject.SetActive(true);
            }
        }
    }
}
