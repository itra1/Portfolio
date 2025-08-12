using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneDownfallBlock : BlockBase {

  public GameObject prefab;

  public override void Initiate() {
    base.Initiate();

    listPool.Clear();
    listPool.Add(prefab.name, new KeyValuePair<GameObject, int>(prefab, 7));

    LevelPooler.Instance.AddPool(POOL_KEY, listPool);

  }

}
