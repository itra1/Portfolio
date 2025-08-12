using UnityEngine;
using System;
using System.Collections.Generic;
using ExEvent;
using Game.User;

/// <summary>
/// Типы зомби
/// </summary>
public enum EnemyType {
	None = 0,
	Tusila = 1,             // Шаман
	HipStar = 2,            // Хипстер
	Mazila = 3,             // Имперец шутер
	Makdaun = 4,            // Жирдяй
	BorodatoePevico = 5,    // Певица
	Hunweibin = 6,          // Красный партизан
	Ekstrimalchik = 7,      // Скейтер
	Metrosek = 8,           // Модник
	YaLegenda = 9,          // Черный властилин
	Shustrik = 10,          // Шустрик
	MegaNedr = 11,          // Гик призыватель
	Cheburator = 12,        // Чебуратор
	Sterva = 13,            // Ворон
	Bat = 14,               // Летучая мышь
	Amurka = 15,            // Невеста Джека
	Fanatic = 16,           // Фанатик
	DremuchiyRisovod = 17,  // Красный крестьянин
	Diversant = 18,         // Красный базовый азиат
	Spaceman = 19,          // Космонавт
	MadamSufrajo = 20,      // Катапульта
	VoinPustini = 21,       // Воин пустыни
	Imperec = 22,           // Имперец рукопашный
	DiskoZorb = 23          // Диско шар
}

/// <summary>
/// Структура генерации
/// </summary>
[System.Serializable]
public struct EnemyStruct {
	public EnemyType type;                                  // Тип врага
	public GameObject prefab;                               // Объект врага
	public int cacheSize;                                   // Количество для кеша
}

public enum EnemyTypeWave {
	flood,
	wave,
	boss
}
[System.Serializable]
public struct SurvivalEnemy {
	public EnemyTypeWave mode;
	public EnemyType type;
	public float timeGenerate;
}

/// <summary>
/// Параметры генерации
/// </summary>
[System.Serializable]
public struct EnemyGenerateParametrs {
	public EnemyType type;
	public float timeGenerate;
	public int level;               // Уровень прокачки персонажа
	public bool isGenerate;
	public int count;
	public bool keyEnemy;
}
/// <summary>
/// Генератор врагов
/// </summary>
[RequireComponent(typeof(Pool))]
public class EnemysSpawn : Singleton<EnemysSpawn> {

	public static event Action<bool> OnBaffSpeed;
	public static event Action<GameObject> OnSpawnEnemy;

	public EnemyGenerateParametrs[] enemyGenerateParametrs;

	public string enemyPrefabsList;

	public CharacterSize characterSize;

	public List<GameObject> enemyPrefabs = new List<GameObject>();
    public AudioGroup bossAppearAudioGroup;
    public AudioSource audioSource;
	private Pool pool;

	private int _enemyInBattle;

	private int enemyInBattle {
		get { return _enemyInBattle; }
		set {
			_enemyInBattle = value;
		}
	}

	public bool isGenerate;

	public float timeEndGenerate;

	float maxLeftPosition;
	public static float maxLeftPositionX {
		get {
			return Instance.maxLeftPosition + 1;
		}
	}

    private Vector3 _bossPosition;

	protected override void Awake() {
		base.Awake();
		pool = GetComponent<Pool>();
		enemyPrefabs.Clear();
	}

    protected void Start() {
        _bossPosition = new Vector3(CameraController.rightPoint.x + 1,
                                    DecorationManager.Instance.loaderLocation.roadSize.min +
                                    (DecorationManager.Instance.loaderLocation.roadSize.max -
                                    DecorationManager.Instance.loaderLocation.roadSize.min) * .5f, 0);
    }

    protected override void OnDestroy() {
		base.OnDestroy();
		pool.DestroyAll();
		enemyPrefabs.Clear();
	}


	[ExEvent.ExEventHandler(typeof(BattleEvents.StartBattle))]
	void BattleStart(BattleEvents.StartBattle startEvent) {
    
		enemyInBattle = 0;
		GetConfig();

		StructProcessor();
		pool.CreatePool(enemyList, transform);
		if (UserManager.Instance.ActiveBattleInfo.Mode != PointMode.survival && UserManager.Instance.ActiveBattleInfo.Mode != PointMode.arena)
			timeEndGenerate = enemyGenerateParametrs[enemyGenerateParametrs.Length - 1].timeGenerate;

		UseBaffSpead(false);
		FindKeyEnemy();
	}

