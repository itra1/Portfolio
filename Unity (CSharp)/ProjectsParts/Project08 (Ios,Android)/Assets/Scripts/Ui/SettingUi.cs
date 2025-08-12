using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.WSA;
using VoxelBusters.NativePlugins;

public class SettingUi : UiPanel {

	public LanuageData companyLanguage;
	public List<LanuageLibrary> _companyLanguageList = new List<LanuageLibrary>();
	private LanuageLibrary _activeLangCompany;
	private int _activePageCompany;

	public LanuageData translateLanuage;
	public List<LanuageLibrary> _translateLanguageList = new List<LanuageLibrary>();
	private LanuageLibrary _activeTranslateLang;
	private int _activePageTranslate;

	public Animation animComp;

	public RectTransform musicButton;
	public RectTransform effectsButton;

	public RectTransform gameCenterIosButton;
	public RectTransform gameCenterAndroidButton;
	public RectTransform exitAppButton;

	private bool _locationChange;

	protected override void OnEnable() {
		base.OnEnable();

		if (LanguageManager.Instance.activeLanuage.code == "ru") {
			companyLanguage.main.gameObject.SetActive(true);
			translateLanuage.main.anchoredPosition = new Vector2(0, -169);
		} else {
			companyLanguage.main.gameObject.SetActive(false);
			translateLanuage.main.anchoredPosition = new Vector2(0, 30);
		}

		GameService.OnAuthEvent += ChangeButtonCount;

		//translateLanuage.main.gameObject.SetActive(LanguageManager.Instance.activeLanuage.code == "en");

		ChangeButtonCount();

		InitCompanyLanuage();
		InitTranslateLanuage();

		ConfirmMainLanuage();
		ConfirmSecondaryLanuage();

		AudioManager.Instance.library.PlayWindowOpenAudio();
	}

	protected override void OnDisable() {
		base.OnDisable();
		GameService.OnAuthEvent -= ChangeButtonCount;

		if (_locationChange) {
			ExEvent.PlayerEvents.OnChangeCompany.Call(PlayerManager.Instance.company.actualCompany,true);
		}

	}

	private void ChangeButtonCount() {
#if UNITY_ANDROID
		if (gameCenterAndroidButton != null)
			gameCenterAndroidButton.gameObject.SetActive(true);
		exitAppButton.gameObject.SetActive(exitAppButton != null && GameService.Instance.isAuth);
#else
		if(gameCenterAndroidButton != null)
		gameCenterAndroidButton.gameObject.SetActive(false);
		if (exitAppButton != null)
		exitAppButton.gameObject.SetActive(false);
#endif
#if UNITY_IOS
		if (gameCenterIosButton != null)
		gameCenterIosButton.gameObject.SetActive(true);
#else
		if (gameCenterIosButton != null)
			gameCenterIosButton.gameObject.SetActive(false);
#endif

#if UNITY_IOS
		IosPosition();
#else
		AndroidPosition();
#endif
	}

	private void AndroidPosition() {

		if (GameService.Instance.isAuth) {
			musicButton.anchoredPosition = new Vector2(-210 + 0 * 140, 0);
			effectsButton.anchoredPosition = new Vector2(-210 + 1 * 140, 0);
			gameCenterAndroidButton.anchoredPosition = new Vector2(-210 + 2 * 140, 0);
			exitAppButton.anchoredPosition = new Vector2(-210 + 3 * 140, 0);
		} else {

			musicButton.anchoredPosition = new Vector2(-140 + 0 * 140, 0);
			effectsButton.anchoredPosition = new Vector2(-140 + 1 * 140, 0);
			gameCenterAndroidButton.anchoredPosition = new Vector2(-140 + 2 * 140, 0);
			//exitAppButton.anchoredPosition = new Vector2(-210 + 3 * 140, 0);
		}
	}

	private void IosPosition() {
		musicButton.anchoredPosition = new Vector2(-140 + 0 * 140, 0);
		effectsButton.anchoredPosition = new Vector2(-140 + 1 * 140, 0);
		gameCenterIosButton.anchoredPosition = new Vector2(-140 + 2 * 140, 0);
	}

	private void InitCompanyLanuage() {
		_companyLanguageList.Clear();

		PlayerManager.Instance.company.companies.ForEach((elem) => {
			LanuageLibrary ll = LanguageManager.Instance.lanuageLibrary.Find(x => x.code == elem.short_name);
			if (ll != null) {
				_companyLanguageList.Add(ll);
			}
		});
		_activePageCompany = 0;
		for (int i = 0; i < _companyLanguageList.Count; i++) {
			if (_companyLanguageList[i].code == PlayerManager.Instance.company.actualCompany)
				_activePageCompany = i;
		}
	}

