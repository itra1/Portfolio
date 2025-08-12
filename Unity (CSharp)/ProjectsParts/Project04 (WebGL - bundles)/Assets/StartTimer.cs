using System.Collections;
using System.Collections.Generic;
using ExEvent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartTimer : EventBehaviour {

	public GameObject panel;
	public Image imageBack;
	public TextMeshProUGUI timerText;

	private int timeWait = 0;

	private long endTime;

	public Color roundStartBack;
	public Color roundBack;

	private BattlePhase phase;

	private enum BattlePhase {
		none,
		start,
		round
	}

	private void Start() {
		StartCoroutine(Timer());
	}
	
	[ExEvent.ExEventHandler(typeof(BattleEvents.BattleUpdate))]
	public void ChangeStatePlayer(BattleEvents.BattleUpdate battleUpdate) {

		if (battleUpdate.battleStart.battle_info.round_time_start >= battleUpdate.battleStart.battle_info.round_time_now) {
			phase = BattlePhase.start;
			panel.SetActive(true);
			timeWait =
				(int) (battleUpdate.battleStart.battle_info.round_time_start - battleUpdate.battleStart.battle_info.round_time_now);
			imageBack.color = roundStartBack;
			return;
		}

		phase = BattlePhase.round;
		panel.SetActive(true);
		timeWait =
				 (int)battleUpdate.battleStart.battle_info.time;
		imageBack.color = roundBack;


	}

	IEnumerator Timer() {
		while (true) {
			yield return new WaitForSeconds(1);

			timeWait--;

			if (timeWait <= 0) timeWait = 0;

			if (phase == BattlePhase.start) {
				timerText.text = "Round wait " + timeWait;
			}
			else if (phase == BattlePhase.round) {
				timerText.text = System.String.Format("{0:00}:{1:00}", (int)(timeWait/60), (int)(timeWait%60));
			}

		}

	}
	

}
