using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Контроллер входящих команд от контроллерв
/// </summary>
public class InputController : MonoBehaviour {

	public List<KeyCode> ListKeyDown;

	public void Update() {

		if (Input.anyKeyDown) {
			ListKeyDown.ForEach(CheckKeyDown);

		} else {
			ListKeyDown.ForEach(CheckKeyUp);
		}

		//if (Input.anyKeyDown) {
		//	ListKeyDown.ForEach(CheckKeyDown);
		//}

		if (Input.mouseScrollDelta != Vector2.zero) {
			ExEvent.GameEvents.MouseScroll.Call(Input.mouseScrollDelta.y);
		}
		/*
		if (Input.GetMouseButtonDown(0)) {

		}

		if (Input.GetMouseButtonUp(0)) {

		}

		if (Input.GetMouseButtonDown(1)) {

		}

		if (Input.GetMouseButtonUp(1)) {

		}
		*/
	}

	public void CheckKeyDown(KeyCode keyKode) {
		if (Input.GetKeyDown(keyKode))
			ExEvent.GameEvents.KeyDown.Call(keyKode);
	}

	public void CheckKeyUp(KeyCode keyKode) {
		if (Input.GetKeyUp(keyKode)) {
			ExEvent.GameEvents.KeyUp.Call(keyKode);
		}
	}

}
