using System.Collections;
using System.Collections.Generic;
using ExEvent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using  Game.User;

public class BattleUiLevelStats : MonoBehaviour {
  
	[ExEvent.ExEventHandler(typeof(BattleEvents.BattlePhaseChange))]
	private void BattlePhaseChange(BattleEvents.BattlePhaseChange battlePhase) {
		if (battlePhase.phase == BattlePhase.start) { }
	}
	
	public void PauseButton() {
		UIController.ClickPlay();
		BattleManager.Instance.Pause();
	}

}
