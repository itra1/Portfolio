using System;
using KingBird.Ads;
using UnityEngine;
using UnityEngine.UI;

public class DemoUI : MonoBehaviour {
    [SerializeField] private InputField _applicationIdIF;
    [SerializeField] private Text _statusLabel;
    [SerializeField] private Text _infoLabel;
    [SerializeField] private Image _iconImage;

#if UNITY_ANDROID
    private string _apiKey = "fab971cb-0c98-4b1e-b4ad-3b7b1a900d33";
#elif UNITY_IOS
    private string _apiKey = "501b333b-8755-4dda-9900-cce46a3f35fe";
#endif
    void Start() {
        _applicationIdIF.text = _apiKey;
    }

    public void Update() {
        _statusLabel.text = "IsInit: " + KingBirdAds.IsInit() + ", IsReady(Banner):" +
                            KingBirdAds.IsReady(BannerPlacement.BannerSquareInApp) +
                            ", IsReady(FullBanner):" +
                            KingBirdAds.IsReady(BannerPlacement.BannerFullScreenStatic);
    }

    public void InitButtonClick() {
        KingBirdAds.Initialize(new AdsInitOptions().SetApplicationId(_applicationIdIF.text).SetLocale("en-US").SetFullBannerMinLaunch(0).SetFullBannerMinGameTime(0));
    }

    private void UpdateInfo(AdsShowResult showResult) {
        if (showResult.ResultType != AdsShowResultType.Finished) return;
        switch (showResult.Placement) {
            case BannerPlacement.BannerSquareInApp:
                var bannerInfo = showResult.BannerData;
                if (bannerInfo != null)
                    _infoLabel.text = JsonUtility.ToJson(bannerInfo);
                else 
                    _infoLabel.text = "";
                
                _iconImage.sprite = bannerInfo.GetSprite();
                break;
        }
        
        Debug.Log("DemoUI | UpdateInfo: AdsType " + showResult.Placement + ", ResultType " +
                  showResult.ResultType + ", ShowOptionsType " + showResult.ShowOptionsType);
        switch (showResult.ResultType) {
            case AdsShowResultType.Finished:
                Debug.Log("The ad was successfully shown.");
                //
                // YOUR CODE TO REWARD THE GAMER
                // Give coins etc.
                break;
            case AdsShowResultType.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case AdsShowResultType.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
        
    }

    public void ShowBannerClick() {
        KingBirdAds.Show(BannerPlacement.BannerSquareInApp,
            new AdsShowOptions().SetShowOptionsType(AdsShowOptionsType.OnlyInfo).OnComplete(UpdateInfo));
    }

    public void ClickBannerButton() {
        KingBirdAds.SendClick(BannerPlacement.BannerSquareInApp);
    }

    public void ShowFullBannerClick() {
        KingBirdAds.Show(BannerPlacement.BannerFullScreenStatic,
            new AdsShowOptions().SetShowOptionsType(AdsShowOptionsType.FullScreenWithWait).OnComplete(UpdateInfo));
    }

    public void ClickFullBannerButton() {
        KingBirdAds.SendClick(BannerPlacement.BannerFullScreenStatic);
    }

    public void ClearButtonClick() {
        PlayerPrefs.DeleteAll();
    }
    
}