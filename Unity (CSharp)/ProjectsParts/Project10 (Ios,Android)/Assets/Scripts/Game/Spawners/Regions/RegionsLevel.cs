using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionsLevel : Regions {


  public override void Init() {
    base.Init();
    SetRegion();
  }

  /// <summary>
  /// Инициализация стартового реиона
  /// </summary>
  protected override void SetRegion() {
    base.SetRegion();

    type = GameManager.activeLevelData.region;

  }
}
