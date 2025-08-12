using UnityEngine;
using System.Collections;
#if PLUGIN_UNITYADS
using UnityEngine.Advertisements; // Using the Unity Ads namespace.
#endif

public class UnityAdsController : Singleton<UnityAdsController> {
	
	public string gameId;
	public bool enableTestMode = false;
	
	public delegate void CallBackFunt(bool result);
	public CallBackFunt callBackFunt;

	void Start() {
		StartCoroutine(Inicializate());
		
#if PLUGIN_UNITYADS
		Advertisement.Show();
#endif
	}

	IEnumerator Inicializate() {


#if UNITY_EDITOR
		enableTestMode = true;
#endif
#if PLUGIN_UNITYADS
#if !UNITY_ADS // If the Ads service is not enabled...
		if (Advertisement.isSupported) { // If the platform is supported,
			Advertisement.Initialize(gameId, enableTestMode); // initialize Unity Ads.
		}

#endif
		while (!Advertisement.isInitialized || !Advertisement.IsReady()) {
			yield return new WaitForSeconds(0.5f);
		}
#else
        yield return null;
#endif
	}

	public static bool adsReady {
		get {
#if PLUGIN_UNITYADS
			return Advertisement.isInitialized && Advertisement.IsReady();
#else
      return false;
#endif
		}
	}

	public static void ShowVideo(CallBackFunt func) {
#if PLUGIN_UNITYADS
		Instance.callBackFunt = func;
		ShowOptions options = new ShowOptions();
		options.resultCallback = Instance.HandleShowResult;
		Advertisement.Show(null, options);
#endif
	}

#if PLUGIN_UNITYADS
	public void HandleShowResult(ShowResult result) {

		if (callBackFunt != null) {
			switch (result) {
				case ShowResult.Finished:
					callBackFunt(true);
					break;
				case ShowResult.Skipped:
					callBackFunt(true);
					break;
				case ShowResult.Failed:
					callBackFunt(false);
					break;
			}
			callBackFunt = null;
		}
	}
#endif

}