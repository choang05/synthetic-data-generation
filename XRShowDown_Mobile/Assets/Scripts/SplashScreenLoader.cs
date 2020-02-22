using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenLoader : MonoBehaviour
{
    public void Load(string scenename)
    {
        SceneManager.LoadScene(scenename);
    }
}