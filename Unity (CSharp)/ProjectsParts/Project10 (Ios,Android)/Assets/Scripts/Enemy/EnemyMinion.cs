using UnityEngine;
using System.Collections;
using Spine.Unity;


/// <summary>
/// Дополнительное управление врагом в качестве миньена
/// </summary>
public class EnemyMinion : MonoBehaviour {

  public enum MinionPhase { idle, tremor, critical }

  public MinionPhase minionPhase;

  [SerializeField]
  SkeletonAnimation skeletonAnimation;           // Анимация Скелета
                                                 /// <summary>
                                                 /// Основной контроллер врага
                                                 /// </summary>
  ClassicEnemy enemy;
  /// <summary>
  /// Простая анимация поддержки
  /// </summary>
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string idleAnim = "";
  /// <summary>
  /// Легкая тряска
  /// </summary>
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string tremorAnim = "";
  /// <summary>
  /// Легкая тряска
  /// </summary>
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string crititcalAnim = "";

  /// <summary>
  /// Анимация бега
  /// </summary>
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string runIdleAnim = "";

  /// <summary>
  /// Непосредственный босс миньена
  /// </summary>
  EnemyBoss boss;
  /// <summary>
  /// Инициализация босса
  /// </summary>
  bool initBoss;

  void OnEnable() {
    enemy = GetComponent<ClassicEnemy>();
    minionPhase = MinionPhase.idle;
  }

  void LateUpdate() {
    if (initBoss) {
      initBoss = false;
      enemy.SetAnimation(runIdleAnim , true);
      enemy.AddAnimation(1,idleAnim , true);
      //enemy.MoveFunction = null;
    }
  }

  public void SetBoss(EnemyBoss newBoss) {
    boss = newBoss;
    boss.OnDeadMinion += DeadMinion;
    initBoss = true;
  }

  void DeadMinion() {

    transform.localPosition += new Vector3(-0.4f , 0f , 0f);

    if (minionPhase == MinionPhase.idle) {
      minionPhase = MinionPhase.tremor;
      enemy.SetAnimation(tremorAnim , true);
    } else if (minionPhase == MinionPhase.tremor) {
      minionPhase = MinionPhase.critical;
      enemy.SetAnimation(crititcalAnim , true);
    }
  }
}
