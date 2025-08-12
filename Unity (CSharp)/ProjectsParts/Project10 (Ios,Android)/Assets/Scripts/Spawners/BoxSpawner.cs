using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(BoxSpawner))]
public class BoxSpawnerEditor : Editor {

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Copy")) {
			((BoxSpawner)target).CopyData();
		}

	}

}

#endif


/// <summary>
/// Категории ящиков
/// </summary>
public enum BoxCategory {
	weapon,
	magnet,
	hearth,
	blackMark,
	shield,
	magic
}

[System.Serializable]
public struct BoxLibrary {
	public BoxCategory category;
	public List<BoxTypeParam> prefabs;

	[System.Serializable]
	public struct BoxTypeParam {
		public BoxType type;
		public GameObject prefab;
	}


	public BoxType GetTypeBox() {
		List<float> prob = new List<float>();
		float sum = 0;
		prob.Add(sum);
		for (int i = 0; i < prefabs.Count; i++)
			sum += 1;
		prob = prob.ConvertAll(x => x / sum);
		return prefabs[BinarySearch.RandomNumberGenerator(prob)].type;
	}

	public GameObject GetPrefab() {
		BoxType need = GetTypeBox();
		return prefabs.Find(c => c.type == need).prefab;
	}

}

/// <summary>
/// Структура типов эффектов ящика
/// </summary>
[System.Serializable]
public enum BoxType {
	weaponTrap,
	weaponSabel,
	weaponGun,
	weaponBomb,
	weaponMolotov,
	weaponShip,
	shield,
	tarzan,
	pirat,
	bullet,
	cristall,
	blackPoint,
	magnet,
	hearth,
	weaponChocolate,
	weaponCandy,
	weaponFlowers,
	power
};

/// <summary>
/// Параметры генерации ящиков
/// </summary>
[System.Serializable]
public struct BoxSpawn {
	public string name;                                         // Имя
	public BoxSpawnPrefabs[] prefab;                                   // Ссылка на объект
	public List<BoxParametrs> parametrs;
	//[HideInInspector]
	public float nextDistance;
	//[HideInInspector]
	public float probability;
	//[HideInInspector]
	public float nextDistanceCalc;
}

[System.Serializable]
public struct BoxSpawnPrefabs {
	public GameObject prefab;
	[Range(0, 1)]
	public float probability;
}

[System.Serializable]
public struct weaponGenerateParamerts {

	[System.Serializable]
	public struct weaponProbability {
		public string name;
		public GameObject box;
		public float probability;
	}

	public weaponProbability[] chance;
}
[System.Serializable]
public struct WeaponBoxParametrs {
	public float distance;
	public int count;
}
[System.Serializable]
public struct BoxParametrs {
	public float distance;
	[Range(0, 1)]
	public float probability;
}

/// <summary>
/// Генератор ящиков
/// </summary>
public class BoxSpawner : MonoBehaviour {

  public const string POOLER_KEY = "BoxSpawner";

  public static BoxSpawner instance;                  // Ссылка на собственный экземпляр

	public List<BoxLibrary> boxLibrary;

	public BoxSpawn[] boxSpawnParametrs;                // Параметры генерации ящиков
	private float boxSpawnNextDistanceCalc;             // Следующая дистанция для рассчета
	private float boxSpawnNextDistanceSpawn;            // Следующая дистанция для генерации

	public List<weaponGenerateParamerts> weaponParamtrs;

	public List<WeaponBoxParametrs> weaponBoxParametr;
	private bool weaponBeforeBoost;
	private List<float> weaponBoxDistance;
	[HideInInspector]
	public List<float> wpn;                                        // МАссив доступных для генерации
	[HideInInspector]
	public List<string> wpnName;
	float weaponNextDistanceCalc;

	[HideInInspector]
	public float magnetProbability;                          // Коеффициент генерации марнита
	public float[] magnetGenerateParametrs;           // Параметры выбора коеффициента генерации магнита

