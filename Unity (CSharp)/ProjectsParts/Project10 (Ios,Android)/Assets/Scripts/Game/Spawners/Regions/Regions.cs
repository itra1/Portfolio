using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Regions: Singleton<Regions> {
	
	public GameObject cryptoBg;                                           // Фон
	public GameObject cryptoBgFull;                                       // Фон
	public RegionType _type;                                       // Текущая декорация
  public static RegionType type {
    get {
      return Instance._type;
    }
    set {
      Instance._type = value;
      ExEvent.RunEvents.RegionChange.CallAsync(Instance._type);
    }
  }

  protected virtual void Start() {
    Init();
  }

  public virtual void Init() {
    SetRegion();
  }
  
  protected virtual void SetRegion() {

  }

}

/// <summary>
/// Тип участка карты
/// </summary>
[Flags]
[System.Serializable]
public enum RegionType {
  Crypt,
  Holes,
  Jungle,
  Ship,
  ShipRoom,
  Forest,
  None
}