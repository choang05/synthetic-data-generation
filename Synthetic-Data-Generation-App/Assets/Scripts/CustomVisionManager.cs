using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class CustomVisionManager : MonoBehaviour
{
    //Static instance which allows it to be accessed by any other script.
    public static CustomVisionManager instance = null;

    //Custom Vision variables
    [Header("CustomVision Settings")]
    CustomVisionTrainingClient client;
    public string endpoint = "https://lootbox.cognitiveservices.azure.com/";
    public string trainingKey = "9e6e0d2201a14de3beb1dcd137223d49";
    public string projectID = "a8d69bbf-9fcb-4a44-9f6e-7b3aa1e2fff0";
    public bool createNewProject;
    private static List<string> importedImages;

    /// <summary>
    /// This is where we actually utrain the model
    /// </summary>
    /// <returns></returns>
    private async Task TrainModelAsync()
    {
        client = new CustomVisionTrainingClient();
        client.ApiKey = trainingKey;
        client.Endpoint = endpoint;

        var project = await client.GetProjectAsync(new Guid(projectID));

        var iteration = await client.TrainProjectAsync(project.Id);

        // The returned iteration will be in progress, and can be queried periodically to see when it has completed
        while (iteration.Status == "Training")
        {
            await Task.Delay(1000);

            // Re-query the iteration to get it's updated status
            iteration = await client.GetIterationAsync(project.Id, iteration.Id);
        }

        // The iteration is now trained. Publish it to the prediction end point.
        var publishedModelName = "treeClassModel";
        var predictionResourceId = "<target prediction resource ID>";
        await client.PublishIterationAsync(project.Id, iteration.Id, publishedModelName, predictionResourceId);
        Debug.Log("Training successful!");
    }

    /// <summary>
    /// This is where we actually upload the images
    /// </summary>
    /// <param name="objName"></param>
    /// <returns></returns>
    private async Task UploadModelAsync(string objName)
    {
        client = new CustomVisionTrainingClient();
        client.ApiKey = trainingKey;
        client.Endpoint = endpoint;

        var project = await client.GetProjectAsync(new Guid(projectID));

        try
        {
            var tag = (await client.CreateTagAsync(project.Id, objName));

            // this loads the images to be uploaded from disk into memory
            importedImages = Directory.GetFiles(Path.Combine("TRAINING_DATA", objName)).ToList();

            // Create tag list
            var tagList = new List<Guid>() { tag.Id };

            // Images can be uploaded one at a time
            foreach (var image in importedImages)
            {
                // using (var stream = new MemoryStream(File.ReadAllBytes(image)))
                using (var stream = File.OpenRead(image))
                {
                    client.CreateImagesFromData(project.Id, stream, tagList);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log($"{nameof(CaptureManager)}.{nameof(uploadModel)} could not complete upload for {objName}. {e.Message}");
            if (e.InnerException != null)
            {
                Debug.Log($"{nameof(CaptureManager)}.{nameof(uploadModel)} Additional info: {e.InnerException.Message}");
            }
        }
    }

    /// <summary>
    /// This is where we initiate communication with Custom Vision to upload the images.
    /// </summary>
    /// <param name="objName"></param>
    [ContextMenu(nameof(uploadModel))]
    public async void uploadModel(string objName)
    {
        try
        {
            await UploadModelAsync(objName);
        }
        catch (Exception e)
        {
            Debug.Log($"{nameof(CaptureManager)}.{nameof(uploadModel)} could not complete upload for {objName}. {e.Message}");
            if (e.InnerException != null)
            {
                Debug.Log($"{nameof(CaptureManager)}.{nameof(uploadModel)} Additional info: {e.InnerException.Message}");
            }
        }
    }

    /// <summary>
    /// This is where we initiate communication with Custom Vision to train the model.
    /// </summary>
    [ContextMenu(nameof(trainModel))]
    public async void trainModel()
    {
        try
        {
            await TrainModelAsync();
        }
        catch (Exception e)
        {
            Debug.Log($"{nameof(CaptureManager)}.{nameof(trainModel)} could not complete train. {e.Message}");
            if (e.InnerException != null)
            {
                Debug.Log($"{nameof(CaptureManager)}.{nameof(trainModel)} Additional info: {e.InnerException.Message}");
            }
        }
    }
}