	void RestartWave() {
		try {
			for (int i = 0; i < enemyGenerateParametrs.Length; i++) {
				enemyGenerateParametrs[i].isGenerate = false;
				enemyGenerateParametrs[i].count = 1;
			}
			timeEndGenerate = enemyGenerateParametrs[enemyGenerateParametrs.Length - 1].timeGenerate;
			if (UserManager.Instance.ActiveBattleInfo.Mode == PointMode.farm)
				CalcFarm();
		} catch {

		}
	}

	void OnEnable() {
		//GetConfig();

		if (PlayerController.Instance != null)
			maxLeftPosition = PlayerController.Instance.transform.position.x;
		else
			maxLeftPosition = -9.52f;
	}

	void Update() {

		if (BattleManager.battlePhase != BattlePhase.battle)
			return;

		BattleManager.Instance.speedIncrement = enemyInBattle <= 0 ? 5 : 1;

		//timeProgress += Time.deltaTime;
		if (isBaffSpeed && timeStopBaffSpead < Time.time)
			UseBaffSpead(false);

		if (isGenerate) {

			switch (UserManager.Instance.ActiveBattleInfo.Mode) {
				case PointMode.survival:
					GenerateSurvival();
					break;
				case PointMode.appendix:
				case PointMode.farm:
				case PointMode.company:
					GenerateEnemyTimerLevel();
					break;
				case PointMode.arena:
					if (arenaGeenrateWave)
						GenerateEnemyTimerLevel();
					break;
			}

			//if(User.instance.activeBattleInfo.mode == PointMode.survival) {
			//	GenerateSurvival();
			//} else
			//	GenerateEnemyTimerLevel();
		}

	}

	bool isBaffSpeed;
	float timeStopBaffSpead;
	public void UseBaffSpead(bool flag) {
		isBaffSpeed = flag;
		if (isBaffSpeed)
			timeStopBaffSpead = Time.time + 3f;
		else {
			timeStopBaffSpead = Time.time;
		}
		if (OnBaffSpeed != null)
			OnBaffSpeed(isBaffSpeed);
	}

	List<KeyValuePair<float, float>> generateYposition = new List<KeyValuePair<float, float>>();

	bool arenaGeenrateWave;

	public void GenerateEnemyWave(int group, int level) {
		UserManager.Instance.ActiveBattleInfo.Group = group;
		UserManager.Instance.ActiveBattleInfo.Level = level;
		GetConfig();
		BattleManager.Instance.NewWawe();
		GetConfigLevelWave();
		arenaGeenrateWave = true;
	}

	void FindKeyEnemy() {
		//BattleManager.Instance.timeBattle
		for (int i = 0; i < enemyGenerateParametrs.Length; i++) {
			if (enemyGenerateParametrs[i].keyEnemy && !enemyGenerateParametrs[i].isGenerate) {
				ExEvent.BattleEvents.ChangeKeyEnemy.Call(true, enemyGenerateParametrs[i].type);
				return;
			}
		}
		ExEvent.BattleEvents.ChangeKeyEnemy.Call(false, EnemyType.None);
	}

	void GenerateKeyEnemy(Enemy enemy) {
		ExEvent.BattleEvents.GenerateKeyEnemy.Call(enemy);
        bossAppearAudioGroup.Play(audioSource, 0);
	}

	public void DeactiveAllEnemy(bool clearWawe = false, bool reload = false) {
		Enemy[] enemyList = transform.GetComponentsInChildren<Enemy>();
		foreach (Enemy one in enemyList) {
			one.OnDead -= EnemyDead;
			one.gameObject.SetActive(false);
		}
		enemyInBattle = 0;
		if (clearWawe) {
			arenaGeenrateWave = false;
			enemyGenerateParametrs = new EnemyGenerateParametrs[0];
		}
		if (reload)
			RestartWave();

	}


