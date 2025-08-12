using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldResourceIncrementator: ResourceIncrementatorBehaviour {
  public override void Increment(float value) {
    UserManager.Instance.Gold += (int)value;
  }
}
