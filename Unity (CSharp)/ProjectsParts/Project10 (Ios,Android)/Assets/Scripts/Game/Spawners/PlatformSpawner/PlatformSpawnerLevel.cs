using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

/// <summary>
/// Генератор в режиме уровней
/// </summary>
[Serializable]
public class PlatformSpawnerLevel : IPlatformSpawner {

	/// <summary>
	/// Параметры дистанции
	/// </summary>
	[System.Serializable]
	public struct GenerateParametrs {
		public float distantion;
		public int count;
		public List<PlatformMaterials> material;
		public List<PlatformCategories> category;
	}

	/// <summary>
	/// Вероятность генерации
	/// </summary>
	[System.Serializable]
	public struct PlatformMaterials {
		public PlatformMaterial type;
		public float value;
	}

	/// <summary>
	/// Вероятность генерации
	/// </summary>
	[System.Serializable]
	public struct PlatformCategories {
		public PlatformCategory type;
		public float value;
	}

	PlatformSpawner platformSpawner;

	/// <summary>
	/// Позиция генерации
	/// </summary>
	Vector3 generatePosition {
		get { return new Vector3(CameraController.displayDiff.transform.position.x + CameraController.displayDiff.right * 1.2f, RunnerController.Instance.mapHeight + Random.Range(1f, 2f), 0); }
	}

  private float distantionCalc;                                   // Дистанция рассчета новой генерации
  private float perionGeneration;                                 // Период генерации
  private List<PlatformMaterial> materialList = new List<PlatformMaterial>();             // Вероятность генерации платформ по материалу
  private List<float> materialCD = new List<float>();             // Вероятность генерации платформ по материалу
  private List<PlatformCategory> categoryList = new List<PlatformCategory>();             // Вероятность генерации платформ по категории
  private List<float> categoryCD = new List<float>();             // Вероятность генерации платформ по категории
  private List<PlatformType> typeList = new List<PlatformType>();             // Вероятность генерации платформ по категории
  private List<float> typeCD = new List<float>();             // Вероятность генерации платформ по категории
	public float platformWidth;                            // Ширина платформы
	private float breakWidth;                               // Размер ямы
	private int thisNumber = 0;                             // Счетчик созданных объектов
	private GameObject obj;                                 // Сгенерированный объект
	private PlatformType lastType;                          // Последний сгенерированный тип
	private int bridgeCount = 0;                            // Секция в построении моста
	private List<Transform> generateList;                   // Лист сгенерированных объектов
	private float timeToCheckDeactive;                      // Время для новой проверки

	public List<GenerateParametrs> generateParametrs = new List<GenerateParametrs>();       // Параметры генерации

	public void Init(PlatformSpawner parent) {
		platformSpawner = parent;
		distantionCalc = 0;
		timeToCheckDeactive = Time.time + 10;
		generateList = new List<Transform>();
		GetConfig();
		TypeProbability();
	}

	public void Update() {

		if(distantionCalc <= RunnerController.playerDistantion) {
			GenerateParametrs newGeneration = generateParametrs.Find(x => x.distantion > distantionCalc);
			//GenerateParametrs generateActive = generateParametrs.Find(x => x.distantion == distantionCalc);
			CalcProbability(newGeneration);
			distantionCalc = newGeneration.distantion;
		}
		Generate();
		DestroyOld();
	}

	void Generate() {
		while((PlatformSpawner.lastSpawnPosition.x + platformWidth + breakWidth) <= platformSpawner.displayDiff.rightDif(3)) {
			GeneratePlatforms();
		}
	}

	void DestroyOld() {
		if(timeToCheckDeactive > Time.time) return;
		timeToCheckDeactive = Time.time + 10;
		generateList.ForEach(CheckOut);
	}

	void CheckOut(Transform one) {
		if(!one.gameObject.activeInHierarchy) return;
		if(one.position.x < platformSpawner.displayDiff.leftDif(4)) one.gameObject.SetActive(false);
	}

