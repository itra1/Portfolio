using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CoinsCounterUi : ExEvent.EventBehaviour {

	private Text _textUi;
	private Text textUi {
		get {
			return (_textUi = _textUi ?? GetComponent<Text>());
		}
	}

	private void OnEnable() {
		textUi.text = UserManager.coins.ToString();
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.RunEvents.CoinsChange))]
	private void CoinsChangeEvent(ExEvent.RunEvents.CoinsChange e) {

		textUi.text = UserManager.coins.ToString();
	}

}
