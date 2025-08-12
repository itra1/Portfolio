using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.Jack;

namespace Road.Spawners.Pets {

  public class PetSpawnerSurvival: PetSpawner {

    [System.Serializable]
    public struct GenerateParametrs {
      [System.Serializable]
      public struct Probability {
        [Range(0, 1)]
        public float probility;
        public PetsTypes probability;
      }

      public FloatSpan distance;
      public float randDistance {
        get {
          return Random.Range(distance.min, distance.max);
        }
      }
      public Probability[] probability;

      public float nextDistance;     // Следующая дистанция генерации
      public PetsTypes nextType; // Следующий сгенерированный тип
    }

    private Collider[] objCol;
    private bool checkConfirm;

    private GenerateParametrs nextGenerate;
    public Queue<GenerateParametrs> generateQueue = new Queue<GenerateParametrs>();

    protected override void Start() {
      base.Start();
      OnChangeLevel();
      CalcNextGenerate();
    }

    void GenerateRun() {

      if (nextGenerate.nextDistance > 0 && RunnerController.playerDistantion >= nextGenerate.nextDistance) {
        objCol = Physics.OverlapBox(new Vector3(CameraController.displayDiff.rightDif(1.5f), CameraController.displayDiff.transform.position.y, transform.position.z), new Vector3(2, 2, 2));

        checkConfirm = true;

        foreach (Collider oneColl in objCol) {
          if (System.Array.IndexOf(new string[] { "Barrier", "jumpUp", "jumpDown", "Hending", "RollingStone" }, oneColl.tag) >= 0)
            checkConfirm = false;
        }

        if (checkConfirm) {
          GeneratePet();
          CalcNextGenerate();
        } else {
          nextGenerate.nextDistance += 5;
        }

      }

    }

    void OnChangeLevel() {
      GetConfig();
    }

    private void Update() {
      if (RunnerController.Instance.runnerPhase != RunnerPhase.run) return;
      GenerateRun();
    }

    void CalcNextGenerate() {

      do {

        if (generateQueue.Count > 0)
          nextGenerate = generateQueue.Dequeue();
        else
          InfinityIncrement();

      } while (!CalcProbability());

    }

    private bool CalcProbability() {
      float sum = 0;
      List<float> gen = new List<float>();
      gen.Add(sum);

      List<PetsTypes> tmp = new List<PetsTypes>();

      for (int i = 0; i < nextGenerate.probability.Length; i++) {
        if (nextGenerate.probability[i].probability == PetsTypes.dino)
          nextGenerate.probability[i].probility += 0.03f * levelDino;
        if (nextGenerate.probability[i].probability == PetsTypes.spider)
          nextGenerate.probability[i].probility += 0.03f * levelSpider;
        if (nextGenerate.probability[i].probability == PetsTypes.bat)
          nextGenerate.probability[i].probility += 0.03f * levelBat;

        tmp.Add(nextGenerate.probability[i].probability);
        sum += nextGenerate.probability[i].probility;
        gen.Add(sum);
      }

      if (sum < 1) {
        sum += 1 - sum;
        tmp.Add(PetsTypes.none);
      }

      PetsTypes[] readyPets = tmp.ToArray();

      gen = gen.ConvertAll(x => x / sum);

      int needNum = BinarySearch.RandomNumberGenerator(gen);

      nextGenerate.nextType = readyPets[needNum];

      if (nextGenerate.nextType != PetsTypes.none) {
        nextGenerate.nextDistance = nextGenerate.randDistance;
        return true;
      }
      return false;
    }

    [ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.LevelChange))]
    private void LevelChange(ExEvent.GameEvents.LevelChange eventData) {
      GetConfig();
    }
    
    private void GeneratePet() {
      GameObject obj = new GameObject();

      if (nextGenerate.nextType == PetsTypes.dino) {
        obj = Pooler.GetPooledObject("DinoPet");
        obj.transform.position = new Vector3(CameraController.displayDiff.rightDif(1.5f), RunnerController.Instance.mapHeight + 0.2f, transform.position.z);
      }

      if (nextGenerate.nextType == PetsTypes.spider) {
        obj = Pooler.GetPooledObject("SpiderPet");
        obj.transform.position = new Vector3(CameraController.displayDiff.rightDif(1.5f), RunnerController.Instance.mapHeight + 0.2f, transform.position.z);
      }

      if (nextGenerate.nextType == PetsTypes.bat) {
        obj = Pooler.GetPooledObject("BatPet");
        obj.transform.position = new Vector3(CameraController.displayDiff.rightDif(1.5f), CameraController.displayDiff.topDif(0.5f), transform.position.z);
      }

      RunnerController.petActivate(nextGenerate.nextType, false);

      obj.transform.parent = transform;
      obj.SetActive(true);

    }

    /// <summary>
    /// Бесконечный инкремент параметров
    /// </summary>
    private void InfinityIncrement() {
      nextGenerate.distance.min = nextGenerate.distance.max;
      nextGenerate.distance.max = nextGenerate.distance.min + 200;
    }

    public void GetConfig() {

      List<Configuration.Survivles.Item> itemsData = Config.Instance.config.survival.items;
      Configuration.Survivles.Item oneItem;
      Configuration.Survivles.Item oneItemAlter;
      int startCol = 1;
      float distance = 0;
      float distanceLast = 0;
      bool isEnd = false;

      generateQueue.Clear();

      for (int i = 0; i < itemsData.Count; i++) {
        if (i < startCol || isEnd) continue;

        oneItem = itemsData[i];
        oneItemAlter = itemsData[i - 1];

        int arraySize = 0 + ((oneItemAlter.spider > 0) ? 1 : 0)
                          + (oneItem.dino > 0 ? 1 : 0)
                          + (oneItemAlter.bat > 0 ? 1 : 0);

        if (arraySize == 0) continue;

        GenerateParametrs oneBarrier = new GenerateParametrs();

        try {
          distance = oneItem.dist;
          distanceLast = distance;
        } catch {
          distance = distanceLast + 200;
          isEnd = true;
        }

        oneBarrier.distance.min = distance;

        if (i < itemsData.Count - 2)
          oneBarrier.distance.max = Config.Instance.config.levels[GameManager.activeLevel + 1].distance;

        if (i == itemsData.Count - 1)
          oneBarrier.distance.max = oneBarrier.distance.min + 200;

        oneBarrier.probability = new GenerateParametrs.Probability[arraySize];

        int arrNum = 0;
        if (oneItemAlter.spider > 0) {
          oneBarrier.probability[arrNum].probability = PetsTypes.spider;
          oneBarrier.probability[arrNum].probility = oneItemAlter.spider * 0.01f;
          arrNum++;
        }
        if (oneItem.dino > 0) {
          oneBarrier.probability[arrNum].probability = PetsTypes.dino;
          oneBarrier.probability[arrNum].probility = oneItemAlter.dino * 0.01f;
          arrNum++;
        }
        if (oneItemAlter.bat > 0) {
          oneBarrier.probability[arrNum].probability = PetsTypes.bat;
          oneBarrier.probability[arrNum].probility = oneItemAlter.bat * 0.01f;
          arrNum++;
        }

        generateQueue.Enqueue(oneBarrier);
      }
    }

  }
}