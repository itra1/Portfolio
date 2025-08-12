using System;
using System.Collections.Generic;
using UnityEngine;
using ExEvent;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
using UnityEngine.UI;

public class LocationsGamePlay : UiPanel {

	public SwipePanel swipePanel;

	public Animation animComponent;
	private LocationsWorld _locationWord;

	private float _deltaSpeed;
	public TextUGUIScale title;

	public LocalizationUiText allIslandDescription;

	protected override void OnEnable() {
		base.OnEnable();
		if (_locationWord == null)
			_locationWord = WorldManager.Instance.GetWorld(WorldType.locations).GetComponent<LocationsWorld>();

		CalcDelta();
		CalcDeltaDrag();
		ChangeLocation();

		//_couchPanel = UIManager.Instance.GetPanel<CoinsUi>();

#if UNITY_ANDROID

		//allIslandDescription.SetCode("location.noAds");

#endif


		//internetAvalable = NetManager.Instance.internetAwalable;
		ChangeNetStatus();

		oneLocationButton.interactable = true;
		allLocationsButton.interactable = true;
		_isSwipe = false;
	}

	private CoinsUi _couchPanel;


	private bool isTouch;
	private float touchPos;

	private void Update() {
		CheckDrag();
	}

	private void OnApplicationPause(bool pause) {
		if (!pause)
			CheckDrag();
	}

	private void CancelDrag() {
		_isSwipe = false;
		isTouch = Input.GetMouseButton(0) || Input.touches.Length > 0;

		if (isTouch) {
			if (Input.GetMouseButton(0))
				touchPos = Input.mousePosition.x;
			else {
				touchPos = Input.touches[0].position.x;
			}
			_locationWord.PointerDown();
		} else {
			_locationWord.PointerUp();
		}
	}

	void CheckDrag() {
		if (_couchPanel.isActiveAndEnabled)
			return;

		if (!isTouch) {
			CancelDrag();
		}


		isTouch = Input.GetMouseButton(0) || Input.touches.Length > 0;

		if (isTouch) {
			if (isTouch != oldTouch) {
				_locationWord._lastDelta = 0;
				startDrag = GetPointerPosition();
			}

			if (!_isSwipe) {
				if (Mathf.Abs(startDrag.x - GetPointerPosition().x) > Camera.main.pixelWidth / 25) {
					_isSwipe = true;
					touchPos = GetPointerPosition().x;
				}
			}

			if (_isSwipe) {
				Drag(GetPointerPosition().x - touchPos);
				touchPos = GetPointerPosition().x;
			}

		}
		oldTouch = isTouch;
	}

	private Vector2 GetPointerPosition() {
		if (Input.GetMouseButton(0)) {
			return Input.mousePosition;
		} else {
			return Input.touches[0].position;
		}
	}

	private bool oldTouch;
	private Vector3 startDrag;
	private bool _isSwipe = false;

	void Drag(float delta) {
		if (delta == 0 || !_isSwipe) return;
		_locationWord.Swipe(delta * deltaWight);
	}

