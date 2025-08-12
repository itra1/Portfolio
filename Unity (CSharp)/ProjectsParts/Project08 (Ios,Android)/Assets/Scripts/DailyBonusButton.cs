using System;
using UnityEngine;

public class DailyBonusButton : ExEvent.EventBehaviour {

	public Animation animComp;

	public GameObject activeGraphic;
	public GameObject deactiveGraphic;
	public GameObject button;

	public Timer timer;
	
  private void OnEnable() {
	  DailyBonus.OnChangePhase += OnChangePhase;
	  Init();
  }

	private void OnDisable() {
		DailyBonus.OnChangePhase -= OnChangePhase;
	}

	private void OnChangePhase(DailyBonus.Phases phase) {
		Init();
	}

	private void Init() {
		if (DailyBonus.Instance.phase == DailyBonus.Phases.none) {
			if (button.activeInHierarchy) {
				animComp.Play("hide");
			}
			else {
				button.SetActive(false);
			}
			return;
		}

		deactiveGraphic.SetActive(DailyBonus.Instance.phase == DailyBonus.Phases.wait);
		activeGraphic.SetActive(DailyBonus.Instance.phase == DailyBonus.Phases.ready);

		if (!button.activeInHierarchy) {
			button.SetActive(true);
		}
		animComp.Play("show");

		if (DailyBonus.Instance.phase == DailyBonus.Phases.wait) {

			var actualDate = DateTimeOffset.UtcNow.DateTime;
			if (actualDate < DailyBonus.Instance.updateServerTime)
				timer.StartTimer(DailyBonus.Instance.nextShow - DailyBonus.Instance.serverTime);
			else {
				timer.StartTimer(DailyBonus.Instance.nextShow - DailyBonus.Instance.serverTime - (actualDate - DailyBonus.Instance.updateServerTime));
			}
		}
	}
	
	public void Click() {
		AudioManager.Instance.library.PlayClickAudio();
		DailyBonus.Instance.OpenDialog();
	}


}
