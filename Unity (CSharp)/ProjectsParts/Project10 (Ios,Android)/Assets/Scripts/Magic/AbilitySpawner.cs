using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySpawner: MonoBehaviourBase {

  public MagicTypes magicType;
  protected bool isActive;
  protected int step;
  protected float timeStep;
  public GameObject prefab;

  public virtual bool Activate() {
    isActive = true;
    Player.Jack.PlayerController.Instance.MagicAttack();
    RunnerController.Instance.AddBlockChangeMap(this);
    Invoke(Play, 1);
    return true;
  }

  public virtual bool Deactive() {
    isActive = false;
    magicType = MagicTypes.none;
    RunnerController.Instance.RemoveBlockChangeMap(this);
    return true;
  }

  protected virtual void Play() {
    step = 0;
  }

}


// Типы магии
public enum MagicTypes {
  none,                                               // Отсутствие магии
  bullet,                                             // Снаряд
  pirats                                              // Пираты
}