	private float deltaWight;
	void CalcDeltaDrag() {

		float delta = Camera.main.pixelWidth;

		float leftX = Camera.main.ViewportToWorldPoint(Vector3.zero).x;
		float rightX = Camera.main.ViewportToWorldPoint(Vector3.right).x;

		float wight = rightX - leftX;
		deltaWight = wight / delta;
	}


	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnChangeGamePhase))]
	private void ChangeGamePhase(ExEvent.GameEvents.OnChangeGamePhase changePhase) {
		//swipePanel.gameObject.SetActive(GameManager.gamePhase == GamePhase.locations);
	}

	private void CalcDelta() {
		_deltaSpeed = (Camera.main.ViewportToWorldPoint(Vector3.right) - Camera.main.ViewportToWorldPoint(Vector3.zero)).x / Camera.main.pixelWidth;
	}

	public override void Show(Action OnShow = null) {
		base.Show(OnShow);
		animComponent.Play("show");
	}

	public override void Hide(Action OnHide = null) {
		base.Hide(OnHide);
		animComponent.Play("hide");
	}

	public void HideArrow() {
		animComponent.Play("hideArrow");
	}

	public void ShowArrow() {
		animComponent.Play("showArrow");
	}

	public void BackButton() {
		Debug.Log("BackButton");
		AudioManager.Instance.library.PlayClickAudio();
		GameManager.Instance.ToBack();
	}

	public void PlayButton() {
		//_locationWord.SelectFocusIsland();
	}
	public void PlayButtonDown() {
		_swipe = false;
		//_locationWord.SelectFocusIsland();
	}
	public void PlayButtonUp(bool isFreePlay = false) {
		AudioManager.Instance.library.PlayClickAudio();
		if (!_swipe) {

			if (isFreePlay) {

				_locationWord.SelectFocusIsland();
			}
			else {
				_locationWord.SelectFocusIsland();
			}

		}
	}

	private bool _swipe;
	[ExEventHandler(typeof(GameEvents.SwipeLocation))]
	public void SwipeLocation(GameEvents.SwipeLocation cll) {
		_swipe = true;
	}

	public void LeftArrow() {
		//AudioManager.Instance.library.PlayClickAudio();
		_locationWord.LeftArrow();
	}

	public void RightArrow() {
		//AudioManager.Instance.library.PlayClickAudio();
		_locationWord.RightArrow();
	}

	public List<GameObject> arrowList;
	public GameObject leftArrow;
	public GameObject rightArrow;
	public GameObject playButton;
	public GameObject byeButton;
	public GameObject byeAllButton;
	public GameObject videoButton;
	public GameObject rateUsButton;
	public GameObject shareButton;
	public GameObject freePlayButton;
	public AchiveLocationIcon achiveIcon;
	//public GameObject needInternet;
	public GameObject needBeforeIslandText;

	[ExEvent.ExEventHandler(typeof(GameEvents.NetworkChange))]
	public void NetworkChange(GameEvents.NetworkChange net) {
		//internetAvalable = net.isActive;
		ChangeNetStatus();
	}

	private void ChangeNetStatus() {
		//needInternet.SetActive(!internetAvalable);
	}

	public void ChangeLocation() {

		if (_locationWord == null)
			_locationWord = WorldManager.Instance.GetWorld(WorldType.locations).GetComponent<LocationsWorld>();

		var island = _locationWord.GetFocusIsland();

		//bool beforeOpen = true;
		//bool beforeComplete = true;
		//if (!island.isOpen && island.num > 0) {
		//	beforeComplete = PlayerManager.Instance.company.CheckCompleteLocation(island.num - 1);

		//	for (int i = 0; i < _locationWord.islandList.Count; i++) {
		//		if (i == island.num - 1)
		//			beforeOpen = _locationWord.islandList[i].isOpen;
		//	}

		//}

		string actualcode = LanguageManager.Instance.activeLanuage.code;

		var company = PlayerManager.Instance.company.companies.Find(x => x.short_name == actualcode);

		if (company == null) {
			company = PlayerManager.Instance.company.companies.Find(x => x.short_name == "en");
		}

		leftArrow.gameObject.SetActive(island.num != 0);
    
		if (island.isShare) {
			rightArrow.gameObject.SetActive(false);
			rateUsButton.gameObject.SetActive(true);
			shareButton.gameObject.SetActive(true);
			playButton.gameObject.SetActive(false);
			freePlayButton.gameObject.SetActive(false);
			//achiveIcon.gameObject.SetActive(false);
			byeButton.gameObject.SetActive(false);
			byeAllButton.gameObject.SetActive(false);
			needBeforeIslandText.gameObject.SetActive(false);
			//videoButton.gameObject.SetActive(false);
		} else {
			rightArrow.gameObject.SetActive(true);
			rateUsButton.gameObject.SetActive(false);
			shareButton.gameObject.SetActive(false);
			playButton.gameObject.SetActive(island.isBye /*&& !island.isComplete*/ && island.beforeComplete);
			//needBeforeIslandText.gameObject.SetActive(!island.isOpen && !beforeComplete && beforeOpen);
			needBeforeIslandText.gameObject.SetActive(/*island.isBye && */!island.beforeComplete);

			//achiveIcon.gameObject.SetActive(island.isComplete);
			if (island.isComplete) {
				//achiveIcon.SetAchiveNum(island.num);
			}

			//byeButton.gameObject.SetActive(!island.isOpen && beforeComplete);

			//needBeforeIslandText.gameObject.SetActive(false);
			//byeButton.gameObject.SetActive(!island.isBye/* && island.beforeComplete*/);
			byeButton.gameObject.SetActive(false);
			byeAllButton.gameObject.SetActive(false);
			//freePlayButton.gameObject.SetActive(false);

#if UNITY_ANDROID
			// Пока отключили на android бесплатную игру
			//freePlayButton.gameObject.SetActive(!island.isBye && island.beforeComplete);
#endif
			//byeAllButton.gameObject.SetActive(!island.isBye);
			//videoButton.gameObject.SetActive(!island.isOpen);
		}

		if (BillingManager.Instance.isInizialized) {
			//var product = BillingManager.Instance.GetLocationProduct(island.num);

			//if (product != null && !PlayerManager.Instance.company.CheckAccessLocation((product as LocationProduct).locationNum) &&
			//		product.product != null)

			//	priceText.SetText(product.product.metadata.localizedPriceString, LanguageManager.Instance.activeLanuage.code);
			////priceText.SetText(product.product.metadata.localizedPrice.ToString() + " " +
			////										product.product.metadata.isoCurrencyCode);

			//var productAll = BillingManager.Instance.GetBillingProduct(IapType.locationAll);

			//if (productAll.Count > 0 && productAll[0].product != null)
			//	priceAllText.SetText(productAll[0].product.metadata.localizedPriceString, LanguageManager.Instance.activeLanuage.code);
			////priceAllText.SetText(productAll[0].product.metadata.localizedPrice.ToString() + " " +
			////											 productAll[0].product.metadata.isoCurrencyCode);
		} else {
			priceText.SetText("");
			priceAllText.SetText("");
		}


	}
	public TextUiScale priceText;
	public TextUiScale priceAllText;

	public Button oneLocationButton;
	public Button allLocationsButton;

	public void FreePlay() {
		PlayButtonDown();
		PlayButtonUp(true);
	}

	public void ByeButton() {
		//var island = _locationWord.GetFocusIsland();
		//BillingManager.Instance.ByeLocation(island.num, () => {
		//	oneLocationButton.interactable = true;
		//});
		//oneLocationButton.interactable = false;
	}

	public void ChangeStatusArrow(bool isDeactive) {
		arrowList.ForEach(x => x.SetActive(!isDeactive));
	}
	
	public void RateUsButton() {
		GameManager.Instance.RateUsButton();
	}
	
	public void ShareButton() {

		string sendText = LanguageManager.GetTranslate("share.link") + " ";

#if UNITY_ANDROID
		sendText += "https://play.google.com/store/apps/details?id=com.kingbirdgames.candywords";
#endif

#if UNITY_IOS
		sendText += "https://itunes.apple.com/app/id1294114269";
#endif

		GetComponent<Sharing>().Share(sendText);
		/*
		var share = GameObject.FindObjectOfType<VoxelBusters.NativePlugins.Sharing>();

		MailShareComposer shar = new MailShareComposer();
		//shar.

		share.ShowView(shar, (_result) => { });
		*/
	}

	public override void ManagerClose() {
		BackButton();
	}
}