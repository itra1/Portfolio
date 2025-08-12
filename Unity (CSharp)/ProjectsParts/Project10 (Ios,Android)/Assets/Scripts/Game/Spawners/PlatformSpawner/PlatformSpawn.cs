using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawn : SpawnBase<PlatformSpawn> {

  public static System.Action breackGenEnd;

  [Layer]
  public int layerId;

  public GameObject stonePlatformPrefab;
  public GameObject woodPlatformPrefab;
  public GameObject logPlatformPrefab;

  public override string POOL_KEY {
    get {
      return "PLATFORM";
    }
  }

  public static Vector3 lastSpawnPosition;

  protected virtual void Start() { }

  protected virtual void Update() { }

  public virtual GameObject Spawn(GameObject prefab, Vector3 position) {
    return null;
  }
  public virtual GameObject Spawn(Vector3 position, PlatformType targetType = PlatformType.none) {
    return null;
  }

}
