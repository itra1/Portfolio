using System;
using UnityEngine;
using UnityEngine.UI;

public class BoostPanel : PanelUi {

	public Action<BoostType> OnActive;

	float timePanel = 5f;           // Время отображения панели
	float timeThis;                 // Текущее время
	bool timerStop;

	bool activate;

	public GameObject runBoost;
	public Text runBoostCountText;
	int runBoostCount;
	public GameObject skateBoost;
	public Text skateBoostCountText;
	int skateBoostCount;
	public GameObject barrelBoost;
	public Text barrelBoostCountText;
	int barrelBoostCount;
	public GameObject millWellBoost;
	public Text millWellBoostCountText;
	int millWellBoostCount;
	public GameObject shipBoost;
	public Text shipBoostCountText;
	int shipBoostCount;

	public Text title;
	public Text timerText;

	string sec;

	Vector3 positionDisplay;
	bool _isOpen;

	protected override void OnEnable() {
		base.OnEnable();
		activate = false;
		_isOpen = true;

		runBoost.SetActive(false);
		skateBoost.SetActive(false);
		barrelBoost.SetActive(false);
		millWellBoost.SetActive(false);
		shipBoost.SetActive(false);

		runBoostCount = UserManager.Instance.runBoost;                               // Буст
		skateBoostCount = UserManager.Instance.skateBoost;                           // Буст
		barrelBoostCount = UserManager.Instance.barrelBoost;                         // Буст
    millWellBoostCount = UserManager.Instance.millWhellBoost;                    // Буст
		shipBoostCount = UserManager.Instance.shipBoost;                         // Буст

		int allSum = (runBoostCount>0?1:0) + (skateBoostCount>0?1:0) + (barrelBoostCount>0?1:0) + (millWellBoostCount>0?1:0) + (shipBoostCount>0?1:0);

		int sizeSum = (allSum-1) * 200;

		int num = 0;
		Vector2 tempPos;

		if (runBoostCount > 0) {
			tempPos = runBoost.GetComponent<RectTransform>().anchoredPosition;
			runBoost.GetComponent<RectTransform>().anchoredPosition = new Vector2(num * 200 - sizeSum / 2, tempPos.y);
			runBoostCountText.text = runBoostCount.ToString();
			runBoost.SetActive(true);
			num++;
		}

		if (skateBoostCount > 0) {
			tempPos = skateBoost.GetComponent<RectTransform>().anchoredPosition;
			skateBoost.GetComponent<RectTransform>().anchoredPosition = new Vector2(num * 200 - sizeSum / 2, tempPos.y);
			skateBoostCountText.text = skateBoostCount.ToString();
			skateBoost.SetActive(true);
			num++;
		}

		if (barrelBoostCount > 0) {
			tempPos = barrelBoost.GetComponent<RectTransform>().anchoredPosition;
			barrelBoost.GetComponent<RectTransform>().anchoredPosition = new Vector2(num * 200 - sizeSum / 2, tempPos.y);
			barrelBoostCountText.text = barrelBoostCount.ToString();
			barrelBoost.SetActive(true);
			num++;
		}

		if (millWellBoostCount > 0) {
			tempPos = millWellBoost.GetComponent<RectTransform>().anchoredPosition;
			millWellBoost.GetComponent<RectTransform>().anchoredPosition = new Vector2(num * 200 - sizeSum / 2, tempPos.y);
			millWellBoostCountText.text = millWellBoostCount.ToString();
			millWellBoost.SetActive(true);
			num++;
		}

		if (shipBoostCount > 0) {
			tempPos = shipBoost.GetComponent<RectTransform>().anchoredPosition;
			shipBoost.GetComponent<RectTransform>().anchoredPosition = new Vector2(num * 200 - sizeSum / 2, tempPos.y);
			shipBoostCountText.text = shipBoostCount.ToString();
			shipBoost.SetActive(true);
			num++;
		}
		
		title.text = LanguageManager.GetTranslate("run_UseVideo");
		sec = LanguageManager.GetTranslate("run_Sec");

		timerText.text = timePanel + " " + sec;
		if (timeThis == 0)
			timeThis = timePanel;
	}

	protected override void OnDisable() {
		base.OnDisable();
		if (OnClose != null) OnClose();
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

		//UiController.ClickButtonAudio();

		_isOpen = false;
		GetComponent<Animator>().SetTrigger("close");

		//Invoke("ClosePanel", 0.9f);
		Helpers.Invoke(this, ClosePanel, 0.9f);
	}

	public void ButtonOk() {
		timerStop = true;
	}

	void Close() {
		timerStop = false;
		timeThis = 0;
		UiController.ClickButtonAudio();
		_isOpen = false;
		GetComponent<Animator>().SetTrigger("active");
		//Invoke("ClosePanel", 1.5f);
		Helpers.Invoke(this, ClosePanel, 1.5f);
	}

	void ClosePanel() {
		gameObject.SetActive(false);
	}

	public void RunButton() {

		if (activate) return;
		activate = true;

		runBoostCount--;
    UserManager.Instance.runBoost = runBoostCount;
		runBoostCountText.text = runBoostCount.ToString();
		//RunnerController.instance.ActivateBoostInRun(BoostType.speed);
		if (OnActive != null) OnActive(BoostType.speed);
		Close();
	}

	public void SkateButton() {

		if (activate) return;
		activate = true;

    UserManager.Instance.skateBoost = --skateBoostCount;
		skateBoostCountText.text = skateBoostCount.ToString();
		//RunnerController.instance.ActivateBoostInRun(BoostType.skate);
		if (OnActive != null) OnActive(BoostType.skate);
		Close();
	}

	public void BarrelButton() {

		if (activate) return;
		activate = true;
    
    UserManager.Instance.barrelBoost = --barrelBoostCount;
		barrelBoostCountText.text = barrelBoostCount.ToString();
		//RunnerController.instance.ActivateBoostInRun(BoostType.barrel);
		if (OnActive != null) OnActive(BoostType.barrel);
		Close();
	}

	public void MillWellButton() {

		if (activate) return;
		activate = true;

    UserManager.Instance.millWhellBoost = --millWellBoostCount;
		millWellBoostCountText.text = millWellBoostCount.ToString();
		//RunnerController.instance.ActivateBoostInRun(BoostType.millWheel);
		if (OnActive != null) OnActive(BoostType.millWheel);
		Close();
	}

	public void ShipButton() {

		if (activate) return;
		activate = true;
    
    UserManager.Instance.shipBoost = --shipBoostCount;
		shipBoostCountText.text = shipBoostCount.ToString();
		//RunnerController.instance.ActivateBoostInRun();
		if (OnActive != null) OnActive(BoostType.ship);
		Close();
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
		Close();
	}
}
