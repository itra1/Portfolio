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
  
	protected override void Awake() {
		base.Awake();
		DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR
		//ExEvent.LoadEvents.LoadProgress.Call(1);
#else
		LoadGraphicBundle();
#endif
	}


	private void LoadGraphicBundle() {
		StartCoroutine(GetBundleUrl("graphic.main", "GraphicLibrary", (obj) => {

		}));
	}

	IEnumerator GetBundleUrl(string url, string title, Action<object> onLoad) {

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
