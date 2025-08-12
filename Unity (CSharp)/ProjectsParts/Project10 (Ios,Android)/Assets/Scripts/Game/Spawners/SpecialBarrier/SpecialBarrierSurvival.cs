using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpecialBarrierSurvival : SpecialBarrier {
	
	public SpecialBarrierGenerateParam[] specialBarrierParam;                           // Параметры генерации
	private bool isSpecialBarrier;                                                              // Флаг выполнения спициального барьера
	private RunSpawner runSpawner;
	private float? nextDistanceCalc;                                                            // Дистанция для рассчета срабатывания
	private float? nextDistanceActive;                                                          // Дистанция срабатывания препядствия
	private SpecialBarriersTypes? activeSpecialBarrier;                                         // Тип барьера для активации
	private List<float> cd = new List<float>();

	protected override void Init(RunSpawner runSpawner) {
		this.runSpawner = runSpawner;
		
		for (int i = 0; i < specialBarrierParam.Length; i++) {
			if (!specialBarrierParam[i].generate && specialBarrierParam[i].distance.min <= RunnerController.playerDistantion && i != 0) {
				specialBarrierParam[i].generate = true;
			}
		}
		CalcDistForCheckProbSpecialBarrier();

		spawners.ForEach(x=>x.EndBarrier = EventComplete);

	}

	protected override void UpdateProcess() {
		base.UpdateProcess();

		if (specialBarrierParam.Length == 0) return;
		UpdateSpecalBarrier();
	}

	void UpdateSpecalBarrier() {
		if (!isSpecialBarrier) {
			if (nextDistanceCalc == null)
				CalcDistForCheckProbSpecialBarrier();
			if (nextDistanceCalc != null && nextDistanceCalc <= RunnerController.playerDistantion && nextDistanceActive == null)
				CalcDistForActivSpecialBarrier();
			if (nextDistanceActive != null && activeSpecialBarrier != null && nextDistanceActive <= RunnerController.playerDistantion)
				ActivateSpecialBarrier();
		}
	}

	/// <summary>
	/// Активация специального барьера
	/// </summary>
	void ActivateSpecialBarrier() {

		if (runSpawner.CheckGenerate(10)) {
			runSpawner.lockedGenerate = true;

			spawners.ForEach((elem) => {
				elem.Generate(true, activeSpecialBarrier);
				ExEvent.RunEvents.SpecialBarrier.CallAsync(true, activeSpecialBarrier);
			});
			
			isSpecialBarrier = true;
		}
		
	}

	private void EventComplete() {
		nextDistanceCalc = null;
		nextDistanceActive = null;
		isSpecialBarrier = false;
		activeSpecialBarrier = null;
		runSpawner.lockedGenerate = false;
	}


	/// <summary>
	/// Рассчет дистанции для активации
	/// </summary>
	void CalcDistForActivSpecialBarrier() {
		for (int i = 0; i < specialBarrierParam.Length; i++) {
			if (nextDistanceActive == null && !specialBarrierParam[i].generate && nextDistanceCalc <= specialBarrierParam[i].distance.min) {
				specialBarrierParam[i].generate = true;

				if (Random.value <= specialBarrierParam[i].probability) {
					nextDistanceActive = specialBarrierParam[i].distance.min + Random.Range(20, specialBarrierParam[i].distance.max - specialBarrierParam[i].distance.min);
					activeSpecialBarrier = CalcNextSpecialBarrier(specialBarrierParam[i].types);
				} else {
					specialBarrierParam[i].generate = false;
					nextDistanceActive = null;
					nextDistanceCalc = (specialBarrierParam[i].distance.max - specialBarrierParam[i].distance.min) / 2 + specialBarrierParam[i].distance.min;
				}
				return;
			} else if (nextDistanceActive == null && !specialBarrierParam[i].generate && nextDistanceCalc > specialBarrierParam[i].distance.min && nextDistanceCalc < specialBarrierParam[i].distance.max) {

				specialBarrierParam[i].generate = true;
				if (Random.value <= specialBarrierParam[i].probability) {
					nextDistanceActive = nextDistanceCalc + Random.Range(20, specialBarrierParam[i].distance.max - (float)nextDistanceCalc);
					activeSpecialBarrier = CalcNextSpecialBarrier(specialBarrierParam[i].types);
				} else {
					nextDistanceActive = null;
					nextDistanceCalc = null;
				}
			}
		}
	}


	/// <summary>
	/// Рассчет необходимого типа барьера
	/// </summary>
	/// <returns></returns>
	SpecialBarriersTypes CalcNextSpecialBarrier(SpecialBarriersProbability[] arrayForCalc) {
		cd = new List<float>();
		float sum = 0;
		cd.Add(sum);

		foreach (SpecialBarriersProbability param in arrayForCalc) {
			sum += param.shareProbability;
			cd.Add(sum);
		}

		if (sum != 1f)
			cd = cd.ConvertAll(x => x / sum);

		return arrayForCalc[BinarySearch.RandomNumberGenerator(cd)].type;
	}

	/// <summary>
	/// Рассчет дистанции для проверки вероятности генерации
	/// </summary>
	void CalcDistForCheckProbSpecialBarrier() {
		for (int i = 0; i < specialBarrierParam.Length; i++) {
			if (i == specialBarrierParam.Length - 2 && specialBarrierParam[i].distance.min <= RunnerController.playerDistantion)
				InfinityIncrementSpecialBarrier();
			if (nextDistanceCalc == null && !specialBarrierParam[i].generate && specialBarrierParam[i].distance.min > RunnerController.playerDistantion) {
				nextDistanceCalc = specialBarrierParam[i].distance.min;
			}
		}
	}


	void InfinityIncrementSpecialBarrier() {
		SpecialBarrierGenerateParam[] tmp = new SpecialBarrierGenerateParam[specialBarrierParam.Length + 1];

		for (int i = 0; i < specialBarrierParam.Length; i++)
			tmp[i] = specialBarrierParam[i];

		tmp[tmp.Length - 1].distance.min = specialBarrierParam[specialBarrierParam.Length - 1].distance.max;
		tmp[tmp.Length - 1].distance.max = specialBarrierParam[specialBarrierParam.Length - 1].distance.max + 200;
		tmp[tmp.Length - 1].generate = false;
		tmp[tmp.Length - 1].probability = specialBarrierParam[specialBarrierParam.Length - 1].probability;
		tmp[tmp.Length - 1].types = specialBarrierParam[specialBarrierParam.Length - 1].types;
		specialBarrierParam = tmp;

	}

	protected override void GetConfig() {
		List<Configuration.Survivles.Item> itemsData = Config.Instance.config.survival.items;
		Dictionary <string,object> oneItem;
		int startCol = 1;
		float distance;
		bool isEnd = false;

		List<SpecialBarrierGenerateParam> specialBarrierParamTmp  = new List<SpecialBarrierGenerateParam>();

		for (int i = 0; i < itemsData.Count; i++) {
			if (i < startCol || isEnd) continue;

			if (itemsData[i].trap > 0) continue;
			SpecialBarrierGenerateParam oneBarrier = new SpecialBarrierGenerateParam();

			try {
				distance = itemsData[i].dist;
			} catch {
				distance = specialBarrierParamTmp[specialBarrierParamTmp.Count - 1].distance.min + 200;
				isEnd = true;
			}

			oneBarrier.distance.min = distance;

			if (i < itemsData.Count - 2)
				oneBarrier.distance.max = itemsData[i].dist;

			if (i == itemsData.Count - 1)
				oneBarrier.distance.max = oneBarrier.distance.min + 200;

			oneBarrier.generate = false;
			oneBarrier.probability = itemsData[i].trap * 0.01f;
			oneBarrier.types = new SpecialBarriersProbability[2];
			oneBarrier.types[0].type = SpecialBarriersTypes.airArrow;
			oneBarrier.types[0].shareProbability = 50;
			oneBarrier.types[1].type = SpecialBarriersTypes.airStone;
			oneBarrier.types[1].shareProbability = 50;
			specialBarrierParamTmp.Add(oneBarrier);
		}
		specialBarrierParam = specialBarrierParamTmp.ToArray();
	}

}
