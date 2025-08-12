using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Структура для пулинга
/// </summary>
[System.Serializable]
public struct PoolObject {
  public GameObject prefab;           // Структура пула
  public int bufferSize;              // Количество, инициализируется в pool
}
/// <summary>
/// Генератор объектов
/// </summary>
public class Pool : MonoBehaviour {

  private Dictionary<string,List<GameObject>> pooledObjects = new Dictionary<string, List<GameObject>>();
  

  /// <summary>
  /// Создание структуры для генерации
  /// </summary>
  /// <param name="list">Список объектов</param>
  /// <param name="parent">родитель для объектов</param>
  /// <param name="cachуSize">Количество объектов</param>
  /// <param name="listTram">Список для контроля извне</param>
  public void CreatePool(Dictionary<string,KeyValuePair<GameObject,int>> list , Transform parent , int cachуSize = 1 , List<Transform> listTram = null) {
    foreach(string entry in list.Keys) {
			if (pooledObjects.ContainsKey(entry))
				continue;
      List<GameObject> subPool = new List<GameObject>();
      for(int i = 0 ; i < list[entry].Value ; i++) {
        GameObject obj = (GameObject)Instantiate(list[entry].Key);
        obj.SetActive(false);
        obj.transform.parent = parent;
        subPool.Add(obj);
        if (listTram != null) listTram.Add(obj.transform);
      }
      pooledObjects.Add(entry , subPool);
    }
  }
  
  public void CreatePool(List<KeyValuePair<GameObject , int>> list , Transform parent , int cachуSize = 1 , List<Transform> listTram = null) {

    Dictionary<string,KeyValuePair<GameObject,int>> tmp = new Dictionary<string, KeyValuePair<GameObject, int>>();

    for (int i = 0 ; i < list.Count ; i++)
      tmp.Add(i.ToString() , new KeyValuePair<GameObject , int>(list[i].Key , list[i].Value));

    CreatePool(tmp , parent , cachуSize , listTram);

  }

  public GameObject GetPooledObject(int numObject , List<Transform> listTram = null , bool canGrow = true) {
    return GetPooledObject(numObject.ToString() , listTram , canGrow);
  }
  /// <summary>
  /// Запрос на получение объекта из пула
  /// </summary>
  /// <param name="nameObject">Имя</param>
  /// <param name="listTram">Структура для контроля извне</param>
  /// <param name="canGrow">Разрешение на создание новых объектов, в случае если пул весь используется</param>
  /// <returns></returns>
  public GameObject GetPooledObject(string nameObject , List<Transform> listTram = null , bool canGrow = true) {
    for (int j = 0 ; j < pooledObjects[nameObject].Count ; j++) {
      if (!pooledObjects[nameObject][j].activeInHierarchy)
        return pooledObjects[nameObject][j];
    }

    if (canGrow) {
      GameObject obj = (GameObject)Instantiate(pooledObjects[nameObject][0]);
      obj.transform.parent = pooledObjects[nameObject][0].transform.parent;
      pooledObjects[nameObject].Add(obj);
      if (listTram != null) listTram.Add(obj.transform);
      return obj;
    }

    return null;
  }

  /// <summary>
  /// Отключение все объектов в пуле
  /// </summary>
  public void DeactiveAll() {
    foreach(string ent in pooledObjects.Keys) {
      for (int j = 0 ; j < pooledObjects[ent].Count ; j++) {
        pooledObjects[ent][j].SetActive(false);
      }
    }

  }

	public void DestroyAll() {
		foreach (string ent in pooledObjects.Keys) {
			for (int j = 0; j < pooledObjects[ent].Count; j++) {
				Destroy(pooledObjects[ent][j]);
			}
		}
		pooledObjects.Clear();
	}
  
}
