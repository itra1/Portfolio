using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace KingBird.Ads {
    public class KingBirdAds : MonoBehaviour {
        private const string API_URL = "http://ads.kingbirdgames.com/ads/app/1.0";

        private const string API_REQUEST_URL = "/randomBanner?token={APIKey}&uid={UID}&placement={PlacementId}&locale={Locale}";

        private const string API_VIEW_URL = "/banner/{bannerId}/view?token={APIKey}&uid={UID}";
        private const string API_CLICK_URL = "/banner/{bannerId}/click?token={APIKey}&uid={UID}";
        
        private const string ADS_UID_PLAYERPREF = "kingbird_ads_uid_playerpref";
        private const string INIT_COUNTER_PLAYERPREFS = "kingbird_ads_init_counter";
        private const string LAST_FULL_BANNER_TIME_PLAYERPREFS = "kingbird_last_full_banner_time";

        private const string BaseDataPath = "KingBirdAds/";
        
        private const float CHECK_RECEIVE_COOLDOWN = 5f;
        private const float FULL_BANNER_TIMER_INTERVAL = 60f;

        private static string _apiKey;

        private static readonly Dictionary<BannerPlacement, BannerClientData> _banners = new Dictionary<BannerPlacement, BannerClientData>();
        private static readonly Dictionary<BannerPlacement, BannerClientData> _bannersShown = new Dictionary<BannerPlacement, BannerClientData>();
        private static Texture2D _lastTexture2D;
        private static string _UID;
        private static int _initCounter;
        private static float _lastFullBannerShow;
        private static int _fullBannerMinLaunch;
        private static float _fullBannerMinGameTime;
        private static string _locale;
        private static GameObject _fullScreenBannerPrefab;
        private static bool _isInit;

        private static KingBirdAds _instance;

        public static KingBirdAds Instance {
            get {
                if (_instance != null) return _instance;
                var go = new GameObject("[KingBirdAds]", typeof(KingBirdAds));
                _instance = go.GetComponent<KingBirdAds>();
                DontDestroyOnLoad(go);
                return _instance;
            }
        }
        
        public void Start() {
            _fullScreenBannerPrefab = Resources.Load<GameObject>(BaseDataPath + "FullScreenBanner");
        }
        
        private void OnEnable() {
            if(IsInit()) StartLifeTimeCoroutines();
        }

        private void StartLifeTimeCoroutines() {
            StartCoroutine(CheckReceive());
            StartCoroutine(FullBannerTimeCounter());            
        }

        public static void Initialize(AdsInitOptions adsInitOptions) {
            
            _apiKey = adsInitOptions.ApplicationId;
            _banners.Clear();
            _bannersShown.Clear();
            InitUID();

            _locale = ConvertLanguageCodeToLocale(adsInitOptions.Locale);
            _fullBannerMinGameTime = adsInitOptions.FullBannerMinGameTime;
            _fullBannerMinLaunch = adsInitOptions.FullBannerMinLaunch;

            _isInit = true;            
            Instance.StopAllCoroutines();
            Instance.StartLifeTimeCoroutines();
            
            InitCounter();
            InitLastFullBannerShow();
        }

        public static bool IsReady(BannerPlacement bannerPlacement) {
            if (IsInit() == false) return false;
            return _banners.ContainsKey(bannerPlacement) && _banners[bannerPlacement].IsShown() == false && _banners[bannerPlacement].IsFullReady();
        }

        public static bool IsInit() {
            return _isInit;
        }

        public static void Show(BannerPlacement bannerPlacement, AdsShowOptions showOptions) {
            if (IsInit() == false) {
                Debug.LogError("KingBirdAds is not initialized");
                return;
            }
            
            var bannerNotFound = _banners.ContainsKey(bannerPlacement) == false || _banners[bannerPlacement].IsShown() == true;
            var fullScreenBlocked = bannerPlacement == BannerPlacement.BannerFullScreenStatic &&
                                     _lastFullBannerShow < _fullBannerMinGameTime && _initCounter < _fullBannerMinLaunch;
            
            if (bannerNotFound || fullScreenBlocked) {
                showOptions.ResultCallback(new AdsShowResult {
                    Placement = bannerPlacement,
                    ResultType = AdsShowResultType.Failed,
                    ShowOptionsType = showOptions.ShowOptionsType
                });
                return;
            }

            Instance.StartCoroutine(Instance.SendView(bannerPlacement));

            _banners[bannerPlacement].Show();

            if (!_bannersShown.ContainsKey(bannerPlacement)) {
                _bannersShown.Add(bannerPlacement, _banners[bannerPlacement]);
            } else {
                _bannersShown[bannerPlacement] = _banners[bannerPlacement];                
            }
          
            switch (showOptions.ShowOptionsType) {
                case AdsShowOptionsType.OnlyInfo:
                    if (showOptions.ResultCallback != null) {
                        showOptions.ResultCallback(new AdsShowResult {
                            Placement = bannerPlacement,
                            ResultType = AdsShowResultType.Finished,
                            ShowOptionsType = showOptions.ShowOptionsType,
                            BannerData = _banners.ContainsKey(bannerPlacement) ? _banners[bannerPlacement] : null
                        });
                    }

                    break;
                case AdsShowOptionsType.FullScreenWithWait:
                    var go = Instantiate(showOptions.BannerPrefab == null ? _fullScreenBannerPrefab : showOptions.BannerPrefab).GetComponent<FullScreenBannerWindow>();
                    var bannerId = _banners[bannerPlacement].id;
                    go.Show(showOptions, _banners[bannerPlacement], (click) => {
                        if (click) SendClick(bannerPlacement, bannerId);
                        if (showOptions.ResultCallback != null) {
                            showOptions.ResultCallback(new AdsShowResult {
                                Placement = bannerPlacement,
                                ResultType = AdsShowResultType.Finished,
                                ShowOptionsType = showOptions.ShowOptionsType,
                            });
                        }
                    });
                    break;
            }
        }

        private IEnumerator CheckReceive() {
            var waiter = new WaitTimerYieldInstruction(CHECK_RECEIVE_COOLDOWN);
            while (true) {
                yield return CheckBanner();
                yield return CheckFullBanner();
                waiter.Reset(CHECK_RECEIVE_COOLDOWN);
                yield return waiter;
            }
        }

        private IEnumerator FullBannerTimeCounter() {
            var waiter = new WaitTimerYieldInstruction(FULL_BANNER_TIMER_INTERVAL);
            while (true) {
                waiter.Reset(FULL_BANNER_TIMER_INTERVAL);
                yield return waiter;
                _lastFullBannerShow += FULL_BANNER_TIMER_INTERVAL;
                PlayerPrefs.SetFloat(LAST_FULL_BANNER_TIME_PLAYERPREFS, _lastFullBannerShow);
                PlayerPrefs.Save();
            }
        }

        private IEnumerator CheckBanner() {
            if (IsReady(BannerPlacement.BannerSquareInApp)) yield break;

            var url = GetRequestURL(BannerPlacement.BannerSquareInApp);
            var www = new WWW(url);
            yield return www;
      
            if (string.IsNullOrEmpty(www.error) == false) {
                Debug.LogError("KingBirdAds | CheckBanner | error: " + www.error);
                yield break;
            } 
            
            Debug.Log("KingBirdAds | CheckBanner | receive: " + www.text);

            BannerClientData banner;
            try {
                banner = JsonUtility.FromJson<BannerClientData>(www.text);
            } catch (Exception e) {
                _banners[BannerPlacement.BannerSquareInApp] = null;
                Debug.LogException(e);
                yield break;
            }

            yield return GetTexture2DFromUrl(banner.url);
            banner.SetTexture2D(_lastTexture2D);        
            _banners[BannerPlacement.BannerSquareInApp] = banner;
        }

        private IEnumerator CheckFullBanner() {
            if (IsReady(BannerPlacement.BannerFullScreenStatic)) yield break;

            var url = GetRequestURL(BannerPlacement.BannerFullScreenStatic);
            var www = new WWW(url);
            yield return www;

      if (string.IsNullOrEmpty(www.error) == false) {
                Debug.LogError("KingBirdAds | CheckFullBanner | error: " + www.error);
                yield break;
            } 
            
            Debug.Log("KingBirdAds | CheckFullBanner | receive: " + www.text);

            BannerClientData banner;
            try {
                banner = JsonUtility.FromJson<BannerClientData>(www.text);
            } catch (Exception e) {
                _banners[BannerPlacement.BannerFullScreenStatic] = null;
                Debug.LogException(e); 
                yield break;
            }

            yield return GetTexture2DFromUrl(banner.url);
            banner.SetTexture2D(_lastTexture2D);
            _banners[BannerPlacement.BannerFullScreenStatic] = banner;            
        }

        private static string GetRequestURL(BannerPlacement bannerPlacement) {
            var url = API_URL + API_REQUEST_URL;
            url = url.Replace("{APIKey}", _apiKey);
            url = url.Replace("{UID}", _UID);
            url = url.Replace("{PlacementId}", ((int) bannerPlacement).ToString());
            url = url.Replace("{Locale}", _locale);
            return url;
        }

        private static string GetViewURL(int bannerId) {
            var url = API_URL + API_VIEW_URL;
            url = url.Replace("{APIKey}", _apiKey);
            url = url.Replace("{UID}", _UID);
            url = url.Replace("{bannerId}", bannerId.ToString());
            return url;
        }

        private static string GetClickURL(int bannerId) {
            var url = API_URL + API_CLICK_URL;
            url = url.Replace("{APIKey}", _apiKey);
            url = url.Replace("{UID}", _UID);
            url = url.Replace("{bannerId}", bannerId.ToString());
            return url;
        }

        private IEnumerator GetTexture2DFromUrl(string url) {
            var www = new WWW(url);
            yield return www;
            try {
                _lastTexture2D = www.texture;
            } catch (Exception e) {
                _lastTexture2D = null;
                Debug.LogException(e);
            }
        }

        private IEnumerator SendView(BannerPlacement bannerPlacement) {
            var url = GetViewURL(GetCurrentBannerId(bannerPlacement));
            var www = new WWW(url);
            yield return www;

      if (!string.IsNullOrEmpty(www.error)) {
                Debug.LogError("KingBirdAds | SendView | error: " + www.error);
            } else {
                Debug.Log("KingBirdAds | SendView | receive: " + www.text);
            }
        }

        public static void SendClick(BannerPlacement bannerPlacement, int bannerId = 0) {
            if (_bannersShown.ContainsKey(bannerPlacement) == false) return;
            if (_bannersShown[bannerPlacement].IsClicked() == true) return;
            Instance.StartCoroutine(Instance.SendClickInternal(bannerPlacement, bannerId));
        }

        private IEnumerator SendClickInternal(BannerPlacement bannerPlacement, int bannerId = 0) {
            _bannersShown[bannerPlacement].Click();
            var url = GetClickURL(bannerId == 0 ? GetCurrentBannerId(bannerPlacement) : bannerId);
            var www = new WWW(url);
            yield return www;

      if (!string.IsNullOrEmpty(www.error)) {
                Debug.LogError("KingBirdAds | SendClick | error: " + www.error);
                _bannersShown[bannerPlacement].NoClick();
            } else {
                Debug.Log("KingBirdAds | SendClick | receive: " + www.text);
            }
        }

        private static string GetUUID() {
            var bytes = Guid.NewGuid().ToByteArray();
            return string.Format("{3:x2}{2:x2}{1:x2}{0:x2}-{5:x2}{4:x2}-{7:x2}{6:x2}-{8:x2}{9:x2}-{10:x2}{11:x2}{12:x2}{13:x2}{14:x2}{15:x2}", 
                bytes[0], bytes[1], bytes[2], bytes[3],
                bytes[4], bytes[5], bytes[6], bytes[7], 
                bytes[8], bytes[9], bytes[10], bytes[11], 
                bytes[12], bytes[13], bytes[14], bytes[15]);
        }

        private static void InitUID() {
            if (PlayerPrefs.HasKey(ADS_UID_PLAYERPREF)) {
                _UID = PlayerPrefs.GetString(ADS_UID_PLAYERPREF);
            } else {
                _UID = GetUUID();
                PlayerPrefs.SetString(ADS_UID_PLAYERPREF, _UID);
                PlayerPrefs.Save();
            }
        }

        private static void InitCounter() {
            if (PlayerPrefs.HasKey(INIT_COUNTER_PLAYERPREFS)) {
                _initCounter = PlayerPrefs.GetInt(INIT_COUNTER_PLAYERPREFS);
                _initCounter++;
            } else {
                _initCounter = 1;
            }

            PlayerPrefs.SetInt(INIT_COUNTER_PLAYERPREFS, _initCounter);
            PlayerPrefs.Save();
        }

        private static void InitLastFullBannerShow() {
            if (PlayerPrefs.HasKey(LAST_FULL_BANNER_TIME_PLAYERPREFS)) {
                _lastFullBannerShow = PlayerPrefs.GetFloat(LAST_FULL_BANNER_TIME_PLAYERPREFS);
            } else {
                _lastFullBannerShow = 0;
                PlayerPrefs.SetFloat(LAST_FULL_BANNER_TIME_PLAYERPREFS, _lastFullBannerShow);
                PlayerPrefs.Save();
            }
        }

        private static string ConvertLanguageCodeToLocale(string languageCode) {
            var locale = languageCode;
            try {
                locale = CultureInfo.CreateSpecificCulture(languageCode).Name;
            } catch (Exception e) {
                Debug.LogException(e);
            }

            return locale;
        }

        private int GetCurrentBannerId(BannerPlacement bannerPlacement) {
            return _banners.ContainsKey(bannerPlacement) ? _banners[bannerPlacement].id : -1;
        }
    }
}