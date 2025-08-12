using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Playables;

namespace KingBird.AdMob {
    public class AdMobManager : MonoBehaviour {
        private static AdMobManager _instance;

        public static AdMobManager Instance {
            get {
                if (_instance != null) return _instance;

                var go = new GameObject("[AdMob Manager]", typeof(AdMobManager));
                DontDestroyOnLoad(go);
                _instance = go.GetComponent<AdMobManager>();
                return _instance;
            }
        }

        private string _appId;

        // Rewarded
        private RewardBasedVideoAd _rewardBasedVideo;
        private bool _rewardedInit;
        private Action<string, double> _rewardedSuccess = null;
        private Action<string> _rewardedError = null;
        private RewardedVideoState _rewardedShowState = RewardedVideoState.None;
        private string _rewardedUnitId;
        private string _rewardType;
        private double _rewardAmount;

        // Interstitial
        private bool _interstitialInit;
        private readonly Dictionary<string, InterstitialHolder> _interstitials = new Dictionary<string, InterstitialHolder>();

        private void Update() {
            CheckRewarded();
            UpdateInterstitial();
        }

        private void CheckRewarded() {
            if (_rewardedInit == false) return;

            switch (_rewardedShowState) {
                case RewardedVideoState.None:
                    // Handled by RewardedAdRequester
                    return;                
                case RewardedVideoState.Rewarded:
                    RewardedSuccess(_rewardType, _rewardAmount);
                    RequestRewardVideo();
                    break;
                case RewardedVideoState.ManuallyClosed:
                    RewardedError("Rewarded video is closed");
                    RequestRewardVideo();
                    break;
                default:
                    // Nothing to do
                    break;
            }
        }

        private void UpdateInterstitial() {
            if (_interstitialInit == false) return;

            using (var iterator = _interstitials.GetEnumerator()) {
                while (iterator.MoveNext()) {
                    iterator.Current.Value.Update();
                }
            }
        }

        [Obsolete("Use InitRewarded() instead")]
        public void Init(string appId, string unitId) {
            InitRewarded(appId, unitId);
        }

        public void InitRewarded(string appId, string unitId) {
            if (_rewardedInit) return;
            _rewardedInit = true;
            _appId = appId;
            _rewardedUnitId = unitId;

            MobileAds.SetiOSAppPauseOnBackground(true);

            MobileAds.Initialize(_appId);

            _rewardBasedVideo = RewardBasedVideoAd.Instance;
            _rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
            _rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
            _rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;
            _rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;
            _rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
            _rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;

            StopCoroutine(RewardedAdRequester());
            StartCoroutine(RewardedAdRequester());
        }
        
        public void LoadInterstitial(string unitId, bool autoReload = true) {
            if (_interstitialInit == false) {
                _interstitialInit = true;
                MobileAds.SetiOSAppPauseOnBackground(true);
            } 
            
            if (_interstitials.ContainsKey(unitId) == false) 
                _interstitials.Add(unitId, new InterstitialHolder(unitId, autoReload));

            _interstitials[unitId].Load();
        }

        [Obsolete("Use IsRewardedReady() instead")]
        public bool IsReady() {
            return _rewardBasedVideo != null && _rewardBasedVideo.IsLoaded();
        }

        public bool IsRewardedReady() {
            return _rewardBasedVideo != null && _rewardBasedVideo.IsLoaded();
        }

        public bool IsInterstitialReady(string unitId) {
            return _interstitials.ContainsKey(unitId) && _interstitials[unitId].IsReady();
        }
        
        // Used for "autoReload == false"
        public bool IsInterstitialCompleted(string unitId) {
            return _interstitials.ContainsKey(unitId) == false || _interstitials[unitId].IsInterstitialCompleted();
        }

        public void ShowRewardVideo(Action<string, double> success, Action<string> error = null) {
            _rewardedShowState = RewardedVideoState.None;

            _rewardedSuccess = success;
            _rewardedError = error;

            if (_rewardBasedVideo.IsLoaded()) {
                _rewardedShowState = RewardedVideoState.Opening;
                _rewardBasedVideo.Show();
            } else {
                _rewardedShowState = RewardedVideoState.None;
                RewardedError("Reward based video ad is not ready yet");
            }
        }
        
