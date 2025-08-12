using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;

namespace KingBird.Ads {

	public class AdsBanner : MonoBehaviour {

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
			Init();
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

		public void CloseButton() {
			if (AdsKingBird.Instance.phase != AdsKingBird.Phases.showing) return;
			Close();
		}

		private void OnShowBanner(AdsShowResult banner) {
			if (isVisible) return;
			isVisible = true;
			dialog.SetActive(true);
			
			SetSpriteBanner(banner.BannerData.GetSprite());
			
			animComponent.Play("show");
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
			animComponent.Play("hide");
			isVisible = false;
			AdsKingBird.Instance.CloseBanner();
		}

		public void HideComplete() {
			dialog.SetActive(false);
		}

	}

}