	void GeneratePlatforms() {
		obj = null;

		if((lastType == PlatformType.bridge | lastType == PlatformType.bridgeIdle) && bridgeCount >= 0 && !obj) {
			if(platformSpawner.generateBreack)
				bridgeCount = 0;

			if(bridgeCount >= 1) {
				obj = Spawn(new Vector2(PlatformSpawner.lastSpawnPosition.x + platformWidth, platformSpawner.thisHeight), PlatformType.bridgeIdle);
				bridgeCount--;
			} else if(bridgeCount == 0) {
				obj = Spawn(new Vector2(PlatformSpawner.lastSpawnPosition.x + platformWidth, platformSpawner.thisHeight), PlatformType.idle);
				bridgeCount--;
			}
		}

		//обрыв
		if(lastType == PlatformType.breaks & !obj) {
			if((PlatformSpawner.lastSpawnPosition.x + platformWidth) + breakWidth <= platformSpawner.displayDiff.rightDif(3)) {
				obj = Spawn(new Vector2(PlatformSpawner.lastSpawnPosition.x + platformWidth + breakWidth, platformSpawner.thisHeight), PlatformType.breaksEnd);
				breakWidth = 0;
				//thisBreak = false;
				RunnerController.Instance.mapHeight = obj.transform.position.y;
				platformSpawner.OnBreackEnd();
			}
		}

		// Обычный сектор
		if((lastType == PlatformType.idle || lastType == PlatformType.breaksEnd) && !obj) {
			if(!platformSpawner.generateBreack)
				obj = Spawn(new Vector2(PlatformSpawner.lastSpawnPosition.x + platformWidth, platformSpawner.thisHeight));
			else {
				obj = Spawn(new Vector2(PlatformSpawner.lastSpawnPosition.x + platformWidth, platformSpawner.thisHeight), PlatformType.breaks);
				platformSpawner.generateBreack = false;
			}
		}

	}

	public GameObject Spawn(Vector2 pos, PlatformType platformType = PlatformType.none) {
		// Инкремент счетчика созданных объектов
		thisNumber++;

		PlatformMaterial needMath = materialList[BinarySearch.RandomNumberGenerator(materialCD)];

		string prefabName = "";

		switch(needMath) {
			case PlatformMaterial.stone:
				prefabName = platformSpawner.stonePlatformPrefab.name;
				break;
			case PlatformMaterial.log:
				prefabName = platformSpawner.logPlatformPrefab.name;
				break;
			case PlatformMaterial.wood:
				prefabName = platformSpawner.stonePlatformPrefab.name;
				break;
		}

		PlatformCategory needCat = categoryList[BinarySearch.RandomNumberGenerator(categoryCD)];

		if(platformType == PlatformType.none)
			platformType = typeList[BinarySearch.RandomNumberGenerator(typeCD)];

		if(platformType == PlatformType.bridge || platformType == PlatformType.bridgeIdle || platformType == PlatformType.breaks || platformType == PlatformType.breaksEnd)
			needCat = PlatformCategory.idle;


		lastType = platformType;
		GameObject obj = LevelPooler.Instance.GetPooledObject(PlatformSpawner.POOLER_KEY, prefabName,generateList);
		PlatformDecor platDecor = obj.GetComponent<PlatformDecor>();
		platDecor.Init(platformType, needCat);

		if(lastType == PlatformType.bridge)
			bridgeCount = Random.Range((int)platformSpawner.bridgeLenght.x, (int)platformSpawner.bridgeLenght.y);

		if(lastType == PlatformType.breaks) {
			obj.tag = "jumpUp";
			platDecor.boxCollider.enabled = true;
			if (platformSpawner.needsDistBreack > 0) {
				breakWidth = platformSpawner.needsDistBreack;
				platformSpawner.needsDistBreack = 0;
			} else
				breakWidth = Random.Range(platformSpawner.breackLenght.x, platformSpawner.breackLenght.y);

			// Изменение высоты платформы не допускается во время тутора
			if(RunnerController.Instance.runnerPhase != RunnerPhase.tutorial) {
				float chace = Random.value;
				if(chace > 0 && chace < 0.9 /*&& !spiderPetActive*/)
					platformSpawner.thisHeight = Random.Range(platformSpawner.HeightMin, platformSpawner.HeightMax);
			}
		}

		obj.transform.position = new Vector3(pos.x, pos.y, platformSpawner.transform.position.z);
		obj.SetActive(true);
		obj.GetComponent<PlatformDecor>().Inicialize();

		if (lastType != PlatformType.breaks) {
			if(lastType == PlatformType.breaksEnd) {
				breakWidth = 0;
				platDecor.boxCollider.enabled = true;
				obj.tag = "jumpDown";
			} else {
				platDecor.boxCollider.enabled = false;
				obj.tag = "Untagged";
			}
		}

		SpriteRenderer rend = obj.transform.Find("Graphic").GetComponent<SpriteRenderer>();

		if(obj.GetComponent<PlatformDecor>())
			obj.GetComponent<PlatformDecor>().countThis = thisNumber;

		if(rend != null) {
			rend.sortingLayerID = platformSpawner.layerId;
		} else {
			PlatformDecor decor = obj.GetComponent<PlatformDecor>();
			if(decor != null)
				decor.SetOrder(0, platformSpawner.layerId);
		}

		if(platformSpawner.region == RegionType.Forest) {
			platformSpawner.GenerateIsland(obj.transform.position);
		}

    PlatformSpawner.lastSpawnPosition = obj.transform.position;
		return obj;
	}

