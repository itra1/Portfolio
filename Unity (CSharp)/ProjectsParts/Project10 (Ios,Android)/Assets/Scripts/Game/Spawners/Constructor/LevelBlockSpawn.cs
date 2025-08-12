using System.Collections;
using System.Linq;
using System.Collections.Generic;
using EditRun;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Выполнить поиск
/// </summary>
public class LevelBlockSpawn: Singleton<LevelBlockSpawn> {

  public float spawnBlockDistance;

  private const string POOL_KEY = "levels";

  private RunnerPhase runnerPhase;

  private BlockBase _activeBlock;
  private BlockBase activeBlock {
    get {
      return _activeBlock;
    }
    set {
      _activeBlock = value;
    }
  }
  private Queue<BlockBase> blockQueue = new Queue<BlockBase>();

  [HideInInspector]
  public DisplayDiff displayDiff;                    // Смещение видимой части

  private static int blockNum = -1;

  public static float lastSpawnDistance;
  private float startDistanceBlock;

  private void Start() {
    blockNum = -1;
    displayDiff = CameraController.Instance.CalcDisplayDiff(transform.position.z);

    blockQueue.Clear();
    listPool.Clear();

    lastSpawnDistance = (GameManager.activeLevelData.moveVector == MoveVector.left && Application.isPlaying ? -30 : 0);
    startDistanceBlock = lastSpawnDistance;

    //PoolGenerate();
    //LoadQueue(GameManager.activeLevelData);
    GameManager.activeLevelData.runBlocks.ForEach(x => x.Initiate());
    NextBlock();

    LevelPooler.Instance.AddPool(POOL_KEY, listPool);
    //activeBlock = GetNextObject();
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.RunEvents.RunPhaseChange))]
  private void RunPhaseChange(ExEvent.RunEvents.RunPhaseChange eventData) {
    runnerPhase = eventData.newPhase;
  }

  private void InitiateBlocks() {
    GameManager.activeLevelData.runBlocks.ForEach(x => x.Initiate());
  }

  //private void PoolGenerate() {

  //  for (int i = 0; i < GameManager.activeLevelData.runBlocks.Count; i++) {

  //    List<SpawnObjectReady> spawnList = GameManager.activeLevelData.runBlocks[i].saveObject.OrderBy(x => x.position.x).ToList();

  //    for (int j = 0; j < spawnList.Count; j++) {
  //      if (!listPool.Keys.Contains(spawnList[j].spawnObject.prefab.gameObject.name))
  //        listPool.Add(spawnList[j].spawnObject.prefab.gameObject.name, new KeyValuePair<GameObject, int>(spawnList[j].spawnObject.prefab.gameObject, 10));
  //    }
  //  }
  //}

  private bool NextBlock() {

    blockNum++;

    if (GameManager.activeLevelData.runBlocks.Count <= blockNum) return false;

    activeBlock = GameManager.activeLevelData.runBlocks[blockNum];
    startDistanceBlock += GameManager.activeLevelData.runBlocks[blockNum].blockDistantion + 3f;

    activeBlock.Ready(startDistanceBlock);

    activeBlock.onComplete = () => {
      Debug.Log("On next");
      NextBlock();
    };

    return true;

    //List<SpawnObjectReady> spawnList = GameManager.activeLevelData.runBlocks[blockNum].saveObject.OrderBy(x => x.position.x).ToList();

    //for (int j = 0; j < spawnList.Count; j++) {

    //  SpawnObjectReady inst = spawnList[j].GetClone();
    //  inst.runPosition = new Vector3(spawnList[j].position.x + startDistanceBlock, spawnList[j].position.y);
    //  spawnQueue.Enqueue(inst);

    //}
    //startDistanceBlock += GameManager.activeLevelData.runBlocks[blockNum].blockDistantion + 3f;
    //return true;

  }

  //private void LoadQueue(LevelData generateLevel) {

  //  for (int i = 0; i < generateLevel.runBlocks.Count; i++) {

  //    List<SpawnObjectReady> spawnList = generateLevel.runBlocks[i].saveObject.OrderBy(x => x.position.x).ToList();

  //    for (int j = 0; j < spawnList.Count; j++) {

  //      SpawnObjectReady inst = spawnList[j].GetClone();
  //      inst.runPosition = new Vector3(spawnList[j].position.x + startDistanceBlock, spawnList[j].position.y);
  //      spawnQueue.Enqueue(inst);

  //      if (!listPool.Keys.Contains(spawnList[j].spawnObject.prefab.gameObject.name))
  //        listPool.Add(spawnList[j].spawnObject.prefab.gameObject.name, new KeyValuePair<GameObject, int>(spawnList[j].spawnObject.prefab.gameObject, 10));
  //    }

  //    startDistanceBlock += generateLevel.runBlocks[i].blockDistantion + 3f;

  //  }

  //}

  private void Update() {
    activeBlock.Update();

    //if (activeObject == null) return;
    //Generate();
  }

  //private void Generate() {

  //  while (activeObject != null && activeObject.runPosition.x < CameraController.Instance.transform.position.x + displayDiff.right * 2f) {
  //    if (activeObject != null) {
  //      Spawn(activeObject);
  //    }
  //    activeObject = GetNextObject();
  //  }

  //}

  private Vector3 spawnPosition;

  private void Spawn(SpawnObjectReady objectElement) {

    spawnPosition = new Vector3((objectElement.runPosition.x) * (GameManager.activeLevelData.moveVector == MoveVector.left && Application.isPlaying ? -1 : 1),
                                          objectElement.runPosition.y,
                                          0);

    if(objectElement.spawnObject.spawnType == SpawnType.platform) {
      GameObject platform = PlatformSpawn.Instance.Spawn(objectElement.spawnObject.prefab.gameObject, spawnPosition);
      platform.GetComponent<PlatformDecor>().SetForm(int.Parse(objectElement.spawnObject.parametrs.Find(x => x.key == "form").value));
      return;
    }

    GameObject inst = LevelPooler.Instance.GetPooledObject(POOL_KEY,objectElement.spawnObject.prefab.gameObject.name);
    inst.SetActive(true);
    inst.transform.SetParent(transform);

    inst.transform.position = spawnPosition;

    switch (objectElement.spawnObject.spawnType) {
      case SpawnType.platform:
        break;
    }

  }

  private BlockBase GetNextObject() {

    //if (blockQueue.Count <= 0 && !NextBlock()) return null;

    return blockQueue.Count > 0 ? blockQueue.Dequeue() : null;
  }

  public void DestroyObjects() {
    while (transform.childCount > 0) {
      if (Application.isPlaying)
        Destroy(transform.GetChild(0).gameObject);
      else
        DestroyImmediate(transform.GetChild(0).gameObject);
    }
  }

  private static void SpawnBlock(float startBlock, float startRegion, float endRegion, RunBlock runBlock) {
    List<SpawnObjectReady> readySpawn =
      runBlock.saveObject.FindAll(x => x.position.x >= startRegion - startBlock && x.position.x <= endRegion + 2.5f - startBlock);

    foreach (var spawnObjectElement in readySpawn) {
      SpawnObjectEditor(spawnObjectElement, startBlock);
    }
  }


  public static void SpawnObjectEditor(SpawnObjectReady objectElement, float startBlock) {

    //return;

    //GameObject inst = Instantiate(objectElement.spawnObject.prefab.gameObject);
    //inst.SetActive(true);
    //inst.transform.SetParent(transform);

    //inst.transform.position = new Vector3((objectElement.position.x + startBlock) * (GameManager.activeLevelData.moveVector == MoveVector.left && Application.isPlaying ? -1 : 1),
    //                                      objectElement.position.y,
    //                                      0);

    //switch (objectElement.spawnObject.spawnType) {
    //  case SpawnType.platform:
    //    inst.GetComponent<PlatformDecor>().SetForm(int.Parse(objectElement.spawnObject.parametrs.Find(x => x.key == "form").value));
    //    break;
    //}

  }

  Dictionary<string, KeyValuePair<GameObject, int>> listPool = new Dictionary<string, KeyValuePair<GameObject, int>>();
  //public void LoadActiveScene(LevelData levelData) {
  //  StartCoroutine(SpawnObjectCorotine(levelData));
  //}

