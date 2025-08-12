using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformConstructorSpawn: PlatformSpawn {
  
  protected override void Start() {
    base.Start();

    List<KeyValuePair<GameObject, int>> prefList = new List<KeyValuePair<GameObject, int>>() {
      new KeyValuePair<GameObject, int>(stonePlatformPrefab ,20),
      new KeyValuePair<GameObject, int>(woodPlatformPrefab, 10),
      new KeyValuePair<GameObject, int>(logPlatformPrefab, 10)
    };
    LevelPooler.Instance.AddPool(POOL_KEY, LevelPooler.Instance.CreateStructure(prefList, out cd));

  }

  public override GameObject Spawn(GameObject prefab, Vector3 position) {

    GameObject inst = LevelPooler.Instance.GetPooledObject(POOL_KEY, prefab.gameObject.name);
    inst.SetActive(true);
    inst.transform.position = position;
    return inst;
  }

}