	[HideInInspector]
	public float hearthProbability;                          // Коеффициент генерации дополнительного сердца
	public float[] hearthGenerateParametrs;           // Параметры выбора коеффициента генерации сердца

	private RunnerPhase runnerPhase = RunnerPhase.start;

	private List<GameObject> generateList = new List<GameObject>();
	private float nextGenerate;
  
	//public BoxLevel boxLevel;
	private IBoxSpawner boxSpawner;
	public BoxSpawnerLevel spawnerLevel;
	public BoxSpawnerSurvival spawnerSurvival;

	private void Start() {

		if (GameManager.activeLevelData.gameMode == GameMode.none) {
			gameObject.SetActive(false);
			return;
		}

		StartBase();
	}

	private void StartBase() {
		instance = this;

		switch (GameManager.activeLevelData.gameMode) {
			case GameMode.survival:
				boxSpawner = spawnerSurvival;
				break;
			case GameMode.levelsClassic:
				boxSpawner = spawnerLevel;
				break;
			default:
				break;
		}

		runnerPhase = RunnerPhase.start;

		GetConfig();
    if(boxSpawner != null)
		  boxSpawner.Init(this);
		//boxLevel.Init(this);
		InitValue();
		weaponBoxDistance = new List<float>();

		RunnerController.OnChangeRunnerPhase += ChangePhase;
		GameManager.OnChangeLevel += GetConfig;

		UpdateValue();

		StructProcessor();
	}



	public ClothesBonus boxClothes;
	public ClothesBonus heartClothes;
	public ClothesBonus magnetClothes;

	void InitValue() {

		ProcessesWeapon();

		magnetProbability = magnetGenerateParametrs[PlayerPrefs.GetInt("magnetPerk")];
		hearthProbability = hearthGenerateParametrs[PlayerPrefs.GetInt("healthPerk")];

		boxClothes = Config.GetActiveCloth(ClothesSets.moreBox);
		heartClothes = Config.GetActiveCloth(ClothesSets.heart);
		magnetClothes = Config.GetActiveCloth(ClothesSets.magnet);
	}

	void UpdateValue() {
		InitValue();
	}

	void OnDestroy() {
		RunnerController.OnChangeRunnerPhase -= ChangePhase;
		GameManager.OnChangeLevel -= GetConfig;
	}

	public void CopyData() {
		spawnerSurvival.boxSpawnParametrs = boxSpawnParametrs;
	}

	void ChangePhase(RunnerPhase runnerPhase) {

    if(boxSpawner != null)
		  boxSpawner.ChangePhase(runnerPhase);

		//if (runnerPhase == RunnerPhase.postBoost && newPhase == RunnerPhase.run) {
		//	//for(int i = 0; i < boxSpawnParametrs.Length; i++)
		//	//  boxSpawnParametrs[i].afterBoost = true;
		//}

		//if (newPhase == RunnerPhase.run) {

		//	weaponNextDistanceCalc = weaponBoxParametr.Find(x => x.distance > RunnerController.playerDistantion).distance;

		//	boxSpawnNextDistanceCalc = 999999999999;
		//	boxSpawnNextDistanceSpawn = 99999999999;

		//	for (int i = 0; i < boxSpawnParametrs.Length; i++) {
		//		boxSpawnParametrs[i].nextDistance = 99999999999;
		//		boxSpawnParametrs[i].nextDistanceCalc = 9999999999;

		//		for (int j = 0; j < boxSpawnParametrs[i].parametrs.Count; j++) {
		//			if (boxSpawnParametrs[i].parametrs[j].distance <= RunnerController.playerDistantion && j != 0) {
		//				boxSpawnParametrs[i].nextDistanceCalc = boxSpawnParametrs[i].parametrs[j].distance;

		//			} else {
		//				if (boxSpawnParametrs[i].nextDistanceCalc > boxSpawnParametrs[i].parametrs[j].distance) {
		//					boxSpawnParametrs[i].nextDistanceCalc = boxSpawnParametrs[i].parametrs[j].distance;
		//				}
		//				if (boxSpawnNextDistanceCalc > boxSpawnParametrs[i].nextDistanceCalc)
		//					boxSpawnNextDistanceCalc = boxSpawnParametrs[i].nextDistanceCalc;
		//			}
		//		}
		//	}
		//}

		//runnerPhase = newPhase;
	}