#if UNITY_EDITOR

  public static void SpawnEditor(LevelData generateLevel) {
    //IEnumerator e = SpawnObjectCorotine(generateLevel);
    //while (e.MoveNext()) ;
  }

  #endif

  //  static IEnumerator SpawnObjectCorotine(LevelData generateLevel = null) {

  //    LevelData activeLevel = generateLevel;

  //    activeLevel = activeLevel ?? GameManager.activeLevelData;

  //    if (activeLevel.gameMode != GameMode.levelsConstructor || activeLevel.runBlocks.Count == 0) yield break;

  //    float startRegionDistance = (GameManager.activeLevelData.moveVector == MoveVector.left && Application.isPlaying ? -30 : 0);
  //    float startBlockDistance = startRegionDistance;
  //    float endBlockDistance = 0;
  //    float maxDistantion = activeLevel.levelDistantion;
  //    int blockIndex = 0;

  //    while (endBlockDistance < maxDistantion) {

  //      RunBlock runBlock = blockIndex < activeLevel.runBlocks.Count ? activeLevel.runBlocks[blockIndex] : null;

  //      if (runBlock == null) yield break;

  //      float distanceGenerate = Mathf.Min(100, runBlock.blockDistantion + startBlockDistance - startRegionDistance);

  //      endBlockDistance = startRegionDistance + distanceGenerate;

  //      SpawnBlock(startBlockDistance, startRegionDistance, endBlockDistance, runBlock);

  //      if (endBlockDistance >= startBlockDistance + distanceGenerate) {
  //        blockIndex++;
  //        endBlockDistance += 2.5f;
  //        startBlockDistance = endBlockDistance;
  //      }

  //      yield return null;
  //      startRegionDistance = endBlockDistance;
  //    }

  //  }

}
