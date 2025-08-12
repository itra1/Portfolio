using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class BundleManager : Singleton<BundleManager> {

	public struct LoadBundle {
		public string url;
		public string title;
		public bool isScene;
		public Action<object> OnLoad;
	}

	private void Start() { }

	private Queue<LoadBundle> bundleQueue = new Queue<LoadBundle>();

	public static void GetBundle(string url, string title, Action<object> OnLoad, bool isScene = false) {

		Instance.GetBundle_(url, title, OnLoad, isScene);
	}

	public void GetBundle_(string url, string title, Action<object> OnLoad, bool isScene = false) {

		Debug.Log(url + " " + title);

		LoadBundle lb = new LoadBundle();
		lb.title = title;
		lb.url = url;
		lb.isScene = isScene;
		lb.OnLoad = (Action<object>)OnLoad;

		bundleQueue.Enqueue(lb);

		if (loadCor == null)
			loadCor = StartCoroutine(LoadBundles());

		//StartCoroutine(GetBundleUrl(url, title, OnLoad));
	}

	private Coroutine loadCor;

	IEnumerator LoadBundles() {

		while (bundleQueue.Count > 0) {
			var cur = bundleQueue.Dequeue();
			yield return GetBundleUrl(cur.url, cur.title, cur.OnLoad, cur.isScene);
		}
		loadCor = null;
	}

	IEnumerator GetBundleUrl(string url, string title, Action<object> onLoad, bool isScene = false) {

		string urlString = Parametrs.Instance.bundleServer + url;
		Debug.Log("Get bundle: " + urlString);
		Debug.Log(urlString + ".manifest");

		UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(urlString + ".manifest");
		request.Send();
		
		while (!request.isDone) {
			yield return null;
		}
		
		Hash128 hashString = (default(Hash128));

		if (request.downloadHandler.text.Contains("ManifestFileVersion")) {
			var hashRow = request.downloadHandler.text.ToString().Split("\n".ToCharArray())[5];
			hashString = Hash128.Parse(hashRow.Split(':')[1].Trim());
		} else {
			yield break;
		}
		
		request = UnityWebRequestAssetBundle.GetAssetBundle(urlString, hashString,0);

		request.Send();
		while (!request.isDone) {
			yield return null;
			if (isScene)
				ExEvent.LoadEvents.LoadProgress.Call(String.Format("Загрузка локации {0:0.00}%", request.downloadProgress * 100), request.downloadProgress);
			else
				ExEvent.LoadEvents.LoadProgress.Call(String.Format("Загрузка модели {0:0.00}%", request.downloadProgress * 100), request.downloadProgress);
		}

		AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);

		if (isScene) {
			string[] scenes = bundle.GetAllScenePaths();
			if (onLoad != null) onLoad(scenes);
			yield break;
		}
		AssetBundleRequest requestBandle = null;

		requestBandle = bundle.LoadAssetAsync<object>(title);

		yield return requestBandle;
		
		if (onLoad != null) onLoad(requestBandle.asset);
		bundle.Unload(false);
	}



	//IEnumerator GetBundleUrl(string url, string title, Action<object> onLoad, bool isScene = false) {

	//	string urlString = Parametrs.Instance.bundleServer + url;
	//	Debug.Log("Get bundle: " + urlString);
	//	UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.GetAssetBundle(urlString, 0);

	//	request.Send();
	//	while (!request.isDone) {
	//		yield return null;
	//		if (isScene)
	//			ExEvent.LoadEvents.LoadProgress.Call(String.Format("Загрузка локации {0:0.00}%", request.downloadProgress * 100), request.downloadProgress);
	//		else
	//			ExEvent.LoadEvents.LoadProgress.Call(String.Format("Загрузка модели {0:0.00}%", request.downloadProgress * 100), request.downloadProgress);
	//	}

	//	AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);

	//	if (isScene) {
	//		string[] scenes = bundle.GetAllScenePaths();
	//		if (onLoad != null) onLoad(scenes);
	//		yield break;
	//	}
	//	AssetBundleRequest requestBandle = null;

	//	requestBandle = bundle.LoadAssetAsync<object>(title);

	//	yield return requestBandle;

	//	Debug.Log("Загрузка завершена: " + urlString);
	//	if (onLoad != null) onLoad(requestBandle.asset);
	//	bundle.Unload(false);
	//}


}
