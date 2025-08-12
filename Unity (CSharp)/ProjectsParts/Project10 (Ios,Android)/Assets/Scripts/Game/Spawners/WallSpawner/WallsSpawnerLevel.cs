using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(WallsSpawnerLevel))]
public class WallsSpawnerLevelEditor: Editor {

  int id = 0;
  string[] layerName;
  int[] layerId;

  void OnEnable() {
    layerId = new int[SortingLayer.layers.Length];
    layerName = new string[SortingLayer.layers.Length];
    for (int i = 0; i < SortingLayer.layers.Length; i++) {
      layerName[i] = SortingLayer.layers[i].name;
      layerId[i] = SortingLayer.layers[i].id;
    }
  }

  public override void OnInspectorGUI() {

    WallsSpawnerLevel script = (WallsSpawnerLevel)target;

    for (int i = 0; i < layerId.Length; i++)
      if (layerId[i] == script.layerId) id = i;

    id = EditorGUILayout.Popup("Layer ID:", id, layerName);
    script.layerId = layerId[id];
    base.OnInspectorGUI();
  }
}

#endif


[System.Serializable]
public class WallsSpawnerLevel : WallsSpawner {

	/// <summary>
	/// Параметры дистанции
	/// </summary>
	[System.Serializable]
	public struct GenerateParametrs {
		public float distantion;
		public int count;
		public List<WallCategories> category;
	}

	/// <summary>
	/// Вероятность генерации
	/// </summary>
	[System.Serializable]
	public struct WallCategories {
		public WallCategory type;
		public float value;
	}
  
	/// <summary>
	/// Позиция генерации
	/// </summary>
	Vector3 generatePosition {
		get {
			return new Vector3(CameraController.displayDiff.transform.position.x + CameraController.displayDiff.right * 1.2f, RunnerController.Instance.mapHeight + Random.Range(1f, 2f), 0);
		}
	}

	float distantionCalc = 0;                                       // Дистанция рассчета новой генерации
	float perionGeneration;                                         // Период генерации
	List<WallCategory> categoryList = new List<WallCategory>();     // Вероятность генерации платформ по категории
	List<float> categoryCD = new List<float>();                     // Вероятность генерации платформ по категории
	private GameObject obj;                                         // Сгенерированный объект
	private WallType lastType = WallType.none;                      // Последний сгенерированный тип

	public List<GenerateParametrs> generateParametrs;       // Параметры генерации

	protected override void Start() {
		GetConfig();

    lastSpawnPosition = Vector3.left * 50;

    base.Start();
  }



  [ContextMenu("copy")]
  private void CopyOldData() {

    WallsSpawner ws = GetComponent<WallsSpawner>();
    this.walls = new List<Wall>(ws.walls);

  }

  public void Update() {

		if (distantionCalc <= RunnerController.playerDistantion) {
			GenerateParametrs newGeneration = generateParametrs.Find(x => x.distantion > distantionCalc);
			CalcProbability(newGeneration);
			distantionCalc = newGeneration.distantion;
		}
		Generate();
		DestroyOld();
	}
	void Generate() {

		if (GameManager.activeLevelData.moveVector == MoveVector.left) {
			while (lastSpawnPosition.x - (wallWidth * (IsIpad ? 1.1f : 1)) - (ruptureWidth * (IsIpad ? 1.1f : 1)) >= displayDiff.leftDif(1.7f)) {
				GenerateWall();
			}
		}
		else {
			while (lastSpawnPosition.x + (wallWidth * (IsIpad ? 1.1f : 1)) + (ruptureWidth * (IsIpad ? 1.1f : 1)) <= displayDiff.rightDif(1.7f)) {
				GenerateWall();
			}
		}
    
  }

	void DestroyOld() {
		if (timeToCheckDeactive > Time.time)
			return;
		timeToCheckDeactive = Time.time + 10;
		generateList.ForEach(CheckOut);
	}
	
	void CheckOut(Transform one) {
		if (!one.gameObject.activeInHierarchy)
			return;
		if ((GameManager.activeLevelData.moveVector == MoveVector.left && one.position.x > displayDiff.rightDif(3)) || one.position.x < displayDiff.leftDif(3))
			one.gameObject.SetActive(false);
	}

	void GenerateWall() {

		generateDecor = false;              // Декорация не сгенерирована 

		WallType wall = WallType.none;

		if (ruptureWidth > 0)
			wall = WallType.ruptureEnd;
		
		if (GameManager.activeLevelData.moveVector == MoveVector.left) {
			GameObject genObject = Spawn(new Vector3(lastSpawnPosition.x - (wallWidth * (IsIpad ? 1.1f : 1)) - (ruptureWidth * (IsIpad ? 1.1f : 1)), transform.position.y, transform.position.z), wall);

			if (GameManager.activeLevelData.gameMode == GameMode.survival && thisNumber >= nextTorchGenerale && (lastType == WallType.idle) && !generateDecor && RunnerController.Instance.activeLevel != ActiveLevelType.ship) {
				if (Random.value <= 0.4f) {
					genObject.GetComponent<WallDecor>().SetTorch();
					generateDecor = true;
					nextTorchGenerale = thisNumber + 1;
				}
			} else if (GameManager.activeLevelData.gameMode != GameMode.survival && torchGenerateCount > 0) {
				if (genObject.GetComponent<WallDecor>().SetTorch())
					torchGenerateCount--;
			}
		} else {
			GameObject genObject = Spawn(new Vector3(lastSpawnPosition.x + (wallWidth * (IsIpad ? 1.1f : 1)) + (ruptureWidth * (IsIpad ? 1.1f : 1)), transform.position.y, transform.position.z), wall);

			if (GameManager.activeLevelData.gameMode == GameMode.survival && thisNumber >= nextTorchGenerale && (lastType == WallType.idle) && !generateDecor && RunnerController.Instance.activeLevel != ActiveLevelType.ship) {
				if (Random.value <= 0.4f) {
					genObject.GetComponent<WallDecor>().SetTorch();
					generateDecor = true;
					nextTorchGenerale = thisNumber + 1;
				}
			} else if (GameManager.activeLevelData.gameMode != GameMode.survival && torchGenerateCount > 0) {
				if (genObject.GetComponent<WallDecor>().SetTorch())
					torchGenerateCount--;
			}

		}
		
	}

