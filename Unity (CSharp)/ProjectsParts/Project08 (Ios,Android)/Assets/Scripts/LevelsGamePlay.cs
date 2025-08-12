using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelsGamePlay : UiPanel {

	public TextUiScale titleText;
	public GameObject locker;

	public override void ManagerClose() {
		if (locker.activeInHierarchy) return;
		BackButton();
	}

	public void BackButton() {
		AudioManager.Instance.library.PlayClickAudio();
		GameManager.Instance.ToBack();
	}

	protected override void OnEnable() {
		base.OnEnable();

		var local = PlayerManager.Instance.company.GetActualLocation();
		
		string actualcode = LanguageManager.Instance.activeLanuage.code;

		var company = PlayerManager.Instance.company.companies.Find(x => x.short_name == actualcode);

		if (company == null) {
			company = PlayerManager.Instance.company.companies.Find(x => x.short_name == "en");
		}

		titleText.SetText(company.locations[PlayerManager.Instance.company.actualLocationNum].title, LanguageManager.Instance.activeLanuage.code);

		titleText.SetText(local.title, PlayerManager.Instance.company.actualCompany);

	}
	
}
