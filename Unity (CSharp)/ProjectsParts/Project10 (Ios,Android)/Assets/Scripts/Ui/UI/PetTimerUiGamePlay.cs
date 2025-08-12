using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Player.Jack;

public class PetTimerUiGamePlay : ExEvent.EventBehaviour {

	public GameObject panel;
	
	[SerializeField]
	Image timer;
	[SerializeField]
	GameObject pointsTimer;
	public GameObject petBatIcon;
	public GameObject petSpiderIcon;
	public GameObject petDinoIcon;
	float timerStart;
	float timeWork;
	float timerPercent;
	bool petActive;

	private void OnEnable() {
		InitPets();
	}

	private void OnDisable() {
		DeInitPets();
	}
	
	void InitPets() {
		RunnerController.petsChange += PetEvent;
	}

	void DeInitPets() {
		RunnerController.petsChange -= PetEvent;
	}

	void PetEvent(PetsTypes pet, bool useFlag, float timeUse) {
		if (useFlag)
			StartTimer(pet, timeUse);
		else if (timerStart + timeWork > Time.time)
			DesctivePetTimer();
	}

	public void StartTimer(PetsTypes pet, float timeUse) {

		petBatIcon.SetActive(false);
		petSpiderIcon.SetActive(false);
		petDinoIcon.SetActive(false);
		petActive = true;
		switch (pet) {
			case PetsTypes.bat:
				petBatIcon.SetActive(true);
				break;
			case PetsTypes.dino:
				petDinoIcon.SetActive(true);
				break;
			case PetsTypes.spider:
				petSpiderIcon.SetActive(true);
				break;
		}

		panel.SetActive(true);

		timerStart = Time.time;
		timeWork = timeUse;
		pointsTimer.transform.localEulerAngles = Vector3.zero;
		timer.fillAmount = 1;
	}

	void Update() {
		if (!panel.activeInHierarchy) return;

		timerPercent = (timerStart + timeWork - Time.time) / timeWork;
		timer.fillAmount = timerPercent;
		pointsTimer.transform.localEulerAngles = new Vector3(0, 0, (1 - timerPercent) * 360);

		if (timerPercent <= 0 && petActive) DesctivePetTimer();

	}
	void DesctivePetTimer() {
		petActive = false;
		panel.SetActive(false);
	}

}