	public GameObject Spawn(Vector3 position, WallType wallType = WallType.none) {
		// Инкремент счетчика созжанных объектов
		thisNumber++;

		WallCategory needCat = categoryList[BinarySearch.RandomNumberGenerator(categoryCD)];

		if (wallType == WallType.none)
			wallType = WallType.idle;
		else
			needCat = WallCategory.idle;

		if (needCat == WallCategory.hole)
			wallType = WallType.ruptureStart;

		GameObject obj = LevelPooler.Instance.GetPooledObject(POOLER_KEY, wallPrefab.name, generateList);

		obj.GetComponent<WallDecor>().Init(wallType, needCat);
		obj.transform.position = new Vector3(position.x, position.y, transform.position.z);

		if (IsIpad)
			obj.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);

		WallDecor decor = obj.GetComponent<WallDecor>();

		if (decor != null) {
			if (decor != null)
				decor.SetOrder(0, layerId);
			else {
				Debug.Log(obj.name);
			}
		}

		lastType = wallType;

		// Если был сгенерирован разрыв, рассчитываем ширину
		if (lastType == WallType.ruptureStart) {
			ruptureWidth = Random.Range(0f, 13.2f);
		} else {
			ruptureWidth = 0;
		}

    lastSpawnPosition = obj.transform.position;
		obj.SetActive(true);
		return obj;
	}
	
	void CalcProbability(GenerateParametrs parametrs) {
		categoryCD.Clear();
		categoryList.Clear();

		float sum = 0;
		categoryCD.Add(sum);

		foreach (WallCategories cat in parametrs.category) {
			sum += cat.value;
			categoryCD.Add(sum);
			categoryList.Add(cat.type);
		}

		if (sum < 1) {
			sum += 1 - sum;
			categoryCD.Add(sum);
			categoryList.Add(WallCategory.idle);
		}

		categoryCD = categoryCD.ConvertAll(x => x / sum);
	}


	/// <summary>
	/// Бесконечный инкремент (на всякий случай)
	/// </summary>
	void InfinityIncrement() {
		GenerateParametrs param = new GenerateParametrs();
		param.count = generateParametrs[generateParametrs.Count - 1].count;
		param.distantion = generateParametrs[generateParametrs.Count - 1].distantion - generateParametrs[generateParametrs.Count - 2].distantion;
		param.category = new List<WallCategories>(generateParametrs[generateParametrs.Count - 1].category);
		generateParametrs.Add(param);
	}
	/// <summary>
	/// Получение настроек
	/// </summary>
	protected override void GetConfig() {
    base.GetConfig();


    List<Configuration.Levels.Road> itemsData = Config.Instance.config.activeLevel.road;
		List<Configuration.Levels.Decoration> decorData = Config.Instance.config.activeLevel.decoration;

		generateParametrs = new List<GenerateParametrs>();
		float allWallParam = 1;

		for (int i = 0; i < itemsData.Count; i++) {
			GenerateParametrs newParam = new GenerateParametrs();
			newParam.category = new List<WallCategories>();
			allWallParam = 1;

			newParam.distantion = itemsData[i].distantion;
			if (decorData[i].decor > 0)
				newParam.count = decorData[i].decor;

			if (decorData[i].decorWall > 0) {
				WallCategories box = new WallCategories();
				box.type = WallCategory.decor;
				box.value = decorData[i].decorWall * 0.01f;
				allWallParam = box.value;
				newParam.category.Add(box);
			}

			if (decorData[i].wallBreack > 0) {
				WallCategories box = new WallCategories();
				box.type = WallCategory.hole;
				box.value = decorData[i].wallBreack * 0.01f;
				allWallParam = box.value;
				newParam.category.Add(box);
			}

			if (decorData[i].wallDestroy > 0) {
				WallCategories box = new WallCategories();
				box.type = WallCategory.destroy;
				box.value = decorData[i].wallDestroy * 0.01f;
				allWallParam = box.value;
				newParam.category.Add(box);
			}

			if (decorData[i].wallDestroyMini > 0) {
				WallCategories box = new WallCategories();
				box.type = WallCategory.gap;
				box.value = decorData[i].wallDestroyMini * 0.01f;
				allWallParam = box.value;
				newParam.category.Add(box);
			}

			if (allWallParam > 0) {
				WallCategories box = new WallCategories();
				box.type = WallCategory.idle;
				box.value = allWallParam;
				newParam.category.Add(box);
			}

			generateParametrs.Add(newParam);
		}

	}

}