        public void ShowInterstitialVideo(string unitId, Action<bool> complete = null) {
            if (_interstitials.ContainsKey(unitId) == true) {
                _interstitials[unitId].ShowInterstitialVideo(complete);
            } else {
                Debug.LogError("ERROR: Interstitial not created");
                if (complete != null) complete(false);
            }
        }

        private void OnEnable() {
            if (_rewardedInit) {
                StopCoroutine(RewardedAdRequester());
                StartCoroutine(RewardedAdRequester());
            }         
        }

        // Automatic back-up unit in case of unexpected errors
        private IEnumerator RewardedAdRequester() {
            var waiter = new WaitForSeconds(30f);
            while (true) {
                if (IsRewardedReady()) {
                    yield return waiter;
                    continue;
                }

                if (_rewardedShowState != RewardedVideoState.Loading)
                    RequestRewardVideo();
                yield return waiter;
            }
        }

        private AdRequest CreateAdRequest() {
            return new AdRequest.Builder()
                //.AddTestDevice(AdRequest.TestDeviceSimulator)
                //.AddTestDevice("0123456789ABCDEF0123456789ABCDEF")
                //.AddKeyword("game")
                //.SetGender(Gender.Male)
                //.SetBirthday(new DateTime(1985, 1, 1))
                //.TagForChildDirectedTreatment(false)
                //.AddExtra("color_bg", "9B30FF")
                .Build();
        }

        private void RequestRewardVideo() {
            _rewardedShowState = RewardedVideoState.Loading;

            this._rewardBasedVideo.LoadAd(this.CreateAdRequest(), _rewardedUnitId);
        }



        #region RewardBasedVideo callback handlers

        private void HandleRewardBasedVideoLoaded(object sender, EventArgs args) {
            _rewardedShowState = RewardedVideoState.Loaded;

            var ad = (RewardBasedVideoAd) sender;

            // Android MediationAdapterClassName()
            // com.google.ads.mediation.facebook.FacebookAdapter
            // com.google.ads.mediation.admob.AdMobAdapter
            // com.google.ads.mediation.unity.UnityAdapter

            // iOS MediationAdapterClassName()
            // GADMAdapterUnity
            // GADMAdapterGoogleAdMobAds
            // ?? FaceBook ??

            print("HandleRewardBasedVideoLoaded event received: " + ad.MediationAdapterClassName());
        }

        private void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
            _rewardedShowState = RewardedVideoState.None;

            print("HandleRewardBasedVideoFailedToLoad event received with message: " + args.Message);
        }

        private void HandleRewardBasedVideoOpened(object sender, EventArgs args) {
            _rewardedShowState = RewardedVideoState.Opened;

            print("HandleRewardBasedVideoOpened event received");
        }

        private void HandleRewardBasedVideoStarted(object sender, EventArgs args) {
            _rewardedShowState = RewardedVideoState.Started;

            print("HandleRewardBasedVideoStarted event received");
        }

        private void HandleRewardBasedVideoClosed(object sender, EventArgs args) {
            if (_rewardedShowState == RewardedVideoState.Rewarded) return;

            // Do work in main thread, `void Update()`
            _rewardedShowState = RewardedVideoState.ManuallyClosed;
        }

        private void HandleRewardBasedVideoRewarded(object sender, Reward args) {

      Debug.Log("HandleRewardBasedVideoRewarded");

            _rewardType = args.Type;
            _rewardAmount = args.Amount;

            // Do work in main thread, `void Update()`
            _rewardedShowState = RewardedVideoState.Rewarded;
        }

        #endregion

        private void RewardedSuccess(string type, double amount) {

      Debug.Log("RewardedSuccess");

            _rewardedShowState = RewardedVideoState.None;

            if (_rewardedSuccess != null) {
                try {
                    _rewardedSuccess(type, amount);
                } catch (Exception e) {
                    Debug.LogException(e);
                }
            }
            _rewardedError = null;
            _rewardedSuccess = null;
        }

        private void RewardedError(string message) {
            _rewardedShowState = RewardedVideoState.None;

            print(message);

            if (_rewardedError != null) {
                try {
                    _rewardedError(message);
                } catch (Exception e) {
                    Debug.LogException(e);
                }
            }
            _rewardedError = null;
            _rewardedSuccess = null;
        }        
    }
}