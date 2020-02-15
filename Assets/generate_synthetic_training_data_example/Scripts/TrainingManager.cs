using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

public class TrainingManager : MonoBehaviour
{
    CustomVisionTrainingClient client;
    public string endpoint;
    public string trainingKey;
    public string projectID;
    public bool createNewProject;
    private static MemoryStream testImage;

    private static List<string> importedImages;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu(nameof(trainTest))]
    public void trainTest()
    {
        client = new CustomVisionTrainingClient();
        client.ApiKey = trainingKey;
        client.Endpoint = endpoint;

        var project = client.GetProject(new Guid(projectID));

        var tagCup = client.GetTags(project.Id)[0];

        LoadImagesFromDisk();

        // Images can be uploaded one at a time
        foreach (var image in importedImages)
        {
            using (var stream = new MemoryStream(File.ReadAllBytes(image)))
            {
                client.CreateImagesFromData(project.Id, stream, new List<Guid>() { tagCup.Id });
            }
        }

        var iteration = client.TrainProject(project.Id);

        // The returned iteration will be in progress, and can be queried periodically to see when it has completed
        while (iteration.Status == "Training")
        {
            Thread.Sleep(1000);

            // Re-query the iteration to get it's updated status
            iteration = client.GetIteration(project.Id, iteration.Id);
        }

        // The iteration is now trained. Publish it to the prediction end point.
        var publishedModelName = "treeClassModel";
        var predictionResourceId = "<target prediction resource ID>";
        client.PublishIteration(project.Id, iteration.Id, publishedModelName, predictionResourceId);
    }

    private static void LoadImagesFromDisk()
    {
        // this loads the images to be uploaded from disk into memory
        importedImages = Directory.GetFiles(Path.Combine("Images", "Cup")).ToList();
        testImage = new MemoryStream(File.ReadAllBytes(Path.Combine("Images", "Test\\test_image.jpg")));
    }
}
