using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KingBird.Ads {
	
	public class AdsUiButton : MonoBehaviour {

		public Animation animComponent;
		public GameObject buttonElement;
		
		private void OnEnable() {
			AdsKingBird.OnChangePhase += OnChangePhase;
			if (AdsKingBird.Instance.phase == AdsKingBird.Phases.showing) {
				Init();
				AdsKingBird.Instance.phase = AdsKingBird.Phases.ready;
			}
			else {
				Init();
			}
		}

		private void OnDisable() {
			AdsKingBird.OnChangePhase -= OnChangePhase;
		}

		private void OnChangePhase(AdsKingBird.Phases phase) {
			Init();
		}

		private void OnShowBanner() {
			animComponent.Play("hide");
		}

		void Init() {

			if (AdsKingBird.Instance.miniBanner == null) return;

			if (AdsKingBird.Instance.phase == AdsKingBird.Phases.ready) {

				if (!buttonElement.activeInHierarchy) {
					buttonElement.SetActive(true);
					animComponent.Play("show");
				}
			}
			else {
				if (buttonElement.activeInHierarchy) {
					animComponent.Play("hide");
				}
				else {
					buttonElement.SetActive(false);
				}
			}
			
		}

		public void ButtonClick() {
			AdsKingBird.Instance.ClickBannerButton();
		}

		public void HideComplete() {
			buttonElement.SetActive(false);
			//AdsManager.Instance.CloseBanner();
		}

	}
}