	#region Генерация врага
	public void GenerateEnemyTimerLevel() {
		for (int i = 0; i < enemyGenerateParametrs.Length; i++) {
			if (!enemyGenerateParametrs[i].isGenerate && enemyGenerateParametrs[i].timeGenerate < BattleManager.Instance.timeBattle) {

				bool enemyGenerate = false;

				foreach (GameObject one in enemyPrefabs) {
					if (one.GetComponent<Enemy>().enemyType == enemyGenerateParametrs[i].type) {
						enemyGenerate = true;
						GameObject instant = GenerateEnemyBuName(one.name, enemyGenerateParametrs[i].keyEnemy == false ? Vector3.zero : _bossPosition);

						instant.GetComponent<Enemy>().SetLevel(enemyGenerateParametrs[i].level);

						enemyGenerateParametrs[i].count--;

						if (enemyGenerateParametrs[i].count == 0)
							enemyGenerateParametrs[i].isGenerate = true;

						if (enemyGenerateParametrs[i].keyEnemy) GenerateKeyEnemy(instant.GetComponent<Enemy>());
					}
				}

				if (!enemyGenerate) {
					enemyGenerateParametrs[i].isGenerate = true;

					if (enemyGenerateParametrs[enemyGenerateParametrs.Length - 1].timeGenerate < BattleManager.Instance.timeBattle) {
						BattleEventEffects.Instance.VisualEffect(BattleEffectsType.lastEnemy, Vector3.zero);
						if (enemyInBattle <= 0)
							BattleManager.Instance.ZombyComplited();
					}
				}

				//if(enemyGenerateParametrs[i].keyEnemy) FindKeyEnemy();

			}
			if (enemyGenerateParametrs[i].timeGenerate > BattleManager.Instance.timeBattle)
				return;
		}
	}

	public void GenerateEnemyByNum(int num) {
		string prefabName = enemyPrefabs.Find(x => x.GetComponent<Enemy>().enemyType == (EnemyType)num).name;
		GenerateEnemyBuName(prefabName, Vector3.zero);
	}

	public GameObject GenerateEnemyBuName(string prefabName, Vector3 targetPosition) {
		GameObject instant = GetEnemy(prefabName);

		Enemy enemyComp = instant.GetComponent<Enemy>();

		Vector3 genPosition = targetPosition;

		if (genPosition == Vector3.zero) {
			genPosition = new Vector3(CameraController.rightPoint.x + 1,
																												UnityEngine.Random.Range(DecorationManager.Instance.loaderLocation.roadSize.min, DecorationManager.Instance.loaderLocation.roadSize.max), 0);

			if (generateYposition.Count > 0) {
				int repear = 0;
				while (repear < 10 && generateYposition.Exists(x => x.Key >= Time.time - 0.1f && x.Value > genPosition.y - 0.25f && x.Value < genPosition.y + 0.25f)) {
					repear++;
					genPosition = new Vector3(CameraController.rightPoint.x + 1,
																									UnityEngine.Random.Range(DecorationManager.Instance.loaderLocation.roadSize.min, DecorationManager.Instance.loaderLocation.roadSize.max), 0);
				}
			}
		}



		generateYposition.Add(new KeyValuePair<float, float>(Time.time, genPosition.y));

		instant.transform.position = genPosition;
		if (isBaffSpeed)
			enemyComp.OnBaffSpead(isBaffSpeed);
		enemyComp.SetSpriteOrder((int)(System.Math.Round(instant.transform.position.y, 3) * -1000));
		//if (enemyComp.skeletonAnimation != null)
		//	enemyComp.skeletonAnimation.GetComponent<MeshRenderer>().sortingOrder = (int)(System.Math.Round(instant.transform.position.y, 3) * -1000);

		enemyComp.Init();

		instant.SetActive(true);
		if (OnSpawnEnemy != null) OnSpawnEnemy(instant);
		return instant;
	}

	public GameObject coinsPrefab;

