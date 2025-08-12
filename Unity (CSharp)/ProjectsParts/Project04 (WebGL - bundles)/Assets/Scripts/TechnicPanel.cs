using ExEvent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TechnicPanel : EventBehaviour {

	public Button attackModeButton;
	public Button moveModeButton;

	private void OnEnable() {
		ChangePanels();
	}

	[ExEvent.ExEventHandler(typeof(BattleEvents.OnChangeActionMode))]
	private void OnChangeIsOsada(BattleEvents.OnChangeActionMode osada) {
		ChangePanels();
	}
	
	private void ChangePanels() {
		attackModeButton.gameObject.SetActive(!PlayersManager.Instance.isAttackMode);
		moveModeButton.gameObject.SetActive(PlayersManager.Instance.isAttackMode);
	}

	public void EndRoundButton() {
		//PlayersManager.Instance.EndRoundButton(() => { }, () => {
		//	ActiveButton(true);
		//});

		ActiveButton(false);
		NetworkManager.Instance.TechnicEndRound((res) => {
			ActiveButton(true);
		});
	}

	public void AttackModeButton() {
		PlayersManager.Instance.isAttackMode = true;
	}

	public void MoveModeButton() {
		PlayersManager.Instance.isAttackMode = false;
	}

	public void LeaveTechnic() {
		NetworkManager.Instance.TechnicLeave((res) => {
			PlayersManager.Instance.isAttackMode = false;
		});
	}

	void ActiveButton(bool isActive) {
		attackModeButton.interactable = isActive;
		moveModeButton.interactable = isActive;
	}

}
