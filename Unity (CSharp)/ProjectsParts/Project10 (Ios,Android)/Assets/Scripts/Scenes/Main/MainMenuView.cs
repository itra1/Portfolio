using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using ExEvent;

/// <summary>
/// Scene: main 
/// 
/// </summary>
public class MainMenuView : PanelUi {

	public Animator settingAnim;

	public Action<GameMode, GameLocation> OnSelectGameMode;
	public Action OnSetting;
	public Action OnCredit;
	public Action OnGameCenter;

	private void Start() {
		OnEnableFacebook();
#if UNITY_IOS
		GameCengetInit();
#else
		gameCenterButton.SetActive(false);
#endif

#if UNITY_IOS

		if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneX) {
			RectTransform settingRect = settingAnim.GetComponent<RectTransform>();
			settingRect.anchoredPosition = new Vector2(100, settingRect.anchoredPosition.y);

			RectTransform fbRect = facebookButton.GetComponent<RectTransform>();
			fbRect.anchoredPosition = new Vector2(200, fbRect.anchoredPosition.y);

			RectTransform gcRect = gameCenterButton.GetComponent<RectTransform>();
			gcRect.anchoredPosition = new Vector2(290, gcRect.anchoredPosition.y);
		}

#endif

	}

	protected override void OnEnable() {
		base.OnEnable();
		AudioManager.OnMusic += OnMusicChange;
		AudioManager.OnEffect += OnEffectChange;
		OnMusicChange(AudioManager.Instance.isMusic);
		OnEffectChange(AudioManager.Instance.isEffect);
	}

	protected override void OnDestroy() {
		base.OnDestroy();
		AudioManager.OnMusic -= OnMusicChange;
		AudioManager.OnEffect -= OnEffectChange;
	}

	void OnMusicChange(bool flag) {
		settingAnim.SetBool("music", flag);
	}


	void OnEffectChange(bool flag) {
		settingAnim.SetBool("effect", flag);
	}

	public void OnCompanyButton() {
		UiController.ClickButtonAudio();
		if (OnSelectGameMode != null) OnSelectGameMode(GameMode.levelsConstructor, GameLocation.classic);
	}

	public void OnSurvivalButton() {
		UiController.ClickButtonAudio();
		if (OnSelectGameMode != null) OnSelectGameMode(GameMode.survival, GameLocation.classic);
	}

	public void OnSettingButton() {
		UiController.ClickButtonAudio();
		settingAnim.SetBool("show", !settingAnim.GetBool("show"));
	}

	public void OnSettingMusic() {
		UiController.ClickButtonAudio();
		AudioManager.Instance.isMusic = !AudioManager.Instance.isMusic;
	}

	public void OnSettingSound() {
		UiController.ClickButtonAudio();
		AudioManager.Instance.isEffect = !AudioManager.Instance.isEffect;
	}

	public void OnSettingCredit() {
		UiController.ClickButtonAudio();
		CreditPanel creditPanel = UiController.ShowUi<CreditPanel>();
		creditPanel.gameObject.SetActive(true);
	}

	public GameObject gameCenterButton;

	private void GameCengetInit() {
		gameCenterButton.SetActive(true);
	}

	public void OnGameCenterButton() {
		if (OnGameCenter != null) OnGameCenter();
	}


	public override void BackButton() {
		Application.Quit();
	}

	#region Facebook

	public GameObject facebookButton;
	public Image fbAvatar;                          // Картинка аватара
	bool fbAuth;                                    // Флаг авторизации

	void OnEnableFacebook() {
		FBController.authorizationComplited += FacebookAuth;
		facebookButton.SetActive(true);
		FacebookAuth();
	}

	void OnDisableFacebook() {
		FBController.authorizationComplited -= FacebookAuth;
	}

	// Событие Нажатия кнопки
	public void FBLogin() {
#if PLUGIN_FACEBOOK
		UiController.ClickButtonAudio();
		FBController.fbLogin(FacebookAuth);
#endif
	}

	/// <summary>
	/// Ответ при авторизации
	/// </summary>
	void FacebookAuth() {
		if (FBController.CheckFbLogin) {
			facebookButton.GetComponent<Animator>().SetTrigger("auth");
			StartCoroutine(WaitFb());
		}
	}

	void facebookLoginEvent() {
		if (fbAvatar.GetComponent<Image>().sprite == null)
			StartCoroutine(WaitFb());
	}

	IEnumerator WaitFb() {
		int ii = 0;
		while (ii < 240) {
			ii++;
			yield return new WaitForSeconds(0.5f);
			if (FBController.instance != null && FBController.instance.userName != "") {
				StartCoroutine(DownloadUserAvatar(FBController.instance.userPictureUrl));
				ii = 240;
				yield return 0;
			}
		}
	}

	IEnumerator DownloadUserAvatar(string avaUrl) {
		WWW www = new WWW(avaUrl);
		yield return www;

		if (www.texture.width >= 50 || www.texture.height >= 50)
			fbAvatar.GetComponent<Image>().sprite = Sprite.Create(www.texture, new Rect(0, 0, 50, 50), new Vector2(0, 0));
	}


	#endregion

}
