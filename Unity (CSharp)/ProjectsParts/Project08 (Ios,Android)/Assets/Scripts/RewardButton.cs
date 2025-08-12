using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardButton : MonoBehaviour {

	public GameObject button;

	public GameObject activeBack;
	public GameObject deactiveBack;
	public GameObject activeText;
	public GameObject deactiveText;

	public Image icon;
	public GameObject backIcon;

	private bool isReady = false;
	private bool isOpened = false;

	private void OnEnable() {
		Init();
		//PlayerManager.Instance.achives.OnAdded += ChengeAchive;
	}

	private void OnDisable() {
		//PlayerManager.Instance.achives.OnAdded -= ChengeAchive;
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.PlayerEvents.OnAddDecor))]
	public void ChengeAchive(ExEvent.PlayerEvents.OnAddDecor decor) {
		Init();
	}

	private void Init() {
		isOpened = PlayerManager.Instance.achives.CheckExistsAchive(PlayerManager.Instance.company.actualLocationNum);
		var gp = GraphicManager.Instance.link.achiveList[PlayerManager.Instance.company.actualLocationNum];
		isReady = PlayerManager.Instance.company.CheckFullStarLocation(PlayerManager.Instance.company.actualLocationNum);
		icon.sprite = gp.mini;
		//button.gameObject.SetActive(!isOpened);
		icon.GetComponent<Animation>().enabled = !isOpened;

		if(isReady)
			ExEvent.PlayerEvents.OnChangeCompany.Call(PlayerManager.Instance.company.actualCompany, true);

		if (isOpened) {
			activeBack.gameObject.SetActive(isOpened);
			activeText.gameObject.SetActive(isOpened);
			deactiveBack.gameObject.SetActive(!isOpened);
			deactiveText.gameObject.SetActive(!isOpened);
			backIcon.gameObject.SetActive(!isOpened);
			return;
		}

		icon.GetComponent<Animation>().enabled = isReady;
		SetStatus();

		if (isReady) {
			ClickButton();
		}

	}

	public void SetStatus() {
		activeBack.gameObject.SetActive(isReady);
		activeText.gameObject.SetActive(isReady);
		deactiveBack.gameObject.SetActive(!isReady);
		deactiveText.gameObject.SetActive(!isReady);
		backIcon.gameObject.SetActive(!isReady);
	}

	public void ClickButton() {
		if (!isReady || isOpened) return;
		PlayerManager.Instance.achives.ClickIcon(PlayerManager.Instance.company.actualLocationNum);
	}

}
