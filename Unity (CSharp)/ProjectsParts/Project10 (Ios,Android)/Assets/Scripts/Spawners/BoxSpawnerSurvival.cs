using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


[Serializable]
public class BoxSpawnerSurvival : IBoxSpawner {

	/// <summary>
	/// Параметры дистанции
	/// </summary>
	[System.Serializable]
	public struct GenerateParametrs {
		public float distantion;
		public int count;
		public List<BoxProbability> param;
	}

	[System.Serializable]
	public struct WeaponBoxParametrs {
		public float distance;
		public int count;
	}

	/// <summary>
	/// Вероятность генерации
	/// </summary>
	[System.Serializable]
	public struct BoxProbability {
		public BoxCategory type;
		public float value;
	}

	private float weaponNextDistanceCalc;
	private BoxSpawner boxSpawner;
	public List<WeaponBoxParametrs> generateParametr;
	public BoxSpawn[] boxSpawnParametrs;                // Параметры генерации ящиков
	private RunnerPhase runnerPhase;
	private float nextDistanceCalc;             // Следующая дистанция для рассчета
	private float nextDistanceSpawn;            // Следующая дистанция для генерации
	private List<float> weaponBoxDistance = new List<float>();
	[HideInInspector]
	public List<float> wpn;                                        // МАссив доступных для генерации
	[HideInInspector]
	public List<string> wpnName;
	private List<GameObject> generateList = new List<GameObject>();
	private bool weaponBeforeBoost;
	private float nextGenerate;

	public void Init(BoxSpawner boxSpawner) {
		this.boxSpawner = boxSpawner;
	}

	public void ChangePhase(RunnerPhase runnerPhase) {

		if (this.runnerPhase == RunnerPhase.postBoost && runnerPhase == RunnerPhase.run) {
			//for(int i = 0; i < boxSpawnParametrs.Length; i++)
			//  boxSpawnParametrs[i].afterBoost = true;
		}

		if (runnerPhase == RunnerPhase.run) {

			weaponNextDistanceCalc = generateParametr.Find(x => x.distance > RunnerController.playerDistantion).distance;

			nextDistanceCalc = 999999999999;
			nextDistanceSpawn = 99999999999;

			for (int i = 0; i < boxSpawnParametrs.Length; i++) {
				boxSpawnParametrs[i].nextDistance = 99999999999;
				boxSpawnParametrs[i].nextDistanceCalc = 9999999999;

				for (int j = 0; j < boxSpawnParametrs[i].parametrs.Count; j++) {
					if (boxSpawnParametrs[i].parametrs[j].distance <= RunnerController.playerDistantion && j != 0) {
						boxSpawnParametrs[i].nextDistanceCalc = boxSpawnParametrs[i].parametrs[j].distance;

					} else {
						if (boxSpawnParametrs[i].nextDistanceCalc > boxSpawnParametrs[i].parametrs[j].distance) {
							boxSpawnParametrs[i].nextDistanceCalc = boxSpawnParametrs[i].parametrs[j].distance;
						}
						if (nextDistanceCalc > boxSpawnParametrs[i].nextDistanceCalc)
							nextDistanceCalc = boxSpawnParametrs[i].nextDistanceCalc;
					}
				}
			}
		}

		this.runnerPhase = runnerPhase;
	}

	public void Update() {
		if (runnerPhase != RunnerPhase.run) return;
		GenerateSurvival();
	}

