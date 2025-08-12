using UnityEngine;
using UnityEngine.UI;

public class VideoPanel : MonoBehaviour {

	float timePanel = 5f;           // Время отображения панели
	float timeThis;                 // Текущее время
	bool timerStop;

	public bool isGarpun;

	public Text title;
	public Text timerText;

	string sec;

	public RectTransform iconObj;

	Vector3 positionDisplay;
	//float speedPiscel;

	bool _isOpen;
	// bool EventActive;

	void OnEnable() {

		title.text = LanguageManager.GetTranslate("run_UseVideo");
		sec = LanguageManager.GetTranslate("run_Sec");
		timerStop = false;
		timerText.text = timePanel + " " + sec;
		//speedPiscel = Display.main.systemWidth * 5;
		if (timeThis == 0)
			timeThis = timePanel;


		_isOpen = true;
		Stop(true);
	}

	void Update() {

		if (!timerStop && timeThis > 0) {
			timeThis -= Time.deltaTime;
			timeThis = timeThis <= 0 ? 0 : timeThis;
			timerText.text = Mathf.Round(timeThis) + " " + sec;
			if (timeThis == 0) ButtonCancel();
		}

	}

	public void ButtonCancel() {
		//if (EventActive) return;
		//EventActive = true;

		UiController.ClickButtonAudio();
		if (isGarpun)
      //GameObject.Find("Player").GetComponent<PlayerController>().PlayerDownYes();
      Player.Jack.PlayerController.Instance.PlayerFallConfirm(false);
		else
			//UiController.RessurectionClose(false);
			_isOpen = false;
		GetComponent<Animator>().SetTrigger("hide");
		Invoke("ClosePanel", 0.9f);
	}

	public void ButtonOk() {
		timerStop = true;
		UnityAdsController.ShowVideo(AddVideoAnsver);
	}

	void Close() {
		timerStop = false;
		//if (EventActive) return;
		//EventActive = true;
		timeThis = 0;
		UiController.ClickButtonAudio();
		GetComponent<Animator>().SetTrigger("active");
		Stop(false);
		Invoke("ClosePanel", 1.5f);
	}

	void Stop(bool flag) {

		//GameObject.Find("ParentCamera").GetComponent<ParentCamera>().cameraStop = flag;
		//GameObject.Find("GameController").GetComponent<RunnerController>().stopCalcDistantion = flag;
		ParentCamera.CameraStop = flag;
		RunnerController.stopCalcDistantion = flag;
    //GameObject.Find("Player").GetComponent<PlayerPlay>().stopRun = flag;
    Player.Jack.PlayerMove.isStopped = flag;
	}

	void MuveIconToPlayer() {
		return;
	}

	void ClosePanel() {
		gameObject.SetActive(false);
	}

	void AddVideoAnsver(bool result) {
		Debug.Log("Video result = " + result);
		if (result) {

			if (isGarpun)
        //GameObject.Find("Player").GetComponent<PlayerController>().ActiveGarpun();
        Player.Jack.PlayerController.Instance.PlayerFallConfirm(true);
			else {
				//UiController.RessurectionClose(true);
			}

			Close();
		} else
			ButtonCancel();
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
}
