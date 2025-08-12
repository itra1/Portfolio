using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Воздушкая атака стрелами
/// </summary>
public class AirStoneAttack : SpecialBarrierAttack {
	public static AirStoneAttack instance;

	public SpecialBarriersTypes barrierType;                  // Тип препядствия

	private bool isActive;

	public GameObject arrowPrefab;                            // Префаб стрелы
	public GameObject incomingPrefab;                         // Префаб стрелы
	
	void Start() {
		//RunSpawner.OnSpecialBarrier += OnSpecialBarrier;
		UpdateValue();
		RunnerController.OnChangeRunnerPhase += OnChangeRunnerPhase;
		instance = this;
	}

	protected override void OnDestroy() {
		base.OnDestroy();
		//RunSpawner.OnSpecialBarrier -= OnSpecialBarrier;
		RunnerController.OnChangeRunnerPhase -= OnChangeRunnerPhase;
	}

	ClothesBonus spearClothes;

	/// <summary>
	/// Обновление при покупке
	/// </summary>
	void UpdateValue() {
		spearClothes = Config.GetActiveCloth(ClothesSets.noAirAttack);
	}

	void Update() {
		if (!isActive) return;
		UseFunction(false);
	}


	void OnChangeRunnerPhase(RunnerPhase newPhase) {
		if (newPhase == RunnerPhase.dead && isActive) {
			thisStep = allSteps;
			readyEnd = true;
		}
	}

	/// <summary>
	/// Подпись на выполнение препядствия
	/// </summary>
	/// <param name="barrier">Тип атаки</param>
	/// <param name="CallBack"></param>
	[ExEvent.ExEventHandler(typeof(ExEvent.RunEvents.SpecialBarrier))]
	public void OnSpecialBarrier(ExEvent.RunEvents.SpecialBarrier specialBarrier) {

		if (!specialBarrier.isActivate || specialBarrier.barrier == null || barrierType != specialBarrier.barrier) return;                     // Не соответствует тип
		
		// В случае полного костюма, возвращаем
		if (spearClothes.full) {
			EndBarrier();
			return;
		}
		
		int val = Random.Range(0, 5);

		//if (specialBarrier.numPgorgamm == null)
		//	specialBarrier.numPgorgamm = Random.Range(0, 5);

		switch (val) {
			case 0:
				UseFunction = Programm1;
				break;
			case 1:
				UseFunction = Programm2;
				break;
			case 2:
				UseFunction = Programm3;
				break;
			case 3:
				UseFunction = Programm4;
				break;
			case 4:
				UseFunction = Programm5;
				break;
		}
		UseFunction(true);

	}


