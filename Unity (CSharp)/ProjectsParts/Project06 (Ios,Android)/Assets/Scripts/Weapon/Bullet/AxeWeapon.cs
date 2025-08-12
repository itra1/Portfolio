using UnityEngine;

namespace Game.Weapon {

  /// <summary>
  /// Контроллер топора
  /// </summary>
  /// </summary>
  public class AxeWeapon: Bullet {
    private bool move;

    public override void OnEnable() {
      move = true;
      base.OnEnable();
    }


    public override void Update() {

      if (!isActive)
        return;
      if (!move)
        return;
      if (transform.position.y <= groundLevel) {
        isActive = false;
        move = false;
        OnGround();

        return;
      }

      Move();
      Rotation();

      ///если оружие ушло вправо за экран дестроить
      if (transform.position.x > CameraController.rightPoint.x + 1f)
        DeactiveThis(null);
      return;

    }

    protected override void DamageEnemy(GameObject enemy) {
      PlayDamageBodyAudio();
      base.DamageEnemy(enemy);
      enemy.GetComponent<Enemy>().SetBleeding(timeWorkBloodOfAxe, periodWorKBlood, damageBlood);
    }



    public LayerMask maskEnemy;

    public AudioBlock damageBodyAudioBlock;

    protected void PlayDamageBodyAudio() {
      damageBodyAudioBlock.PlayRandom(this);
    }

    #region Настройки
    private float timeWorkBloodOfAxe;
    private float periodWorKBlood;
    private int damageBlood;

    public override void GetConfig() {
      base.GetConfig();

      timeWorkBloodOfAxe = wep.param1.Value;
      periodWorKBlood = wep.param2.Value;
      damageBlood = (int)wep.param3.Value;

      //if (config.ContainsKey("timeWorkFlameOfFire"))
      //  timeWorkFlameOfFire = float.Parse((string)config["timeWorkFlameOfFire"]);
      //if (config.ContainsKey("periodWorKFlameOfFire"))
      //  periodWorKFlameOfFire = float.Parse((string)config["periodWorKFlameOfFire"]);
      //if (config.ContainsKey("damageFlameOfFire"))
      //  damageFlameOfFire = float.Parse((string)config["damageFlameOfFire"]);

    }

    #endregion
  }


}