	void Update() {
    if(boxSpawner != null)
		  boxSpawner.Update();
	}

	//void GenerateSurvival() {
	//	if (weaponNextDistanceCalc <= RunnerController.playerDistantion) {
	//		for (int i = 0; i < weaponBoxParametr.Count; i++) {
	//			if (weaponBoxParametr[i].distance == weaponNextDistanceCalc && weaponNextDistanceCalc <= RunnerController.playerDistantion) {

	//				if (i == weaponBoxParametr.Count - 1)
	//					InfinityIncrementWeapon();
	//				if (i != weaponBoxParametr.Count - 1)
	//					weaponNextDistanceCalc = weaponBoxParametr[i + 1].distance;

	//				CalcGenerateWeaponBox(i);
	//			}
	//		}
	//	}

	//	if (weaponBoxDistance.Count > 0) {

	//		float tempBoxDist = 0;

	//		foreach (float weaponBoxDist in weaponBoxDistance) {
	//			if (weaponBoxDist <= RunnerController.playerDistantion) {

	//				if (runner.generateItems) {
	//					int needNum = BinarySearch.RandomNumberGenerator(wpn);

	//					GameObject obj = pool.getPooledObject(wpnName[needNum]);
	//					obj.transform.parent = transform;
	//					obj.SetActive(false);
	//					generateList.Add(obj);
	//					tempBoxDist = weaponBoxDist;
	//				}
	//			}
	//		}

	//		if (tempBoxDist > 0)
	//			weaponBoxDistance.Remove(tempBoxDist);
	//	}

	//	if (boxSpawnNextDistanceCalc <= RunnerController.playerDistantion) {

	//		boxSpawnNextDistanceCalc = 9999999999999;
	//		boxSpawnNextDistanceSpawn = 9999999999999;

	//		for (int i = 0; i < boxSpawnParametrs.Length; i++) {
	//			if (boxSpawnParametrs[i].nextDistanceCalc <= RunnerController.playerDistantion) {
	//				for (int j = 0; j < boxSpawnParametrs[i].parametrs.Count; j++) {
	//					if (boxSpawnParametrs[i].parametrs[j].distance == boxSpawnParametrs[i].nextDistanceCalc && boxSpawnParametrs[i].parametrs[j].distance <= RunnerController.playerDistantion) {


	//						if (j == boxSpawnParametrs[i].parametrs.Count - 1)
	//							InfinityIncrementBoxSpawnElement(i);
	//						if (j != boxSpawnParametrs[i].parametrs.Count - 1) {
	//							boxSpawnParametrs[i].nextDistanceCalc = boxSpawnParametrs[i].parametrs[j + 1].distance;
	//							if (boxSpawnNextDistanceCalc > boxSpawnParametrs[i].nextDistanceCalc)
	//								boxSpawnNextDistanceCalc = boxSpawnParametrs[i].nextDistanceCalc;
	//						}

	//						float probab = boxSpawnParametrs[i].parametrs[j].probability;

	//						if (boxSpawnParametrs[i].name == "Heart")
	//							probab += hearthProbability + (heartClothes.head ? 0.1f : 0) + (heartClothes.spine ? 0.1f : 0) + (heartClothes.accessory ? 0.1f : 0);

	//						if (boxSpawnParametrs[i].name == "Magnet")
	//							probab += magnetProbability + (magnetClothes.head ? 0.1f : 0) + (magnetClothes.spine ? 0.1f : 0) + (magnetClothes.accessory ? 0.1f : 0);

	//						if (Random.value <= probab) {
	//							float nextDistance;
	//							if (boxSpawnParametrs[i].parametrs.Count <= j + 1)
	//								nextDistance = 200;
	//							else
	//								nextDistance = boxSpawnParametrs[i].parametrs[j + 1].distance - boxSpawnParametrs[i].parametrs[j].distance;

