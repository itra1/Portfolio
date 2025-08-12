using System;
using GoogleMobileAds.Api;
using UnityEngine;

namespace KingBird.AdMob {
    public class InterstitialHolder {
        private const float ReloadTimer = 30f;

        private bool _interstitialAutoReload;
        private InterstitialAd _interstitial;
        string _interstitialUnitId = "unused";
        private InterstitialVideoState _interstitialState = InterstitialVideoState.None;
        private Action<bool> _interstitialComplete;

        private float _reloadTimer;

        public InterstitialHolder(string unitId, bool autoReload) {
            _interstitialAutoReload = autoReload;
            _interstitialUnitId = unitId;

            if (_interstitial != null) {
                _interstitial.Destroy();
                _interstitial = null;
            }
        }

        public void Load() {
            _interstitialState = InterstitialVideoState.None;
            _reloadTimer = ReloadTimer;
            RequestInterstitial();
        }

        public void Update() {
            if (_interstitial == null) return;

            CheckInterstitial();
            InterstitialAdRequester();
        }

        // Used for "autoReload == false"
        public bool IsInterstitialCompleted() {
            return _interstitialState == InterstitialVideoState.Completed;
        }

        public bool IsReady() {
            return _interstitial != null && _interstitial.IsLoaded();
        }

        public void ShowInterstitialVideo(Action<bool> complete = null) {
            _interstitialComplete = complete;

            if (IsReady() == true) {
                _interstitialState = InterstitialVideoState.Opening;
                _interstitial.Show();
            } else {
                Debug.Log("Interstitial video ad is not ready yet");
                InterstitialComplete(false);
            }
        }

        private void CheckInterstitial() {
            switch (_interstitialState) {
                case InterstitialVideoState.None:
                    // Handled by InterstitialAdRequester
                    break;
                case InterstitialVideoState.Completed:
                    InterstitialComplete(true);
                    if (_interstitialAutoReload == true) {
                        RequestInterstitial();
                    } else {
                        _interstitial.Destroy();
                        _interstitial = null;
                        // Don't do anything to state, managed in main app
                        _interstitialState = InterstitialVideoState.Completed;
                    }

                    break;
            }
        }
        
        private void InterstitialAdRequester() {            
            _reloadTimer -= Time.deltaTime;
            if (_reloadTimer > 0) return;

            if (IsReady()) {
                _reloadTimer = ReloadTimer;
                return;
            }

            switch (_interstitialState) {
                case InterstitialVideoState.Completed:
                    // Handled by CheckInterstitial()
                    break;
                case InterstitialVideoState.Loading:
                    // Nothing to do
                    break;
                default:
                    RequestInterstitial();
                    break;
            }
            
            _reloadTimer = ReloadTimer;
        }

        private AdRequest CreateAdRequest() {
            return new AdRequest.Builder().Build();
        }

        private void RequestInterstitial() {
            // Clean up interstitial ad before creating a new one.
            if (_interstitial != null) {
                _interstitial.Destroy();
            }

            // Create an interstitial.
            _interstitial = new InterstitialAd(_interstitialUnitId);

            // Register for ad events.
            _interstitial.OnAdLoaded += HandleInterstitialLoaded;
            _interstitial.OnAdFailedToLoad += HandleInterstitialFailedToLoad;
            _interstitial.OnAdOpening += HandleInterstitialOpened;
            _interstitial.OnAdClosed += HandleInterstitialClosed;

            _interstitialState = InterstitialVideoState.Loading;
            // Load an interstitial ad.
            _interstitial.LoadAd(CreateAdRequest());
        }

        #region Interstitial callback handlers

        private void HandleInterstitialLoaded(object sender, EventArgs args) {
            _interstitialState = InterstitialVideoState.Loaded;

            Debug.Log("HandleInterstitialLoaded event received");
        }

        private void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
            _interstitialState = InterstitialVideoState.None;

            // Do not request immediately, using InterstitialAdRequester() routine

            Debug.Log("HandleInterstitialFailedToLoad event received with message: " + args.Message);
        }

        private void HandleInterstitialOpened(object sender, EventArgs args) {
            _interstitialState = InterstitialVideoState.Opened;

            Debug.Log("HandleInterstitialOpened event received");
        }

        private void HandleInterstitialClosed(object sender, EventArgs args) {
            _interstitialState = InterstitialVideoState.Completed;

            Debug.Log("HandleInterstitialClosed event received");
        }

        #endregion

        private void InterstitialComplete(bool state) {
            if (_interstitialComplete != null) {
                try {
                    _interstitialComplete(state);
                } catch (Exception e) {
                    Debug.LogException(e);
                }
            }

            _interstitialComplete = null;
        }
    }
}