	private void InitTranslateLanuage() {
		_translateLanguageList.Clear();

		GameCompany.Word word = PlayerManager.Instance.company.GetActualCompany().locations[0].levels[0].words[0];

		_translateLanguageList.Add(LanguageManager.Instance.lanuageLibrary.Find(x => x.code == "none"));

		word.translations.ForEach((elem) => {
			LanuageLibrary ll = LanguageManager.Instance.lanuageLibrary.Find(x => x.code == elem.lang && x.isWordTranslate);
			if (ll != null) {
				_translateLanguageList.Add(ll);
			}
		});
		_activePageTranslate = 0;
		for (int i = 0; i < _translateLanguageList.Count; i++) {
			if (_translateLanguageList[i].code == PlayerManager.Instance.translateLanuage)
				_activePageTranslate = i;
		}
	}

	public override void Show(Action OnShow = null) {
		base.Show(OnShow);
		AudioManager.Instance.library.PlayWindowOpenAudio();
		animComp.Play("show");
	}

	public override void Hide(Action OnHide = null) {
		base.Hide(OnHide);
		AudioManager.Instance.library.PlayWindowCloseAudio();
		animComp.Play("hide");
	}

	public void CloseButton() {
		//AudioManager.Instance.library.PlayClickAudio();
		if (isClosing) return;
		isClosing = true;
		Hide(() => {
			isClosing = false;
			gameObject.SetActive(false);
		});
	}

	public void GameCenterIosClick() {
		GameService.Instance.IosButtonClick();
	}

	public void GameCenterAndroidClick() {

		GameService.Instance.GoogleButtonClick();
	}

	public void MainArrowLeftButton() {
		AudioManager.Instance.library.PlayClickAudio();
		_activePageCompany--;
		if (_activePageCompany < 0) {
			_activePageCompany = 0;
			_activePageCompany = _companyLanguageList.Count - 1;
		}
		_locationChange = true;
		ConfirmMainLanuage();
	}

	public void MainArrowRightButton() {
		AudioManager.Instance.library.PlayClickAudio();
		_activePageCompany++;
		if (_activePageCompany >= _companyLanguageList.Count)
			_activePageCompany = 0;
		_locationChange = true;
		ConfirmMainLanuage();
	}

	public void SecondArrowLeftButton() {
		AudioManager.Instance.library.PlayClickAudio();
		_activePageTranslate--;
		if (_activePageTranslate < 0)
			_activePageTranslate = _translateLanguageList.Count - 1;
		ConfirmSecondaryLanuage();
	}

	public void SecondArrowRightButton() {
		AudioManager.Instance.library.PlayClickAudio();
		_activePageTranslate++;
		if (_activePageTranslate >= _translateLanguageList.Count)
			_activePageTranslate = 0;
		ConfirmSecondaryLanuage();
	}

	private void ConfirmMainLanuage() {

		_activeLangCompany = _companyLanguageList[_activePageCompany];

		PlayerManager.Instance.company.actualCompany = _activeLangCompany.code;

		var ltp = LanguageManager.Instance.lanuageTypeParam.Find(x => x.code == _activeLangCompany.code);

		if (ltp == null)
			ltp = LanguageManager.Instance.lanuageTypeParam.Find(x => x.code == "en");
		if (ltp == null)
			ltp = LanguageManager.Instance.lanuageTypeParam[0];

		companyLanguage.title.font = ltp.font;
		companyLanguage.icon.sprite = _activeLangCompany.sprite;
		companyLanguage.title.GetComponent<TextUiScale>().SetText(_activeLangCompany.nativeTitle);
		companyLanguage.anim.Play("changeLanuage");


		companyLanguage.leftArrow.SetActive(_activePageCompany > 0);
		companyLanguage.rightArrow.SetActive(_activePageCompany < _companyLanguageList.Count - 1);

		InitTranslateLanuage();
		InitCompanyLanuage();

	}

	private void ConfirmSecondaryLanuage() {

		_activeTranslateLang = _translateLanguageList[_activePageTranslate];

		PlayerManager.Instance.translateLanuage = _activeTranslateLang.code;

		var ltp = LanguageManager.Instance.lanuageTypeParam.Find(x => x.code == _activeTranslateLang.code);

		if (ltp == null)
			ltp = LanguageManager.Instance.lanuageTypeParam.Find(x => x.code == "en");
		if (ltp == null)
			ltp = LanguageManager.Instance.lanuageTypeParam[0];

		translateLanuage.title.font = ltp.font;

		translateLanuage.icon.sprite = _activeTranslateLang.sprite;

		translateLanuage.title.GetComponent<TextUiScale>().SetText(_activeTranslateLang.nativeTitle);
		translateLanuage.anim.Play("changeLanuage");

		translateLanuage.leftArrow.SetActive(_activePageTranslate > 0);
		translateLanuage.rightArrow.SetActive(_activePageTranslate < _translateLanguageList.Count - 1);

		InitTranslateLanuage();
		InitCompanyLanuage();
	}

	public void ProductRestore() {
		AudioManager.Instance.library.PlayClickAudio();
		BillingManager.Instance.RestorePurchases();
	}

	public override void ManagerClose() {
		CloseButton();
	}

	[System.Serializable]
	public struct LanuageData {
		public RectTransform main;
		public Animation anim;
		public Image icon;
		public Text title;
		public GameObject leftArrow;
		public GameObject rightArrow;
	}

}