	//							boxSpawnParametrs[i].nextDistance = boxSpawnParametrs[i].parametrs[j].distance + Random.Range(20, nextDistance);

	//							//if(boxSpawnParametrs[i].afterBoost && boxSpawnParametrs[i].nextDistance <= RunnerController.playerDistantion)
	//							//  boxSpawnParametrs[i].nextDistance = 99999999999;

	//							//if(boxSpawnParametrs[i].afterBoost) boxSpawnParametrs[i].afterBoost = false;

	//						}
	//					}
	//				}
	//			}
	//			if (boxSpawnNextDistanceSpawn > boxSpawnParametrs[i].nextDistance)
	//				boxSpawnNextDistanceSpawn = boxSpawnParametrs[i].nextDistance;
	//		}
	//	}

	//	if (boxSpawnNextDistanceSpawn <= RunnerController.playerDistantion) {
	//		boxSpawnNextDistanceSpawn = 9999999999;

	//		for (int i = 0; i < boxSpawnParametrs.Length; i++) {


	//			if (boxSpawnParametrs[i].nextDistance <= RunnerController.playerDistantion) {
	//				int needNum = 0;
	//				boxSpawnParametrs[i].nextDistance = 99999999;
	//				if (boxSpawnParametrs[i].prefab.Length > 1)
	//					needNum = CalcProbabilityBoxSpawn(boxSpawnParametrs[i]);

	//				GameObject obj = pool.getPooledObject(boxSpawnParametrs[i].prefab[needNum].prefab.name);
	//				obj.transform.parent = transform;
	//				obj.SetActive(false);
	//				generateList.Add(obj);
	//			}

	//			if (boxSpawnNextDistanceSpawn > boxSpawnParametrs[i].nextDistance)
	//				boxSpawnNextDistanceSpawn = boxSpawnParametrs[i].nextDistance;

	//		}
	//	}

	//	if (generateList.Count > 0 && nextGenerate < Time.time) {
	//		Vector3 generatePosition = new Vector3(CameraController.displayDiff.transform.position.x + CameraController.displayDiff.right * 1.2f, runner.thisMapHeight + Random.Range(1f, 2f), 0);
	//		Collider[] objCol = Physics.OverlapSphere(generatePosition, 6f);

	//		bool YesGenerate = true;

	//		foreach (Collider oneColl in objCol) {
	//			if (oneColl.tag == "RollingStone" | oneColl.tag == "jumpUp" | oneColl.tag == "jumpDown")
	//				YesGenerate = false;
	//		}

	//		if (YesGenerate) {
	//			foreach (GameObject obj in generateList) {
	//				obj.transform.position = generatePosition;
	//				obj.SetActive(true);
	//				generateList.RemoveAt(0);
	//				nextGenerate = Time.time + 1f;
	//				break;
	//			}
	//		}
	//	}
	//}

	//void CalcGenerateWeaponBox(int num) {

	//	float nextDistance;

	//	if (weaponBoxParametr.Count <= num + 1)
	//		nextDistance = 200;
	//	else
	//		nextDistance = weaponBoxParametr[num + 1].distance - weaponBoxParametr[num].distance;

	//	int countNeed = Random.Range(weaponBoxParametr[num].count - 1, weaponBoxParametr[num].count + 1) + +(boxClothes.head ? 1 : 0) + (boxClothes.spine ? 1 : 0) + (boxClothes.accessory ? 1 : 0);
	//	weaponBoxDistance = new List<float>();

	//	int tempcount = countNeed;
	//	float lastDistance = weaponBoxParametr[num].distance;

	//	while (weaponBoxDistance.Count < countNeed) {
	//		float dist = nextDistance / tempcount;
	//		float newDist = Random.Range(20, dist);
	//		lastDistance += newDist;
	//		weaponBoxDistance.Add(lastDistance);
	//		nextDistance -= newDist;
	//		tempcount = tempcount - 1;
	//	}

