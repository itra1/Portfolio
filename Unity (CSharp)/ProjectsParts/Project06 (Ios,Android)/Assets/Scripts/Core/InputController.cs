using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Контроллер входящих команд от контроллерв
/// </summary>
public class InputController : MonoBehaviour {
	
  [SerializeField]
	private List<KeyCode> ListKeyDown;

	public void Update() {
		
		if(Input.anyKeyDown) {
			ListKeyDown.ForEach(CheckKeyDown);
		}

		if(Input.mouseScrollDelta != Vector2.zero) {
			ExEvent.GameEvents.MouseScroll.Call(Input.mouseScrollDelta.y);
		}

	}

	public void CheckKeyDown(KeyCode keyKode) {
		if(Input.GetKeyDown(keyKode)) {
			ExEvent.GameEvents.KeyDown.Call(keyKode);
		}
	}

}
