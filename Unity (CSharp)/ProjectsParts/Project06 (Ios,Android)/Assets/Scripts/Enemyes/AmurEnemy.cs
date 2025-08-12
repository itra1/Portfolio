using UnityEngine;
using System.Collections;

public class AmurEnemy : Enemy {

  /// <summary>
  /// Позиция для генерации сняряда
  /// </summary>
  public Transform transformGenerateBullet;



  protected override void Awake() {
    base.Awake();
  }

  public override void Update() {
    base.Update();

    if(phase == Phase.dead) return;

    if(phase != Phase.dead && phase != Phase.attack && transform.position.x <= 0) {
      SetPhase(Phase.attack);
    }

  }

  #region Аттака

  /// <summary>
  /// Название префаба из пулер менеджера
  /// </summary>
  public string bulletPrefab;
	
  /// <summary>
  /// Выполнение актаки
  /// </summary>
  public override void Attack() {
    base.Attack();

    SetPhase(Phase.attack);

    if(attackLastTime + attackWaitTime <= Time.time) {
      ShootBulletInstantiate();
    }

  }

  /// <summary>
  /// Генерация жкземпляра
  /// </summary>
  void ShootBulletInstantiate() {

    GameObject bulletInstance = PoolerManager.Spawn(bulletPrefab);
    bulletInstance.transform.position = transformGenerateBullet.position;
    bulletInstance.GetComponent<AmurBullet>().SetParametrs(0, 0);
    bulletInstance.SetActive(true);

    attackLastTime = Time.time;
  }

  #endregion
  
}
