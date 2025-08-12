using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyResourceIncrementator: ResourceIncrementatorBehaviour {
  public override void Increment(float value) {
    UserEnergy.Instance.energy += value;
  }
}
