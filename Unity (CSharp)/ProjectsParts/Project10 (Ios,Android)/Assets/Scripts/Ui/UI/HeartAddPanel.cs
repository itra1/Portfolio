using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// панель добавления сердца в забеге
/// </summary>
public class HeartAddPanel : PanelUi {

	public Action OnActive;

	float timePanel = 5f;           // Время отображения панели
	float timeThis;                 // Текущее время

	public Text title;
	public Text timerText;
	public Text countText;

	string sec;

	bool EventActive;

	bool _isOpen;
	protected override void OnEnable() {
		base.OnEnable();

		_isOpen = true;

		title.text = LanguageManager.GetTranslate("run_UseHeartAdd");
		sec = LanguageManager.GetTranslate("run_Sec");

		int ressurectionCount = PlayerPrefs.GetInt("liveAddPerk");
		countText.text = ressurectionCount.ToString();
		timerText.text = timePanel + " " + sec;
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
			if (timeThis == 0)
				ButtonCancel();
		}
	}

	public void ButtonCancel() {

		if (EventActive)
			return;
		EventActive = true;

		_isOpen = false;
		GetComponent<Animator>().SetTrigger("close");
		Invoke("ClosePanel", 0.6f);
	}

	public void ButtonOk() {

		if (EventActive)
			return;
		EventActive = true;

		timeThis = 0;
		UiController.ClickButtonAudio();
		//RunnerController.instance.AddHeartPerk();
		GetComponent<Animator>().SetTrigger("active");
		Invoke("ClosePanel", 1f);
		if (OnActive != null) OnActive();
	}

	void ClosePanel() {
		//RunnerController.StartRunDialogs();
		gameObject.SetActive(false);
	}

	void EventOpen() {
		if (_isOpen)
			AnimClip();
	}

	void EventClose() {
		if (!_isOpen)
			AnimClip();
	}

	void AnimClip() {
		RunnerGamePlay.PlayBonusClip();
	}

	public override void BackButton() {
		ButtonCancel();
	}
}
