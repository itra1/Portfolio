using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceIncrementatorBehaviour : MonoBehaviour {

  public string title;

  public Config.ResourceType type;
  public Sprite iconMini;
  public Sprite iconBig;

  public abstract void Increment(float value);

}
