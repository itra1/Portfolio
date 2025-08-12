using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Road.Spawners.Pets {

  public class PetSpawnerLevels: PetSpawner {

    /// <summary>
    /// Параметры дистанции
    /// </summary>
    [System.Serializable]
    public class GenerateParametrs {
      public float distantion;
      public int count;
      public List<BoxProbability> param;

      /// <summary>
      /// Вероятность генерации
      /// </summary>
      [System.Serializable]
      public struct BoxProbability {
        public Player.Jack.PetsTypes type;
        public float value;
      }

      public float distanceGenerate;
      public Player.Jack.PetsTypes typeGenerate;
    }

    /// <summary>
    /// Позиция генерации
    /// </summary>
    Vector3 generatePosition {
      get { return new Vector3(CameraController.displayDiff.transform.position.x + CameraController.displayDiff.right * 1.2f, RunnerController.Instance.mapHeight + Random.Range(1f, 2f), 0); }
    }

    private GenerateParametrs nextGenerate;
    public Queue<GenerateParametrs> generateParametrs = new Queue<GenerateParametrs>(); // Параметры генерации

    protected override void Start() {
      base.Start();

      GetConfig();
      CalcNextGenerate();

    }

    public void Update() {

      if (RunnerController.Instance.runnerPhase != RunnerPhase.run || nextGenerate == null) return;

      if (nextGenerate.distanceGenerate <= RunnerController.playerDistantion && CheckReadyGenerate()) {
        Generate();
        CalcNextGenerate();
      }

    }

    [ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.LevelChange))]
    private void LevelChange(ExEvent.GameEvents.LevelChange eventData) {
      GetConfig();
    }

    public void GeneratePet() {
      GameObject obj = new GameObject();

      if (nextGenerate.typeGenerate == Player.Jack.PetsTypes.dino) {
        obj = Pooler.GetPooledObject("DinoPet");
        obj.transform.position = new Vector3(CameraController.displayDiff.rightDif(1.5f), RunnerController.Instance.mapHeight + 0.2f, transform.position.z);
      }

      if (nextGenerate.typeGenerate == Player.Jack.PetsTypes.spider) {
        obj = Pooler.GetPooledObject("SpiderPet");
        obj.transform.position = new Vector3(CameraController.displayDiff.rightDif(1.5f), RunnerController.Instance.mapHeight + 0.2f, transform.position.z);
      }

      if (nextGenerate.typeGenerate == Player.Jack.PetsTypes.bat) {
        obj = Pooler.GetPooledObject("BatPet");
        obj.transform.position = new Vector3(CameraController.displayDiff.rightDif(1.5f), CameraController.displayDiff.topDif(0.5f), transform.position.z);
      }

      RunnerController.petActivate(nextGenerate.typeGenerate, false);

      obj.transform.parent = transform;
      obj.SetActive(true);

    }

    void Generate() {
      GeneratePet();
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

    void CalcNextGenerate() {

      do {

        if (generateParametrs.Count > 0)
          nextGenerate = generateParametrs.Dequeue();
        else {
          nextGenerate = null;
          break;
        }

      } while (!CalcProbability());

    }
    private bool CalcProbability() {

      // Если присутствуют ящики под генерацию
      if (nextGenerate.count <= 0) return false;

      // Готовим массив под выбор
      List<float> tempCd = new List<float>();

      float sum1 = 0;
      tempCd.Add(sum1);
      foreach (GenerateParametrs.BoxProbability one in nextGenerate.param) {
        sum1 += one.value;
        tempCd.Add(sum1);
      }

      int number = BinarySearch.RandomNumberGenerator(tempCd);
      nextGenerate.typeGenerate = nextGenerate.param[number].type;
      return true;

    }

    /// <summary>
    /// Бесконечный инкремент (на всякий случай)
    /// </summary>
    void InfinityIncrement() {
      nextGenerate.distantion += 200;
    }

    /// <summary>
    /// Получение настроек
    /// </summary>
    public void GetConfig() {

      List<Configuration.Levels.Pet> itemsData = Config.Instance.config.activeLevel.pets;

      generateParametrs.Clear();

      for (int i = 0; i < itemsData.Count; i++) {
        GenerateParametrs newParam = new GenerateParametrs();
        newParam.param = new List<GenerateParametrs.BoxProbability>();

        newParam.distantion = itemsData[i].distantion;
        newParam.count = (int)itemsData[i].pets;

        if (itemsData[i].bat > 0) {
          GenerateParametrs.BoxProbability box = new GenerateParametrs.BoxProbability();
          box.type = Player.Jack.PetsTypes.bat;
          box.value = itemsData[i].bat * 0.01f;
          newParam.param.Add(box);
        }

        if (itemsData[i].dino > 0) {
          GenerateParametrs.BoxProbability box = new GenerateParametrs.BoxProbability();
          box.type = Player.Jack.PetsTypes.dino;
          box.value = itemsData[i].dino * 0.01f;
          newParam.param.Add(box);
        }

        if (itemsData[i].spider > 0) {
          GenerateParametrs.BoxProbability box = new GenerateParametrs.BoxProbability();
          box.type = Player.Jack.PetsTypes.spider;
          box.value = itemsData[i].spider * 0.01f;
          newParam.param.Add(box);
        }

        generateParametrs.Enqueue(newParam);
      }

    }


  }

}