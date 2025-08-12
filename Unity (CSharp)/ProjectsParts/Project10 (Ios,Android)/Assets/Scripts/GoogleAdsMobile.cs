using System;
using GoogleMobileAds.Api;
using UnityEngine;

/// <summary>
/// Контроллер рекламы Google AdMob
/// </summary>
public class GoogleAdsMobile : Singleton<GoogleAdsMobile> {

	public string androidAppId;
	public string iosAppId;
	
	private string appId {
		get {
#if UNITY_ANDROID
			return androidAppId;
#elif UNITY_IOS
			return iosAppId;
#else
			return "unexpected_platform";
#endif
		}
	}

	private RewardBasedVideoAd rewardBasedVideo;

	public bool IsLoaded {
		get { return rewardBasedVideo.IsLoaded(); }
	}

	private void Start() {
		
		MobileAds.Initialize(appId);

		RewardedInit();
		RequestRewardedVideo();

	}

	#region Rewarded video

	public string androidRewardedAddId;
	public string iosRewardedAppId;

	private void RewardedInit() {

		rewardBasedVideo = RewardBasedVideoAd.Instance;
		// Called when an ad request has successfully loaded.
		rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
		//// Called when an ad request failed to load.
		rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
		//// Called when an ad is shown.
		rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;
		//// Called when the ad starts to play.
		rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;
		// Called when the user should be rewarded for watching a video.
		rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
		// Called when the ad is closed.
		rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
		// Called when the ad click caused the user to leave the application.
		rewardBasedVideo.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;
		
	}

	private string rewardedId {
		get {
#if UNITY_ANDROID
			return androidRewardedAddId;
#elif UNITY_IOS
			return iosRewardedAppId;
#else
			return "unexpected_platform";
#endif
		}
	}

	public void RequestRewardedVideo() {
		
		AdRequest request = new AdRequest.Builder().Build();
		rewardBasedVideo.LoadAd(request, rewardedId);
	}

	public void HandleRewardBasedVideoLoaded(object sender, EventArgs args) {
		Debug.Log("HandleRewardBasedVideoLoaded event received");
	}

	public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
		Debug.Log(
				"HandleRewardBasedVideoFailedToLoad event received with message: "
												 + args.Message);
	}

	public void HandleRewardBasedVideoOpened(object sender, EventArgs args) {
		Debug.Log("HandleRewardBasedVideoOpened event received");
	}

	public void HandleRewardBasedVideoStarted(object sender, EventArgs args) {
		Debug.Log("HandleRewardBasedVideoStarted event received");
	}

	public void HandleRewardBasedVideoClosed(object sender, EventArgs args) {
		AdRequest request = new AdRequest.Builder().Build();
		rewardBasedVideo.LoadAd(request, rewardedId);
		Debug.Log("HandleRewardBasedVideoClosed event received");
	}

	public void HandleRewardBasedVideoRewarded(object sender, Reward args) {

		isRewarded = true;
		
	}

	private void Update() {

		if (isRewarded) {
			isRewarded = false;
			if (callbackActive != null) {
				callbackActive();
				callbackActive = null;
			}
		}

	}
	
	private void OnApplicationPause(bool pause) {
		if (!pause && isRewarded) {
			isRewarded = false;
			if (callbackActive != null) {
				callbackActive();
				callbackActive = null;
			}
		}
	}
	private bool isRewarded = false;


	public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args) {
		Debug.Log("HandleRewardBasedVideoLeftApplication event received");
	}

	public void ShowRewardVideo(Action callback) {

#if UNITY_EDITOR
		callback();
#else
		if (rewardBasedVideo.IsLoaded()) {
			callbackActive = callback;
			rewardBasedVideo.Show();
		}
#endif



	}

	#endregion

	private Action callbackActive;

}
