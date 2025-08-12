using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Config: Singleton<Config> {
  
  public List<ResourceIncrementatorBehaviour> playerResource;

  public enum ResourceType {
    gold,
    energy,
    bulletPistol,
    bulletObrez,
    bulletUzi,
    bulletAutomat,
    hardCache,
    weapon,
    pumpGun,
    mp5Gun
  }

}