	//	if (weaponBeforeBoost) {
	//		weaponBeforeBoost = false;
	//		weaponBoxDistance.RemoveAll(x => x <= RunnerController.playerDistantion);
	//	}

	//}

	/// <summary>
	/// Подготовка структуры под генерацию
	/// </summary>
	void ProcessesWeapon() {
		float sum = 0;
		wpn.Clear();
		wpnName.Clear();
		wpn.Add(sum);

		int weaponBox = PlayerPrefs.GetInt("weaponBox");

		List<GameObject> tmp = new List<GameObject>();

		for (int i = 0; i < weaponParamtrs[weaponBox].chance.Length; i++) {
			tmp.Add(weaponParamtrs[weaponBox].chance[i].box);
			sum += weaponParamtrs[weaponBox].chance[i].probability;
			wpn.Add(sum);
			wpnName.Add(weaponParamtrs[weaponBox].chance[i].box.name);
		}

		wpn = wpn.ConvertAll(x => x / sum);
	}

	#region Бесконечная генерация

	/// <summary>
	/// Бесконечный инкремент параметров оружия
	/// </summary>
	//void InfinityIncrementWeapon() {

	//	WeaponBoxParametrs weaponNewParam = new WeaponBoxParametrs();
	//	weaponNewParam.distance = weaponBoxParametr[weaponBoxParametr.Count - 1].distance + 200;
	//	weaponNewParam.count = weaponBoxParametr[weaponBoxParametr.Count - 1].count;
	//	weaponBoxParametr.Add(weaponNewParam);
	//}


	/// <summary>
	/// Бесконечный инкремент генератора ящиков
	/// </summary>
	//void InfinityIncrementBoxSpawnElement(int boxNum) {

	//	BoxParametrs newParamtr = new BoxParametrs();
	//	newParamtr.distance = boxSpawnParametrs[boxNum].parametrs[boxSpawnParametrs[boxNum].parametrs.Count - 1].distance + 200;
	//	newParamtr.probability = boxSpawnParametrs[boxNum].parametrs[boxSpawnParametrs[boxNum].parametrs.Count - 1].probability;
	//	boxSpawnParametrs[boxNum].parametrs.Add(newParamtr);
	//}

	//int CalcProbabilityBoxSpawn(BoxSpawn boxParametr) {
	//	float sum = 0;
	//	List<float> param = new List<float>();
	//	param.Add(sum);

	//	List<GameObject> tmp = new List<GameObject>();

	//	for (int i = 0; i < boxParametr.prefab.Length; i++) {
	//		tmp.Add(boxParametr.prefab[i].prefab);
	//		sum += boxParametr.prefab[i].probability;
	//		param.Add(sum);
	//	}

	//	param = param.ConvertAll(x => x / sum);

	//	return BinarySearch.RandomNumberGenerator(param);

	//}

	/// <summary>
	/// Генерация специального ящика сверху
	/// </summary>
	public void GetTopWeaponBox() {
		GameObject obj = LevelPooler.Instance.GetPooledObject(POOLER_KEY, "BoxSabel");
		obj.transform.parent = transform;
		obj.transform.position = new Vector3(Random.Range(CameraController.displayDiff.leftDif(0.9f), CameraController.displayDiff.rightDif(0.9f)), CameraController.displayDiff.topDif(1.2f), 0);
		obj.GetComponent<BoxController>().SetDropDown();
		obj.SetActive(true);
	}

	#endregion

	void StructProcessor() {
		Dictionary<string, KeyValuePair<GameObject, int>> boxListName = new Dictionary<string, KeyValuePair<GameObject, int>>();

		foreach (BoxLibrary oneLib in boxLibrary)
			oneLib.prefabs.ForEach(x => boxListName.Add(x.prefab.name, new KeyValuePair<GameObject, int>(x.prefab, 2)));

		LevelPooler.Instance.AddPool(POOLER_KEY, boxListName);

	}

