using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class SpecialBarrierLevel : SpecialBarrier {
	
	/// <summary>
	/// Параметры дистанции
	/// </summary>
	[System.Serializable]
	public struct GenerateParametrs {
		public float distantion;
		public int count;
		public List<BoxProbability> param;
	}

	/// <summary>
	/// Вероятность генерации
	/// </summary>
	[System.Serializable]
	public struct BoxProbability {
		public SpecialBarriersTypes type;
		public float value;
	}

	/// <summary>
	/// Позиция генерации
	/// </summary>
	Vector3 generatePosition {
		get { return new Vector3(CameraController.displayDiff.transform.position.x + CameraController.displayDiff.right * 1.2f, RunnerController.Instance.mapHeight + Random.Range(1f, 2f), 0); }
	}

	public SpecialBarrierGenerateParam[] specialBarrierParam;                           // Параметры генерации
	public List<GenerateParametrs> generateParametrs = new List<GenerateParametrs>(); // Параметры генерации
	private RunSpawner runSpawner;

	private float distantionCalc;               // Дистанция рассчета новой генерации
	private float distantionGenerate;           // Следующая дистанция для генерации
	private float perionGeneration;             // Период генерации
	private List<SpecialBarriersTypes> generateList;
	private SpecialBarriersTypes activeBarrier;

	
	protected override void Init(RunSpawner runSpawner) {
		this.runSpawner = runSpawner;
		generateList = new List<SpecialBarriersTypes>();
		distantionCalc = 0;

		spawners.ForEach(x => x.EndBarrier = EventComplete);
	}

	protected override void UpdateProcess() {
		base.UpdateProcess();

		if(specialBarrierParam.Length == 0) return;

		if (distantionCalc >= 0 && distantionCalc <= RunnerController.playerDistantion) {
			GenerateParametrs newPosition = generateParametrs.Find(x => x.distantion > distantionCalc);
			if (newPosition.distantion == 0) {
				distantionCalc = -1;
				return;
			}
			CalcDistantions(newPosition);
			CalcGenerateList(newPosition);
			CalcNextDistanceGenerate();
			distantionCalc = newPosition.distantion;
		}

		if (distantionGenerate >= 0 && distantionGenerate <= RunnerController.playerDistantion && generateList.Count > 0 && CheckReadyGenerate()) {
			Generate();
			CalcNextDistanceGenerate();
		}
	}

	void Generate() {
		activeBarrier = generateList[0];
		runSpawner.lockedGenerate = true;

		spawners.ForEach((elem) => {
			elem.Generate(true, activeBarrier);
		});


		//ExEvent.RunEvents.SpecialBarrier.Call(true, activeBarrier, () => {
		//	runSpawner.lockedGenerate = false;
		//	distantionGenerate = -1;
		//	ExEvent.RunEvents.SpecialBarrier.Call(false, activeBarrier, null, null);
		//}, null);

		generateList.RemoveAt(0);
		
	}


	private void EventComplete() {
		runSpawner.lockedGenerate = false;
		distantionGenerate = -1;
	}

	/// <summary>
	/// Проверка возможности генерации
	/// </summary>
	/// <returns></returns>
	public bool CheckReadyGenerate() {
		Collider[] objCol = Physics.OverlapSphere(generatePosition, 6f);

		foreach (Collider oneColl in objCol) {
			if (oneColl.tag == "RollingStone" | oneColl.tag == "jumpUp" | oneColl.tag == "jumpDown")
				return false;
		}
		return true;
	}
	
	/// <summary>
	/// Рассчет следующей генерации с учотом периода
	/// </summary>
	void CalcNextDistanceGenerate() {
		if (generateList.Count > 0)
			distantionGenerate += Random.Range(0, perionGeneration);
		else
			distantionGenerate = -1;
	}

	/// <summary>
	/// Рассчет массива под генерацию
	/// </summary>
	/// <param name="parametrs">Структура</param>
	void CalcGenerateList(GenerateParametrs parametrs) {
		// Если присутствуют ящики под генерацию
		if (parametrs.count <= 0) return;
		generateList.Clear();

		// Готовим массив под выбор
		List<float> tempCd = new List<float>();

		float sum1 = 0;
		tempCd.Add(sum1);
		foreach (BoxProbability one in parametrs.param) {
			sum1 += one.value;
			tempCd.Add(sum1);
		}

		int boxCount = parametrs.count;

		while (boxCount > 0) {
			boxCount--;
			int number = BinarySearch.RandomNumberGenerator(tempCd);
			generateList.Add(parametrs.param[number].type);
		}

	}


	/// <summary>
	/// Бесконечный инкремент (на всякий случай)
	/// </summary>
	void InfinityIncrement() {
		GenerateParametrs param = new GenerateParametrs();
		param.count = generateParametrs[generateParametrs.Count - 1].count;
		param.distantion = generateParametrs[generateParametrs.Count - 1].distantion - generateParametrs[generateParametrs.Count - 2].distantion;
		param.param = new List<BoxProbability>(generateParametrs[generateParametrs.Count - 1].param);
		generateParametrs.Add(param);
	}

	/// <summary>
	/// Рассчет начала генерации и периода
	/// </summary>
	/// <param name="parametrs"></param>
	void CalcDistantions(GenerateParametrs parametrs) {
		if (parametrs.count <= 0) return;
		distantionGenerate = distantionCalc;
		perionGeneration = (parametrs.distantion - distantionCalc) / parametrs.count;
	}

	protected override void GetConfig() {
		List<Configuration.Levels.SpecialBarrier> itemsData = Config.Instance.config.activeLevel.specialBarrier;
		int startCol = 0;
		float distance;
		bool isEnd = false;

		List<SpecialBarrierGenerateParam> specialBarrierParamTmp  = new List<SpecialBarrierGenerateParam>();
		generateParametrs.Clear();

		for (int i = 0; i < itemsData.Count; i++) {
			if (i < startCol || isEnd) continue;

			if (itemsData[i].trap > 0) continue;
			SpecialBarrierGenerateParam oneBarrier = new SpecialBarrierGenerateParam();
			GenerateParametrs oneParam = new GenerateParametrs();

			try {
				distance = itemsData[i].distantion;
			} catch {
				distance = specialBarrierParamTmp[specialBarrierParamTmp.Count - 1].distance.min + 200;
				isEnd = true;
			}

			oneBarrier.distance.min = distance;
			oneParam.distantion = distance;

			if (i < itemsData.Count - 2)
				oneBarrier.distance.max = itemsData[i].distantion;

			if (i == itemsData.Count - 1)
				oneBarrier.distance.max = oneBarrier.distance.min + 200;

			oneParam.count = (int)itemsData[i].trap;
			oneParam.param = new List<BoxProbability>();

			oneBarrier.generate = false;
			oneBarrier.probability = itemsData[i].trap * 0.01f;
			oneBarrier.types = new SpecialBarriersProbability[2];
			oneBarrier.types[0].type = SpecialBarriersTypes.airArrow;
			oneBarrier.types[0].shareProbability = itemsData[i].airArrow * 0.01f;
			oneBarrier.types[1].type = SpecialBarriersTypes.airStone;
			oneBarrier.types[1].shareProbability = itemsData[i].airStone * 0.01f;

			BoxProbability one = new BoxProbability();
			one.type = oneBarrier.types[0].type;
			one.value = oneBarrier.types[0].shareProbability;
			oneParam.param.Add(one);

			one = new BoxProbability();
			one.type = oneBarrier.types[1].type;
			one.value = oneBarrier.types[1].shareProbability;
			oneParam.param.Add(one);


			specialBarrierParamTmp.Add(oneBarrier);
			generateParametrs.Add(oneParam);
		}
		specialBarrierParam = specialBarrierParamTmp.ToArray();
	}

}
