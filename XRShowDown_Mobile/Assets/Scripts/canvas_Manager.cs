using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canvas_Manager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject welcome;
    public GameObject main;
    public GameObject productD;
    public GameObject processing;
    public GameObject helperS;
    public GameObject archive;
    public GameObject settings;
    public GameObject capturedImage;

    [Header("UI misc panels")]
    public GameObject imageProcessingAnimationPanel;
    public Transform validImagePanel;
    public Transform invalidImagePanel;

    void Start()
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
        settings.gameObject.SetActive(false);

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

        while (!isImageValid)
        {
            currentElapsedTime += 1;

            //  DEBUG (give 3 seconds to process)
            if (currentElapsedTime >= 7)
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

            if (isValid)
            {
                Debug.Log("Image valid!");
                capturedImage.SetActive(true);
            }
            else
            {
                Debug.Log("Image invalid!");
                helperS.gameObject.SetActive(true);
            }
        }
    }
}
