using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelPooler: Singleton<LevelPooler> {

  public Dictionary<string, GenerateData> objectLibrary = new Dictionary<string, GenerateData>();

  protected override void OnDestroy() {
    base.OnDestroy();
  }

  [System.Serializable]
  public class GenerateData {
    public Transform parent;
    public Dictionary<string, List<GameObject>> poolObjects;
    public List<List<GameObject>> poolList;
  }

  /// <summary>
  /// Добавление элементов в пул генерации, с создание пула по необходимости
  /// </summary>
  /// <param name="sourceName"></param>
  /// <param name="list"></param>
  /// <param name="instanceList"></param>
  public void AddPool(string sourceName, Dictionary<string, KeyValuePair<GameObject, int>> list, List<Transform> instanceList = null) {

    GenerateData genSource = objectLibrary.ContainsKey(sourceName) ? objectLibrary[sourceName] : null;

    if (genSource == null)
      genSource = CreatePoolData(sourceName);

    foreach (string entry in list.Keys) {

      if (genSource.poolObjects.ContainsKey(entry))
        continue;

      list[entry].Key.gameObject.SetActive(false);
      List<GameObject> subPool = new List<GameObject>();

      for (int i = 0; i < list[entry].Value; i++) {
        GameObject obj = (GameObject)Instantiate(list[entry].Key, genSource.parent);
        obj.SetActive(false);
        subPool.Add(obj);

        if (instanceList != null) instanceList.Add(obj.transform);
      }

      genSource.poolObjects.Add(entry, subPool);
    }

  }

  public void AddPool(string sourceName, List<KeyValuePair<GameObject, int>> list, List<Transform> instanceList = null) {

    GenerateData genSource = objectLibrary.ContainsKey(sourceName) ? objectLibrary[sourceName] : null;

    if (genSource == null)
      genSource = CreatePoolData(sourceName);

    foreach (KeyValuePair<GameObject, int> entry in list) {

      if (genSource.poolList.Exists(x => x.Exists(y => y.gameObject.Equals(entry.Key))))
        continue;

      List<GameObject> subPool = new List<GameObject>();
      entry.Key.SetActive(false);
      for (int i = 0; i < entry.Value; i++) {
        GameObject obj = (GameObject)Instantiate(entry.Key, genSource.parent);

        obj.SetActive(false);

        subPool.Add(obj);
        if (instanceList != null) instanceList.Add(obj.transform);
      }

      genSource.poolList.Add(subPool);
    }

  }
  
  private GenerateData CreatePoolData(string sourceName) {

    GenerateData data =  new GenerateData() {
        parent = (new GameObject()).transform,
        poolObjects = new Dictionary<string, List<GameObject>>(),
        poolList = new List<List<GameObject>>()
      };

    data.parent.transform.parent = transform;
#if UNITY_EDITOR
    data.parent.name = sourceName;
#endif

    objectLibrary.Add(sourceName, data);

    return data;

  }

  public GameObject GetPooledObject(string sourceName, string name, List<Transform> listTram = null, bool canGrow = true) {

    GenerateData genData = objectLibrary[sourceName];

    if (genData == null)
      throw new System.Exception("No data");

    for (int j = 0; j < genData.poolObjects[name].Count; j++) {
      if (!genData.poolObjects[name][j].activeInHierarchy)
        return genData.poolObjects[name][j];
    }

    if (canGrow) {
      GameObject obj = Instantiate(genData.poolObjects[name][0]);
      obj.SetActive(false);
      obj.transform.parent = genData.poolObjects[name][0].transform.parent;
      genData.poolObjects[name].Add(obj);
      if (listTram != null) listTram.Add(obj.transform);
      return obj;
    }

    return null;
  }

  public GameObject GetPooledObject(string sourceName, int i, List<Transform> listTram = null, bool canGrow = true) {

    GenerateData genData = objectLibrary[sourceName];

    if (genData == null)
      throw new System.Exception("No data");

    for (int j = 0; j < genData.poolList[i].Count; j++) {
      if (!genData.poolList[i][j].activeInHierarchy)
        return genData.poolList[i][j];
    }

    if (canGrow) {
      GameObject obj = (GameObject)Instantiate(genData.poolList[i][0]);
      obj.transform.parent = genData.poolList[i][0].transform.parent;
      genData.poolList[i].Add(obj);
      if (listTram != null && !listTram.Contains(obj.transform))
        listTram.Add(obj.transform);

      return obj;
    }

    return null;
  }

  public Transform GetTransformParent(string sourceName) {
    return objectLibrary[sourceName].parent;
  }

  public void DeactiveAll() {

    GenerateData genData = objectLibrary[name];

    if (genData == null)
      throw new System.Exception("No data");

    foreach (string entry in genData.poolObjects.Keys)
      genData.poolObjects[entry].ForEach(x => x.gameObject.SetActive(false));

  }

  public Dictionary<string, KeyValuePair<GameObject, int>> CreateStructure(List<GameObject> objectList, out List<float> cd) {

    List<KeyValuePair<GameObject, int>> objectPrefabList = new List<KeyValuePair<GameObject, int>>();

    objectList.ForEach(elem => {
      objectPrefabList.Add(new KeyValuePair<GameObject, int>(elem, 1));
    });

    return CreateStructure(objectPrefabList, out cd);
  }

  public Dictionary<string, KeyValuePair<GameObject, int>> CreateStructure(List<KeyValuePair<GameObject, int>> objectList, out List<float> cd) {

    Dictionary<string, KeyValuePair<GameObject, int>> prefabList = new Dictionary<string, KeyValuePair<GameObject, int>>();
    cd = new List<float>();

    float sum1 = 0;

    cd.Add(sum1);

    for (int i = 0; i < objectList.Count; i++) {
      prefabList.Add(objectList[i].Key.name, new KeyValuePair<GameObject, int>(objectList[i].Key, objectList[i].Value));
      sum1 += objectList[i].Value;
      cd.Add(sum1);
    }

    return prefabList;
  }

}
