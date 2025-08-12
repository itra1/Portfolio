using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBase : ScriptableObject {

  public System.Action onComplete;

  public string title;
  public BlockType type;

  [HideInInspector]
  public DisplayDiff displayDiff;                    // Смещение видимой части

  protected Dictionary<string, KeyValuePair<GameObject, int>> listPool = new Dictionary<string, KeyValuePair<GameObject, int>>();

  public virtual float blockDistantion { get { return 0; } }

  public enum BlockType {
    objects,
    stoneDownfall,
    arrowAttack,
    boost
  }

  public string POOL_KEY {
    get {
      return "RunBlock";
    }
  }

  public void Complete(float position) {
    LevelBlockSpawn.lastSpawnDistance = position + 2.5f;
    if (onComplete != null) onComplete();
  }

  public virtual void Initiate() { }

  public virtual void Ready(float startDistance) { }

  public virtual void Update() { }
}