	public void EnemyDead(Enemy enemy) {

		BattleEventEffects.Instance.VisualEffect(BattleEffectsType.killEnemy, enemy.transform.position, enemy);

		if (Array.Exists(new EnemyType[] { EnemyType.Tusila, EnemyType.HipStar, EnemyType.Metrosek }, x => x == enemy.enemyType) && UnityEngine.Random.value < 0.14f) {
			BattleEventEffects.Instance.VisualEffect(BattleEffectsType.killTusModHip, enemy.transform.position, this);
		}

		if (Array.Exists(new EnemyType[] { EnemyType.BorodatoePevico, EnemyType.YaLegenda, EnemyType.MegaNedr }, x => x == enemy.enemyType)) {
			BattleEventEffects.Instance.VisualEffect(BattleEffectsType.bossKill, transform.position, this);
		}

		if (UserManager.Instance.ActiveBattleInfo.Mode == PointMode.survival || UserManager.Instance.ActiveBattleInfo.Mode == PointMode.arena) return;

		enemyInBattle--;

		GetDrop(enemy);
		if (enemyGenerateParametrs.Length > 0 && enemyGenerateParametrs[enemyGenerateParametrs.Length - 1].timeGenerate < BattleManager.Instance.timeBattle && enemyInBattle <= 0) {

			if (arenaGeenrateWave || UserManager.Instance.ActiveBattleInfo.Mode == PointMode.arena) {
				arenaGeenrateWave = false;
			} else {
				BattleEventEffects.Instance.VisualEffect(BattleEffectsType.lastEnemy, transform.position, this);
				BattleManager.Instance.ZombyComplited();
			}

		}
	}

	public void EnemyDamage(Enemy enemy, GameObject damager, float damageValue) {
		SpawnCoins(enemy, damageValue, enemy.transform.position.x - damager.transform.position.x);
	}


	#region Дроп с врагов

	int coinsPrice = 3;       // стоимость одной монетки

	public int CalcSilverCoins(Enemy enemy, float damageValue) {
		float maxLive = enemy.startLive;
		float liveNow = enemy.liveNow;
		int oldCoins = (int)(maxLive - (liveNow + damageValue)) / coinsPrice;
		int coinsNow = (int)(maxLive - (liveNow)) / coinsPrice;
		int coinsNeed = coinsNow - oldCoins;

		if (liveNow < coinsPrice && damageValue > 0) {
			if (maxLive % coinsPrice > 0)
				coinsNeed++;
		}
		return coinsNeed;
	}

	void SpawnCoins(Enemy enemy, float damageValue, float delta) {
		int coinsNeed = CalcSilverCoins(enemy, damageValue);
		if (coinsNeed > 0)
			GenerateCoins(enemy.transform.position, coinsNeed, delta);
	}

	/// <summary>
	/// Генерация монет при смерти врага
	/// </summary>
	/// <param name="position">Позиция</param>
	/// <param name="count">Количество</param>
	/// <param name="isGold">Это золотая монетка</param>
	void GenerateCoins(Vector3 position, int count, float delta, bool isGold = false) {
		//for(int i = 0; i < count; i++) {
		GameObject coin = PoolerManager.Spawn("Coins");
		coin.transform.position = position + Vector3.up;

		Coin cmp = coin.GetComponent<Coin>();

		cmp.isGold = isGold;
		cmp.nomination = count;
		cmp.deltaPosition = delta;
		coin.SetActive(true);

		if (!isGold) {
			BattleManager.Instance.silverCoins += count;
			UserManager.Instance.silverCoins.Value += count;
			UserManager.Instance.Experience += count;
		} else {
			BattleManager.Instance.goldCoins += count;
			if (UserManager._instance.ActiveBattleInfo.Mode != PointMode.survival && !ZbCatScene.CatSceneManager.Instance.isSpecLevel)
				ZbCatScene.CatSceneManager.Instance.ShowCatScene(11, () => { });
		}
		//}
	}

	/// <summary>
	/// Дроп боя
	/// </summary>
	public void GetDrop(Enemy enemy) {
		if (UserManager.Instance.ActiveBattleInfo.Mode == PointMode.survival)
			SurvivalDrop(enemy);
		else
			CompanyDrop(enemy);
	}

	/// <summary>
	/// Дроп сурвивла
	/// </summary>
	void SurvivalDrop(Enemy enemy) {
		if (UnityEngine.Random.value > 0.02f)
			return;

		List<Drop> getDropList = BattleManager.Instance.getDropList;

	  Game.Weapon.WeaponType newWeap = Game.Weapon.WeaponType.axe;
		if (getDropList.Count >= 10) {
			newWeap = getDropList[UnityEngine.Random.Range(0, getDropList.Count)].weaponType;
		} else {
			List<Configuration.SurvivleDrop> survDrop = GameDesign.Instance.allConfig.survivalDrop.FindAll(x => x.time <= BattleManager.Instance.timeBattle);

			do {
				newWeap = (Game.Weapon.WeaponType)survDrop[UnityEngine.Random.Range(0, survDrop.Count)].id;
			} while (!Game.User.UserWeapon.Instance.ExistWeaponType(newWeap));
		}

		Drop newDrop = new Drop();
		newDrop.type = DropType.weapon;
		newDrop.weaponType = newWeap;
		newDrop.count = getDropList.Find(x => x.weaponType == newWeap).count + 1;
		getDropList.RemoveAll(x => x.weaponType == newWeap);
		getDropList.Add(newDrop);
	}

