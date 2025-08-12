using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpawn: SpawnBase<WallSpawn> {
  public override string POOL_KEY {
    get {
      return "wall";
    }
  }

  [Layer]
  public int layer;

  public GameObject prefab;

  protected virtual void Start() { }

  protected virtual void Update() { }

  public virtual GameObject Spawn(GameObject prefab, Vector3 position) {
    return null;
  }
  public virtual GameObject Spawn(Vector3 position, PlatformType targetType = PlatformType.none) {
    return null;
  }

}
