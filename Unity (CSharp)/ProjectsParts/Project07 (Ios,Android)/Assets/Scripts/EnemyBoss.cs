using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class EnemyBoss : MonoBehaviour, IPointerDownHandler {

	//количество жизней
	float startLive = 5;

	void Start() { }


	void Update() { }

	public void OnPointerDown(PointerEventData eventData) {
		Debug.Log("Tap enemy");
		DamageEnemy();


	}

	//нанесение урона
	void DamageEnemy() {

		//проигрыш анимации

		startLive = startLive - 2.5f;
		if (startLive < 1) {
			if
				(DeadBossEvent != null) DeadBossEvent(this);
			Destroy(gameObject);
		}


	}


	public static event System.Action<EnemyBoss> DeadBossEvent;

}