	/// <summary>
	/// Предупреждающий знак
	/// </summary>
	/// <param name="needPosition"></param>
	void IncomingGenerate(Vector3 needPosition) {

		GameObject targetInstanceInc = Instantiate(incomingPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		targetInstanceInc.GetComponent<IncomingTopIcons>().OnActive += GenerateArrow;
		targetInstanceInc.GetComponent<IncomingTopIcons>().SetActiveAnim();
		targetInstanceInc.transform.position = needPosition;
		targetInstanceInc.SetActive(true);
	}

	/// <summary>
	/// Генерация валуна
	/// </summary>
	/// <param name="posX"></param>
	public void GenerateArrow(float posX) {

		GameObject arrow = Instantiate(arrowPrefab, new Vector3(posX, CameraController.displayDiff.transform.position.y + CameraController.displayDiff.top + 1.5f, 0), Quaternion.identity);
		arrow.SetActive(true);

		arrow.GetComponent<AirStone>().OnDestroyEvent = () => {
			bulletList.Remove(arrow);

			if (readyEnd && bulletList.Count == 0) {
				isActive = false;
				readyEnd = false;
				EndBarrier();
			}

		};

		bulletList.Add(arrow);
		
	}

	private float allSteps;                     // Количество шагов
	private float thisStep;                     // Текущий шаг
	private float lastTimeGenerate;             // Последнее время генерации
	private float timeWait;                     // Время ожидания между шагами

	private bool readyEnd;

	/// <summary>
	/// Камни выпадают друг за другом в хаотичном порядке по всей карте. 10 камней
	/// </summary>
	void Programm1(bool init = false) {
		if (init) {
			allSteps = 10;
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

		if (thisStep == allSteps) readyEnd = true;
	}

	/// <summary>
	/// камни выпадают партиями друг за другом. В рандомных местах по 3 штуки, 5 партий подряд.
	/// </summary>
	void Programm2(bool init = false) {
		if (init) {
			allSteps = 5;
			thisStep = 0;
			timeWait = Random.Range(0.7f, 1.2f);
			readyEnd = false;
			isActive = true;
		}

		if (lastTimeGenerate + timeWait > Time.time || readyEnd)
			return;

		lastTimeGenerate = Time.time;
		thisStep++;

		float elementDist = (CameraController.displayDiff.rightDif(0.9f) - CameraController.displayDiff.leftDif(0.9f)) / 9;
		float needPosition = Random.Range(0, 7);

		IncomingGenerate(new Vector3(CameraController.displayDiff.leftDif(0.9f) + elementDist * needPosition, CameraController.displayDiff.topDif(0.9f), 0));
		IncomingGenerate(new Vector3(CameraController.displayDiff.leftDif(0.9f) + elementDist * (needPosition + 1), CameraController.displayDiff.topDif(0.9f), 0));
		IncomingGenerate(new Vector3(CameraController.displayDiff.leftDif(0.9f) + elementDist * (needPosition + 2), CameraController.displayDiff.topDif(0.9f), 0));

		if (thisStep >= allSteps)
			readyEnd = true;

	}

	/// <summary>
	/// Камни выпадают по сценарию. 3 камня рядом, еще три камня рядом, 4 камня, разбитые по два и еще 4 камня разбитые по два в рандомных местах. 
	/// </summary>
	void Programm3(bool init = false) {
		if (init) {
			allSteps = 4;
			thisStep = 0;
			timeWait = Random.Range(0.7f, 1.2f);
			readyEnd = false;
			isActive = true;
		}

		if (lastTimeGenerate + timeWait > Time.time || readyEnd)
			return;

		lastTimeGenerate = Time.time;
		thisStep++;

		float elementDist = (CameraController.displayDiff.rightDif(0.9f) - CameraController.displayDiff.leftDif(0.9f)) / 9;
		float needPosition = 0;
		if (thisStep <= 2) {
			needPosition = Random.Range(0, 8);

			IncomingGenerate(new Vector3(CameraController.displayDiff.leftDif(0.9f) + elementDist * needPosition, CameraController.displayDiff.topDif(0.9f), 0));
			IncomingGenerate(new Vector3(CameraController.displayDiff.leftDif(0.9f) + elementDist * (needPosition + 1), CameraController.displayDiff.topDif(0.9f), 0));
			IncomingGenerate(new Vector3(CameraController.displayDiff.leftDif(0.9f) + elementDist * (needPosition + 2), CameraController.displayDiff.topDif(0.9f), 0));

		} else {
			needPosition = Random.Range(0, 3);
			IncomingGenerate(new Vector3(CameraController.displayDiff.leftDif(0.9f) + elementDist * needPosition, CameraController.displayDiff.topDif(0.9f), 0));
			IncomingGenerate(new Vector3(CameraController.displayDiff.leftDif(0.9f) + elementDist * (needPosition + 1), CameraController.displayDiff.topDif(0.9f), 0));
			needPosition = Random.Range(4, 8);
			IncomingGenerate(new Vector3(CameraController.displayDiff.leftDif(0.9f) + elementDist * needPosition, CameraController.displayDiff.topDif(0.9f), 0));
			IncomingGenerate(new Vector3(CameraController.displayDiff.leftDif(0.9f) + elementDist * (needPosition + 1), CameraController.displayDiff.topDif(0.9f), 0));
		}


		if (thisStep >= allSteps)
			readyEnd = true;

	}

	/// <summary>
	/// Камни выпадают по сценарию. Края-центр-края. 5 раз. 
	/// </summary>
	void Programm4(bool init = false) {
		if (init) {
			allSteps = 5;
			thisStep = 0;
			timeWait = Random.Range(0.7f, 1.2f);
			readyEnd = false;
			isActive = true;
		}

		if (lastTimeGenerate + timeWait > Time.time || readyEnd)
			return;

		lastTimeGenerate = Time.time;
		thisStep++;

		float elementDist = (CameraController.displayDiff.rightDif(0.9f) - CameraController.displayDiff.leftDif(0.9f)) / 9;

		if (thisStep % 2 == 1) {

			IncomingGenerate(new Vector3(CameraController.displayDiff.leftDif(0.9f) + 0, CameraController.displayDiff.topDif(0.9f), 0));
			IncomingGenerate(new Vector3(CameraController.displayDiff.leftDif(0.9f) + elementDist * 1, CameraController.displayDiff.topDif(0.9f), 0));
			IncomingGenerate(new Vector3(CameraController.displayDiff.leftDif(0.9f) + elementDist * 2, CameraController.displayDiff.topDif(0.9f), 0));

			IncomingGenerate(new Vector3(CameraController.displayDiff.leftDif(0.9f) + elementDist * 7, CameraController.displayDiff.topDif(0.9f), 0));
			IncomingGenerate(new Vector3(CameraController.displayDiff.leftDif(0.9f) + elementDist * 8, CameraController.displayDiff.topDif(0.9f), 0));
			IncomingGenerate(new Vector3(CameraController.displayDiff.leftDif(0.9f) + elementDist * 9, CameraController.displayDiff.topDif(0.9f), 0));

		} else {
			IncomingGenerate(new Vector3(CameraController.displayDiff.leftDif(0.9f) + elementDist * 3, CameraController.displayDiff.topDif(0.9f), 0));
			IncomingGenerate(new Vector3(CameraController.displayDiff.leftDif(0.9f) + elementDist * 4, CameraController.displayDiff.topDif(0.9f), 0));
			IncomingGenerate(new Vector3(CameraController.displayDiff.leftDif(0.9f) + elementDist * 5, CameraController.displayDiff.topDif(0.9f), 0));
			IncomingGenerate(new Vector3(CameraController.displayDiff.leftDif(0.9f) + elementDist * 6, CameraController.displayDiff.topDif(0.9f), 0));
		}

		if (thisStep >= allSteps)
			readyEnd = true;

	}

	int startVector;
	int thisVector;
	int startPosition;
	int thisPosition;

	/// <summary>
	/// Камни выпадают по сценарию последовательного выпадения камней в одну сторону, затем в обратную
	/// </summary>
	void Programm5(bool init = false) {
		if (init) {
			allSteps = 16;
			thisStep = 0;
			timeWait = Random.Range(0.6f, 1f);
			readyEnd = false;
			isActive = true;
			startVector = Random.value < 0.5f ? 1 : -1;
			thisVector = startVector;

			startPosition = startVector > 0 ? 0 : 8;
			thisPosition = startPosition;
		}

		if (lastTimeGenerate + timeWait > Time.time || readyEnd)
			return;

		lastTimeGenerate = Time.time;
		thisStep++;
		float elementDist = (CameraController.displayDiff.rightDif(0.9f) - CameraController.displayDiff.leftDif(0.9f)) / 9;

		IncomingGenerate(new Vector3(CameraController.displayDiff.leftDif(0.9f) + elementDist * thisPosition, CameraController.displayDiff.topDif(0.9f), 0));

		if ((startVector > 0 && thisPosition == 8) || (startVector < 0 && thisPosition == 0))
			thisVector *= -1;

		thisPosition += 1 * thisVector;

		if (thisStep >= allSteps)
			readyEnd = true;
	}

	public override void Generate(bool isActive, SpecialBarriersTypes? type) {

	}
}
