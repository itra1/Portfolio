using UnityEngine;
using System.Collections;
using Spine.Unity;

namespace Pet {

  /// <summary>
  /// Пет динозавр
  /// </summary>
  public class DinoPet: Pet {

    /// <summary>
    /// Звук атаки
    /// </summary>
    public AudioClip attackClip;

    [SpineAnimation(dataField: "skeletonAnimation")]
    public string attackAnim = "";
    [SpineAnimation(dataField: "skeletonAnimation")]
    public string jumpAnim = "";

    /// <summary>
    /// Объект не соприкосается с землей
    /// </summary>
    protected override void NotGroundActive() {
      base.NotGroundActive();
      SetAnimation(jumpAnim, true);
    }

    protected override void OnTriggerEnter2D(Collider2D col) {
      base.OnTriggerEnter2D(col);

      if (col.gameObject.tag == "Enemy") {
        if (col.transform.position.x > transform.position.x && phase == Phase.uses) {
          if (shotTime > Time.time) {
            col.GetComponent<Enemy>().Damage(WeaponTypes.pet, 100, transform.position, DamagePowen.level1, 0);
          }
        }
      }

    }

    /// <summary>
    /// Время выполнения атаки
    /// </summary>
    float shotTime;
    /// <summary>
    /// Время ожидания атаки
    /// </summary>
    float shotTimeWait;

    /// <summary>
    /// .Выполнение атаки
    /// </summary>
    public override void Shoot() {
      if (phase != Phase.uses)
        return;
      if (shotTimeWait <= Time.time) {
        shotTimeWait = Time.time + 0.7f;
        shotTime = Time.time + 0.5f;
        AddAnimation(1, attackAnim, false, 0);
        AudioManager.PlayEffect(attackClip, AudioMixerTypes.runnerEffect);
      }
    }

  }
}