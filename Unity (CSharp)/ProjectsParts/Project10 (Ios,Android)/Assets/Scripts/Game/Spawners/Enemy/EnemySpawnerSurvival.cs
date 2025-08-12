using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemySpawnerSurvival : IEnemySpawner {

	[System.Serializable]
	public struct EnemyLibraries {
		public EnemyTypes type;              // Дистанция генерации
		public int points;
	}

	[System.Serializable]
	public struct EnemyGeneraetParametrs {
		public float distance;              // Дистанция генерации
		public int balls;                   // Количество очков для покупки
		public int count;                   // Число на генерацию
		public List<EnemyGroup> probility;      // Натройки вероятности
	}

	[System.Serializable]
	public struct EnemyGroup {
		public EnemyTypes type;
		[Range(0,1)]
		public float probility;
	}

	public List<EnemyGeneraetParametrs> enemyParametrsGenerate;
	public List<EnemyLibraries> enemyLibrary;
	private float nextNormalGenerateDistance;                 // Следующая дистанция для генерации
	private EnemySpawner enemySpawner;
	private RunnerPhase runnerPhase;
	//private List<KeyValuePair<EnemyTypes,float>> generateListLevel = new List<KeyValuePair<EnemyTypes,float>>();
	private List<EnemyTypes> deadEnemyList = new List<EnemyTypes>();
	private System.Action GenerateFunc;
	public FloatSpan generateDiapoz;
	//private float nextGenerate;
	private float nextGenerateBlock;
	private List<EnemyTypes> generateList = new List<EnemyTypes>();
	private int fillEnemyPoint;
	private int countInGroup;
	private int thisNumInGroup;
	private float podBlockDistance;
	private float nextPodDistance;
	private int deadEnemyCount = 20;
	private int thisDeadEnemyCount = 0;
	private int deadShootCount = 10;
	private int thisDeadShootCount = 0;
	private float generateShot;
	private bool generateShotNow;
	private float generateDistantion;
	private bool generateDistNow;
	private float generateTime;                     // Время для генерации
	private bool firstBoost = true;                 // Влаг первой генерации
	private List<EnemyTypes> enemyListPet = new List<EnemyTypes>();

	public void Init(EnemySpawner enemySpawner) {
		this.enemySpawner = enemySpawner;
		GetConfig();
		CreatePool();
		GenerateFunc = RunGenerate;
		nextNormalGenerateDistance = enemyParametrsGenerate[0].distance;

		RunnerController.OnChangeRunnerPhase += ChangePhase;
		RunnerController.petsChange += PetsChange;
		//RunSpawner.OnSpecialBarrier += OnSpecialBarrier;
		//RunSpawner.OnHandingPlatform += OnHandingPlatform;
		GameManager.OnChangeLevel += GetConfig;

	}

	void CreatePool() {

		List<EnemyTypes> poolList = new List<EnemyTypes>();
		enemyLibrary.ForEach(x => poolList.Add(x.type));
		enemySpawner.CreatePoolSpawn(poolList);
	}

	public void DeInit() {

		RunnerController.OnChangeRunnerPhase -= ChangePhase;
		RunnerController.petsChange -= PetsChange;
		//RunSpawner.OnSpecialBarrier -= OnSpecialBarrier;
		//RunSpawner.OnHandingPlatform -= OnHandingPlatform;
		GameManager.OnChangeLevel -= GetConfig;
	}

	public void ChangePhase(RunnerPhase runnerPhase) {

		if (this.runnerPhase == RunnerPhase.dead && runnerPhase == RunnerPhase.run) {
			GeneralEnemy[] enemyes = enemySpawner.GetComponentsInChildren<GeneralEnemy>();
			foreach (GeneralEnemy enemy in enemyes) {
				enemy.Damage(WeaponTypes.none, 1000, Vector3.zero, DamagePowen.level2);
			}
			if (deadCloudInstance != null) {
				MonoBehaviour.Destroy(deadCloudInstance);
			}
		}

		// Небольная уловка, после буста пересчитываем начальное значение время, как будто это пробежали на минимальной скорости
		if (this.runnerPhase == RunnerPhase.postBoost && runnerPhase == RunnerPhase.run) {
			float thisDistance = RunnerController.playerDistantion;

			// После буста сразу отбрасываем не сгенерированные группы
			for (int i = 0; i < enemyParametrsGenerate.Count; i++) {
				if (enemyParametrsGenerate[i].distance <= RunnerController.playerDistantion) {
					nextNormalGenerateDistance = enemyParametrsGenerate[i].distance;
				}

				//if(nextNormalGenerateDistance == enemyParametrsGenerate[i].distance && enemyParametrsGenerate[i].distance <= RunnerController.playerDistantion && i < enemyParametrsGenerate.Length - 1) {
				//  enemyParametrsGenerate[i].balls = (int)(enemyParametrsGenerate[i].balls * ((enemyParametrsGenerate[i + 1].distance - RunnerController.playerDistantion) / (enemyParametrsGenerate[i + 1].distance - enemyParametrsGenerate[i].distance)));
				//}
			}
		}

		// Определяем время начала генерации врагов
		if (runnerPhase == RunnerPhase.run) {
			GenerateFunc = RunGenerate;

			if (deadCloudInstance != null) {
				deadCloudInstance.GetComponent<DeadCloudController>().NoPlayer();
				deadCloudInstance = null;
			}
			//nextGenerate = Time.time + Random.Range(generateDiapoz.min, generateDiapoz.max);
		}

		if (runnerPhase == RunnerPhase.boost) {
			GenerateFunc = BoostGenerate;
		}

		if ((this.runnerPhase & (RunnerPhase.dead | RunnerPhase.lowEnergy)) == 0 && (runnerPhase & (RunnerPhase.dead | RunnerPhase.lowEnergy)) != 0 ) {
			GenerateFunc = DeadGenerate;
			deadEnemyList.Clear();
			GeneralEnemy[] enemyes = enemySpawner.GetComponentsInChildren<GeneralEnemy>();

			foreach (GeneralEnemy enemy in enemyes) {
				deadEnemyList.Add(enemy.enemyType);
			}
		}

		this.runnerPhase = runnerPhase;
	}

	public void Update() {
		GenerateFunc();

		if (runnerPhase == RunnerPhase.run) {
			if (isDinoPet) PetsCheck();
		}

	}

	public void DeadCloud() {
		if (deadCloudInstance == null)
			deadCloudInstance = MonoBehaviour.Instantiate(enemySpawner.deadCloud, Vector3.zero, Quaternion.identity) as GameObject;
	}

	void RunGenerate() {
		if (nextNormalGenerateDistance <= RunnerController.playerDistantion) {
			EnemyGeneraetParametrs enemGenerate = enemyParametrsGenerate.Find(x=>x.distance <= RunnerController.playerDistantion && x.distance == nextNormalGenerateDistance);
			int index = enemyParametrsGenerate.FindIndex(x=>x.distance == nextNormalGenerateDistance);
			if (enemGenerate.distance == enemyParametrsGenerate[enemyParametrsGenerate.Count - 1].distance)
				InfinityIncrement();
			else
				nextNormalGenerateDistance = enemyParametrsGenerate[index + 1].distance;

			CalcGenerateGroup(enemGenerate, index);
		}

		if (generateList.Count > 0 && nextGenerateBlock <= Time.time && nextPodDistance < RunnerController.playerDistantion) {
			GenerateFromGroup(ref generateList);
		}
		if (deadEnemyList.Count > 0 && nextGenerateBlock <= Time.time) {
			GenerateFromGroup(ref deadEnemyList);
		}
		if (isDinoPet && lastDistanceEnemyGenerateForPet <= RunnerController.playerDistantion - 3) {
			if (enemyListPet.Count > 0 && GenerateFromGroup(ref enemyListPet))
				lastDistanceEnemyGenerateForPet = RunnerController.playerDistantion;

			CalcNewEnemyesPet();
		}




		//if(nextNormalGenerateDistance <= RunnerController.playerDistantion) {
		//	for(int i = 0; i < enemyParametrsGenerate.Length; i++) {
		//		if(nextNormalGenerateDistance == enemyParametrsGenerate[i].distance && enemyParametrsGenerate[i].distance <= RunnerController.playerDistantion) {

		//			if(i == enemyParametrsGenerate.Length - 1) InfinityIncrement();

		//			if(i != enemyParametrsGenerate.Length - 1) {
		//				nextNormalGenerateDistance = enemyParametrsGenerate[i + 1].distance;
		//			}

		//			CalcGenerateGroup(enemyParametrsGenerate[i], i);
		//			enemyInWawe.Clear();
		//		}
		//	}
		//}

		//if(generateList.Count > 0 && nextGenerateBlock <= Time.time && nextPodDistance < RunnerController.playerDistantion) {
		//	GenerateFirstEnemy(ref generateList);
		//}
		//if(deadEnemyList.Count > 0 && nextGenerateBlock <= Time.time) {
		//	GenerateFirstEnemy(ref deadEnemyList);
		//}
		//if(dinoPetGenerate && lastDistanceEnemyGenerateForPet <= RunnerController.playerDistantion - 3) {
		//	if(enemyListPet.Count > 0 && GenerateFirstEnemy(ref enemyListPet))
		//		lastDistanceEnemyGenerateForPet = RunnerController.playerDistantion;

		//	CalcNewEnemyesPet();
		//}




	}


	private void CalcGenerateGroup(EnemyGeneraetParametrs calcGroup, int numGroup, bool usePoints = true) {

		List<float> tempCd = new List<float>();

		float sum1 = 0;
		tempCd.Add(sum1);

		foreach (EnemyGroup enemtOne in calcGroup.probility) {
			sum1 += enemtOne.probility;
			tempCd.Add(sum1);
		}

		tempCd = tempCd.ConvertAll(x => x / sum1);

		int points = calcGroup.balls - (int)(( enemySpawner.enemyClothes.head ? calcGroup.balls * 0.1f : 0 )
																					+ ( enemySpawner.enemyClothes.spine ? calcGroup.balls * 0.1f : 0 )
																					+ ( enemySpawner.enemyClothes.accessory ? calcGroup.balls * 0.1f : 0 ));

		if (fillEnemyPoint > 0) {
			points -= fillEnemyPoint;
			fillEnemyPoint = 0;
		}

		while (points > 0) {
			int num = BinarySearch.RandomNumberGenerator(tempCd);

			EnemyLibraries lib = enemyLibrary.Find(x=>x.type == calcGroup.probility[num].type);
			generateList.Add(lib.type);
			points -= lib.points;
		}

		countInGroup = Mathf.CeilToInt(generateList.Count / 3f);
		thisNumInGroup = 0;
		podBlockDistance = (enemyParametrsGenerate[numGroup + 1].distance - calcGroup.distance) / 3;


	}

	int GetEnemyPoint(EnemyTypes typeEnemy) {
		try {
			return enemyLibrary.Find(x => x.type == typeEnemy).points;
		} catch {
			return 0;
		}
	}

	/// <summary>
	/// Генарция врагов при обычном забеге
	/// </summary>
	/// <param name="enemyType">Тип сгенерированного врага</param>
	/// <returns></returns>
	GameObject GenerateOneEnemy(EnemyTypes enemyType) {

		if (enemyType == EnemyTypes.none) return null;

		GameObject obj = enemySpawner.GetInstantEnemy(enemyType);

		if (enemySpawner.moneyClothes.full && obj.GetComponent<ClassicEnemy>())
			obj.GetComponent<ClassicEnemy>().deadCoins = GetEnemyPoint(enemyType);

		if (enemyType == EnemyTypes.aztecForward && !enemySpawner.enemyClothes.full)
			obj.transform.position = new Vector3(CameraController.displayDiff.transform.position.x + CameraController.displayDiff.right * 1.45f, RunnerController.Instance.mapHeight + 1f, 0);
		else
			obj.transform.position = new Vector3(CameraController.displayDiff.transform.position.x + CameraController.displayDiff.left * 1.3f, RunnerController.Instance.mapHeight + 1f, 0);

		obj.transform.parent = enemySpawner.transform;
		obj.GetComponent<GeneralEnemy>().ChangePhase(runnerPhase);
		obj.SetActive(true);

		return obj;
	}

	private GameObject GenerateFromGroup(ref List<KeyValuePair<EnemyTypes, float>> enemyList) {

		if (CheckGenerateConfirm()) {
			GameObject obj =  GenerateOneEnemy(enemyList[0].Key);
			enemyList.RemoveAt(0);
			nextGenerateBlock = Time.time + Random.Range(1f, 2f);

			//if(enemyList.Count > 0)
			//	nextGenerate = enemyList[0].Value;
			//else
			//	nextGenerate = -1;
			return obj;
		} else {
			nextGenerateBlock = Time.time + 1;
		}
		return null;
	}

	private GameObject GenerateFromGroup(ref List<EnemyTypes> enemyList) {

		if (CheckGenerateConfirm()) {
			//int[] tempArr = enemyList.ToArray();
			GameObject obj = GenerateOneEnemy(enemyList[0]);
			enemyList.RemoveAt(0);
			nextGenerateBlock = Time.time + Random.Range(1f, 2f);
			thisNumInGroup++;
			if (countInGroup == thisNumInGroup) {
				thisNumInGroup = 0;
				nextPodDistance = podBlockDistance + RunnerController.playerDistantion;
				if (nextPodDistance > nextNormalGenerateDistance) nextPodDistance = nextNormalGenerateDistance;
			}
			return obj;
		} else {

			nextGenerateBlock = Time.time + 1;
			return null;
		}
	}


	/// <summary>
	/// Бесконечный инкремент параметров
	/// </summary>
	void InfinityIncrement() {

		EnemyGeneraetParametrs tmp = new EnemyGeneraetParametrs();
		tmp.probility = enemyParametrsGenerate[enemyParametrsGenerate.Count - 1].probility;
		tmp.balls = enemyParametrsGenerate[enemyParametrsGenerate.Count - 1].balls;
		tmp.distance = enemyParametrsGenerate[enemyParametrsGenerate.Count - 1].distance + 500;
		tmp.count = enemyParametrsGenerate[enemyParametrsGenerate.Count - 1].count;
		enemyParametrsGenerate.Add(tmp);
	}

	/// <summary>
	/// Генерация врагов при бусте
	/// </summary>
	void BoostGenerate() {
		if (!generateDistNow) {
			if (firstBoost) {
				generateTime = Time.time + 0.8f;
				firstBoost = false;
			} else {
				generateTime = Time.time + Random.Range(0.4f, 0.5f);
			}

			generateDistNow = true;
		}

		if (generateDistNow && generateTime <= Time.time) {
			int iii = Random.Range(0, 3);
			GameObject obj = enemySpawner.GetInstantEnemy(enemyLibrary[iii].type);

			if (enemySpawner.moneyClothes.full && obj.GetComponent<ClassicEnemy>())
				obj.GetComponent<ClassicEnemy>().deadCoins = GetEnemyPoint(enemyLibrary[iii].type);

			obj.transform.position = new Vector3(CameraController.displayDiff.transform.position.x + CameraController.displayDiff.right * 1.3f, RunnerController.Instance.mapHeight + 0.1f, 0);
			obj.transform.parent = enemySpawner.transform;
			obj.GetComponent<GeneralEnemy>().ChangePhase(runnerPhase);
			generateDistNow = false;
			obj.SetActive(true);
		}
	}

	/// <summary>
	/// Генерация врагов при сметри игрока
	/// </summary>
	void DeadGenerate() {
		if (RunnerController.Instance.runSpeedActual >= 4) return;

		if (!generateDistNow && thisDeadEnemyCount < deadEnemyCount) {
			generateShot = Time.time + Random.Range(0.1f, 0.2f);
			generateDistNow = true;
		}

		if (generateDistNow && generateShot <= Time.time) {
			GameObject obj = enemySpawner.GetInstantEnemy(enemyLibrary[Random.Range(0 , 4)].type);
			obj.transform.position = new Vector3(CameraController.displayDiff.transform.position.x + CameraController.displayDiff.left * 1.3f, RunnerController.Instance.mapHeight + 1f, 0);
			obj.transform.parent = enemySpawner.transform;
			obj.GetComponent<GeneralEnemy>().ChangePhase(runnerPhase);
			obj.SetActive(true);
			generateDistNow = false;
			thisDeadEnemyCount++;
		}

		//if (!generateShotNow && thisDeadShootCount < deadShootCount) {
		//	generateDistantion = Time.time + Random.Range(0.1f, 0.2f);
		//	generateShotNow = true;
		//}

		//if (generateShotNow && generateDistantion <= Time.time) {
		//	Vector3 newPosition = new Vector3(CameraController.displayDiff.transform.position.x + CameraController.displayDiff.left * 1.3f, RunnerController.Instance.thisMapHeight + Random.Range(2f, 4f), 0);
		//	GameObject spear = Pooler.GetPooledObject("EnemySpear");
		//	spear.transform.position = newPosition;
		//	spear.SetActive(true);

		//	//obj.GetComponent<Shoot>().disableAudio = true;
		//	generateShotNow = false;
		//	thisDeadShootCount++;
		//}
	}

	public bool CheckGenerateConfirm() {
		Collider[] checkColliderInGeneratePos = Physics.OverlapSphere(new Vector3(CameraController.displayDiff.transform.position.x + CameraController.displayDiff.left * 1.3f, RunnerController.Instance.mapHeight + 1f, 0), 4f);
		foreach (Collider oneColl in checkColliderInGeneratePos) {
			if (oneColl.tag == "Barrier" | oneColl.tag == "jumpUp" | oneColl.tag == "jumpDown")
				return false;
		}
		return true;
	}


	#region Работа при петах

	private bool isDinoPet;
	private float petsTime;
	private float lastDistanceEnemyGenerateForPet;
	void PetsChange(Player.Jack.PetsTypes pet, bool useFlag, float timeUse = 0) {
		if (!useFlag) isDinoPet = false;

		if (pet == Player.Jack.PetsTypes.dino && useFlag) {
			enemyListPet = new List<EnemyTypes>();
			isDinoPet = true;
			petsTime = Time.time + timeUse - 3;
		}

	}

	void PetsCheck() {
		if (isDinoPet && petsTime <= Time.time) isDinoPet = false;
	}

	void CalcNewEnemyesPet() {
		if (!isDinoPet || petsTime <= Time.time) return;

		if (lastDistanceEnemyGenerateForPet <= RunnerController.playerDistantion - 10) {
			if (Random.value <= 0.3f) {
				int count = Random.Range(1, 3);
				for (int i = 0; i < count; i++)
					enemyListPet.Add(EnemyTypes.aztecForward);
			} else
				lastDistanceEnemyGenerateForPet = RunnerController.playerDistantion;
		}
	}

	#endregion

	#region Необходимость остановки врагов

	List<string> stopedList = new List<string>();

	/// <summary>
	/// Статус остановки врагов
	/// </summary>
	public bool enemyStop { get { return stopedList.Count > 0; } }



	#endregion

	#region Реакция на события специального барьера

	public void OnSpecialBarrier(ExEvent.RunEvents.SpecialBarrier specialBarrier) {

		if (specialBarrier.barrier != SpecialBarriersTypes.stickyFlor) {
			if (specialBarrier.isActivate) {
				if (!stopedList.Contains("specialBarrier"))
					stopedList.Add("specialBarrier");
			} else {
				stopedList.Remove("specialBarrier");
			}
		}
		enemySpawner.OnStopEnemy(enemyStop);
	}

	#endregion

	#region Навесные платформы

	//void OnHandingPlatform(bool isActivate, int type, RunSpawner.SpecialBarrierEnd calback = null) {

	//	if (isActivate && (type == 3 || type == 5 || type == 4)) {
	//		if (!stopedList.Contains("handingPlatform"))
	//			stopedList.Add("handingPlatform");
	//		else {
	//			stopedList.Remove("handingPlatform");
	//		}
	//	}
	//	enemySpawner.OnStopEnemy(enemyStop);
	//}

	public void OnSpecialPlatform(ExEvent.RunEvents.SpecialPlatform specialPlatform) {

		if (specialPlatform.isActivate && (specialPlatform.typ == 3 || specialPlatform.typ == 4 || specialPlatform.typ == 5)) {
			if (!stopedList.Contains("handingPlatform"))
				stopedList.Add("handingPlatform");
			else {
				stopedList.Remove("handingPlatform");
			}
		}
		enemySpawner.OnStopEnemy(enemyStop);
	}

	#endregion

	#region Boss

	void OnBossEnemy(bool isBoss) {

		if (isBoss) {
			if (!stopedList.Contains("boss"))
				stopedList.Add("boss");
		} else {
			stopedList.Remove("boss");
		}
		enemySpawner.OnStopEnemy(enemyStop);
	}

	#endregion

	#region Облако смерти

	private GameObject deadCloudInstance;

	/// <summary>
	/// Создание облака смерти
	/// </summary>
	public void CreateDeadCloud() {
		if (deadCloudInstance != null) {
			deadCloudInstance = MonoBehaviour.Instantiate(enemySpawner.deadCloud, Vector3.zero, Quaternion.identity) as GameObject;
		}
	}

	#endregion

	public void GetConfig() {
		List<Configuration.Survivles.EnemyPrice> itemsData = Config.Instance.config.survival.enemyPrice;
		int startCol = 1;
		float distance;
		bool isEnd = false;

		List<EnemyGeneraetParametrs> enemyGeneraetParametrsTmp  = new List<EnemyGeneraetParametrs>();

		for (int i = 0; i < itemsData.Count; i++) {
			if (i < startCol || isEnd) continue;
      
			EnemyGeneraetParametrs oneEnemy = new EnemyGeneraetParametrs();

			try {
				distance = itemsData[i].dist;
			} catch {
				distance = enemyGeneraetParametrsTmp[enemyGeneraetParametrsTmp.Count - 1].distance + 200;
				isEnd = true;
			}

			oneEnemy.distance = distance;
			oneEnemy.balls = itemsData[i].balls;

			List<EnemyGroup> enemyGroup = new List<EnemyGroup>();

			if (itemsData[i].azteck > 0) {
				EnemyGroup oneEnemyGr = new EnemyGroup();
				oneEnemyGr.type = EnemyTypes.aztec;
				oneEnemyGr.probility = itemsData[i].azteck * 0.01f;
				enemyGroup.Add(oneEnemyGr);
			}
			if (itemsData[i].azteckForward > 0) {
				EnemyGroup oneEnemyGr = new EnemyGroup();
				oneEnemyGr.type = EnemyTypes.aztecForward;
				oneEnemyGr.probility = itemsData[i].azteckForward * 0.01f;
				enemyGroup.Add(oneEnemyGr);
			}
			if (itemsData[i].boomerang > 0) {
				EnemyGroup oneEnemyGr = new EnemyGroup();
				oneEnemyGr.type = EnemyTypes.warriorBoomerang;
				oneEnemyGr.probility = itemsData[i].boomerang * 0.01f;
				enemyGroup.Add(oneEnemyGr);
			}
			if (itemsData[i].empty > 0) {
				EnemyGroup oneEnemyGr = new EnemyGroup();
				oneEnemyGr.type = EnemyTypes.none;
				oneEnemyGr.probility = itemsData[i].empty * 0.01f;
				enemyGroup.Add(oneEnemyGr);
			}
			if (itemsData[i].fatAzteck > 0) {
				EnemyGroup oneEnemyGr = new EnemyGroup();
				oneEnemyGr.type = EnemyTypes.fatZombie;
				oneEnemyGr.probility = itemsData[i].fatAzteck * 0.01f;
				enemyGroup.Add(oneEnemyGr);
			}
			if (itemsData[i].gigant > 0) {
				EnemyGroup oneEnemyGr = new EnemyGroup();
				oneEnemyGr.type = EnemyTypes.warriorGiant;
				oneEnemyGr.probility = itemsData[i].gigant * 0.01f;
				enemyGroup.Add(oneEnemyGr);
			}
			if (itemsData[i].handingAzteck > 0) {
				EnemyGroup oneEnemyGr = new EnemyGroup();
				oneEnemyGr.type = EnemyTypes.headlessZombie;
				oneEnemyGr.probility = itemsData[i].handingAzteck * 0.01f;
				enemyGroup.Add(oneEnemyGr);
			}
			if (itemsData[i].spearAzteck > 0) {
				EnemyGroup oneEnemyGr = new EnemyGroup();
				oneEnemyGr.type = EnemyTypes.aztecSpear;
				oneEnemyGr.probility = itemsData[i].spearAzteck * 0.01f;
				enemyGroup.Add(oneEnemyGr);
			}
			oneEnemy.probility = enemyGroup;
			enemyGeneraetParametrsTmp.Add(oneEnemy);
		}
		enemyParametrsGenerate = enemyGeneraetParametrsTmp;
	}

}
