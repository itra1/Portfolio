using System.Collections;
using System.Collections.Generic;
using ExEvent;
using UnityEngine;

public class InfoPanel : EventBehaviour {

	public GameObject enemyPanel;
	public GameObject osadaPanel;

	private void OnEnable() {
		ChangePanels();
	}

	[ExEvent.ExEventHandler(typeof(BattleEvents.OnChangeIsOsada))]
	private void OnChangeIsOsada(BattleEvents.OnChangeIsOsada osada) {
		ChangePanels();
	}
	
	private void ChangePanels() {
		enemyPanel.SetActive(!PlayersManager.Instance.isOsada);
		osadaPanel.SetActive(PlayersManager.Instance.isOsada);
	}


	public void SetSelectPlayer(PlayerBehaviour selectPlayer) {

		if (PlayersManager.Instance.isOsada) return;

		enemyPanel.GetComponent<EnemyPanel>().SetSelectPlayer(selectPlayer);
	}

}