	void CompanyDrop(Enemy enemy) {

		if (UnityEngine.Random.value > 0.05f)
			return;

		Drop[] readyDrop = BattleManager.Instance.readyDrop;
		Drop[] getDrop = BattleManager.Instance.getDrop;

		bool ready = false;
		for (int i = 0; i < readyDrop.Length; i++) {
			if (readyDrop[i].count > 0)
				ready = true;
		}
		if (!ready)
			return;
		int selectNum;

		while (ready) {
			selectNum = UnityEngine.Random.Range(0, readyDrop.Length);
			if (readyDrop[selectNum].count > 0) {
				readyDrop[selectNum].count--;
				ready = false;

				for (int j = 0; j < getDrop.Length; j++) {
					if (getDrop[j].type == readyDrop[selectNum].type
						&& (readyDrop[selectNum].type == DropType.superCoins
							|| readyDrop[selectNum].weaponType == getDrop[j].weaponType
							)
						) {
						getDrop[j].count++;
						if (readyDrop[selectNum].type == DropType.superCoins)
							GenerateCoins(enemy.transform.position, 1, 0, true);
					}
				}

			}
		}
	}

	#endregion

	/// <summary>
	/// Возвращает всех текущих врагов
	/// </summary>
	public static Enemy[] GetAllEnemy {
		get {
			return Instance.GetComponentsInChildren<Enemy>();
		}
	}

	/// <summary>
	/// Генерация одного врага
	/// </summary>
	/// <param name="enemyName"></param>
	/// <returns></returns>
	public GameObject GetEnemy(string enemyName) {
		GameObject inst = pool.GetPooledObject(enemyName);

		// Применяем параметр размера
		if (characterSize.useSize)
			inst.transform.localScale = new Vector3(characterSize.size, characterSize.size, characterSize.size);

		enemyInBattle++;
		inst.transform.parent = transform;
		inst.GetComponent<Enemy>().OnDead += EnemyDead;
		inst.GetComponent<Enemy>().OnDamageEvnt += EnemyDamage;

		EnemyType enemyType = inst.GetComponent<Enemy>().enemyType;

		if (UserManager.Instance.ActiveBattleInfo.Mode == PointMode.company && UserManager.Instance.ActiveBattleInfo.Group != 40 && !ZbCatScene.CatSceneManager.Instance.isSpecLevel) {
			switch (enemyType) {
				case EnemyType.Diversant:
					if (!ZbCatScene.CatSceneManager.Instance.CheckShowCatScene(6)) {
						CatSceneEnemyHelper csh = inst.AddComponent<CatSceneEnemyHelper>();
						csh.catSceneNum = "6";
						csh.distantion = 9;
					}
					//ZbCatScene.CatSceneManager.Instance.ShowCatScene(6, () => { });
					break;
				case EnemyType.Hunweibin:
					if (!ZbCatScene.CatSceneManager.Instance.CheckShowCatScene(13)) {
						CatSceneEnemyHelper csh = inst.AddComponent<CatSceneEnemyHelper>();
						csh.catSceneNum = "13";
						csh.distantion = 9;
					}
					//ZbCatScene.CatSceneManager.Instance.ShowCatScene(13, () => { });
					break;
				case EnemyType.Ekstrimalchik:
					if (!ZbCatScene.CatSceneManager.Instance.CheckShowCatScene(14)) {
						CatSceneEnemyHelper csh = inst.AddComponent<CatSceneEnemyHelper>();
						csh.catSceneNum = "14";
						csh.distantion = 9;
					}
					//ZbCatScene.CatSceneManager.Instance.ShowCatScene(14, () => { });
					break;
				case EnemyType.MadamSufrajo:
					if (!ZbCatScene.CatSceneManager.Instance.CheckShowCatScene(15)) {
						CatSceneEnemyHelper csh = inst.AddComponent<CatSceneEnemyHelper>();
						csh.catSceneNum = "15";
						csh.distantion = 10.5f;
					}
					//ZbCatScene.CatSceneManager.Instance.ShowCatScene(15, () => { });
					break;
				case EnemyType.Shustrik:
					if (!ZbCatScene.CatSceneManager.Instance.CheckShowCatScene(25)) {
						CatSceneEnemyHelper csh = inst.AddComponent<CatSceneEnemyHelper>();
						csh.catSceneNum = "25";
						csh.distantion = 9;
					}
					//ZbCatScene.CatSceneManager.Instance.ShowCatScene(25, () => { });
					break;
			}
		}


		return inst;
	}

