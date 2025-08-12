using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUi : ExEvent.EventBehaviour {

  public List<GameObject> shootLightEffect;

  private Animator _animator;
  [HideInInspector]
  public Animator animator {
    get {
      if (_animator == null)
        _animator = GetComponent<Animator>();
      return _animator;
    }
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.WeaponShoot))]
  public void EnemyClick(ExEvent.BattleEvents.WeaponShoot eventData) {
    animator.Play("shoot");
  }
}