	void TypeProbability() {
		typeList.Clear();
		typeCD.Clear();

		float sum = 0;
		typeCD.Add(sum);

		sum += 1;
		typeCD.Add(sum);
		typeList.Add(PlatformType.idle);

		sum += 0.01f;
		typeCD.Add(sum);
		typeList.Add(PlatformType.bridge);

		typeCD = typeCD.ConvertAll(x => x / sum);

	}

	void CalcProbability(GenerateParametrs parametrs) {
		materialCD.Clear();
		materialList.Clear();
		categoryCD.Clear();
		categoryList.Clear();

		float sum1 = 0;
		materialCD.Add(sum1);

		foreach(PlatformMaterials mat in parametrs.material) {
			sum1 += mat.value;
			materialCD.Add(sum1);
			materialList.Add(mat.type);
		}

		float sum2 = 0;
		categoryCD.Add(sum2);

		foreach(PlatformCategories cat in parametrs.category) {
			sum2 += cat.value;
			categoryCD.Add(sum2);
			categoryList.Add(cat.type);
		}

		materialCD = materialCD.ConvertAll(x => x / sum1);
		categoryCD = categoryCD.ConvertAll(x => x / sum2);

	}

	/// <summary>
	/// Проверка возможности генерации
	/// </summary>
	/// <returns></returns>
	public bool CheckReadyGenerate() {
		Collider[] objCol = Physics.OverlapSphere(generatePosition, 6f);

		foreach(Collider oneColl in objCol) {
			if(oneColl.tag == "RollingStone" | oneColl.tag == "jumpUp" | oneColl.tag == "jumpDown")
				return false;
		}
		return true;
	}

	/// <summary>
	/// Бесконечный инкремент (на всякий случай)
	/// </summary>
	void InfinityIncrement() {
		GenerateParametrs param = new GenerateParametrs();
		param.count = generateParametrs[generateParametrs.Count - 1].count;
		param.distantion = generateParametrs[generateParametrs.Count - 1].distantion - generateParametrs[generateParametrs.Count - 2].distantion;
		param.material = new List<PlatformMaterials>(generateParametrs[generateParametrs.Count - 1].material);
		param.category = new List<PlatformCategories>(generateParametrs[generateParametrs.Count - 1].category);
		generateParametrs.Add(param);
	}
	/// <summary>
	/// Получение настроек
	/// </summary>
	public void GetConfig() {
		Configuration.Levels.Level configData = Config.Instance.config.activeLevel;
		List<Configuration.Levels.Road> itemsData = configData.road;
		List<Configuration.Levels.Decoration> decorData = configData.decoration;
		float platformMaterialSum = 1;

		generateParametrs.Clear();

		for(int i = 0; i < itemsData.Count; i++) {
			GenerateParametrs newParam = new GenerateParametrs();
			newParam.material = new List<PlatformMaterials>();
			newParam.category = new List<PlatformCategories>();
			platformMaterialSum = 1;

      newParam.distantion = decorData[i].distantion;
			if(decorData[i].decor > 0)
				newParam.count = decorData[i].decor;

			if(itemsData[i].stoneRoad > 0) {
				PlatformMaterials box = new PlatformMaterials();
				box.type = PlatformMaterial.stone;
				box.value = itemsData[i].stoneRoad * 0.01f;
				newParam.material.Add(box);
			}

			if(itemsData[i].woodBridge > 0) {
				PlatformMaterials box = new PlatformMaterials();
				box.type = PlatformMaterial.wood;
				box.value = itemsData[i].woodBridge * 0.01f;
				newParam.material.Add(box);
			}

			if(itemsData[i].logs > 0) {
				PlatformMaterials box = new PlatformMaterials();
				box.type = PlatformMaterial.log;
				box.value = itemsData[i].logs * 0.01f;
				newParam.material.Add(box);
			}

			if(decorData[i].roadDecor > 0) {
				PlatformCategories box = new PlatformCategories();
				box.type = PlatformCategory.decor;
				box.value = decorData[i].roadDecor * 0.01f;
				platformMaterialSum -= box.value;
				newParam.category.Add(box);
			}

			if(decorData[i].roadDestroy > 0) {
				PlatformCategories box = new PlatformCategories();
				box.type = PlatformCategory.destroy;
				box.value = decorData[i].roadDestroy * 0.01f;
				platformMaterialSum -= box.value;
				newParam.category.Add(box);
			}

			if(platformMaterialSum > 0) {
				PlatformCategories box = new PlatformCategories();
				box.type = PlatformCategory.idle;
				box.value = platformMaterialSum;
				newParam.category.Add(box);
			}

			generateParametrs.Add(newParam);
		}

	}

}
