using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс отвечает за генераю ящиков на уровнях
/// </summary>
[System.Serializable]
public class ObjectsLevel {

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
		public MapObject type;
		public float value;
	}

	RunSpawner mapGenerato;

	/// <summary>
	/// Позиция генерации
	/// </summary>
	Vector3 generatePosition {
		get { return new Vector3(CameraController.displayDiff.transform.position.x + CameraController.displayDiff.right * 1.2f, RunnerController.Instance.mapHeight + Random.Range(1f, 2f), 0); }
	}

	float distantionCalc;               // Дистанция рассчета новой генерации
	float distantionGenerate;           // Следующая дистанция для генерации
	float perionGeneration;             // Период генерации

	public List<GenerateParametrs> generateParametrs; // Параметры генерации

	List<MapObject> generateList;

	public void Init(RunSpawner parent) {
		mapGenerato = parent;
		generateList = new List<MapObject>();
		distantionCalc = 0;
		GetConfig();
	}

	public void Update() {

		if (distantionCalc <= RunnerController.playerDistantion) {
			GenerateParametrs newPosition = generateParametrs.Find(x => x.distantion > distantionCalc);
			CalcDistantions(newPosition);
			CalcGenerateList(newPosition);
			CalcNextDistanceGenerate();
			distantionCalc = newPosition.distantion;
		}

		if (distantionGenerate >= 0 && distantionGenerate <= RunnerController.playerDistantion && generateList.Count > 0 && CheckReadyGenerate()) {
			GenerateBox();
			CalcNextDistanceGenerate();
		}

	}
	void GenerateBox() {
		mapGenerato.GenerateMapObject(generateList[0]);
		generateList.RemoveAt(0);
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
	/// Рассчет начала генерации и периода
	/// </summary>
	/// <param name="parametrs"></param>
	void CalcDistantions(GenerateParametrs parametrs) {
		if (parametrs.count <= 0) return;
		distantionGenerate = distantionCalc;
		perionGeneration = (parametrs.distantion - distantionCalc) / parametrs.count;
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
	/// Получение настроек
	/// </summary>
	public void GetConfig() {
		List<Configuration.Levels.Item> itemsData = Config.Instance.config.activeLevel.items;

		generateParametrs = new List<GenerateParametrs>();

		for (int i = 0; i < itemsData.Count; i++) {
			GenerateParametrs newParam = new GenerateParametrs();
			newParam.param = new List<BoxProbability>();

			newParam.distantion = itemsData[i].distantion;
			newParam.count = itemsData[i].objects;

			if (itemsData[i].spider > 0) {
				BoxProbability box = new BoxProbability();
				box.type = MapObject.spider;
				box.value = itemsData[i].spider * 0.01f;
				newParam.param.Add(box);
			}

			if (itemsData[i].torch > 0) {
				BoxProbability box = new BoxProbability();
				box.type = MapObject.torch;
				box.value = itemsData[i].torch * 0.01f;
				newParam.param.Add(box);
			}
      
			generateParametrs.Add(newParam);
		}

	}

}
