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
		
		//if (Advertisement.isSupported) {
		//	Advertisement.Initialize(gameId);
		//}
		
	}

	private void ShowTest() {
		//Advertisement.Show(getVideoId);
	}
  
  public void ShowVideo(UnityAdsVideoData data, string placementId,  /*System.Action<UnityEngine.Advertisements.ShowResult, */string callback) {

//		if (!Advertisement.IsReady(placementId)) {
//			callback(ShowResult.Failed,"");
//		}
//		byte[] toBytes = Encoding.UTF8.GetBytes(DateTime.Now.ToString());
//		string token = System.Convert.ToBase64String(toBytes);

//#if UNITY_EDITOR
//	  token = "Zx3MfUKNqGrBf3kd8Kn7";
//#endif

//		data.accessToken = token;
		
//	  string sid = System.Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonUtility.ToJson(data)));


//		ShowOptions options = new ShowOptions();
//	  options.gamerSid = sid;
//		options.resultCallback = (res) => {
//			callback(res, token);
//		};
//		Advertisement.Show(placementId, options);
//  }

//	public void PlayVideo(UnityAdsVideoData data, System.Action<bool, string> callback) {
		
//		ShowVideo(data, getVideoId, (status, token) => {
			
//			if (callback != null) callback((status == ShowResult.Finished || status == ShowResult.Skipped), token);
			
//		});

	}


  //void HandleShowResult(ShowResult result) {
  //  if (result == ShowResult.Finished) {
  //    Debug.Log("Video completed - Offer a reward to the player");

  //  } else if (result == ShowResult.Skipped) {
  //    Debug.LogWarning("Video was skipped - Do NOT reward the player");

  //  } else if (result == ShowResult.Failed) {
  //    Debug.LogError("Video failed to show");
  //  }
  //}

	[System.Serializable]
	public class UnityAdsVideoData {
		public int locationId;
		public string accessToken;
		public int levelNum;
	}

}
