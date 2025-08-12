using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    Login,
    Tables
}

public class SceneLoader : MonoBehaviour
{
    public static void LoadScene(SceneType scene)
    {
        SceneManager.LoadScene(GetPathScene(scene));
    }

    public static string GetPathScene(SceneType scene)
    {
        var pathToScene = GameHelper.IsMobile ? "Scenes/Mobile/" : "Scenes/";
        return pathToScene + scene;
    }
    
    public static void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public static void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
