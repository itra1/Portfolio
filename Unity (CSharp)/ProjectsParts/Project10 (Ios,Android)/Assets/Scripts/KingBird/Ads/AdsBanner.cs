using UnityEngine;
using UnityEngine.UI;

namespace KingBird.Ads {

  public class AdsBanner: MonoBehaviour {

    public Animation animComponent;
    public Image bannerImage;
    public GameObject dialog;
    private bool isVisible;

    private void OnEnable() {
      AdsKingBird.OnChangePhase += OnChangePhase;
      Init();
    }

    private void OnDisable() {
      AdsKingBird.OnChangePhase -= OnChangePhase;
    }

    private void OnChangePhase(AdsKingBird.Phases phase) {

      if (phase == AdsKingBird.Phases.ready) {
        SetSpriteBanner(AdsKingBird.Instance.miniBanner.BannerData.GetSprite());
        Init();
      }
    }

    void Init() {

      if (AdsKingBird.Instance.phase == AdsKingBird.Phases.showing) {

        OnShowBanner(AdsKingBird.Instance.miniBanner);

      } else {
        isVisible = false;
        if (dialog.activeInHierarchy) {
          animComponent.Play("hide");
        } else {
          dialog.SetActive(false);
        }
      }

    }

    private void OnShowBanner(AdsShowResult banner) {
      if (isVisible) return;
      isVisible = true;
      dialog.SetActive(true);


      animComponent.Play("show");
    }

    private void SetSpriteBanner(Sprite imageBanner) {
      if (imageBanner == null) return;

      this.bannerImage.sprite = imageBanner;

      //if (imageBanner.rect.width > imageBanner.rect.height) {
      //  bannerImage.rectTransform.sizeDelta = new Vector2(140 / imageBanner.rect.height * imageBanner.rect.width, 140);
      //} else {
      //  bannerImage.rectTransform.sizeDelta = new Vector2(140, 140 / imageBanner.rect.width * imageBanner.rect.height);
      //}
    }

    public void PlayButton() {

      AdsKingBird.Instance.OpenBanner();
      Close();
    }

    private void Close() {
      animComponent.Play("hide");
      isVisible = false;
      AdsKingBird.Instance.CloseBanner();
    }

    public void HideComplete() {
      dialog.SetActive(false);
    }

    public void CloseButton() {
      Close();
    }

    public void OpenBanner() {
      animComponent.Play("show");
    }

    public void ShowBanner() {

    }

  }

}