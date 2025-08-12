using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Графический контроллер
/// </summary>
public class GraphicManager : Singleton<GraphicManager> {

	public GraphicLibrary link;

	public string serverGameIos;
	public string serverGameAndroid;
	public string serverEditorIos;
	public string serverEditorAndroid;

	private string server {
		get {
#if UNITY_EDITOR
#if UNITY_IOS
				return serverEditorIos;
#else
			return serverEditorAndroid;
#endif
#else
#if UNITY_IOS
				return serverGameIos;
#else
				return serverGameAndroid;
#endif
#endif
		}
	}

#if UNITY_EDITOR
	public GraphicLibrary elemLink;
#endif

	protected override void Awake() {
		base.Awake();
		DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR
		link = elemLink;
		//ExEvent.LoadEvents.LoadProgress.Call(1);
#else
		LoadGraphicBundle();
#endif
	}


	private void LoadGraphicBundle() {
		StartCoroutine(GetBundleUrl("graphic.main", "GraphicLibrary", (obj) => {
			Debug.Log(obj);
			link = obj as GraphicLibrary;
		}));
	}

	IEnumerator GetBundleUrl(string url, string title, Action<object> onLoad) {

		//string urlString = server + url;
		//Debug.Log("Get bundle: " + urlString);
		//Debug.Log(urlString + ".manifest");
		
		//Hash128 hashString = (default(Hash128));
		
		//if (PlayerPrefs.HasKey("hash")) {
		//	hashString = Hash128.Parse(PlayerPrefs.GetString("hash"));
		//} else {
		//	UnityWebRequest request = UnityWebRequest.Get(urlString + ".manifest");
		//	request.Send();

		//	while (!request.isDone) {
		//		yield return null;
		//	}

		//	if (request.downloadHandler.text.Contains("ManifestFileVersion")) {
		//		var hashRow = request.downloadHandler.text.Split("\n".ToCharArray())[5];
		//		hashString = Hash128.Parse(hashRow.Split(':')[1].Trim());
		//		PlayerPrefs.SetString("hash", hashString.ToString());
		//	} else {
		//		yield break;
		//	}
		//}
		
		//Debug.Log(AssetBundle.GetAllLoadedAssetBundles().Count());

		//UnityWebRequest requestBundle = UnityWebRequest.GetAssetBundle(urlString, hashString, 0);
		
		//requestBundle.Send();

		//while (!requestBundle.isDone) {
		//	yield return null;
		//	Debug.Log(requestBundle.downloadProgress);
		//	ExEvent.LoadEvents.LoadProgress.Call(requestBundle.downloadProgress);
		//}
		//ExEvent.LoadEvents.LoadProgress.Call(1);
		
		//AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(requestBundle);
		AssetBundle bundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/bundle/graphic.main");
		
		AssetBundleRequest requestBandle = null;

		requestBandle = bundle.LoadAssetAsync<object>(title);

		yield return requestBandle;

		if (onLoad != null) onLoad(requestBandle.asset);
		ExEvent.LoadEvents.LoadProgress.Call(1);
		bundle.Unload(false);
	}

}

[System.Serializable]
public struct AchivcGraph {
	public Sprite mini;
	public Sprite maxi;
}
