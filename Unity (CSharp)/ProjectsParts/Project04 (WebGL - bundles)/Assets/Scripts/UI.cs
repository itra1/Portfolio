using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : ExEvent.EventBehaviour {

	public GameObject battleEndPanel;

	private void OnEnable() {
		battleEndPanel.SetActive(false);
	}
	
	[ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.BattleEnd))]
	public void BattleEnd(ExEvent.BattleEvents.BattleEnd battleEnd) {
		battleEndPanel.SetActive(true);
	}

}
