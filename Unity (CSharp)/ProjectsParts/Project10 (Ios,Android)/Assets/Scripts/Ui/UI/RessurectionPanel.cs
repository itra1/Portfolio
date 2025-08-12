using System;
using UnityEngine;
using UnityEngine.UI;

public class RessurectionPanel : PanelUi {

	public Action OnRessurection;
	public Action OnVideo;

	float timePanel = 5f;           // Время отображения панели
	float timeThis;                 // Текущее время

	public Text title;
	public Text timerText;
	public Text countText;

	string sec;

	public RectTransform iconObj;

	Vector3 positionDisplay;
	float speedPiscel;

	bool toPlayer;
	bool EventActive;


	public GameObject ressurectPanel;
	public Text ressurectPanelCountText;
	int ressurectPanelCount;
	public GameObject videoPanel;
	int videoPanelCount;
	bool _isOpen;
	protected override void OnEnable() {
		base.OnEnable();

		title.text = LanguageManager.GetTranslate("run_UseRessurection");
		sec = LanguageManager.GetTranslate("run_Sec");

		ressurectPanelCount = UserManager.Instance.ressurection;

		videoPanelCount = (UnityAdsController.adsReady && GameManager.DayAfterStart(1) ? 1 : 0);

		_isOpen = true;

		int allSum = (ressurectPanelCount>0?1:0) + (videoPanelCount>0?1:0);
		int sizeSum = (allSum-1) * 200;

		int num = 0;
		Vector2 tempPos;

		if (ressurectPanelCount > 0) {
			tempPos = ressurectPanel.GetComponent<RectTransform>().anchoredPosition;
			ressurectPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(num * 200 - sizeSum / 2, tempPos.y);
			ressurectPanelCountText.text = ressurectPanelCount.ToString();
			ressurectPanel.SetActive(true);
			num++;
		}

		if (videoPanelCount > 0 && GameManager.DayAfterStart(1)) {
			tempPos = videoPanel.GetComponent<RectTransform>().anchoredPosition;
			videoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(num * 200 - sizeSum / 2, tempPos.y);
			videoPanel.SetActive(true);
			num++;
		}

		timerText.text = timePanel + " " + sec;
		speedPiscel = Display.main.systemWidth * 5;
		if (timeThis == 0)
			timeThis = timePanel;
	}

	protected override void OnDisable() {
		base.OnDisable();
		if (OnClose != null) OnClose();
	}

	void Update() {

		if (timeThis > 0) {
			timeThis -= Time.deltaTime;
			timeThis = timeThis <= 0 ? 0 : timeThis;
			timerText.text = Mathf.Round(timeThis) + " " + sec;
			if (timeThis == 0) ButtonCancel();
		}

		if (toPlayer) {
			Vector3 targetPos = positionDisplay - iconObj.position;
			Vector3 newPos = Vector3.Normalize(targetPos) * speedPiscel * Time.deltaTime + iconObj.position;
			Vector3 newTarget = positionDisplay - newPos;

			if (Mathf.Sign(targetPos.x) != Mathf.Sign(newTarget.x) || Mathf.Sign(targetPos.y) != Mathf.Sign(newTarget.y)) {
				iconObj.GetComponent<RectTransform>().position = targetPos;
				toPlayer = false;
				if (OnRessurection != null) OnRessurection();
				gameObject.SetActive(false);
				ButtonOk();
			} else {
				iconObj.GetComponent<RectTransform>().position = newPos;
			}

			//iconObj.position = newPos;
		}
	}

	public void ButtonCancel() {
		if (EventActive) return;
		EventActive = true;

		UiController.ClickButtonAudio();
		//UiController.RessurectionClose(false);
		_isOpen = false;
		GetComponent<Animator>().SetTrigger("hide");
		//Invoke("ClosePanel", 0.9f);
		Helpers.Invoke(this, ClosePanel, 0.9f);
	}

	public void ButtonOk() {
		if (EventActive) return;
		EventActive = true;
		timeThis = 0;

	}

	public void RessurectUseButton() {
		if (EventActive) return;
		EventActive = true;
		timeThis = 0;
		UiController.ClickButtonAudio();

    UserManager.Instance.ressurection--;

    GetComponent<Animator>().SetTrigger("active");
	}


	public void VideoUseButton() {

		if (EventActive) return;
		EventActive = true;
		UiController.ClickButtonAudio();
		timeThis = 0;
		UnityAdsController.ShowVideo(AddVideoAnsver);

	}

	void AddVideoAnsver(bool result) {
		Debug.Log("Video result = " + result);
		if (result) {
			//UiController.RessurectionClose(true);
			_isOpen = false;
			GetComponent<Animator>().SetTrigger("hide");
			//Invoke("ClosePanel", 0.9f);
			Helpers.Invoke(this, ClosePanel, 0.9f);
			if (OnVideo != null) OnVideo();
		} else
			ButtonCancel();
	}


	void MuveIconToPlayer() {

		RectTransform newRect = Instantiate(iconObj) as RectTransform;
		newRect.parent = iconObj.parent;
		newRect.localPosition = iconObj.localPosition;
		newRect.localScale = iconObj.localScale;
		iconObj.gameObject.SetActive(false);
		iconObj = newRect;

		Transform playerTrans = Player.Jack.PlayerController.Instance.transform;

		Vector3 playerPoint = Camera.main.WorldToScreenPoint(playerTrans.position);
		positionDisplay = playerPoint;
		toPlayer = true;
	}

	void ClosePanel() {
		gameObject.SetActive(false);
	}

	void EventOpen() {
		if (_isOpen) AnimClip();
	}

	void EventClose() {
		if (!_isOpen) AnimClip();
	}

	void AnimClip() {
		RunnerGamePlay.PlayBonusClip();
	}

	public override void BackButton() {
		ButtonCancel();
	}
}
