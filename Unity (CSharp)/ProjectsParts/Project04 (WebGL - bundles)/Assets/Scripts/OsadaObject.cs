using System.Collections;
using System.Collections.Generic;
using ExEvent;
using UnityEngine;

public class OsadaObject : ExEvent.EventBehaviour {

	public GameObject graphicObject;
	public int posX;
	public int posY;
	public string id;
	public bool startHide;
	public bool isStatic;

	private void OnEnable() {
		if (startHide)
			graphicObject.SetActive(false);
	}

	[ExEvent.ExEventHandler(typeof(BattleEvents.BattleUpdate))]
	public void ChangeStatePlayer(BattleEvents.BattleUpdate battleUpdate) {

		foreach (var key in battleUpdate.battleStart.data.constructions_parse.Keys) {

			if (battleUpdate.battleStart.data.constructions_parse[key].construction_id == id) {

				graphicObject.SetActive(isStatic || battleUpdate.battleStart.data.constructions_parse[key].active == "1");

			}
		}
	}
}