	public void GenerateTakeEffect(Vector3 position) {
		GameObject sfx = Pooler.GetPooledObject("TakeBox");
		sfx.transform.position = position;
		sfx.SetActive(true);
	}

	public GameObject GenerateBox(BoxCategory category, BoxType type) {
		return LevelPooler.Instance.GetPooledObject(POOLER_KEY, boxLibrary.Find(x => x.category == category).prefabs.Find(x => x.type == type).prefab.name);
	}

	#region Настройки

	//ConfigData configData;

	public void GetConfig() {
    if(boxSpawner != null)
		  boxSpawner.GetConfig();
	}

	//void ConfigSurvival() {
	//	configData = Config.instant.config.GetConfigData();

	//	//List<object> itemsData = (List<object>)((Dictionary<string,object>)((List<object>)((Dictionary<string,object>)configData.config)["levels"])[GameManager.activeLevel])["items"];
	//	List<object> itemsData = (List<object>)configData.GetLevelData()["items"];
	//	Dictionary<string, object> oneItem;
	//	int startCol = 1;
	//	float distance;
	//	bool isEnd = false;

	//	weaponBoxParametr = new List<WeaponBoxParametrs>();

	//	for (int j = 0; j < boxSpawnParametrs.Length; j++) {
	//		boxSpawnParametrs[j].parametrs = new List<BoxParametrs>();
	//	}

	//	for (int i = 0; i < itemsData.Count; i++) {
	//		if (i < startCol || isEnd) {
	//			continue;
	//		}
	//		oneItem = (Dictionary<string, object>)itemsData[i];
	//		WeaponBoxParametrs boxParam = new WeaponBoxParametrs();
	//		try {
	//			distance = float.Parse(oneItem["distantion"].ToString());
	//		} catch {
	//			distance = weaponBoxParametr[weaponBoxParametr.Count - 1].distance + 200;
	//			isEnd = true;
	//		}
	//		boxParam.distance = distance;
	//		boxParam.count = int.Parse(oneItem["boxes"].ToString());
	//		weaponBoxParametr.Add(boxParam);

	//		for (int j = 0; j < boxSpawnParametrs.Length; j++) {

	//			switch (boxSpawnParametrs[j].name) {
	//				case "Magnet":
	//					if (oneItem.ContainsKey("magnet") && oneItem["magnet"].ToString() != "0") {
	//						BoxParametrs box = new BoxParametrs();
	//						box.distance = distance;
	//						box.probability = float.Parse(oneItem["magnet"].ToString()) * 0.01f;
	//						boxSpawnParametrs[j].parametrs.Add(box);
	//					}
	//					break;
	//				case "Heart":
	//					if (oneItem.ContainsKey("hearth") && oneItem["hearth"].ToString() != "0") {
	//						BoxParametrs box = new BoxParametrs();
	//						box.distance = distance;
	//						box.probability = float.Parse(oneItem["hearth"].ToString()) * 0.01f;
	//						boxSpawnParametrs[j].parametrs.Add(box);
	//					}
	//					break;
	//				case "Shield":
	//					if (oneItem.ContainsKey("magnet") && oneItem["magnet"].ToString() != "0") {
	//						BoxParametrs box = new BoxParametrs();
	//						box.distance = distance;
	//						box.probability = float.Parse(oneItem["magnet"].ToString()) * 0.01f;
	//						boxSpawnParametrs[j].parametrs.Add(box);
	//					}
	//					break;
	//				case "Magic":
	//					if (oneItem.ContainsKey("magic") && oneItem["magic"].ToString() != "0") {
	//						BoxParametrs box = new BoxParametrs();
	//						box.distance = distance;
	//						box.probability = float.Parse(oneItem["magic"].ToString()) * 0.01f;
	//						boxSpawnParametrs[j].parametrs.Add(box);
	//					}
	//					break;
	//			}
	//		}
	//	}
	//}

	#endregion

}

///// <summary>
///// Класс отвечает за генераю ящиков на уровнях
///// </summary>
//[System.Serializable]
//public class BoxLevel {

