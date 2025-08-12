using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateAssetBundles : Editor {

	[MenuItem("Assets/Build Asset Bundles Ios")]
	static void BuildAllAssetBundlesIos() {
		string assetBundleDirectory = "Assets/AssetBundles/Ios";
		if (!Directory.Exists(assetBundleDirectory)){
			Directory.CreateDirectory(assetBundleDirectory);
		}
		BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.iOS);

	}

	[MenuItem("Assets/Build Asset Bundles Android")]
	static void BuildAllAssetBundlesAndroid() {
		string assetBundleDirectory = "Assets/AssetBundles/Android";
		if (!Directory.Exists(assetBundleDirectory)) {
			Directory.CreateDirectory(assetBundleDirectory);
		}
		BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.Android);

	}

	[MenuItem("Extensions/Build/Ios Ready")]
	private static void ReadyBuidlIos() {
		DeleteExistsStreamingBundles();

		Debug.Log(Application.dataPath);
		string[] stringArr = Directory.GetFiles(Application.dataPath + "/AssetBundles/Ios");

		foreach (var VARIABLE in stringArr) {
			FileInfo fileInfo = new FileInfo(VARIABLE);
			File.Copy(VARIABLE, Application.streamingAssetsPath + "/bundle/" + fileInfo.Name);
		}
		AssetDatabase.Refresh();

	}

	[MenuItem("Extensions/Build/Android Ready")]
	private static void ReadyBuildAndroid() {
		DeleteExistsStreamingBundles();

		string[] stringArr = Directory.GetFiles(Application.dataPath + "/AssetBundles/Android");

		foreach (var VARIABLE in stringArr) {
			FileInfo fileInfo = new FileInfo(VARIABLE);
			File.Copy(VARIABLE, Application.streamingAssetsPath + "/bundle/" + fileInfo.Name);
		}
		AssetDatabase.Refresh();

	}

	private static void DeleteExistsStreamingBundles() {

		string[] stringArr = Directory.GetFiles(Application.streamingAssetsPath + "/bundle");

		foreach (var VARIABLE in stringArr)
			File.Delete(VARIABLE);
	}
	
}
