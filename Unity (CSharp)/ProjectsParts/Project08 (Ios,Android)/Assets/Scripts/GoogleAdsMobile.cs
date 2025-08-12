using System;
using GoogleMobileAds.Api;
using UnityEngine;
using KingBird.AdMob;

/// <summary>
/// Контроллер рекламы Google AdMob
/// </summary>
public class GoogleAdsMobile : Singleton<GoogleAdsMobile> {
  
  private string AppId {
    get {
#if UNITY_ANDROID
      var appId = "ca-app-pub-3525456870772629~7311265343";
#elif UNITY_IPHONE
                var appId = "ca-app-pub-3525456870772629~8753119776";
#else
                var appId = "unexpected_platform";
#endif
      return appId;
    }
  }

  private string RewardedUnitId {
    get {
#if UNITY_ANDROID
      var adUnit = "ca-app-pub-3525456870772629/7241082777";
#elif UNITY_IPHONE
                var adUnit = "ca-app-pub-3525456870772629/2705273093";
#else
                var adUnit = "unexpected_platform";
#endif
      return adUnit;
    }
  }

  private string InterstitialUnitId {
    get {
#if UNITY_ANDROID
      var adUnit = "ca-app-pub-3525456870772629/2641368885";
#elif UNITY_IPHONE
                var adUnit = "ca-app-pub-3525456870772629/4408664551";
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
    if (AdMobManager.Instance.IsRewardedReady()) {
      AdMobManager.Instance.ShowRewardVideo(
        success: (type, amount) => successCallback(type, amount),
        error: (message) => errorCallback(message)
        );
    }
  }

  public void ShowInterstitialVideo(Action<bool> complete = null) {
    if (AdMobManager.Instance.IsInterstitialReady(InterstitialUnitId)) {
      AdMobManager.Instance.ShowInterstitialVideo( InterstitialUnitId,
                    complete: (isCompl) => complete(isCompl));
    }
  }

  public bool NewLevelInterestionReady() {
    return AdMobManager.Instance.IsInterstitialReady(InterstitialUnitId);
  }

}
