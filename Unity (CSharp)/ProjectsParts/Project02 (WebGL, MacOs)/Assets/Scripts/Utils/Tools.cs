#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Tools
{
    [MenuItem("Tools/Open Splash Scene", false, 1)]
    public static void OpenSplashScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Splash.unity");
    }

    
    [MenuItem("Tools/Open Login Scene", false, 1)]
    public static void OpenLoginScene()
    {
        EditorSceneManager.OpenScene($"Assets/Scenes/{SceneType.Login}.unity");
    }

    [MenuItem("Tools/Open Tables Scene", false, 2)]
    public static void OpenTablesScene()
    {
        EditorSceneManager.OpenScene($"Assets/Scenes/{SceneType.Tables}.unity");
    }
    
    [MenuItem("Tools/Open Login Mobile Scene", false, 3)]
    public static void OpenLoginMobileScene()
    {
        EditorSceneManager.OpenScene($"Assets/Scenes/Mobile/{SceneType.Login}.unity");
    }

    [MenuItem("Tools/Open Tables Mobile Scene", false, 4)]
    public static void OpenTablesMobileScene()
    {
        EditorSceneManager.OpenScene($"Assets/Scenes/Mobile/{SceneType.Tables}.unity");
    }
}
#endif
