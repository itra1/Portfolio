using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(CoinsSpawner))]
public class CoinsSpawnerEditor: Editor {

  int group;

  public override void OnInspectorGUI() {
    base.OnInspectorGUI();

    group = EditorGUILayout.IntField("Group:", group);

    if (GUILayout.Button("Generate group")) {
      ((CoinsSpawner)target).EditorGenerate(group);
    }

  }
}

#endif

[System.Serializable]
public struct coinsProbability {
  public int nomination;
  public float probability;
}
[System.Serializable]
public struct CoinsPositing {
  public List<Vector3> diff;
}

[System.Serializable]
public struct coinsGenerateParamerts {
  public coinsProbability[] chance;
}

/// <summary>
/// Генерация монет
/// </summary>
public class CoinsSpawner: Singleton<CoinsSpawner> {

  //private List<Transform> generateList = new List<Transform>();
  public coinsGenerateParamerts[] coinsParametr;
  private readonly List<KeyValuePair<GameObject, int>> coinsList; //Список
  public int[] coinsNomination;
  private Vector3 genPosition;
  private List<float> wpn;

  public List<CoinsPositing> classicCoinsGroup;
  public LayerMask layer2Level;
  private RegionType region;

  public GameObject coinPrefab;

  void Start() {
    UpdateValue();
  }

  void UpdateValue() {
    if (GameManager.activeLevelData.gameMode != GameMode.levelsConstructor)
      ProcessesCoins();
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.RunEvents.RegionChange))]
  void ChangeMap(ExEvent.RunEvents.RegionChange region) {
    this.region = region.newType;
  }

  public static void GenOneMonetToPlayer(Vector3 pos, int value) {
    GameObject obj = Pooler.GetPooledObject(Instance.coinPrefab.name);
    obj.transform.position = pos;
    obj.SetActive(true);
    obj.GetComponent<Coin>().SetNomination(value);
    obj.GetComponent<Coin>().GoToPlayer();
  }

  /// <summary>
  /// Подготовка структуры для генерации
  /// </summary>
  void ProcessesCoins() {
    float sum = 0;
    wpn = new List<float>();
    wpn.Add(sum);

    List<int> gen = new List<int>();

    int coinNom = PlayerPrefs.GetInt("coinsPerk", 0);

    for (int i = 0; i < coinsParametr[coinNom].chance.Length; i++) {
      gen.Add(coinsParametr[coinNom].chance[i].nomination);
      sum += coinsParametr[coinNom].chance[i].probability;
      wpn.Add(sum);
    }

    coinsNomination = gen.ToArray();

    wpn = wpn.ConvertAll(x => x / sum);
  }

  // Функция генерации блока монет
  [ExEvent.ExEventHandler(typeof(ExEvent.RunEvents.CoinsGenerate))]
  void GenerateCoinsGroup(ExEvent.RunEvents.CoinsGenerate coinsGenerate) {

    genPosition = coinsGenerate.position;
    if (coinsGenerate.barrierType == 2)
      GenerateMoney2();
    else if (coinsGenerate.barrierType == 1)
      GenerateMoney1();
    else
      GenerateMoney();
  }


#if UNITY_EDITOR

  public void EditorGenerate(int num) {
    genPosition = new Vector3(SceneView.lastActiveSceneView.pivot.x, SceneView.lastActiveSceneView.pivot.y, 0);
    GenerateCoinsGroup(num);
  }

  public void SaveGroup() {
    Coin[] listObj = FindObjectsOfType<Coin>();

    CoinsPositing group = new CoinsPositing();
    group.diff = new List<Vector3>();

    foreach (Coin one in listObj)
      group.diff.Add(one.gameObject.transform.position - new Vector3(SceneView.lastActiveSceneView.pivot.x, SceneView.lastActiveSceneView.pivot.y, 0));

    classicCoinsGroup.Add(group);

  }

  public void CleartGroup() {
    Coin[] listObj = FindObjectsOfType<Coin>();
    foreach (Coin one in listObj)
      DestroyImmediate(one.gameObject);
  }

#endif

  void GenerateCoinsGroup(int num) {

    float startY = genPosition.y;
    float startX = genPosition.x;

    foreach (Vector3 diffPos in classicCoinsGroup[num].diff)
      Spawn(new Vector2(startX + diffPos.x, startY + diffPos.y));
  }

  void GenerateMoney(int rnd = -1) {

    if (region == RegionType.ShipRoom) {
      if (Physics.CheckSphere(genPosition, 8f, layer2Level)) return;

      int[] coinsGroup = new int[] { 0, 1, 2, 7, 13, 14, 27, 28 };

      rnd = coinsGroup[Random.Range(0, coinsGroup.Length)];
      GenerateCoinsGroup(rnd);

    } else {

      if (Physics.CheckSphere(genPosition, 8f, layer2Level)) return;

      rnd = Random.Range(0, 27);
      GenerateCoinsGroup(rnd);

      if (rnd < 0) rnd = Random.Range(1, 28);
    }
  }

  void GenerateMoney1() {
    int rnd = Random.Range(0, 2);
    GenerateCoinsGroup(rnd);
  }

  void GenerateMoney2() {
    GenerateCoinsGroup(1);
  }

  public void Spawn(Vector2 pos, int num = 0, bool toPlayer = false) {

    if (!Application.isPlaying) {

      GameObject obj = Instantiate(coinPrefab);
      obj.transform.position = pos;
      obj.SetActive(true);
      obj.GetComponent<Coin>().SetNomination(1);
    } else {
      GameObject obj = Pooler.GetPooledObject(Instance.coinPrefab.name);
      obj.transform.position = pos;
      obj.SetActive(true);
      obj.GetComponent<Coin>().SetNomination(coinsNomination[BinarySearch.RandomNumberGenerator(wpn)]);
      if (toPlayer) obj.GetComponent<Coin>().GoToPlayer();
    }
  }

  readonly float checkTimeDeactive = 5;
  void Update() {
    // скрываем объекты, если вышли из зоны видимости

    //if(checkTimeDeactive <= Time.time) {
    //  checkTimeDeactive = Time.time + 5;
    //  for(int i = 0; i < generateList.Count; i++) {
    //    if(generateList[i].gameObject.activeInHierarchy &&
    //        (generateList[i].position.x < CameraController.displayDiff.leftDif(5)
    //        || generateList[i].position.x > CameraController.displayDiff.rightDif(5))) {
    //      generateList[i].gameObject.SetActive(false);
    //    }
    //  }
    //}

  }

}
