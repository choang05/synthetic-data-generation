using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
    public string projectID = "65b2a73e-688a-4b59-b574-b9a5d3d2bba6";
    public string resourceID = "/subscriptions/bd2cd714-0307-4ce0-a94b-5ee900b7965d/resourceGroups/Lootbox/providers/Microsoft.CognitiveServices/accounts/Lootbox";
    public bool createNewProject;
    private static List<string> importedImages;

    /// <summary>
    /// This is where we initiate communication with Custom Vision to upload the images.
    /// </summary>
    /// <remarks>
    /// This method should ONLY be called from the context menu. If calling from code, use the async Task version below.
    /// </remarks>
    /// <param name="objName"></param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [ContextMenu(nameof(UploadModel))]
    public async void UploadModel(string objName)
    {
        try
        {
            await UploadModelAsync(objName);
        }
        catch (Exception e)
        {
            Debug.Log($"{nameof(CaptureManager)}.{nameof(UploadModel)} could not complete upload for {objName}. {e.Message}");
            if (e.InnerException != null)
            {
                Debug.Log($"{nameof(CaptureManager)}.{nameof(UploadModel)} Additional info: {e.InnerException.Message}");
            }
        }
    }

    /// <summary>
    /// This is where we actually upload the images
    /// </summary>
    /// <param name="objName"></param>
    /// <returns></returns>
    public async Task UploadModelAsync(string objName)
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

            Debug.Log($"About to upload {importedImages.Count} images.");

            // Images can be uploaded one at a time
            foreach (var image in importedImages)
            {
                // using (var stream = new MemoryStream(File.ReadAllBytes(image)))
                using (var stream = File.OpenRead(image))
                {
                    Debug.Log($"Uploading image: {image} ...");
                    client.CreateImagesFromData(project.Id, stream, tagList);
                }
            }

            Debug.Log($"Image upload complete.");
        }
        catch (Exception e)
        {
            Debug.Log($"{nameof(CaptureManager)}.{nameof(UploadModel)} could not complete upload for {objName}. {e.Message}");
            if (e.InnerException != null)
            {
                Debug.Log($"{nameof(CaptureManager)}.{nameof(UploadModel)} Additional info: {e.InnerException.Message}");
            }
        }
    }

    /// <summary>
    /// This is where we initiate communication with Custom Vision to train the model.
    /// </summary>
    /// <remarks>
    /// This method should ONLY be called from the context menu. If calling from code, use the async Task version below.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [ContextMenu(nameof(TrainModel))]
    public async void TrainModel()
    {
        try
        {
            await TrainModelAsync();
        }
        catch (Exception e)
        {
            // Debug.Log($"Exception Type: {e.GetType().Name}");
            Debug.Log($"{nameof(CaptureManager)}.{nameof(TrainModel)} could not complete train. {e.Message}");
            if (e.InnerException != null)
            {
                Debug.Log($"{nameof(CaptureManager)}.{nameof(TrainModel)} Additional info: {e.InnerException.Message}");
            }
            var cve = e as CustomVisionErrorException;
            if (cve != null)
            {
                Debug.Log($"{nameof(CaptureManager)}.{nameof(TrainModel)} Custom Vision info:\r\n HResult: {cve.HResult} \r\n Message: {cve.Message}, \r\n Status Code: {cve.Response.StatusCode} \r\n Reason: {cve.Response.ReasonPhrase} \r\n Content: {cve.Response.Content}");
            }
        }
    }

    /// <summary>
    /// This is where we actually utrain the model
    /// </summary>
    /// <returns></returns>
    public async Task TrainModelAsync()
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
        var publishedModelName = "TestClassModel";
        await client.PublishIterationAsync(project.Id, iteration.Id, publishedModelName, resourceID);
        Debug.Log("Training successful!");
    }

    /// <summary>
    /// This is where we initiate the process of resetting the Custom Vision project.
    /// </summary>
    [ContextMenu(nameof(resetModel))]
    public async void resetModel()
    {
        try
        {
            await ResetModelAsync();
        }
        catch (Exception e)
        {
            // Debug.Log($"Exception Type: {e.GetType().Name}");
            Debug.Log($"{nameof(CaptureManager)}.{nameof(resetModel)} could not complete reset. {e.Message}");
            if (e.InnerException != null)
            {
                Debug.Log($"{nameof(CaptureManager)}.{nameof(resetModel)} Additional info: {e.InnerException.Message}");
            }
            var cve = e as CustomVisionErrorException;
            if (cve != null)
            {
                Debug.Log($"{nameof(CaptureManager)}.{nameof(resetModel)} Custom Vision info:\r\n HResult: {cve.HResult} \r\n Message: {cve.Message}, \r\n Status Code: {cve.Response.StatusCode} \r\n Reason: {cve.Response.ReasonPhrase} \r\n Content: {cve.Response.Content}");
            }
        }
    }

    /// <summary>
    /// This is where we actually reset the project
    /// </summary>
    /// <returns></returns>
    private async Task ResetModelAsync()
    {
        client = new CustomVisionTrainingClient();
        client.ApiKey = trainingKey;
        client.Endpoint = endpoint;

        var project = await client.GetProjectAsync(new Guid(projectID));

        var images = client.GetImagesByIds(project.Id);

        Debug.Log("Reset successful!");
    }
}
