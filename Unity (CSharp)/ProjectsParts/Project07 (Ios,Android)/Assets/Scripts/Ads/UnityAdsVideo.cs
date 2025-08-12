using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdsVideo : Singleton<UnityAdsVideo> {

  public string appleId = "1621133";
  public string googleId = "1621134";
  
  private string gameId {
    get {
#if UNITY_IOS
      return appleId;
#elif UNITY_ANDROID
      return googleId;
#else
	    return null;
#endif
    }
  }
	
	public string iosVideoId;
	public string androidVideoId;
	
	private string getVideoId {
		get {
#if UNITY_IOS
			return iosVideoId;
#elif UNITY_ANDROID
      return androidVideoId;
#else
      return null;
#endif
		}
	}

	private void Start() {

    if (Advertisement.isSupported) {
      Advertisement.Initialize(gameId);
    }

  }

	private void ShowTest() {
		Advertisement.Show(getVideoId);
	}
  
  public void ShowVideo(string placementId,  System.Action<UnityEngine.Advertisements.ShowResult> callback) {

    if (!Advertisement.IsReady(placementId)) {
      callback(ShowResult.Failed);
    }

    ShowOptions options = new ShowOptions();
    options.resultCallback = (res) => {
      callback(res);
    };
    Advertisement.Show(placementId, options);
  }

  public void PlayVideo(System.Action<bool> callback) {

    ShowVideo(getVideoId, (status) => {

      if (callback != null) callback((status == ShowResult.Finished || status == ShowResult.Skipped));

    });

  }


  void HandleShowResult(ShowResult result) {
    if (result == ShowResult.Finished) {
      Debug.Log("Video completed - Offer a reward to the player");

    } else if (result == ShowResult.Skipped) {
      Debug.LogWarning("Video was skipped - Do NOT reward the player");

    } else if (result == ShowResult.Failed) {
      Debug.LogError("Video failed to show");
    }
  }
  
}
