using UnityEngine;

namespace Game.Weapon {


  /// <summary>
  /// Контроллер кирпича
  /// </summary>
  public class BrickWeapon: Bullet {
    protected override void SpawnGroundSfx() {

      int count = Random.Range(5, 8);
      for (int i = 0; i < count; i++)
        base.SpawnGroundSfx();
    }
  }

}