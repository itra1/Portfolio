using System;
using GoogleMobileAds.Api;
using UnityEngine;
using KingBird.AdMob;

/// <summary>
/// Контроллер рекламы Google AdMob
/// </summary>
public class GoogleAdsMobile: Singleton<GoogleAdsMobile> {

  private string AppId {
    get {
#if UNITY_ANDROID
      var appId = "ca-app-pub-3525456870772629~6660492912";
#elif UNITY_IPHONE
      var appId = "ca-app-pub-3525456870772629~7166075168";
#else
      var appId = "unexpected_platform";
#endif
      return appId;
    }
  }

  private string RewardedUnitId {
    get {
#if UNITY_ANDROID
      var adUnit = "ca-app-pub-3525456870772629/7534165855";
#elif UNITY_IPHONE
      var adUnit = "ca-app-pub-3525456870772629/8464104147";
#else
      var adUnit = "unexpected_platform";
#endif
      return adUnit;
    }
  }

  private string InterstitialUnitId {
    get {
#if UNITY_ANDROID
      var adUnit = "ca-app-pub-3525456870772629/6085352816";
#elif UNITY_IPHONE
      var adUnit = "ca-app-pub-3525456870772629/9864815728";
#else
      var adUnit = "unexpected_platform";
#endif
      return adUnit;
    }
  }

  private void Start() {
    AdMobManager.Instance.InitRewarded(AppId, RewardedUnitId);
    AdMobManager.Instance.LoadInterstitial(InterstitialUnitId);
  }

  public void ShowRewardVideo(Action<string, double> successCallback, Action<string> errorCallback = null) {

#if UNITY_EDITOR

    if (successCallback != null) successCallback("", 1);
    return;

#endif

    if (AdMobManager.Instance.IsRewardedReady()) {
      AdMobManager.Instance.ShowRewardVideo(
        success: (type, amount) => successCallback(type, amount),
        error: (message) => errorCallback(message)
        );
    } else {
      errorCallback("Not load");
    }
  }

  public void ShowInterstitialVideo(Action<bool> complete = null) {
    if (AdMobManager.Instance.IsInterstitialReady(InterstitialUnitId)) {
      AdMobManager.Instance.ShowInterstitialVideo(InterstitialUnitId,
                    complete: (isCompl) => complete(isCompl));
    }
  }

  public bool NewLevelInterestionReady() {
    return AdMobManager.Instance.IsInterstitialReady(InterstitialUnitId);
  }

}
