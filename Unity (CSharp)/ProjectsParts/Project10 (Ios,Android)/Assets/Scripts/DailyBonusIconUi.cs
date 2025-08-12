using System;
using UnityEngine;
using UnityEngine.UI;

public class DailyBonusIconUi : PanelUiBase {

	[SerializeField]
	private GameObject activePanel;
	[SerializeField]
	private GameObject deactivePanel;

	private string _nextInText;
	public Text timerText;

	private DateTime nextDate;

	private void OnEnable() {
		_nextInText = LanguageManager.GetTranslate("mgp_NextIn");
		DailyBonusManager.OnChangePhase += ChangePhase;
		ChangePhase();
	}

	private void OnDisable() {
		DailyBonusManager.OnChangePhase -= ChangePhase;
	}

	private void ChangePhase() {
		activePanel.SetActive(DailyBonusManager.Instance.phase == DailyBonusManager.Phases.ready);
		deactivePanel.SetActive(DailyBonusManager.Instance.phase == DailyBonusManager.Phases.wait);
		nextDate = DailyBonusManager.Instance.nextShow;
	}

	private void Update() {
		TimerUpdate();
	}

	private void TimerUpdate() {
		if (DailyBonusManager.Instance.phase != DailyBonusManager.Phases.wait) return;
		SetTimer(_nextInText, nextDate - DateTime.Now);
	}

	private void SetTimer(string title, TimeSpan delta) {
		timerText.text = String.Format("{0} {1}:{2:00}:{3:00}", title, delta.Hours, delta.Minutes, delta.Seconds);
	}

	public void Click() {
		DailyBonusManager.Instance.IconClick();
	}
	
}
