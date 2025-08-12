using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.User;

public class ProfileChangeParametr : MonoBehaviour {

	public Action<int> OnConfirm;

	public Image icon;

  [SerializeField]
  private TMPro.TextMeshProUGUI title;

	private int _maxValue;
	private int _byeValue;
  [SerializeField]
  private TMPro.TextMeshProUGUI countValue;

	public GameObject healthIcon;
	public GameObject powerIcon;
	public GameObject energyIcon;
	
	public void SetData(UserStat userStat, int value) {

		string _title = "Увеличить параметр ";

		switch (userStat) {
			case UserStat.energy:
				_title += "энергию?";
				break;
			case UserStat.health:
				_title += "жизни?";
				break;
			case UserStat.power:
			default:
				_title += "сила?";
				break;
		}

		title.text = _title;

		healthIcon.SetActive(userStat == UserStat.health);
		powerIcon.SetActive(userStat == UserStat.power);
		energyIcon.SetActive(userStat == UserStat.energy);

		_maxValue = value;
		_byeValue = value;
		ConfirmValue();
	}

	private void ConfirmValue() {
		countValue.text = _byeValue.ToString();
	}

	public void IncButton() {
		UIController.ClickPlay();
		_byeValue = Mathf.Min(_maxValue, _byeValue + 1);
		ConfirmValue();
	}

	public void DecButton() {
		UIController.ClickPlay();
		_byeValue = Mathf.Max(0, _byeValue - 1);
		ConfirmValue();
	}

	public void CancelButton() {
		UIController.ClickPlay();
		StartClose();
	}

	public void ConfirmButton() {
		UIController.ClickPlay();
		if (OnConfirm != null) OnConfirm(_byeValue);
		StartClose();
	}
	
	public void StartClose() {
		gameObject.SetActive(false);
	}

}
