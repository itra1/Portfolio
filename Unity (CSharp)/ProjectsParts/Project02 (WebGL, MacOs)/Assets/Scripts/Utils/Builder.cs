using System;
using Debug = UnityEngine.Debug;
using System.IO;
using UnityEngine;
using System.Globalization;
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

public class Builder
{
    //[MenuItem("Tools/Builds/Build Windows", false, 1)]
    public static void BuildWindows()
    {
        Build(BuildOptions.None, BuildTarget.StandaloneWindows);
    }
    
    //[MenuItem("Tools/Builds/Build Windows Dev", false, 2)]
    public static void BuildWindowsDev()
    {
        Build(BuildOptions.Development, BuildTarget.StandaloneWindows);
    }
    
    //[MenuItem("Tools/Builds/Build Android", false, 3)]
    public static void BuildAndroid()
    {
        Build(BuildOptions.None, BuildTarget.Android);
    }

    //[MenuItem("Tools/Builds/Build Android Dev", false, 4)]
    public static void BuildAndroidDev()
    {
        Build(BuildOptions.Development, BuildTarget.Android);
    }

    private static void Build(BuildOptions options, BuildTarget target)
    {
        var scenes = new List<string>();
        foreach (var scene in (SceneType[]) Enum.GetValues(typeof(SceneType)))
        {
            var path = "Assets/" + SceneLoader.GetPathScene(scene) + ".unity";
            if (File.Exists(path)) scenes.Add(path);
        }

        var extension = ".apk";
        switch (target)
        {
            case BuildTarget.Android:
            {
                PlayerSettings.Android.keystoreName = @"\Key\GorillaPoker.keystore";
                PlayerSettings.Android.keystorePass = "GorillaPoker0fD3ath42";
                PlayerSettings.Android.keyaliasName = "GorillaPoker";
                PlayerSettings.Android.keyaliasPass = "GorillaPoker0fD3ath42";

                extension = ".apk";
                break;
            }
            case BuildTarget.StandaloneWindows:
            {
                extension = ".exe";
                break;
            }
        }
        var version = (float.Parse(Application.version, CultureInfo.InvariantCulture.NumberFormat)  + 0.01f).ToString("G", CultureInfo.InvariantCulture);
        var nameBuild = ($"GorillaPoker_{DateTime.Now:yyyyMMdd}_v{version}.{extension}");
        
        it.Logger.Log($"Building project for platform {target} into '{nameBuild}'");
        BuildPipeline.BuildPlayer( scenes.ToArray(), "Build/"  + nameBuild, target, options);
    }
}
#endif
