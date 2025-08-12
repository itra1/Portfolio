using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUi : UiPanel {

	public Animation animComponent;

	private string _hideAnim = "hide";
	private string _showAnim = "show";

	public CoinsWidget coinsPanel;
	public Transform coinsTransform;

	protected override void OnEnable() {
		base.OnEnable();
		KingBird.Ads.AdsKingBird.Instance.OpenWindow();
		noAdsButton.gameObject.SetActive(!(PlayerManager.Instance.company.byeAllLocation || PlayerManager.Instance.noAds));
	}
	
	public void ShowCoinsPanel() {
		coinsPanel.ShowWidget();
	}

	public void HideCoinsPanel() {
		coinsPanel.HideWidget();
	}

	public void AddCoinsPanel() {
		coinsPanel.AddCoins();
	}

	public void MoreGamesButton() {
		AudioManager.Instance.library.PlayClickAudio();
		GameManager.Instance.MoreGames();
	}

	public void SettingButton() {
		//AudioManager.Instance.library.PlayClickAudio();
		GameManager.Instance.Setting();
	}

	public void ShopButton() {
		//AudioManager.Instance.library.PlayClickAudio();
		GameManager.Instance.Shop();
	}

	public void GetBonusButton() {
		AudioManager.Instance.library.PlayClickAudio();
		GameManager.Instance.GetBonus();
	}

	public void PlayButton() {
		AudioManager.Instance.library.PlayClickAudio();
		GameManager.Instance.MainPlay();
	}

	public override void Show(Action OnShow = null) {
		base.Show(OnShow);
		animComponent.Play(_showAnim);
	}

	public override void Hide(Action OnHide = null) {
		base.Hide(OnHide);
		animComponent.Play(_hideAnim);
	}

	public override void ManagerClose() {

		ExitGameQuestion panel = (ExitGameQuestion) UIManager.Instance.GetPanel(UiType.exitGameQuestion);

		if (panel.isActiveAndEnabled) {
			panel.Hide();
		}
		else {
			panel.gameObject.SetActive(true);
			panel.OnCancel = () => {
				panel.Hide();
			};
			panel.OnExit = () => {
				Application.Quit();
			};
		}

	}

	public GameObject noAdsButton;

	public void NoAdsClick() {

		BillingManager.Instance.ByeProduct(BillingManager.Instance.GetBillingProduct(IapType.locationAll)[0], () => {
			PlayerManager.Instance.noAds = true;
			noAdsButton.gameObject.SetActive(!(PlayerManager.Instance.company.byeAllLocation || PlayerManager.Instance.noAds));
			PlayerManager.Instance.Save();
		});
	}

	//public void ShowRewardedVideo() {
	//	GoogleAdsMobile.Instance.ShowRewardVideo();
	//}

}
