
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ArrowParametrs {
	[HideInInspector]
	public bool generate;               // Флаг сгенерированной группы
	public float distance;
	[Range(0, 1)]
	public float probility;
}

/// <summary>
/// Камнепад
/// </summary>
public class AirArrowAttack : SpecialBarrierAttack {

	private RunnerPhase runnerPhase;
	public static AirArrowAttack backLink;

	public GameObject targetEffectPref;                     // Префаб индикатора
	public GameObject arrowPrefab;                          // Префаб камня


	private bool isActive;                                          // Флаг выполняющегося процесса


	void Start() {
		backLink = this;
		spearClothes = Config.GetActiveCloth(ClothesSets.noAirAttack);
		//RunSpawner.OnSpecialBarrier += OnSpecialBarrier;
		RunnerController.OnChangeRunnerPhase += OnChangeRunnerPhase;
	}

	protected override void OnDestroy() {
		base.OnDestroy();
		//RunSpawner.OnSpecialBarrier -= OnSpecialBarrier;
		RunnerController.OnChangeRunnerPhase -= OnChangeRunnerPhase;
	}


	void OnChangeRunnerPhase(RunnerPhase newPhase) {
		if (newPhase == RunnerPhase.dead && isActive) {
			thisStep = allSteps;
			readyEnd = true;
		}
	}

	public override void Generate(bool isActive, SpecialBarriersTypes? type) {

		if(!isActive || type == null || type != this.type) return;;
		
		// В случае полного костюма, возвращаем
		if (spearClothes.full) {
			EndBarrier();
			return;
		}

		switch (Random.Range(0, 2)) {
			case 0:
				UseFunction = Programm1;
				break;
			case 1:
				UseFunction = Programm2;
				break;
		}
		UseFunction(true);

	}
	
	/// <summary>
	/// Калбак о окончании работы
	/// </summary>
	void CallBackEndSpecial() {
		EndBarrier();
	}

	ClothesBonus spearClothes;

	/// <summary>
	/// Следующая дистанция для генерации
	/// </summary>
	float waitNextDistance;

	void Update() {
		if (!isActive) return;
		UseFunction(false);
	}

	/// <summary>
	/// Генерация предупреждающего знака
	/// </summary>
	/// <param name="needPosition"></param>
	void IncomingGenerate(Vector3 needPosition) {

		GameObject targetInstanceInc = Instantiate(targetEffectPref, Vector3.zero, Quaternion.identity) as GameObject;
		targetInstanceInc.GetComponent<IncomingTopIcons>().OnActive += GenerateArrow;
		targetInstanceInc.GetComponent<IncomingTopIcons>().moveToPlayerX = true;
		targetInstanceInc.GetComponent<IncomingTopIcons>().SetActiveAnim();
		targetInstanceInc.transform.position = needPosition;
	}

	/// <summary>
	/// Генерация стрелы
	/// </summary>
	/// <param name="posX"></param>
	public void GenerateArrow(float posX) {

		GameObject arrow = Instantiate(arrowPrefab, new Vector3(posX - 4, CameraController.displayDiff.transform.position.y + CameraController.displayDiff.top + 1.5f, 0), Quaternion.identity) as GameObject;
		arrow.SetActive(true);

		arrow.GetComponent<AirArrow>().OnDestroyEvent = () => {
			bulletList.Remove(arrow);

			if (readyEnd && bulletList.Count == 0) {
				isActive = false;
				readyEnd = false;
				EndBarrier();
			}

		};
	}

	float allSteps;                     // Количество шагов
	float thisStep;                     // Текущий шаг
	float lastTimeGenerate;             // Последнее время генерации
	float timeWait;                     // Время ожидания между шагами

	bool readyEnd;
	/// <summary>
	/// Прицел довольно быстро догоняет джека. Несколько друг за другом
	/// </summary>
	void Programm1(bool init = false) {
		if (init) {
			allSteps = Random.Range(5, 7);
			thisStep = 0;
			timeWait = Random.Range(0.4f, 0.7f);
			readyEnd = false;
			isActive = true;
		}

		if (lastTimeGenerate + timeWait > Time.time || readyEnd)
			return;

		lastTimeGenerate = Time.time;
		thisStep++;
		IncomingGenerate(new Vector3(Random.Range(CameraController.displayDiff.leftDif(0.9f), CameraController.displayDiff.rightDif(0.9f)), CameraController.displayDiff.topDif(0.9f), 0));

		if (thisStep >= allSteps)
			readyEnd = true;
	}
	/// <summary>
	/// Прицел довольно быстро догоняет джека. 
	/// </summary>
	void Programm2(bool init = false) {
		if (init) {
			allSteps = 7;
			thisStep = 0;
			timeWait = Random.Range(1f, 1.5f);
			readyEnd = false;
			isActive = true;
		}

		if (lastTimeGenerate + timeWait > Time.time || readyEnd)
			return;

		lastTimeGenerate = Time.time;
		thisStep++;

		IncomingGenerate(new Vector3(Random.Range(CameraController.displayDiff.leftDif(0.9f), CameraController.displayDiff.rightDif(0.9f)), CameraController.displayDiff.topDif(0.9f), 0));
		IncomingGenerate(new Vector3(Random.Range(CameraController.displayDiff.leftDif(0.9f), CameraController.displayDiff.rightDif(0.9f)), CameraController.displayDiff.topDif(0.9f), 0));

		if (thisStep > 4)
			IncomingGenerate(new Vector3(Random.Range(CameraController.displayDiff.leftDif(0.9f), CameraController.displayDiff.rightDif(0.9f)), CameraController.displayDiff.topDif(0.9f), 0));

		if (thisStep >= allSteps)
			readyEnd = true;
	}
}
