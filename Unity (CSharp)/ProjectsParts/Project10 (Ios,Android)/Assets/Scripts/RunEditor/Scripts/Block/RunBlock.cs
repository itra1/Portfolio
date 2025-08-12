using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace EditRun {

  [Serializable]
  public class RunBlock: BlockBase {

    public EditRunLibrary objectLibrary;
    [SerializeField]
    public List<SpawnObjectReady> genObject = new List<SpawnObjectReady>();
    [SerializeField]
    public List<SpawnObjectReady> saveObject = new List<SpawnObjectReady>();
    private Transform mapParent;
    private Transform lastPositionPlatform;

    private SpawnObjectReady nextObject;
    private Queue<SpawnObjectReady> spawnQueue = new Queue<SpawnObjectReady>();
    private Vector3 spawnPosition;

    public float maxHeight;
    public float minHeight;

    private List<SpawnObjectReady> _allPlatform;
    public override float blockDistantion {
      get {
        if (_allPlatform == null || _allPlatform.Count == 0) {
          _allPlatform = saveObject.FindAll(x => x.spawnObject.spawnType == SpawnType.platform).OrderBy(x => x.position.x).ToList();
        }
        if (_allPlatform.Count == 0) return 0;
        return _allPlatform[_allPlatform.Count - 1].position.x - _allPlatform[0].position.x;
      }
    }

    public override void Initiate() {
      base.Initiate();
      displayDiff = CameraController.Instance.CalcDisplayDiff(0);
      spawnQueue.Clear();
      listPool.Clear();

      List<SpawnObjectReady> spawnList = saveObject.OrderBy(x => x.position.x).ToList();

      for (int j = 0; j < spawnList.Count; j++) {

        SpawnObjectReady inst = spawnList[j].GetClone();
        inst.runPosition = new Vector3(spawnList[j].position.x, spawnList[j].position.y);
        spawnQueue.Enqueue(inst);

      }
      
      for (int j = 0; j < spawnList.Count; j++) {
        if (!listPool.Keys.Contains(spawnList[j].spawnObject.prefab.gameObject.name))
          listPool.Add(spawnList[j].spawnObject.prefab.gameObject.name, new KeyValuePair<GameObject, int>(spawnList[j].spawnObject.prefab.gameObject, 10));
      }

      LevelPooler.Instance.AddPool(POOL_KEY, listPool);
      nextObject = GetNextObject();

    }

    private float startDistance = 0;
    public void Init(float startDistance = 0, bool destroyOld = true) {
      mapParent = GameObject.Find("World").transform;
      mapParent.GetComponent<MapParent>().runBlock = this;
#if UNITY_EDITOR
      Selection.objects = new UnityEngine.Object[] { this };
#endif

      this.startDistance = startDistance;

      if (destroyOld)
        DestroyExistsObjects();
      LoadObject();
    }

    public void SaveData() {
      genObject.RemoveAll(x => x.instance == null);
      //saveObject.ForEach(x=>x.position = x.instance.transform.position);
      saveObject.Clear();

      foreach (var genObjectElem in genObject) {
        SpawnObjectReady el = new SpawnObjectReady();
        el.spawnObject = genObjectElem.spawnObject;
        el.position = genObjectElem.instance.transform.position;
        saveObject.Add(el);
      }

#if UNITY_EDITOR
      EditorUtility.SetDirty(this);
#endif
    }

    public void DeleteObject(bool clearList = false) {

      foreach (var VARIABLE in genObject) {
        if (VARIABLE.instance != null)
          DestroyImmediate(VARIABLE.instance);
      }
      DestroyExistsObjects();

      if (clearList) genObject.Clear();
      if (clearList) saveObject.Clear();
    }

    public void DestroyExistsObjects() {

      while (mapParent.childCount > 0)
        DestroyImmediate(mapParent.GetChild(0).gameObject);
    }

    public override void Ready(float startDistance) {
      base.Ready(startDistance);

      Debug.Log(title);
      Debug.Log(LevelBlockSpawn.lastSpawnDistance);

    }

    public void LoadObject() {
      if (saveObject == null) return;
      foreach (var genObjectElem in saveObject) {
        SpawnObjectEditor(genObjectElem.spawnObject).transform.position = new Vector3(startDistance + genObjectElem.position.x, genObjectElem.position.y);
      }
    }

    public GameObject SpawnObjectEditor(SpawnObjectInfo objectElement) {

#if UNITY_EDITOR
      if (objectElement.spawnType == SpawnType.coin && objectElement.parametrs.Count > 0) {

        Vector3 startPos = new Vector3(SceneView.lastActiveSceneView.pivot.x, SceneView.lastActiveSceneView.pivot.y, 0);

        List<Vector3> allDiff =
          CoinsSpawner.Instance.classicCoinsGroup[int.Parse(objectElement.parametrs.Find(x => x.key == "group").value)].diff;

        foreach (var allDiffElem in allDiff) {
          SpawnObjectInfo newCoins = new SpawnObjectInfo();
          newCoins.spawnType = SpawnType.coin;
          newCoins.prefab = objectElement.prefab;
          newCoins.position = new Vector3(startPos.x + allDiffElem.x, startPos.y + allDiffElem.y, 0);
          SpawnObjectEditor_(newCoins);
        }

        // int.Parse(objectElement.parametrs.Find(x => x.key == "form").value)

      } else if (objectElement.prefab != null)
        return SpawnObjectEditor_(objectElement);
#endif
      return null;
    }

    public override void Update() {
      base.Update();
      Generate();

    }

    private void Generate() {

      while (nextObject != null && nextObject.runPosition.x < CameraController.Instance.transform.position.x + displayDiff.right * 2f) {
        if (nextObject != null) {
          Spawn(nextObject);
        }
        nextObject = GetNextObject();
      }

    }

    private SpawnObjectReady GetNextObject() {

      if (spawnQueue.Count <= 0) {
        Complete(spawnPosition.x);
        return null;
      }

      return spawnQueue.Count > 0 ? spawnQueue.Dequeue() : null;
    }

    private void Spawn(SpawnObjectReady objectElement) {

      spawnPosition = new Vector3((objectElement.runPosition.x) * (GameManager.activeLevelData.moveVector == MoveVector.left && Application.isPlaying ? -1 : 1) + this.startDistance + LevelBlockSpawn.lastSpawnDistance,
                                            objectElement.runPosition.y,
                                            0);

      if (objectElement.spawnObject.spawnType == SpawnType.platform) {
        GameObject platform = PlatformSpawn.Instance.Spawn(objectElement.spawnObject.prefab.gameObject, spawnPosition);
        platform.GetComponent<PlatformDecor>().SetForm(int.Parse(objectElement.spawnObject.parametrs.Find(x => x.key == "form").value));
        return;
      }

      GameObject inst = LevelPooler.Instance.GetPooledObject(POOL_KEY, objectElement.spawnObject.prefab.gameObject.name);
      inst.SetActive(true);

      inst.transform.position = spawnPosition;

      switch (objectElement.spawnObject.spawnType) {
        case SpawnType.platform:
          break;
      }

    }

    public GameObject SpawnObjectEditor_(SpawnObjectInfo objectElement) {

#if UNITY_EDITOR

      GameObject inst = (GameObject)PrefabUtility.InstantiatePrefab(objectElement.prefab.gameObject);

      inst.SetActive(true);
      inst.transform.SetParent(mapParent);

      if (SceneView.lastActiveSceneView != null)
        inst.transform.position = objectElement.position != null ? (Vector3)objectElement.position : new Vector3(SceneView.lastActiveSceneView.pivot.x, SceneView.lastActiveSceneView.pivot.y, 0);

      Transform[] listParent = inst.GetComponentsInChildren<Transform>();

      foreach (var elem in listParent) {
        SelectElement newComp = elem.gameObject.AddComponent<SelectElement>();
        newComp.parentObject = inst;
      }

      Selection.objects = new GameObject[] { inst };

      switch (objectElement.spawnType) {
        case SpawnType.platform:
          inst.GetComponent<PlatformDecor>().SetForm(int.Parse(objectElement.parametrs.Find(x => x.key == "form").value));

          if (lastPositionPlatform != null)
            inst.transform.position = lastPositionPlatform.position + new Vector3(2.5f, 0, 0);
          lastPositionPlatform = inst.transform;

          break;
      }

      SpawnObjectReady newObject = new SpawnObjectReady();
      newObject.spawnObject = objectElement;
      newObject.position = inst.transform.position;
      newObject.instance = inst;

      if (genObject == null) genObject = new List<SpawnObjectReady>();
      genObject.Add(newObject);

      EditorUtility.SetDirty(this);

      return inst;
#else
			return null;
#endif
    }

  }



}