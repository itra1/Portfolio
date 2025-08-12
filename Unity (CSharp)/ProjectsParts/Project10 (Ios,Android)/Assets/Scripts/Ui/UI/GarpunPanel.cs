using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GarpunPanel : PanelUi {

	public Action OnGarpun;
	public Action OnVideo;

	private float timePanel = 5f;           // Время отображения панели
	private float timeThis;                 // Текущее время

	public Text title;
	public Text timerText;
	public Text countText;

	string sec;

	public RectTransform iconObjectPref;
	public RectTransform iconObj;

	Vector3 positionDisplay;
	float speedPiscel;

	bool toPlayer;
	bool EventActive;


	public GameObject garpunPanel;
	public Text grpunPanelCountText;
	int garpunPanelCount;
	public GameObject videoPanel;
	int videoPanelCount;

	bool _isOpen;

	protected override void OnEnable() {
		base.OnEnable();
		title.text = LanguageManager.GetTranslate("run_UseGarpun");
		sec = LanguageManager.GetTranslate("run_Sec");

		_isOpen = true;

		garpunPanelCount = UserManager.Instance.savesPerk;

		videoPanelCount = (UnityAdsController.adsReady && GameManager.DayAfterStart(1) ? 1 : 0);


		int allSum = (garpunPanelCount>0?1:0) + (videoPanelCount>0?1:0);
		int sizeSum = (allSum-1) * 200;

		int num = 0;
		Vector2 tempPos;


		if (garpunPanelCount > 0) {
			tempPos = garpunPanel.GetComponent<RectTransform>().anchoredPosition;
			garpunPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(num * 200 - sizeSum / 2, tempPos.y);
			grpunPanelCountText.text = garpunPanelCount.ToString();
			garpunPanel.SetActive(true);
			num++;
		}

		if (videoPanelCount > 0 && GameManager.DayAfterStart(1)) {
			tempPos = videoPanel.GetComponent<RectTransform>().anchoredPosition;
			videoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(num * 200 - sizeSum / 2, tempPos.y);
			videoPanel.SetActive(true);
			num++;
		}


		EventActive = false;

		timerText.text = timePanel + " " + sec;
		speedPiscel = Display.main.systemWidth * 5;
		Stop(true);
		if (timeThis == 0)
			timeThis = timePanel;
	}

	private void Update() {

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
				Stop(false);
        Player.Jack.PlayerController.Instance.PlayerFallConfirm(true);
				gameObject.SetActive(false);
				Destroy(iconObj.gameObject);
				//ButtonOk();
			} else {
				iconObj.GetComponent<RectTransform>().position = newPos;
			}

			//iconObj.position = newPos;
		}
	}

	protected override void OnDisable() {
		base.OnDisable();
		if (OnClose != null) OnClose();
	}

	public void ButtonCancel() {

		if (EventActive) return;
		EventActive = true;

		//Stop(false);
		UiController.ClickButtonAudio();
    Player.Jack.PlayerController.Instance.PlayerFallConfirm(false);
		_isOpen = false;
		GetComponent<Animator>().SetTrigger("hide");
		//gameObject.SetActive(false);
		Invoke("ClosePanel", 0.9f);
	}

	public void GarpunUseButton() {
		if (EventActive) return;
		EventActive = true;
		timeThis = 0;
		UiController.ClickButtonAudio();
		
		GetComponent<Animator>().SetTrigger("active");
		if (OnGarpun != null) OnGarpun();
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
			//PlayerController.instance.PlayerFallConfirm(true);
			_isOpen = false;
			GetComponent<Animator>().SetTrigger("hide");
			Stop(false);
			Invoke("ClosePanel", 0.9f);
			if (OnVideo != null) OnVideo();
		} else
			ButtonCancel();
	}

	void Stop(bool flag) {

		ParentCamera.CameraStop = flag;
		RunnerController.stopCalcDistantion = flag;
    Player.Jack.PlayerMove.isStopped = flag;
	}

	void MuveIconToPlayer() {

		RectTransform newRect = Instantiate(iconObjectPref) as RectTransform;
		newRect.parent = iconObjectPref.parent;
		newRect.localPosition = iconObjectPref.localPosition;
		newRect.localScale = iconObjectPref.localScale;
		iconObjectPref.gameObject.SetActive(false);
		iconObj = newRect;

		positionDisplay = new Vector3(Display.main.systemWidth / 2, -Display.main.systemHeight / 2);
		toPlayer = true;
	}

	void EventOpen() {
		if (_isOpen) AnimClip();
	}

	void ClosePanel() {
		gameObject.SetActive(false);

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
