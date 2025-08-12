using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Пулер общих объектов
/// 
/// предназначен для оптимизации памяти по созданию объектов
/// </summary>
public class PoolerManager : Singleton<PoolerManager>
{

	private Dictionary<string, PoolObjectCache> pool = new Dictionary<string, PoolObjectCache>();

	public PoolObjectCache[] caches;

	protected void Awake()
	{

		for (int i = 0; i < caches.Length; i++)
		{
			caches[i].Initialize();
			pool.Add(caches[i].prefab.name, caches[i]);
		}

		CachesFill();
	}

	public static void AddPool(GameObject prefab, int initCache = 1, int cache = 10, Transform parent = null)
	{
		Instance.AddObjectToPool(prefab, initCache, cache, parent);
	}

	public static void AddPool(PoolObjectCache poolObject)
	{
		Instance.AddObjectToPool(poolObject.prefab, poolObject.initialCacheSize, poolObject.cacheSize, poolObject.parent);
	}

	void AddObjectToPool(GameObject prefab, int initCache = 1, int cache = 10, Transform parent = null)
	{
		if (prefab == null)
		{
			it.Logger.LogError("Ошибка добавления префаба в пул");
			return;
		}

		if (pool.ContainsKey(prefab.name)) return;

		var sourcePrefab = Instantiate(prefab, transform);
		sourcePrefab.SetActive(false);
		sourcePrefab.name = prefab.name;

		PoolObjectCache cacheObject = new PoolObjectCache();
		cacheObject.prefab = sourcePrefab;
		cacheObject.initialCacheSize = initCache;
		cacheObject.cacheSize = cache;
		cacheObject.parent = parent;

		cacheObject.Initialize();
		pool.Add(prefab.name, cacheObject);
		CachesFill(cacheObject);
	}

	public static GameObject Spawn(string prefabName)
	{
		PoolObjectCache cache = FindCacheByName(prefabName);
		if (cache == null)
			return null;

		GameObject obj = cache.GetNextObjectInCache();
		obj.SetActive(true);

		return obj;
	}

	public static void Return(string prefabName, GameObject go)
	{
		PoolObjectCache cache = FindCacheByName(prefabName);
		if (cache == null)
			return;
		go.SetActive(false);

		cache.Return(go);
	}

	private static PoolObjectCache FindCacheByName(string prefabName)
	{
		if (Instance.pool.ContainsKey(prefabName))
			return Instance.pool[prefabName];

		//it.Logger.LogError("Trying get not cached object");
		return null;
	}

	IEnumerator CheckCachesFill(PoolObjectCache poolObject = null)
	{
		bool spawnExists = true;

		while (spawnExists)
		{
			yield return new WaitForSeconds(0.3f);
			spawnExists = false;

			if (poolObject != null)
			{
				if (poolObject.GetSize() < poolObject.cacheSize)
				{
					poolObject.SpawnOne();
					spawnExists = true;
				}
			}
			else
			{
				for (var i = 0; i < caches.Length; i++)
				{
					if (caches[i].GetSize() < caches[i].cacheSize)
					{
						caches[i].SpawnOne();
						spawnExists = true;
					}
				}
			}

		}
	}

	void CachesFill(PoolObjectCache poolObject = null)
	{
		StartCoroutine(CheckCachesFill(poolObject));

	}
}

[System.Serializable]
public class PoolObjectCache
{
	public GameObject prefab;
	public int cacheSize = 10;
	public int initialCacheSize = 0;
	public int currentCacheSize = 0;
	public Transform parent;
	public bool debugLog = false;
	public bool doNotOverrideTransformAfterReturn = false;

	private List<ObjectInst> objects = new List<ObjectInst>();
	private int cacheIndex = 0;

	public class ObjectInst
	{
		public GameObject Go;
		public bool IsLock;
	}

	public void Initialize()
	{
		for (var i = 0; i < initialCacheSize; i++)
		{
			SpawnOne();
		}
	}

	public int GetSize()
	{
		return objects.Count;
	}

	public ObjectInst SpawnOne()
	{
		GameObject go = (GameObject)MonoBehaviour.Instantiate(prefab);
		go.name = prefab.name;
		go.SetActive(false);
		//go.name = go.name + cacheIndex.ToString();
		if (parent != null)
			go.transform.SetParent(parent);
		else
		{
			go.transform.SetParent(PoolerManager.Instance.transform);
		}
		var n = new ObjectInst { Go = go, IsLock = false };
		objects.Add(n);
		cacheIndex++;
		currentCacheSize++;
		return n;
	}

	public void Return(GameObject go)
	{
		go.SetActive(false);
		if (!doNotOverrideTransformAfterReturn)
		{
			//go.name = go.name + cacheIndex.ToString();
			if (parent != null && parent != go.transform.parent)
				go.transform.SetParent(parent);
			else
				go.transform.SetParent(PoolerManager.Instance.transform);
		}
		foreach (var elem in objects)
		{
			if (elem.IsLock && elem.Go == go)
				elem.IsLock = false;
		}

		//objects.AddLast(go);
		//currentCacheSize++;
		//cacheIndex++;
	}


	public GameObject GetNextObjectInCache()
	{
		//GameObject obj = null;

		var dt = objects.Find(x => !x.IsLock);

		if (dt == null)
		{
			if (debugLog)
				it.Logger.LogWarning("Spawner is empty, instantiating new object in runtime");
			dt = SpawnOne();
		}
		GameObject obj = dt.Go;
		dt.IsLock = true;

		//obj = objects.First.Value;
		//objects.RemoveFirst();
		//currentCacheSize--;
		return obj;
	}
}

