using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// Объект конца уровня
/// </summary>
public class LevelEnd : MonoBehaviour {
	public GameObject graphic;

	private void Start() {
		graphic.SetActive(false);
	}
	
	private void OnTriggerEnter2D(Collider2D collision) {

		if (collision.GetComponent<Player.Jack.PlayerController>()) {
			//ExEvent.RunEvents.LevelEnd.Call();
			RunnerController.Instance.LevelEnd();
		}
	}
}
