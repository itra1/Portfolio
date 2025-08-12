using UnityEngine;
using UnityEngine.UI;

namespace KingBird.Ads {

  public class AdsBanner: MonoBehaviour {

    public Animator animComponent;
    public Image bannerImage;

    private void OnEnable() {
      AdsKingBird.OnChangePhase += OnChangePhase;
      Init();

    }

    private void OnDisable() {
      AdsKingBird.OnChangePhase -= OnChangePhase;
    }

    private void OnChangePhase(AdsKingBird.Phases phase) {
      Init();
    }

    void Init() {

      animComponent.SetBool("banner", ((AdsKingBird.Instance.phase & (AdsKingBird.Phases.showing)) != 0));
      if (AdsKingBird.Instance.phase != AdsKingBird.Phases.none)
        OnShowBanner(AdsKingBird.Instance.miniBanner);

    }

    public void CloseButton() {
      if (AdsKingBird.Instance.phase != AdsKingBird.Phases.showing) return;
      Close();
    }

    private void OnShowBanner(AdsShowResult banner) {
      if (banner != null)
        SetSpriteBanner(banner.BannerData.GetSprite());

    }

    public void LabelButton() {

      if (!KingBird.Ads.KingBirdAds.IsReady(BannerPlacement.BannerSquareInApp)) return;

      AdsKingBird.Instance.phase = AdsKingBird.Phases.showing;
      animComponent.SetBool("banner", true);
    }

    private void SetSpriteBanner(Sprite imageBanner) {
      if (imageBanner == null) return;

      this.bannerImage.sprite = imageBanner;

      if (imageBanner.rect.width > imageBanner.rect.height) {
        bannerImage.rectTransform.sizeDelta = new Vector2(175 / imageBanner.rect.height * imageBanner.rect.width, 175);
      } else {
        bannerImage.rectTransform.sizeDelta = new Vector2(175, 175 / imageBanner.rect.width * imageBanner.rect.height);
      }
    }

    public void PlayButton() {

      AdsKingBird.Instance.OpenBanner();
      Close();
    }

    private void Close() {
      animComponent.SetBool("banner", false);
      AdsKingBird.Instance.CloseBanner();
    }

    public void HideComplete() {
    }

  }

}