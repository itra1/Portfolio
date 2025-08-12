using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class BarrierSurvival: BarrierSpawner {

  List<float> cd = new List<float>();
  public List<BarrierGenerateParam> barrierParam;                     // Параметры генерации
  float barrierLastDistanceGroup;           // Последняя дистанция блока
  float barrierNextDistanceGroup;           // Новое окончание дистанции
  int barrierCountInGroup;
  int barrierCountExists;
  float barrierDistanceInGroup;
  float fastGenerateDistance;

  barrierGenerateTypes nextType;                                      // Следуюй тип генерации
  float nextGenerate;                                                 // Следующая генерация
  bool barrierGenerateReady;
  float nextProbility;
  int ghostCount;
  bool groupGenerate;
  public List<GroupPrefabs> barrierPref;                              // Набор объектов для пулинга
  private float runnerDistantion;

  private void Start() {
    GetConfig();
    Init();
  }

  public void Init() {
    barrierLastDistanceGroup = 0;
    barrierNextDistanceGroup = 0;
    barrierCountInGroup = 0;
    fastGenerateDistance = 0;
    CalcNextGenerate();
  }

  public override void CalcNextGenerate() {
    base.CalcNextGenerate();
    nextGenerate = RunnerController.playerDistantion + Random.Range(50f, 100f);
    nextType = GetRandType();
    barrierGenerateReady = true;
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.RunEvents.RunPhaseChange))]
  public void RunPhaseChange(ExEvent.RunEvents.RunPhaseChange eventData) {

    if ((eventData.newPhase & (RunnerPhase.run | RunnerPhase.boost)) != 0) {
      CalcNextGenerate();
    }

  }

  [ExEvent.ExEventHandler(typeof(ExEvent.RunEvents.BreackEnd))]
  public void BreackEnd(ExEvent.RunEvents.BreackEnd eventData) {
    CalcNextGenerate();
  }

  private void Update() {
    if (RunnerController.Instance.runnerPhase != RunnerPhase.run) return;

    if (nextGenerate < RunnerController.playerDistantion) {
      GenerateBarrierSurvival();
    }
  }

  void GenerateBarrierSurvival() {

    if (!RunSpawner.Instance.CheckGenerate(13) || !RunnerController.Instance.generateItems) return;

    runnerDistantion = RunnerController.playerDistantion;

    groupGenerate = false;

    if (RunSpawner.Instance.activePet != Player.Jack.PetsTypes.none && (nextType == barrierGenerateTypes.hending || nextType == barrierGenerateTypes.stoneAndHending))
      nextType = barrierGenerateTypes.stone;

    // Яма
    if (nextType == barrierGenerateTypes.breack) {
      RunSpawner.GenerateBreackNow(0);
      barrierGenerateReady = false;
    }

    // Висячий предмет
    if (nextType == barrierGenerateTypes.hending) {
      if (Random.value <= 0.5f) {
        GameObject obj = MonoBehaviour.Instantiate(barrierPref[1].prefab[Random.Range(0, barrierPref[2].prefab.Count)], new Vector3(RunSpawner.Instance.generateRightBorder, RunnerController.Instance.mapHeight + Random.Range(2.5f, 3f), RunSpawner.Instance.transform.position.z), Quaternion.identity) as GameObject;
        obj.transform.parent = transform.parent;
        obj.SetActive(true);
      } else {
        int generateNumber = GetThisNumber(nextType);
        int startNumb = GetStartNumber(nextType);

        GameObject obj = LevelPooler.Instance.GetPooledObject(RunSpawner.POOLER_KEY, Random.Range(startNumb, startNumb + barrierPref[generateNumber].prefab.Count));
        obj.transform.position = new Vector3(RunSpawner.Instance.generateRightBorder, RunnerController.Instance.mapHeight + Random.Range(2.5f, 3f), RunSpawner.Instance.transform.position.z);
        obj.SetActive(true);
      }

      if (RunSpawner.Instance.coinsGenerateReady && Random.value <= 0.4f && RunSpawner.Instance.lastCoinsGenerate + 15 <= runnerDistantion) {
        RunSpawner.Instance.OnGenerateCoins(new Vector3(RunSpawner.Instance.generateRightBorder, RunnerController.Instance.mapHeight + Random.Range(1f, 2.5f), RunSpawner.Instance.transform.position.z), 2);
        RunSpawner.Instance.lastCoinsGenerate = runnerDistantion;
        RunSpawner.Instance.coinsGenerateReady = false;
      }

      barrierGenerateReady = false;
    }

    // Камень / Двой камень / Камень с висячим предметом
    if (nextType == barrierGenerateTypes.stone || nextType == barrierGenerateTypes.stoneDouble || nextType == barrierGenerateTypes.stoneAndHending) {
      int generateNumber = GetThisNumber(barrierGenerateTypes.stone);
      int startNumb = GetStartNumber(barrierGenerateTypes.stone);

      GameObject obj = LevelPooler.Instance.GetPooledObject(RunSpawner.POOLER_KEY, Random.Range(startNumb, startNumb + barrierPref[generateNumber].prefab.Count));
      obj.transform.position = new Vector3(RunSpawner.Instance.generateRightBorder, RunnerController.Instance.mapHeight + 1.8f/* + (runSpawner.isHandingPlatform ? 4 : 0)*/, RunSpawner.Instance.transform.position.z);
      obj.SetActive(true);

      if (RunSpawner.Instance.activePet == Player.Jack.PetsTypes.spider && obj.GetComponent<BarrierController>())
        obj.GetComponent<BarrierController>().SetTop();
      else
        obj.GetComponent<BarrierController>().SetDown();

      if (RunSpawner.Instance.coinsGenerateReady && Random.value <= 0.4f && RunSpawner.Instance.lastCoinsGenerate + 15 <= runnerDistantion) {
        RunSpawner.Instance.OnGenerateCoins(new Vector3(RunSpawner.Instance.generateRightBorder, RunnerController.Instance.mapHeight + Random.Range(1f, 2.5f), RunSpawner.Instance.transform.position.z), 1);
        RunSpawner.Instance.lastCoinsGenerate = runnerDistantion;
        RunSpawner.Instance.coinsGenerateReady = false;
      }

      if (nextType == barrierGenerateTypes.stoneDouble) {
        nextGenerate = runnerDistantion + Random.Range(7 * 0.8f, 7 * 1.2f);
        groupGenerate = true;
        nextType = barrierGenerateTypes.stone;
      } else if (nextType == barrierGenerateTypes.stoneAndHending) {
        nextGenerate = runnerDistantion + Random.Range(7 * 0.8f, 7 * 1.2f);
        groupGenerate = true;
        nextType = barrierGenerateTypes.hending;
      } else
        barrierGenerateReady = false;
    }

    if (nextType == barrierGenerateTypes.barrels || nextType == barrierGenerateTypes.boxes || nextType == barrierGenerateTypes.puddle || nextType == barrierGenerateTypes.stickyFloor) {
      int generateNumber = GetThisNumber(nextType);
      int startNumb = GetStartNumber(nextType);

      GameObject obj = LevelPooler.Instance.GetPooledObject(RunSpawner.POOLER_KEY, Random.Range(startNumb, startNumb + barrierPref[generateNumber].prefab.Count));
      obj.transform.position = new Vector3(RunSpawner.Instance.generateRightBorder, RunnerController.Instance.mapHeight + 1, RunSpawner.Instance.transform.position.z);
      obj.SetActive(true);
      barrierGenerateReady = false;
    }

    // Скелеты из земли
    if (nextType == barrierGenerateTypes.stoneSkelet || nextType == barrierGenerateTypes.stoneSkeletDouble) {

      int generateNumber = GetThisNumber(barrierGenerateTypes.stoneSkelet);
      int startNumb = GetStartNumber(barrierGenerateTypes.stoneSkelet);

      GameObject obj = LevelPooler.Instance.GetPooledObject(RunSpawner.POOLER_KEY, Random.Range(startNumb, startNumb + barrierPref[generateNumber].prefab.Count));
      obj.transform.position = new Vector3(RunSpawner.Instance.generateRightBorder, RunnerController.Instance.mapHeight, RunSpawner.Instance.transform.position.z);
      obj.SetActive(true);

      if (RunSpawner.Instance.coinsGenerateReady && Random.value <= 0.4f && RunSpawner.Instance.lastCoinsGenerate + 15 <= runnerDistantion) {
        RunSpawner.Instance.OnGenerateCoins(new Vector3(RunSpawner.Instance.generateRightBorder, RunnerController.Instance.mapHeight + Random.Range(1f, 2.5f), RunSpawner.Instance.transform.position.z), 1);
        RunSpawner.Instance.lastCoinsGenerate = runnerDistantion;
        RunSpawner.Instance.coinsGenerateReady = false;
      }

      if (nextType == barrierGenerateTypes.stoneSkeletDouble) {
        nextGenerate = runnerDistantion + Random.Range(7 * 0.8f, 7 * 1.2f);
        groupGenerate = true;
        nextType = barrierGenerateTypes.stoneSkelet;
      } else
        barrierGenerateReady = false;
    }

    // Призраки
    if (nextType == barrierGenerateTypes.ghost || nextType == barrierGenerateTypes.ghostGroup) {

      int generateNumber = GetThisNumber(barrierGenerateTypes.ghost);
      int startNumb = GetStartNumber(barrierGenerateTypes.ghost);

      GameObject obj = LevelPooler.Instance.GetPooledObject(RunSpawner.POOLER_KEY, Random.Range(startNumb, startNumb + barrierPref[generateNumber].prefab.Count));
      obj.transform.position = new Vector3(CameraController.displayDiff.rightDif(1.3f), RunnerController.Instance.mapHeight + (Random.value <= 0.5f ? 1 : 3.5f), RunSpawner.Instance.transform.position.z);
      obj.SetActive(true);

      if (nextType == barrierGenerateTypes.ghostGroup) {
        nextGenerate = runnerDistantion + Random.Range(15 * 0.8f, 15 * 1.2f);
        nextType = barrierGenerateTypes.ghost;
        ghostCount = Random.Range(1, 5);
      }

      if (ghostCount > 0) {
        nextGenerate = runnerDistantion + Random.Range(15 * 0.8f, 15 * 1.2f);
        groupGenerate = true;
        nextType = barrierGenerateTypes.ghost;
        ghostCount--;
      }

      if (ghostCount <= 0)
        barrierGenerateReady = false;
    }
    RunSpawner.Instance.lastGenerate = RunnerController.playerDistantion;
    CalcNextGenerate();
  }

  int GetStartNumber(barrierGenerateTypes barrType) {
    int count = 0;

    foreach (GroupPrefabs obj1 in barrierPref) {
      if (obj1.type != barrType) {
        count += obj1.prefab.Count;
      } else {
        return count;
      }
    }
    return -1;
  }

  int GetThisNumber(barrierGenerateTypes barrType) {
    int count = 0;

    foreach (GroupPrefabs obj1 in barrierPref) {
      if (obj1.type != barrType) {
        count++;
      } else {
        return count;
      }
    }

    return -1;
  }

  barrierGenerateTypes GetRandType() {
    cd.Clear();
    for (int i = 0; i < barrierParam.Count; i++) {
      if (
        (GameManager.activeLevelData.gameMode == GameMode.survival && (nextGenerate >= barrierParam[i].distance && (i == barrierParam.Count - 1 || nextGenerate < barrierParam[i + 1].distance)))
        || (GameManager.activeLevelData.gameMode != GameMode.survival && (nextGenerate <= barrierParam[i].distance))
        ) {
        float sum = 0;
        if (GameManager.activeLevelData.gameMode == GameMode.survival)
          nextProbility = barrierParam[i].probility;
        else
          nextProbility = 1;
        cd.Add(sum);

        if (barrierParam[i].probability.Count == 0) {
          nextGenerate = barrierParam[i].distance;
        }

        foreach (barrierProbability proba in barrierParam[i].probability) {

          if (RunSpawner.Instance.beackClothes.full && proba.type == barrierGenerateTypes.breack) {
            sum += 0;
          } else if (RunSpawner.Instance.hendingClothes.full && barrierGenerateTypes.hending == proba.type) {
            sum += 0;
          } else if (RunSpawner.Instance.defendbarrierClothes.full
                  && System.Array.IndexOf(new barrierGenerateTypes[] { barrierGenerateTypes.stone,
                                                                        barrierGenerateTypes.stoneAndHending,
                                                                        barrierGenerateTypes.stoneDouble,
                                                                        barrierGenerateTypes.stoneSkelet,
                                                                        barrierGenerateTypes.stoneSkeletDouble}, proba.type) >= 0) {
            sum += proba.value * 0.5f;
          } else {
            sum += proba.value;
          }
          cd.Add(sum);
        }
        if (sum != 1f) cd = cd.ConvertAll(x => x / sum);

        return barrierParam[i].probability[BinarySearch.RandomNumberGenerator(cd)].type;
      }
    }
    return barrierGenerateTypes.breack;
  }

  public void GetConfig() {
    List<Configuration.Survivles.Barrier> itemsData = Config.Instance.config.survival.barriers;

    barrierParam = new List<BarrierGenerateParam>();

    for (int i = 0; i < itemsData.Count; i++) {

      BarrierGenerateParam oneBarrier = new BarrierGenerateParam();
      oneBarrier.distance = i * 500;
      try {
        oneBarrier.probility = itemsData[i].probablility * 0.01f;
      } catch {
        oneBarrier.probility = barrierParam[barrierParam.Count - 1].probility + 0.02f;
      }

      List<barrierProbability> barrList = new List<barrierProbability>();

      if (itemsData[i].pit > 0) {
        barrierProbability oneBarr = new barrierProbability();
        oneBarr.value = itemsData[i].pit;
        oneBarr.type = barrierGenerateTypes.breack;
        barrList.Add(oneBarr);
      }
      if (itemsData[i].doubleSkeleton > 0) {
        barrierProbability oneBarr = new barrierProbability();
        oneBarr.value = itemsData[i].doubleSkeleton;
        oneBarr.type = barrierGenerateTypes.stoneDouble;
        barrList.Add(oneBarr);
      }
      if (itemsData[i].doubleStone > 0) {
        barrierProbability oneBarr = new barrierProbability();
        oneBarr.value = itemsData[i].doubleStone;
        oneBarr.type = barrierGenerateTypes.stoneDouble;
        barrList.Add(oneBarr);
      }
      if (itemsData[i].ghost > 0) {
        barrierProbability oneBarr = new barrierProbability();
        oneBarr.value = itemsData[i].ghost;
        oneBarr.type = barrierGenerateTypes.ghost;
        barrList.Add(oneBarr);
      }
      if (itemsData[i].ghostGroup > 0) {
        barrierProbability oneBarr = new barrierProbability();
        oneBarr.value = itemsData[i].ghostGroup;
        oneBarr.type = barrierGenerateTypes.ghostGroup;
        barrList.Add(oneBarr);
      }
      if (itemsData[i].handingBarrier > 0) {
        barrierProbability oneBarr = new barrierProbability();
        oneBarr.value = itemsData[i].handingBarrier;
        oneBarr.type = barrierGenerateTypes.hending;
        barrList.Add(oneBarr);
      }
      if (itemsData[i].spiderNet > 0) {
        barrierProbability oneBarr = new barrierProbability();
        oneBarr.value = itemsData[i].spiderNet;
        oneBarr.type = barrierGenerateTypes.stickyFloor;
        barrList.Add(oneBarr);
      }
      if (itemsData[i].stone > 0) {
        barrierProbability oneBarr = new barrierProbability();
        oneBarr.value = itemsData[i].stone;
        oneBarr.type = barrierGenerateTypes.stone;
        barrList.Add(oneBarr);
      }
      if (itemsData[i].stoneAndHanding > 0) {
        barrierProbability oneBarr = new barrierProbability();
        oneBarr.value = itemsData[i].stoneAndHanding;
        oneBarr.type = barrierGenerateTypes.stoneAndHending;
        barrList.Add(oneBarr);
      }
      if (itemsData[i].stoneSkeleton > 0) {
        barrierProbability oneBarr = new barrierProbability();
        oneBarr.value = itemsData[i].stoneSkeleton;
        oneBarr.type = barrierGenerateTypes.stoneSkelet;
        barrList.Add(oneBarr);
      }

      oneBarrier.probability = barrList;
      barrierParam.Add(oneBarrier);
    }
  }

}