//	/// <summary>
//	/// Параметры дистанции
//	/// </summary>
//	[System.Serializable]
//	public struct GenerateParametrs {
//		public float distantion;
//		public int count;
//		public List<BoxProbability> param;
//	}

//	/// <summary>
//	/// Вероятность генерации
//	/// </summary>
//	[System.Serializable]
//	public struct BoxProbability {
//		public BoxCategory type;
//		public float value;
//	}


//	BoxSpawner boxSpawner;

//	/// <summary>
//	/// Позиция генерации
//	/// </summary>
//	Vector3 generatePosition {
//		get {
//			return new Vector3(CameraController.displayDiff.transform.position.x + CameraController.displayDiff.right * 1.2f, RunnerController.instance.thisMapHeight + Random.Range(1f, 2f), 0);
//		}
//	}

//	float distantionCalc;               // Дистанция рассчета новой генерации
//	float distantionGenerate;           // Следующая дистанция для генерации
//	float perionGeneration;             // Период генерации

//	public List<GenerateParametrs> generateParametrs; // Параметры генерации

//	List<BoxCategory> generateList;

//	public void Init(BoxSpawner parent) {
//		boxSpawner = parent;
//		generateList = new List<BoxCategory>();
//		distantionCalc = 0;
//		GetConfig();
//	}

//	public void Update() {

//		if (distantionCalc <= RunnerController.playerDistantion) {
//			GenerateParametrs newPosition = generateParametrs.Find(x => x.distantion > distantionCalc);
//			CalcDistantions(newPosition);
//			CalcGenerateList(newPosition);
//			CalcNextDistanceGenerate();
//			distantionCalc = newPosition.distantion;
//		}

//		if (distantionGenerate >= 0 && distantionGenerate <= RunnerController.playerDistantion && generateList.Count > 0 && CheckReadyGenerate()) {
//			GenerateBox();
//			CalcNextDistanceGenerate();
//		}

//	}
//	void GenerateBox() {

//		string namePrefab = "";

//		switch (generateList[0]) {
//			case BoxCategory.weapon: {
//					int num = BinarySearch.RandomNumberGenerator(boxSpawner.wpn);
//					try {
//						namePrefab = boxSpawner.boxLibrary.Find(x => x.category == BoxCategory.weapon).prefabs
//							.Find(x => x.prefab.name == boxSpawner.wpnName[num]).prefab.name;
//					} catch {
//						Debug.LogError("Weapon Generate error :" + num);
//					}
//					break;
//				}
//			default: {
//					namePrefab = boxSpawner.boxLibrary.Find(x => x.category == generateList[0]).GetPrefab().name;
//					break;
//				}
//		}

//		GameObject obj = boxSpawner.pool.getPooledObject(namePrefab);
//		obj.transform.parent = boxSpawner.transform;
//		obj.transform.position = generatePosition;
//		obj.SetActive(true);
//		generateList.RemoveAt(0);

//	}

//	/// <summary>
//	/// Проверка возможности генерации
//	/// </summary>
//	/// <returns></returns>
//	public bool CheckReadyGenerate() {
//		Collider[] objCol = Physics.OverlapSphere(generatePosition, 6f);

//		foreach (Collider oneColl in objCol) {
//			if (oneColl.tag == "RollingStone" | oneColl.tag == "jumpUp" | oneColl.tag == "jumpDown")
//				return false;
//		}
//		return true;
//	}

//	/// <summary>
//	/// Рассчет начала генерации и периода
//	/// </summary>
//	/// <param name="parametrs"></param>
//	void CalcDistantions(GenerateParametrs parametrs) {
//		if (parametrs.count <= 0)
//			return;
//		distantionGenerate = distantionCalc;
//		perionGeneration = (parametrs.distantion - distantionCalc) / parametrs.count;
//	}
//	/// <summary>
//	/// Рассчет следующей генерации с учотом периода
//	/// </summary>
//	void CalcNextDistanceGenerate() {
//		if (generateList.Count > 0)
//			distantionGenerate += Random.Range(0, perionGeneration);
//		else
//			distantionGenerate = -1;
//	}
//	/// <summary>
//	/// Рассчет массива под генерацию
//	/// </summary>
//	/// <param name="parametrs">Структура</param>
//	void CalcGenerateList(GenerateParametrs parametrs) {
//		// Если присутствуют ящики под генерацию
//		if (parametrs.count <= 0)
//			return;
//		generateList.Clear();