	Dictionary<string, KeyValuePair<GameObject, int>> enemyList;

	/// <summary>
	/// Подготовка структуры для генерации
	/// </summary>
	void StructProcessor() {
		enemyList = new Dictionary<string, KeyValuePair<GameObject, int>>();
		foreach (GameObject enemy in enemyPrefabs) {
			if (!enemyList.ContainsKey(enemy.name))
				enemyList.Add(enemy.name, new KeyValuePair<GameObject, int>(enemy, 3));
		}
	}

	#endregion

	#region Полоса жизней имени

	public GameObject enemyLiveLinePrefab;         // Префаб полосы жизней

	/// <summary>
	/// Запрос на получение полосы жизней
	/// </summary>
	/// <returns></returns>
	public GameObject GetEnemyLiveLine() {
		return Instantiate(enemyLiveLinePrefab);
	}

	#endregion

	#region Выживание

	public List<SurvivalEnemy> survivalEnemy = new List<SurvivalEnemy>();
	List<EnemyType> floodList = new List<EnemyType>();
	float timeFloodEnemy = 2;
	float nextTimeCalcFlood = 0;
	List<EnemyType> waveList = new List<EnemyType>();
	float timeWaveEnemy = 10;
	float nextTimeCalcWave = 0;
	List<EnemyType> bossList = new List<EnemyType>();
	float timeBossEnemy = 160;
	float nextTimeCalcBoss = 0;

	void InitSurvival() {
		timeFloodEnemy = 2;
		nextTimeCalcFlood = 0;
		timeWaveEnemy = 10;
		nextTimeCalcWave = 0;
		timeBossEnemy = 160;
		nextTimeCalcBoss = 0;
	}

	void GenerateSurvival() {

		if (nextTimeCalcFlood >= 0 && nextTimeCalcFlood <= BattleManager.Instance.timeBattle) {
			bool change = false;
			for (int i = 0; i < survivalEnemy.Count; i++) {
				if (!change && survivalEnemy[i].mode == EnemyTypeWave.flood && survivalEnemy[i].timeGenerate == nextTimeCalcFlood) {
					change = true;
					if (i < survivalEnemy.Count - 2 && survivalEnemy[i + 1].mode == EnemyTypeWave.flood) {
						nextTimeCalcFlood = survivalEnemy[i + 1].timeGenerate;
					}
					if (survivalEnemy[i].type != EnemyType.None)
						floodList.Add(survivalEnemy[i].type);
				}
			}
			if (!change)
				nextTimeCalcFlood = -1;
		}

		if (nextTimeCalcWave >= 0 && nextTimeCalcWave <= BattleManager.Instance.timeBattle) {
			bool change = false;
			for (int i = 0; i < survivalEnemy.Count; i++) {
				if (!change && survivalEnemy[i].mode == EnemyTypeWave.wave && survivalEnemy[i].timeGenerate == nextTimeCalcWave) {
					change = true;
					if (i < survivalEnemy.Count - 2 && survivalEnemy[i + 1].mode == EnemyTypeWave.wave) {
						float newTimeGenerate = -1;
						do {
							if (survivalEnemy[i].type != EnemyType.None)
								waveList.Add(survivalEnemy[i].type);
							i++;
							if (i >= survivalEnemy.Count || survivalEnemy[i].mode != EnemyTypeWave.wave) {
								change = false;
								break;
							}
							newTimeGenerate = survivalEnemy[i].timeGenerate;
						} while (nextTimeCalcWave == newTimeGenerate);
						nextTimeCalcWave = newTimeGenerate;
					}
				}
			}
			if (!change)
				nextTimeCalcWave = -1;
		}

		if (nextTimeCalcBoss >= 0 && nextTimeCalcBoss <= BattleManager.Instance.timeBattle) {
			bool change = false;
			for (int i = 0; i < survivalEnemy.Count; i++) {
				if (!change && survivalEnemy[i].mode == EnemyTypeWave.boss && survivalEnemy[i].timeGenerate == nextTimeCalcBoss) {
					change = true;
					if (i < survivalEnemy.Count - 2 && survivalEnemy[i + 1].mode == EnemyTypeWave.boss) {
						float newTimeGenerate = -1;
						do {
							if (survivalEnemy[i].type != EnemyType.None)
								bossList.Add(survivalEnemy[i].type);
							i++;
							if (i >= survivalEnemy.Count || survivalEnemy[i].mode != EnemyTypeWave.wave) {
								change = false;
								break;
							}
							newTimeGenerate = survivalEnemy[i].timeGenerate;
						} while (nextTimeCalcWave == newTimeGenerate);
						nextTimeCalcBoss = newTimeGenerate;
					}
				}
			}
			if (!change)
				nextTimeCalcBoss = -1;
		}

		if (timeFloodEnemy <= BattleManager.Instance.timeBattle) {
			timeFloodEnemy += 2;
			GenerateOneEnemy(floodList, Vector3.zero, true);
		}
		if (timeWaveEnemy <= BattleManager.Instance.timeBattle) {
			timeWaveEnemy += 10;
			GenerateOneEnemy(waveList, Vector3.zero, true);
		}
		if (timeBossEnemy <= BattleManager.Instance.timeBattle) {
			timeBossEnemy += 160;
		}

	}

