using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Контроллер магии
/// </summary>
public class MagicSpawner: Singleton<MagicSpawner> {
    
  private float magicStep;
  private float timeStep;

  float nextTimeBullet;
  float bulletThis;
  bool bloomActive;

  //void Start() {
  //  Init();
  //}
    
  /// <summary>
  /// Внешняя функция для инициализации магии
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  public bool Magic(MagicTypes type) {

    switch (type) {
      case MagicTypes.pirats:
        return GetComponent<PiratAbilitySpawner>().Activate();
      case MagicTypes.bullet:
        return GetComponent<BulletAbilitySpawner>().Activate();
    }
    return false;
  }
  


  //[System.Serializable]
  //public struct GenerateParametrs {
  //  public float distantion;
  //  public int count;
  //  public List<Probabitity> param;

  //  [System.Serializable]
  //  public struct Probabitity {
  //    public MagicTypes type;
  //    public float value;
  //  }

  //}

  //Vector3 generatePosition {
  //  get { return new Vector3(CameraController.displayDiff.transform.position.x + CameraController.displayDiff.right * 1.2f, RunnerController.Instance.mapHeight + Random.Range(1f, 2f), 0); }
  //}

  ////public List<GenerateParametrs> generateParametrs;

  //float distantionCalc;               // Дистанция рассчета новой генерации
  //float distantionGenerate;           // Следующая дистанция для генерации
  //float perionGeneration;             // Период генерации
  //List<MagicTypes> generateList;

  //public void Init() {
  //  generateList = new List<MagicTypes>();
  //  distantionCalc = 0;
  //  if(GameManager.activeLevelData.gameMode != GameMode.survival) GetConfig();
  //}

  //void GenerateUpdate() {

  //  Debug.Log("GenerateUpdate");

  //  if(distantionCalc <= RunnerController.playerDistantion) {
  //    GenerateParametrs newPosition = generateParametrs.Find(x => x.distantion > distantionCalc);
  //    CalcDistantions(newPosition);
  //    CalcGenerateList(newPosition);
  //    CalcNextDistanceGenerate();
  //    distantionCalc = newPosition.distantion;
  //  }

  //  if(distantionGenerate >= 0 && distantionGenerate <= RunnerController.playerDistantion && generateList.Count > 0 && CheckReadyGenerate()) {
  //    GenerateBox();
  //    CalcNextDistanceGenerate();
  //  }

  //}

	//private void FixedUpdate() {
	//	if (distantionGenerate >= 0 && distantionGenerate <= RunnerController.playerDistantion && generateList.Count > 0)
	//		CheckReadyGenerate();
	//}
	
	///// <summary>
	///// Проверка возможности генерации
	///// </summary>
	///// <returns></returns>
	//public bool CheckReadyGenerate() {
 //   Collider[] objCol = Physics.OverlapSphere(generatePosition, 6f);

 //   foreach(Collider oneColl in objCol) {
 //     if(oneColl.tag == "RollingStone" | oneColl.tag == "jumpUp" | oneColl.tag == "jumpDown")
 //       return false;
 //   }
 //   return true;
 // }

 // void GenerateBox() {
    
 //   BoxType boxType = BoxType.bullet;

 //   switch (generateList[0]) {
 //     case MagicTypes.bullet:
 //       boxType = BoxType.bullet;
 //       break;
 //     case MagicTypes.pirats:
 //       boxType = BoxType.pirat;
 //       break;
 //   }
    
 //   GameObject obj = BoxSpawner.instance.GenerateBox(BoxCategory.magic,boxType);
 //   obj.transform.parent = BoxSpawner.instance.transform;
 //   obj.transform.position = generatePosition;
 //   obj.SetActive(true);
 //   generateList.RemoveAt(0);
 // }
  
 // /// <summary>
 // /// Рассчет начала генерации и периода
 // /// </summary>
 // /// <param name="parametrs"></param>
 // void CalcDistantions(GenerateParametrs parametrs) {
 //   if(parametrs.count <= 0) return;
 //   distantionGenerate = distantionCalc;
 //   perionGeneration = (parametrs.distantion - distantionCalc) / parametrs.count;
 // }
 // /// <summary>
 // /// Рассчет следующей генерации с учотом периода
 // /// </summary>
 // void CalcNextDistanceGenerate() {
 //   if(generateList.Count > 0)
 //     distantionGenerate += Random.Range(0, perionGeneration);
 //   else
 //     distantionGenerate = -1;
 // }
 // /// <summary>
 // /// Рассчет массива под генерацию
 // /// </summary>
 // /// <param name="parametrs">Структура</param>
 // void CalcGenerateList(GenerateParametrs parametrs) {
 //   // Если присутствуют ящики под генерацию
 //   if(parametrs.count <= 0) return;
 //   generateList.Clear();

 //   // Готовим массив под выбор
 //   List<float> tempCd = new List<float>();

 //   float sum1 = 0;
 //   tempCd.Add(sum1);
 //   foreach(GenerateParametrs.Probabitity one in parametrs.param) {
 //     sum1 += one.value;
 //     tempCd.Add(sum1);
 //   }

 //   int boxCount = parametrs.count;

 //   while(boxCount > 0) {
 //     boxCount--;
 //     int number = BinarySearch.RandomNumberGenerator(tempCd);
 //     generateList.Add(parametrs.param[number].type);
 //   }

 // }
 // void GetConfig() {
 //   List<Configuration.Levels.Magic> itemsData = Config.Instance.config.activeLevel.magic;

 //   generateParametrs = new List<GenerateParametrs>();

 //   for(int i = 0; i < itemsData.Count; i++) {

 //     GenerateParametrs newParam = new GenerateParametrs();
 //     newParam.param = new List<GenerateParametrs.Probabitity>();

 //     newParam.distantion = itemsData[i].distantion;
 //     newParam.count = itemsData[i].magic;

 //     if(itemsData[i].bullets > 0) {
 //       GenerateParametrs.Probabitity prob = new GenerateParametrs.Probabitity();
 //       prob.type = MagicTypes.bullet;
 //       prob.value = itemsData[i].bullets * 0.01f;
 //       newParam.param.Add(prob);
 //     }

 //     if(itemsData[i].taran > 0) {
 //       GenerateParametrs.Probabitity prob = new GenerateParametrs.Probabitity();
 //       prob.type = MagicTypes.pirats;
 //       prob.value = itemsData[i].taran * 0.01f;
 //       newParam.param.Add(prob);
 //     }

 //     generateParametrs.Add(newParam);
 //   }
 // }
}
