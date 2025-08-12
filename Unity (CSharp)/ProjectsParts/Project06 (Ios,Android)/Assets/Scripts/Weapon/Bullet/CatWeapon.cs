using UnityEngine;

namespace Game.Weapon {

  /// <summary>
  /// Контроллер кота
  /// </summary>
  public class CatWeapon: Bullet {

    /// <summary>
    /// Свободен
    /// </summary>
    private bool isFree;
    /// <summary>
    /// Ссылка на графческую часть
    /// </summary>
    [SerializeField]
    private GameObject sprite;

    /// <summary>
    /// Правка смешения при ударе
    /// </summary>
    private Vector3 transformCorrect;

    /// <summary>
    /// Враг, которому наночится повреждение
    /// </summary>
    private Transform damageObj;

    /// <summary>
    /// Время начала атаки
    /// </summary>
    private float timeStartDamage;

    /// <summary>
    /// Выполняется аттака
    /// </summary>
    private bool isAttack;

    /// <summary>
    /// Полное время нанесерия дамага
    /// </summary>
    private float damageAllTime;

    /// <summary>
    /// Промежутки времени между повторным нанесением дамага
    /// </summary>
    private float damagePeriodTime;

    /// <summary>
    /// Время последнего дамага
    /// </summary>
    private float timeLastDamage;



    public override void OnEnable() {
      base.OnEnable();
      sprite.transform.localScale = new Vector3(1f, 1f, 1f);
      isAttack = false;
    }

    public override void Update() {
      if (!isAttack && !isFree) {
        base.Update();
        return;
      }

      if (isAttack && !isFree && damageObj != null) {
        DPSDamage(damageObj.gameObject);
        transform.position = damageObj.position + transformCorrect;
      }

      if (isFree) {
        FreeMove();
      }
    }

    /// <summary>
    /// Поворот объекта по движению
    /// </summary>
    public override void Rotation() {
      angleRotation = Vector3.Angle(Vector3.up, velocity);
      transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, -angleRotation + 90);
    }

    /// <summary>
    /// Освобождаем кота
    /// </summary>
    private void SetFreeCat() {
      isFree = true;
      isAttack = false;
      transform.localEulerAngles = Vector3.zero;
      sprite.transform.localScale = new Vector3(-1f, 1f, 1f);
    }

    /// <summary>
    /// Нанесение повреждения
    /// </summary>
    /// <param name="objDamage"></param>
    private void DPSDamage(GameObject objDamage) {

      if (timeStartDamage + damageAllTime >= Time.time) {
        if (timeLastDamage + damagePeriodTime <= Time.time) {
          timeLastDamage = Time.time;
          DamageEnemy(objDamage);
        }
      } else
        SetFreeCat();
    }

    /// <summary>
    /// Движение после освобождения
    /// </summary>
    private void FreeMove() {
      velocity.x = -6;
      if (transform.position.y > groundLevel) {
        velocity.y -= 9 * Time.deltaTime;

      } else
        velocity.y = 0f;

      transform.position += velocity * Time.deltaTime;
    }

    /// <summary>
    /// Событие удара о землю
    /// </summary>
    public override void OnGround() {
      SetFreeCat();
    }

    public override void OnTriggerEnter2D(Collider2D collEnt) {
      if (isFree)
        return;

      if (LayerMask.LayerToName(collEnt.gameObject.layer) == "Enemy") {
        if (damageObj == null) {
          collEnt.GetComponent<Enemy>().SubscribeDead(EnemyDead);
          timeStartDamage = Time.time;
          isAttack = true;


          damageObj = collEnt.transform;
          transformCorrect = transform.position - collEnt.transform.position;
        }
      }
    }

    /// <summary>
    /// Событие смерти врага
    /// </summary>
    private void EnemyDead(Enemy enemy) {
      SetFreeCat();
    }

    public override void GetConfig() {
      base.GetConfig();

      damageAllTime = wep.param1.Value;
      damagePeriodTime = wep.param2.Value;
      _damageValue = wep.param3.Value;

      //damageAllTime = float.Parse(( string )config["DamageTime"]);
      //damagePeriodTime = float.Parse(( string )config["DamagePeriod"]);
    }

  }


}