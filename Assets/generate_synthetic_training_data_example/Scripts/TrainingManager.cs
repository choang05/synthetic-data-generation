using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingManager : MonoBehaviour
{
    CustomVisionTrainingClient client;

    // Start is called before the first frame update
    void Start()
    {
        client = new CustomVisionTrainingClient();
        client.GetProject();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
