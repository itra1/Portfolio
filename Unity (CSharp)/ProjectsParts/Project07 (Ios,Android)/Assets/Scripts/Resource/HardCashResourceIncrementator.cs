using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardCashResourceIncrementator: ResourceIncrementatorBehaviour {
  public override void Increment(float value) {
    UserManager.Instance.HardCash += (int)value;
  }
}
