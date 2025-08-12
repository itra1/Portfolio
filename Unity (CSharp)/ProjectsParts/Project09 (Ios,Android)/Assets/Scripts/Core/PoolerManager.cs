using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Пулер общих объектов
/// 
/// Предназначен для оптимизации памяти по созданию объектов
/// </summary>
public class PoolerManager : MonoBehaviour {

	public static PoolerManager instance;
	private Dictionary<string,List<GameObject>> pooledList = new Dictionary<string,List<GameObject>>();

  [System.Serializable]
  public struct PoolObjects {
    public      string          name;                   // Имя используется для поиска
    public      GameObject      prefab;                 // Префаб объекта
    public      int             cacheSize;              // Количество генерируемых при старте
  }
	
  [SerializeField]
  PoolObjects[] poolObjects;

  void Start() {
    instance = this;
    CreatePool();
  }

  void OnDestroy() {
    instance = null;
  }

  /// <summary>
  /// Создание стартового числа объектов по размеру кеша
  /// </summary>
  public void CreatePool() {
    foreach(PoolObjects one in poolObjects) {
      List<GameObject> subPool = new List<GameObject>();

      for(int i = 0; i < one.cacheSize; i++) {
        GameObject obj = (GameObject)Instantiate(one.prefab);
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        subPool.Add(obj);
      }
      pooledList.Add(one.prefab.name, subPool);
    }
  }

	/// <summary>
	/// Добавление в пул
	/// </summary>
	/// <param name="prefab">Префаб</param>
	/// <param name="cashSize">Количество</param>
	public void AddPool(GameObject prefab, int cashSize = 1) {

		if(pooledList.ContainsKey(prefab.name)) return;

		List<GameObject> subPool = new List<GameObject>();

		for(int i = 0; i < cashSize; i++) {
			GameObject obj = (GameObject)Instantiate(prefab);
			obj.SetActive(false);
			obj.transform.SetParent(transform);
			subPool.Add(obj);
		}
		pooledList.Add(prefab.name, subPool);
	}
  
  /// <summary>
  /// Возвращает объект пула.
  /// 
  /// При необходимости увеличивает размер кеша
  /// </summary>
  /// <param name="nameObject">Имея объекта в структуре poolObjects</param>
  /// <returns>Объект из пула</returns>
  public GameObject GetPooledObject_(string nameObject) {

    for(int j = 0; j < pooledList[nameObject].Count; j++) {
      if(!pooledList[nameObject][j].activeInHierarchy)
        return pooledList[nameObject][j];
    }

    if(pooledList[nameObject] != null) {
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
    return instance.GetPooledObject_(nameObject);
  }
}