	void GenerateOneEnemy(List<EnemyType> sourceNum, Vector3 pos, bool changeLevel = false) {
		if (sourceNum.Count == 0) return;
		EnemyType enem = sourceNum[UnityEngine.Random.Range(0, sourceNum.Count)];
		GameObject enemy = enemyPrefabs.Find(x => x.GetComponent<Enemy>().enemyType == enem);
		GameObject instance = GenerateEnemyBuName(enemy.name, pos);
		if (changeLevel)
			instance.GetComponent<Enemy>().SetLevel(Mathf.FloorToInt(BattleManager.Instance.timeBattle / 80));
	}

	#endregion

	#region Настройки

	/// <summary>
	/// Заполнение данных по списку
	/// </summary>
	public void GetConfig() {

		//int chapterNum = 0;

		characterSize.size = GameDesign.Instance.allConfig.summary.Find(x => x.name == "EnemySize").param1;


		if (UserManager.Instance.ActiveBattleInfo.Mode == PointMode.survival) {
			survivalEnemy.Clear();
			floodList.Clear();
			waveList.Clear();
			bossList.Clear();
			foreach (Configuration.Survivle one in GameDesign.Instance.allConfig.survival) {
				SurvivalEnemy enem = new SurvivalEnemy();
				try {
					enem.type = (EnemyType)one.mobId;
					enem.mode = (EnemyTypeWave)Enum.Parse(typeof(EnemyTypeWave), one.mode);
					enem.timeGenerate = one.time;
					survivalEnemy.Add(enem);
				} catch {
					Debug.Log(one.mobId);
				}
			}

			foreach (string name in Enum.GetNames(typeof(EnemyType))) {
				GameObject inst = ResourceManager.Instance.LoadResources<GameObject>(enemyPrefabsList + name);
				if (inst != null && !enemyPrefabs.Exists(x => x.name == inst.name))
					enemyPrefabs.Add(inst);
			}


			InitSurvival();
			return;
		}

		if (UserManager.Instance.ActiveBattleInfo.Mode == PointMode.arena) {
			foreach (string name in Enum.GetNames(typeof(EnemyType))) {
				GameObject inst = ResourceManager.Instance.LoadResources<GameObject>(enemyPrefabsList + name);
				if (inst != null && !enemyPrefabs.Exists(x => x.name == inst.name))
					enemyPrefabs.Add(inst);
			}
			return;
		}


		GetConfigLevelWave();

		if (UserManager.Instance.ActiveBattleInfo.Mode == PointMode.farm)
			CalcFarm();

		coinsPrice = (int)GameDesign.Instance.allConfig.weapon.Find(x => x.id == (int)Game.Weapon.WeaponType.tomato).damage / 3;

	}