	void GenerateSurvival() {
		if (weaponNextDistanceCalc <= RunnerController.playerDistantion) {
			for (int i = 0; i < generateParametr.Count; i++) {
				if (generateParametr[i].distance == weaponNextDistanceCalc && weaponNextDistanceCalc <= RunnerController.playerDistantion) {

					if (i == generateParametr.Count - 1)
						InfinityIncrementWeapon();
					if (i != generateParametr.Count - 1)
						weaponNextDistanceCalc = generateParametr[i + 1].distance;

					CalcGenerateWeaponBox(i);
				}
			}
		}

		if (weaponBoxDistance.Count > 0) {

			float tempBoxDist = 0;

			foreach (float weaponBoxDist in weaponBoxDistance) {
				if (weaponBoxDist <= RunnerController.playerDistantion) {

					if (RunnerController.Instance.generateItems) {
						int needNum = BinarySearch.RandomNumberGenerator(wpn);

						GameObject obj = LevelPooler.Instance.GetPooledObject(BoxSpawner.POOLER_KEY, boxSpawner.wpnName[needNum]);
						obj.transform.parent = boxSpawner.transform;
						obj.SetActive(false);
						generateList.Add(obj);
						tempBoxDist = weaponBoxDist;
					}
				}
			}

			if (tempBoxDist > 0)
				weaponBoxDistance.Remove(tempBoxDist);
		}

		if (nextDistanceCalc <= RunnerController.playerDistantion) {

			nextDistanceCalc = 9999999999999;
			nextDistanceSpawn = 9999999999999;

			for (int i = 0; i < boxSpawnParametrs.Length; i++) {
				if (boxSpawnParametrs[i].nextDistanceCalc <= RunnerController.playerDistantion) {
					for (int j = 0; j < boxSpawnParametrs[i].parametrs.Count; j++) {
						if (boxSpawnParametrs[i].parametrs[j].distance == boxSpawnParametrs[i].nextDistanceCalc && boxSpawnParametrs[i].parametrs[j].distance <= RunnerController.playerDistantion) {


							if (j == boxSpawnParametrs[i].parametrs.Count - 1)
								InfinityIncrementBoxSpawnElement(i);
							if (j != boxSpawnParametrs[i].parametrs.Count - 1) {
								boxSpawnParametrs[i].nextDistanceCalc = boxSpawnParametrs[i].parametrs[j + 1].distance;
								if (nextDistanceCalc > boxSpawnParametrs[i].nextDistanceCalc)
									nextDistanceCalc = boxSpawnParametrs[i].nextDistanceCalc;
							}

							float probab = boxSpawnParametrs[i].parametrs[j].probability;

							if (boxSpawnParametrs[i].name == "Heart")
								probab += boxSpawner.hearthProbability + (boxSpawner.heartClothes.head ? 0.1f : 0) + (boxSpawner.heartClothes.spine ? 0.1f : 0) + (boxSpawner.heartClothes.accessory ? 0.1f : 0);

							if (boxSpawnParametrs[i].name == "Magnet")
								probab += boxSpawner.magnetProbability + (boxSpawner.magnetClothes.head ? 0.1f : 0) + (boxSpawner.magnetClothes.spine ? 0.1f : 0) + (boxSpawner.magnetClothes.accessory ? 0.1f : 0);

							if (Random.value <= probab) {
								float nextDistance;
								if (boxSpawnParametrs[i].parametrs.Count <= j + 1)
									nextDistance = 200;
								else
									nextDistance = boxSpawnParametrs[i].parametrs[j + 1].distance - boxSpawnParametrs[i].parametrs[j].distance;

								boxSpawnParametrs[i].nextDistance = boxSpawnParametrs[i].parametrs[j].distance + Random.Range(20, nextDistance);

								//if(boxSpawnParametrs[i].afterBoost && boxSpawnParametrs[i].nextDistance <= RunnerController.playerDistantion)
								//  boxSpawnParametrs[i].nextDistance = 99999999999;

								//if(boxSpawnParametrs[i].afterBoost) boxSpawnParametrs[i].afterBoost = false;

							}
						}
					}
				}
				if (nextDistanceSpawn > boxSpawnParametrs[i].nextDistance)
					nextDistanceSpawn = boxSpawnParametrs[i].nextDistance;
			}
		}

		if (nextDistanceSpawn <= RunnerController.playerDistantion) {
			nextDistanceSpawn = 9999999999;

			for (int i = 0; i < boxSpawnParametrs.Length; i++) {


				if (boxSpawnParametrs[i].nextDistance <= RunnerController.playerDistantion) {
					int needNum = 0;
					boxSpawnParametrs[i].nextDistance = 99999999;
					if (boxSpawnParametrs[i].prefab.Length > 1)
						needNum = CalcProbabilityBoxSpawn(boxSpawnParametrs[i]);

					GameObject obj = LevelPooler.Instance.GetPooledObject(BoxSpawner.POOLER_KEY, boxSpawnParametrs[i].prefab[needNum].prefab.name);
					obj.transform.parent = boxSpawner.transform;
					obj.SetActive(false);
					generateList.Add(obj);
				}

				if (nextDistanceSpawn > boxSpawnParametrs[i].nextDistance)
					nextDistanceSpawn = boxSpawnParametrs[i].nextDistance;

			}
		}

		if (generateList.Count > 0 && nextGenerate < Time.time) {
			Vector3 generatePosition = new Vector3(CameraController.displayDiff.transform.position.x + CameraController.displayDiff.right * 1.2f, RunnerController.Instance.mapHeight + Random.Range(1f, 2f), 0);
			Collider[] objCol = Physics.OverlapSphere(generatePosition, 6f);

			bool YesGenerate = true;

			foreach (Collider oneColl in objCol) {
				if (oneColl.tag == "RollingStone" | oneColl.tag == "jumpUp" | oneColl.tag == "jumpDown")
					YesGenerate = false;
			}

			if (YesGenerate) {
				foreach (GameObject obj in generateList) {
					obj.transform.position = generatePosition;
					obj.SetActive(true);
					generateList.RemoveAt(0);
					nextGenerate = Time.time + 1f;
					break;
				}
			}
		}
	}

