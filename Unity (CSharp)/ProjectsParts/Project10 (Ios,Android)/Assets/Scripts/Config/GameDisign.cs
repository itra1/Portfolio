using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Configuration {

  public class GameDisign {

    public List<EnemyParam> enemy;
    public List<Levels.Level> levels;
    public Survivles.Survival survival;

    public Levels.Level activeLevel {
      get {
        return levels[GameManager.activeLevel];
      }
    }
    
  }

}