	void GetConfigLevelWave() {
		int chapterNum = 0;

		List<Configuration.Character> listChar = GameDesign.Instance.allConfig.chapters.FindAll(x => x.chapter == UserManager.Instance.ActiveBattleInfo.Group && x.level == UserManager.Instance.ActiveBattleInfo.Level);

		List<EnemyGenerateParametrs> tempEnemyList = new List<EnemyGenerateParametrs>();

		listChar.ForEach(x => {
      
			EnemyGenerateParametrs tempEnemy = new EnemyGenerateParametrs() {
				isGenerate = false,
				count = 1,
				type = (EnemyType) x.idMob.Value,
				timeGenerate = x.time.Value,
				level = x.mobLevel,
				keyEnemy = x.mobType == "keyEnemy" ? true : false
			};
			
			tempEnemyList.Add(tempEnemy);
   
        if (!enemyPrefabs.Exists(pr => pr.GetComponent<Enemy>().enemyType == tempEnemy.type))
          enemyPrefabs.Add(ResourceManager.Instance.LoadResources<GameObject>(enemyPrefabsList + tempEnemy.type.ToString()));
   
		});

		enemyGenerateParametrs = tempEnemyList.ToArray();

		//foreach (Configuration.Character oneChapter in GameDesign.Instance.allConfig.chapters) {
		//	chapterNum++;

		//	if (User.Instance.activeBattleInfo.group == chapterNum) {

		//		List<object> levels = (List<object>)cp["levels"];
		//		int levelNum = 0;
		//		foreach (object oneLevel in levels) {
		//			levelNum++;
		//			if (User.Instance.activeBattleInfo.level == levelNum) {

		//				Dictionary<string, object> lv = (Dictionary<string, object>)oneLevel;
		//				List<object> enemyList = (List<object>)lv["enemy"];
		//				//enemyGenerateParametrs = new EnemyGenerateParametrs[enemyList.Count];
		//				int enemyNum = 0;
		//				List<EnemyGenerateParametrs> tempEnemyList = new List<EnemyGenerateParametrs>();
		//				foreach (object oneEnemy in enemyList) {
		//					try {
		//						EnemyGenerateParametrs tempEnemy = new EnemyGenerateParametrs();
		//						Dictionary<string, string> enemyElem = (Dictionary<string, string>)oneEnemy;
		//						tempEnemy.isGenerate = false;
		//						tempEnemy.count = 1;
		//						tempEnemy.type = (EnemyType)Int32.Parse(enemyElem["mobId"]);
		//						tempEnemy.timeGenerate = float.Parse(enemyElem["timeSpawn"].ToString().Replace(',', '.'));
		//						tempEnemy.level = int.Parse(enemyElem["levelEnemy"].ToString());
		//						tempEnemy.keyEnemy = enemyElem.ContainsKey("mobType") && enemyElem["mobType"].ToString() == "keyEnemy" ? true : false;
		//						tempEnemyList.Add(tempEnemy);

		//						if (!enemyPrefabs.Exists(x => x.GetComponent<Enemy>().enemyType == tempEnemy.type))
		//							enemyPrefabs.Add(ResourceManager.Instance.LoadResources<GameObject>(enemyPrefabsList + tempEnemy.type.ToString()));

		//						enemyNum++;
		//					} catch { }
		//				}
		//				enemyGenerateParametrs = tempEnemyList.ToArray();

		//			}
		//		}
		//	}

		//}
	}

	void CalcFarm() {
		int doubltCount = 0;
		if (UserManager.Instance.ActiveBattleInfo.FarmPointActive == 3) {
			for (int i = 0; i < enemyGenerateParametrs.Length; i++)
				enemyGenerateParametrs[i].count = 2;
		} else if (UserManager.Instance.ActiveBattleInfo.FarmPointActive == 2) {
			doubltCount = enemyGenerateParametrs.Length / 2;

			while (doubltCount > 0) {
				int num = 0;
				do {
					num = UnityEngine.Random.Range(0, enemyGenerateParametrs.Length);
				} while (enemyGenerateParametrs[num].count > 1);
				enemyGenerateParametrs[num].count = 2;
				doubltCount--;
			}

		}
	}



	#endregion

}

[System.Serializable]
public struct CharacterSize {
	public bool useSize;
	public float size;
}