using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
	float deltaTime = 0.0f;

	public Text fpsText;
	private void Start()
	{
		DontDestroyOnLoad(this.gameObject);
	}
	void Update()
	{
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		fpsText.text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
	}
}