	int CalcProbabilityBoxSpawn(BoxSpawn boxParametr) {
		float sum = 0;
		List<float> param = new List<float>();
		param.Add(sum);

		List<GameObject> tmp = new List<GameObject>();

		for (int i = 0; i < boxParametr.prefab.Length; i++) {
			tmp.Add(boxParametr.prefab[i].prefab);
			sum += boxParametr.prefab[i].probability;
			param.Add(sum);
		}

		param = param.ConvertAll(x => x / sum);

		return BinarySearch.RandomNumberGenerator(param);

	}


	/// <summary>
	/// Бесконечный инкремент генератора ящиков
	/// </summary>
	void InfinityIncrementBoxSpawnElement(int boxNum) {

		BoxParametrs newParamtr = new BoxParametrs();
		newParamtr.distance = boxSpawnParametrs[boxNum].parametrs[boxSpawnParametrs[boxNum].parametrs.Count - 1].distance + 200;
		newParamtr.probability = boxSpawnParametrs[boxNum].parametrs[boxSpawnParametrs[boxNum].parametrs.Count - 1].probability;
		boxSpawnParametrs[boxNum].parametrs.Add(newParamtr);
	}

	void CalcGenerateWeaponBox(int num) {

		float nextDistance;

		if (generateParametr.Count <= num + 1)
			nextDistance = 200;
		else
			nextDistance = generateParametr[num + 1].distance - generateParametr[num].distance;

		int countNeed = Random.Range(generateParametr[num].count - 1, generateParametr[num].count + 1) + +(boxSpawner.boxClothes.head ? 1 : 0) + (boxSpawner.boxClothes.spine ? 1 : 0) + (boxSpawner.boxClothes.accessory ? 1 : 0);
		weaponBoxDistance = new List<float>();

		int tempcount = countNeed;
		float lastDistance = generateParametr[num].distance;

		while (weaponBoxDistance.Count < countNeed) {
			float dist = nextDistance / tempcount;
			float newDist = Random.Range(20, dist);
			lastDistance += newDist;
			weaponBoxDistance.Add(lastDistance);
			nextDistance -= newDist;
			tempcount = tempcount - 1;
		}

		if (weaponBeforeBoost) {
			weaponBeforeBoost = false;
			weaponBoxDistance.RemoveAll(x => x <= RunnerController.playerDistantion);
		}

	}


	/// <summary>
	/// Бесконечный инкремент параметров оружия
	/// </summary>
	void InfinityIncrementWeapon() {

		WeaponBoxParametrs weaponNewParam = new WeaponBoxParametrs();
		weaponNewParam.distance = generateParametr[generateParametr.Count - 1].distance + 200;
		weaponNewParam.count = generateParametr[generateParametr.Count - 1].count;
		generateParametr.Add(weaponNewParam);
	}

	public void GetConfig() {
		List<Configuration.Survivles.Item> itemsData = Config.Instance.config.survival.items;

		int startCol = 1;
		float distance;
		bool isEnd = false;

		generateParametr = new List<WeaponBoxParametrs>();

		for (int j = 0; j < boxSpawnParametrs.Length; j++) {
			boxSpawnParametrs[j].parametrs = new List<BoxParametrs>();
		}

		for (int i = 0; i < itemsData.Count; i++) {
			if (i < startCol || isEnd) {
				continue;
			}

			WeaponBoxParametrs boxParam = new WeaponBoxParametrs();
			try {
				distance = itemsData[i].dist;
			} catch {
				distance = generateParametr[generateParametr.Count - 1].distance + 200;
				isEnd = true;
			}
			boxParam.distance = distance;
			boxParam.count = itemsData[i].boxes;
			generateParametr.Add(boxParam);

			for (int j = 0; j < boxSpawnParametrs.Length; j++) {

				switch (boxSpawnParametrs[j].name) {
					case "Magnet":
						if (itemsData[i].magnet > 0) {
							BoxParametrs box = new BoxParametrs();
							box.distance = distance;
							box.probability = itemsData[i].magnet * 0.01f;
							boxSpawnParametrs[j].parametrs.Add(box);
						}
						break;
					case "Heart":
						if (itemsData[i].hearth > 0) {
							BoxParametrs box = new BoxParametrs();
							box.distance = distance;
							box.probability = itemsData[i].hearth * 0.01f;
							boxSpawnParametrs[j].parametrs.Add(box);
						}
						break;
					case "Shield":
						if (itemsData[i].magnet > 0) {
							BoxParametrs box = new BoxParametrs();
							box.distance = distance;
							box.probability = itemsData[i].magnet * 0.01f;
							boxSpawnParametrs[j].parametrs.Add(box);
						}
						break;
					case "Magic":
						if (itemsData[i].magic > 0) {
							BoxParametrs box = new BoxParametrs();
							box.distance = distance;
							box.probability = itemsData[i].magic * 0.01f;
							boxSpawnParametrs[j].parametrs.Add(box);
						}
						break;
				}
			}
		}
	}

}