//		// Готовим массив под выбор
//		List<float> tempCd = new List<float>();

//		float sum1 = 0;
//		tempCd.Add(sum1);
//		foreach (BoxProbability one in parametrs.param) {
//			sum1 += one.value;
//			tempCd.Add(sum1);
//		}

//		int boxCount = parametrs.count;

//		while (boxCount > 0) {
//			boxCount--;
//			int number = BinarySearch.RandomNumberGenerator(tempCd);
//			generateList.Add(parametrs.param[number].type);
//		}

//	}
//	/// <summary>
//	/// Бесконечный инкремент (на всякий случай)
//	/// </summary>
//	void InfinityIncrement() {
//		GenerateParametrs param = new GenerateParametrs();
//		param.count = generateParametrs[generateParametrs.Count - 1].count;
//		param.distantion = generateParametrs[generateParametrs.Count - 1].distantion - generateParametrs[generateParametrs.Count - 2].distantion;
//		param.param = new List<BoxProbability>(generateParametrs[generateParametrs.Count - 1].param);
//		generateParametrs.Add(param);
//	}
//	/// <summary>
//	/// Получение настроек
//	/// </summary>
//	public void GetConfig() {
//		ConfigData configData = Config.instant.config.GetConfigData();
//		List<object> itemsData = (List<object>)configData.GetLevelData()["bonus"];

//		generateParametrs = new List<GenerateParametrs>();

//		for (int i = 0; i < itemsData.Count; i++) {
//			Dictionary<string, object> oneItem = (Dictionary<string, object>)itemsData[i];
//			GenerateParametrs newParam = new GenerateParametrs();
//			newParam.param = new List<BoxProbability>();

//			newParam.distantion = float.Parse(oneItem["distantion"].ToString());
//			newParam.count = int.Parse(oneItem["bonus"].ToString());

//			if (oneItem.ContainsKey("blackMark") && oneItem["blackMark"].ToString() != "0") {
//				BoxProbability box = new BoxProbability();
//				box.type = BoxCategory.blackMark;
//				box.value = float.Parse(oneItem["blackMark"].ToString()) * 0.01f;
//				newParam.param.Add(box);
//			}

//			if (oneItem.ContainsKey("boxes") && oneItem["boxes"].ToString() != "0") {
//				BoxProbability box = new BoxProbability();
//				box.type = BoxCategory.weapon;
//				box.value = float.Parse(oneItem["boxes"].ToString()) * 0.01f;
//				newParam.param.Add(box);
//			}

//			if (oneItem.ContainsKey("hearth") && oneItem["hearth"].ToString() != "0") {
//				BoxProbability box = new BoxProbability();
//				box.type = BoxCategory.hearth;
//				box.value = float.Parse(oneItem["hearth"].ToString()) * 0.01f;
//				newParam.param.Add(box);
//			}

//			if (oneItem.ContainsKey("magnet") && oneItem["magnet"].ToString() != "0") {
//				BoxProbability box = new BoxProbability();
//				box.type = BoxCategory.magnet;
//				box.value = float.Parse(oneItem["magnet"].ToString()) * 0.01f;
//				newParam.param.Add(box);
//			}

//			if (oneItem.ContainsKey("shield") && oneItem["shield"].ToString() != "0") {
//				BoxProbability box = new BoxProbability();
//				box.type = BoxCategory.shield;
//				box.value = float.Parse(oneItem["shield"].ToString()) * 0.01f;
//				newParam.param.Add(box);
//			}

//			generateParametrs.Add(newParam);
//		}

//	}

//}
