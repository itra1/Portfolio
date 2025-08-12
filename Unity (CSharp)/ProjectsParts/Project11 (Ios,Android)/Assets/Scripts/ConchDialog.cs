using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConchDialog : UiPanel, ICoinsPanelParent, IMiniMenuParent {

	public List<GameObject> graphics;

	public GameObject activeBack;
	public GameObject description;
	public Animation animComp;

	public Animation animCunch;
	//public Animation reward;

	public GameObject takeButton;

	public AnimationHelper conchHelper;

	public GiftElement gift;

	public GameObject cunchObject;
	public GameObject giftyPack;


	public GameObject _gameMiniMenu;
	public GameObject gameMiniMenu { get { return _gameMiniMenu; } }

	public GameMiniMenu _miniMenu;
	public GameMiniMenu miniMenu { get { return _miniMenu; } }


	public Canvas _coinsPanel;
	public Canvas coinsPanel { get { return _coinsPanel; } }

	public CoinsWidget _coinsWidget;
	public CoinsWidget coinsWidget {
		get { return _coinsWidget; }
	}

	public RectTransform _coinsTargetIcon;
	public RectTransform coinsTargetIcon {
		get { return _coinsTargetIcon; }
	}

	protected override void Awake() {
		base.Awake();
		ChangeValue();

		conchHelper.OnEvent1 = () => {
			cunchObject.GetComponent<Animation>().Play("hide");
			cunchObject.GetComponent<AnimationHelper>().OnEvent1 = CounchHideComplete;
		};

	}

	private bool giftReady = true;
	protected override void OnEnable() {
		base.OnEnable();
		ConchManager.OnSetValue += ChangeValue;
		giftyPack.SetActive(false);
		giftReady = true;

		//reward.gameObject.SetActive(ConchManager.Instance.isFull);
		takeButton.gameObject.SetActive(ConchManager.Instance.isFull);
		description.SetActive(!ConchManager.Instance.isFull);

		graphics.ForEach(x => x.GetComponent<Image>().color = new Color(1, 1, 1, 1));

		_coinsPanel.gameObject.SetActive(false);

		_gameMiniMenu.SetActive(false);
		ChangeValue();
	}

	private void ShowGift() {

	}

	protected override void OnDisable() {
		base.OnDisable();
		
			miniMenu.Close();
			_gameMiniMenu.SetActive(false);

		ConchManager.OnSetValue -= ChangeValue;

		if (giftIdleAudioPoint != null && giftIdleAudioPoint.isPlaing)
			giftIdleAudioPoint.Stop(0.3f);
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

	public void ChangeValue() {

		for (int i = 0; i < graphics.Count; i++)
			graphics[i].SetActive(i <= ConchManager.Instance.actualValue);

		if (ConchManager.Instance.actualValue > 10)
			graphics[graphics.Count - 1].SetActive(true);

		CheckActive();
	}

	private void CheckActive() {

		activeBack.SetActive(ConchManager.Instance.isFull);

		if (ConchManager.Instance.isFull) {

			PlayGiftIdleAudio();
			//reward.transform.localScale = Vector3.one;
			return;
		}
	}

	public void CloseButton() {
		//AudioManager.Instance.library.PlayClickAudio();
		if (giftObjectReady) return;
		Hide(() => {
			gameObject.SetActive(false);
		});
	}

	public void TakeButton() {
		AudioManager.Instance.library.PlayClickAudio();

		if (!ConchManager.Instance.isFull) return;

		ConchManager.Instance.Use();
		giftObjectReady = true;
		takeButton.SetActive(false);

		animCunch.Play("EmptyCunch");
		//reward.Play("rewardHide");
		//reward.GetComponent<AnimationHelper>().OnEvent1 = () => {
		//	reward.gameObject.SetActive(false);
		//};

		//gameObject.SetActive(false);
	}


	private bool giftObjectReady = false;

	// Клик
	public void Click() {
		Debug.Log("Click");
	}

	public override void ManagerClose() {
		if (isClosing) return;
		CloseButton();
	}

	/// <summary>
	/// Окончание анимации осьминога
	/// </summary>
	public void CounchHideComplete() {
		giftyPack.GetComponent<Animation>().Play("show");
		giftyPack.SetActive(true);
	}

	public void TakeGift() {
		if (!giftReady) return;
		giftReady = false;
		giftyPack.GetComponent<Animation>().Play("hide");
		giftIdleAudioPoint.Stop(0.3f);

		gift.transform.position = cunchObject.transform.position;

		var useG = gift.pars[UnityEngine.Random.Range(0, gift.pars.Count)];


		gift.Show(useG.type, this, this);
		gift.gameObject.SetActive(true);

		gift.MoveComplete = () => {
			SaveData(useG);
			giftObjectReady = false;
			//DailyBonus.Instance.GetGift();
			Hide(() => {
				gameObject.SetActive(false);
			});
		};
	}

	private void SaveData(GiftElement.GiftElementParam par) {
		switch (par.type) {
			case GiftElement.Type.hintAnyletter:
				PlayerManager.Instance.hintAnyLetter += par.count;
				break;
			case GiftElement.Type.hintLetter:
				PlayerManager.Instance.hintFirstLetter += par.count;
				break;
			case GiftElement.Type.hintWord:
				PlayerManager.Instance.hintFirstWord += par.count;
				break;
			default:
				PlayerManager.Instance.coins += par.count;
				break;
		}
	}

	public void CoinsPanelGiftOpen() {
		coinsPanel.GetComponent<Animation>().Play("show");
		coinsPanel.gameObject.SetActive(true);
		coinsWidget.gameObject.SetActive(true);
		//coinsPanel.sortingOrder = 20;
	}

	public void MiniMenyGiftOpen(bool isLocked) {
		gameMiniMenu.gameObject.SetActive(true);
		miniMenu.Open(isLocked);
		//gameMiniMenyPanel.sortingOrder = 20;
	}

	public AudioClipData giftIdleAudio;
	private AudioPoint giftIdleAudioPoint;

	public void PlayGiftIdleAudio() {
		giftIdleAudioPoint = AudioManager.PlayEffects(giftIdleAudio, AudioMixerTypes.effectUi, giftIdleAudioPoint);
	}

	public void MiniMenyGiftClose(bool isLocked) {
		miniMenu.Close(isLocked);
	}
}
