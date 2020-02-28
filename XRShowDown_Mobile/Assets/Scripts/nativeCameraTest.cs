using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class nativeCameraTest : MonoBehaviour
{
	public RawImage pictureHolder;

    // Start is called before the first frame update
    void Start()
    {
        
    }

	public void TakePicture(int maxSize)
	{
		// Don't attempt to use the camera if it is already open
		if (NativeCamera.IsCameraBusy())
			return;

		NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
		{
			Debug.Log("Image path: " + path);
			if (path != null)
			{
				// Create a Texture2D from the captured image
				Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize);
				if (texture == null)
				{
					Debug.Log("Couldn't load texture from " + path);
					return;
				}

				pictureHolder.texture = texture;

				// Assign texture to a temporary quad and destroy it after 5 seconds
				//pictureHolder.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
				//pictureHolder.transform.forward = Camera.main.transform.forward;
				//pictureHolder.transform.localScale = new Vector3(1f, Screen.height / (float)Screen.width, 1f);

				//Material material = pictureHolder.GetComponent<Renderer>().material;
				//if (!material.shader.isSupported) // happens when Standard shader is not included in the build
				//	material.shader = Shader.Find("Legacy Shaders/Diffuse");
				//material.mainTexture = texture;

				//Destroy(quad, 5f);

				// If a procedural texture is not destroyed manually, 
				// it will only be freed after a scene change
				//Destroy(texture, 5f);
			}
		}, maxSize);

		Debug.Log("Permission result: " + permission);
	}

	public void RecordVideo()
	{
		// Don't attempt to use the camera if it is already open
		if (NativeCamera.IsCameraBusy())
			return;

		NativeCamera.Permission permission = NativeCamera.RecordVideo((path) =>
		{
			Debug.Log("Video path: " + path);
			if (path != null)
			{
				// Play the recorded video
				Handheld.PlayFullScreenMovie("file://" + path);
			}
		});

		Debug.Log("Permission result: " + permission);
	}
}
