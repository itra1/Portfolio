using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartsUiGamePlay : ExEvent.EventBehaviour {

	public GameObject[] hearthItems;

	private void OnEnable() {
		InitLive();
		HealthManagerBase.OnChangeEvent += OnChangeEvent;
	}
	
	private void OnDisable() {
		HealthManagerBase.OnChangeEvent -= OnChangeEvent;
	}

	private void OnChangeEvent(float actualValue, float maxValue) {
		ExEvent.GameEvents.PlayerLiveChange.Call(RunnerController.Instance.healthManager.actualValue, true, true);
	}

	// Активируем значение по жизням
	void InitLive() {
		ExEvent.GameEvents.PlayerLiveChange.Call(RunnerController.Instance.healthManager.actualValue, true, true);
	}

	// Изменяет значение счетчика
	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.PlayerLiveChange))]
	public void SetLive(ExEvent.GameEvents.PlayerLiveChange eventData) {

		for (int i = 0; i < hearthItems.Length; i++) {

			if (!hearthItems[i].activeInHierarchy)
				if (eventData.first && i + 1 <= eventData.value)
					hearthItems[i].SetActive(true);
				else {
					hearthItems[i].SetActive(false);
					continue;
				}
			hearthItems[i].GetComponent<Animator>().SetBool("force", eventData.noAnimActive);
			hearthItems[i].GetComponent<Animator>().SetBool("active", (i + 1 <= eventData.value ? true : false));
		}
	}

}
