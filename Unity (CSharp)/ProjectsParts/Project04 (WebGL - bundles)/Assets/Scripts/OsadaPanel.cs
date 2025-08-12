using ExEvent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OsadaPanel : MonoBehaviour {

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
		attackModeButton.gameObject.SetActive(PlayersManager.Instance.isAttackMode);
		moveModeButton.gameObject.SetActive(!PlayersManager.Instance.isAttackMode);
	}

	public void EndRoundButton() {
		ActiveButton(false);
		PlayersManager.Instance.EndRoundButton(() => { }, () => {
			ActiveButton(true);
		});
	}

	public void AttackModeButton() {
		
	}

	public void MoveModeButton() {
		
	}

	void ActiveButton(bool isActive) {
		attackModeButton.interactable = isActive;
		moveModeButton.interactable = isActive;
	}

}
