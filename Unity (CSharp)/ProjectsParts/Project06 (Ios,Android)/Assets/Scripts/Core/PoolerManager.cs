using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Пулер общих объектов
/// 
/// предназначен для оптимизации памяти по созданию объектов
/// </summary>
public class PoolerManager : Singleton<PoolerManager> {

  private Dictionary<string, PoolObjectCache> pool = new Dictionary<string, PoolObjectCache>();
	
	public PoolObjectCache[] caches;

	protected override void Awake() {
		base.Awake();

		for (int i = 0; i < caches.Length; i++) {
			caches[i].Initialize();
			pool.Add(caches[i].prefab.name, caches[i]);
		}

		CachesFill();
	}

	public static void AddPool(GameObject prefab, int initCache = 1, int cache = 10, Transform parent = null) {
		Instance.AddObjectToPool(prefab, initCache, cache, parent);
	}

	public static void AddPool(PoolObjectCache poolObject) {
		Instance.AddObjectToPool(poolObject.prefab, poolObject.initialCacheSize, poolObject.cacheSize, poolObject.parent);
	}
	
	void AddObjectToPool(GameObject prefab, int initCache = 1, int cache = 10, Transform parent = null) {
		if (prefab == null) {
			Debug.LogError("Ошибка добавления префаба в пул");
			return;
		}

		if (pool.ContainsKey(prefab.name)) return;

		PoolObjectCache cacheObject = new PoolObjectCache();
		cacheObject.prefab = prefab;
		cacheObject.initialCacheSize = initCache;
		cacheObject.cacheSize = cache;
		cacheObject.parent = parent;

		cacheObject.Initialize();
		pool.Add(prefab.name, cacheObject);
		CachesFill(cacheObject);
	}

	public static GameObject Spawn(string prefabName) {
		PoolObjectCache cache = FindCacheByName(prefabName);
		if (cache == null)
			return null;
		
		GameObject obj = cache.GetNextObjectInCache();
		//obj.SetActive(true);

		return obj;
	}

	public static void Return(string prefabName, GameObject go) {
		PoolObjectCache cache = FindCacheByName(prefabName);
		if (cache == null)
			return;

		cache.Return(go);
	}

	private static PoolObjectCache FindCacheByName(string prefabName) {
		if (Instance.pool.ContainsKey(prefabName))
			return Instance.pool[prefabName];

		Debug.LogError("Trying get not cached object");
		return null;
	}
	
	IEnumerator CheckCachesFill(PoolObjectCache poolObject = null) {
		bool spawnExists = true;

		while (spawnExists) {
			yield return new WaitForSeconds(0.3f);
			spawnExists = false;

			if (poolObject != null) {
				if (poolObject.GetSize() < poolObject.cacheSize) {
					poolObject.SpawnOne();
					spawnExists = true;
				}
			} else {
				for (var i = 0; i < caches.Length; i++) {
					if (caches[i].GetSize() < caches[i].cacheSize) {
						caches[i].SpawnOne();
						spawnExists = true;
					}
				}
			}

		}
	}

	void CachesFill(PoolObjectCache poolObject = null) {
		StartCoroutine(CheckCachesFill(poolObject));

	}

}


[System.Serializable]
public class PoolObjectCache {
	public GameObject prefab;
	public int cacheSize = 10;
	public int initialCacheSize = 0;
	public int currentCacheSize = 0;
	public Transform parent;
	public bool debugLog = false;
	public bool doNotOverrideTransformAfterReturn = false;

	private List<GameObject> objects = new List<GameObject>();
	private int cacheIndex = 0;

	public void Initialize() {

		for (var i = 0; i < initialCacheSize; i++) {
			SpawnOne();
		}
	}

	public int GetSize() {
		return objects.Count;
	}

	public GameObject SpawnOne() {
		GameObject go = (GameObject)MonoBehaviour.Instantiate(prefab);

		go.SetActive(false);
		go.name = go.name + cacheIndex.ToString();
		if (parent != null)
			go.transform.SetParent(parent);
		else {
			go.transform.SetParent(PoolerManager.Instance.transform);
		}
		objects.Add(go);
		cacheIndex++;
		currentCacheSize++;
		return go;
	}

	public void Return(GameObject go) {
		go.SetActive(false);
		if (!doNotOverrideTransformAfterReturn) {
			//go.name = go.name + cacheIndex.ToString();
			if (parent != null && parent != go.transform.parent)
				go.transform.SetParent(parent);
		}
		//objects.AddLast(go);
		//currentCacheSize++;
		//cacheIndex++;
	}


	public GameObject GetNextObjectInCache() {
		//GameObject obj = null;

		GameObject obj = objects.Find(x => !x.activeInHierarchy);


		if (objects.Count == 0 || obj == null) {
			if (debugLog)
				Debug.LogWarning("Spawner is empty, instantiating new object in runtime");
			obj = SpawnOne();
		}

		//obj = objects.First.Value;
		//objects.RemoveFirst();
		//currentCacheSize--;
		return obj;
	}
}

