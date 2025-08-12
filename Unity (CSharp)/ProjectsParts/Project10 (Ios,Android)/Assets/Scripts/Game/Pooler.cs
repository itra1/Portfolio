using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Пулер общих объектов
/// 
/// предназначен для оптимизации памяти по созданию объектов
/// </summary>
public class Pooler : Singleton<Pooler> {

	private Dictionary<string, List<GameObject>> pooledList = new Dictionary<string, List<GameObject>>();
	
	[SerializeField]
	private PoolObjects[] poolObjects;

  private Transform parentPoolObjects;

	void Start() {

    parentPoolObjects = transform.Find("PoolsObject");
    if(parentPoolObjects == null) {
      GameObject fld = new GameObject();
      fld.transform.SetParent(transform);
      fld.transform.localPosition = Vector3.zero;
      fld.name = "PoolsObject";
      parentPoolObjects = fld.transform;
    }

		CreatePool();
		StartCoroutine(CacheProgress());
	}
	
	/// <summary>
	/// Создание стартового числа объектов по размеру кеша
	/// </summary>
	public void CreatePool() {

		for (int i = 0; i < poolObjects.Length; i++) {
			List<GameObject> subPool = new List<GameObject>();

			for (int j = 0; j < poolObjects[i].cacheStart; j++) {
				poolObjects[i].prefab.SetActive(false);
				GameObject obj = (GameObject)Instantiate(poolObjects[i].prefab);
				obj.SetActive(false);
				obj.transform.SetParent(parentPoolObjects);
				poolObjects[i].cache++;
				subPool.Add(obj);
			}
			pooledList.Add(poolObjects[i].name, subPool);
		}
	}

	public void AddPool(GameObject inst, int count) {

		if (pooledList.ContainsKey(inst.name))
			return;

		List<GameObject> subPool = new List<GameObject>();
		for (int i = 0; i < count; i++) {
			GameObject obj = (GameObject)Instantiate(inst);
			obj.SetActive(false);
			obj.transform.SetParent(parentPoolObjects);
			subPool.Add(obj);
		}
		pooledList.Add(inst.name, subPool);
	}


	/// <summary>
	/// Возвращает объект пула.
	/// 
	/// При необходимости увеличивает размер кеша
	/// </summary>
	/// <param name="nameObject">Имея объекта в структуре poolObjects</param>
	/// <returns>Объект из пула</returns>
	public GameObject GetPooledObject_(string nameObject) {

		for (int j = 0; j < pooledList[nameObject].Count; j++) {
			if (!pooledList[nameObject][j].activeInHierarchy)
				return pooledList[nameObject][j];
		}

		if (pooledList[nameObject] != null) {
			GameObject obj = Instantiate(pooledList[nameObject][0]);
			obj.transform.SetParent(pooledList[nameObject][0].transform.parent);
			pooledList[nameObject].Add(obj);
			return obj;
		}
		return null;
	}

	/// <summary>
	/// Статичный эквивалент
	/// </summary>
	/// <param name="nameObject"></param>
	/// <returns></returns>
	public static GameObject GetPooledObject(string nameObject) {
		return Instance.GetPooledObject_(nameObject);
	}

	IEnumerator CacheProgress() {

		bool exit = false;

		while (!exit) {
			exit = true;
			for (int i = 0; i < poolObjects.Length; i++) {

				if (poolObjects[i].cache >= poolObjects[i].cacheSize) continue;
				exit = false;

				GameObject obj = (GameObject)Instantiate(poolObjects[i].prefab);
				obj.SetActive(false);
				obj.transform.SetParent(parentPoolObjects);
				poolObjects[i].cache++;

				pooledList[poolObjects[i].name].Add(obj);
			}
			yield return new WaitForSeconds(0.1f);
		}
	}

}

[System.Serializable]
public class PoolObjects {
	public string name;                   // Имя используется для поиска
	public GameObject prefab;             // Префаб объекта
	public int cacheStart;                // Стартовый размер пула
	public int cacheSize;                 // Количество генерируемых при старте
	public int cache;                 // Количество генерируемых при старте
}