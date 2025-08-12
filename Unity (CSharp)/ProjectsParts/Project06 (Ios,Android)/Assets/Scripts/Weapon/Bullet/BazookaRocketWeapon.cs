using UnityEngine;

namespace Game.Weapon {

  /// <summary>
  /// Снаряд базуки
  /// </summary>
  public class BazookaRocketWeapon: Bullet {

    public ParticleSystem emis1;
    public ParticleSystem emis2;
    public ParticleSystem emis3;


    /// <summary>
    /// Целевой объект
    /// </summary>
    private Transform targetObject;

    /// <summary>
    /// Слой с целью
    /// </summary>
    public LayerMask targetMask;

    /// <summary>
    /// Скорость движения
    /// </summary>
    //public float speed;

    /// <summary>
    /// Скорость изменения вектора направления
    /// </summary>
    public float vectorChangeSpeed;

    /// <summary>
    /// Радиус наносимых повреждений
    /// </summary>
    public float damageRadius;


    public GameObject boomPrefabs;

    public override void Start() {
      // GetConfig();
    }

    public override void OnEnable() {
      base.OnEnable();
      groundLevel = DecorationManager.Instance.loaderLocation.roadSize.min;

      targetObject = null;

      velocity = new Vector3(5, 3, 0);

      ParticleSystem.EmissionModule em1 = emis1.emission;
      em1.enabled = true;
      ParticleSystem.EmissionModule em2 = emis2.emission;
      em2.enabled = true;
      ParticleSystem.EmissionModule em3 = emis3.emission;
      em3.enabled = true;

    }

    private void OnDisable() {
      targetObject = null;
    }

    /// <summary>
    /// Движение
    /// </summary>
    public override void Move() {
      if (targetObject != null) {
        velocity += (targetObject.position - transform.position).normalized * vectorChangeSpeed;
      }
      transform.position += velocity.normalized * speed * Time.deltaTime;
      //rigidBody.velocity = velocity.normalized * speed;
      //rigidBody.MovePosition(transform.position + velocity.normalized * speed * Time.deltaTime);
    }

    /// <summary>
    /// Поворот объекта по движению
    /// </summary>
    public override void Rotation() {
      angleRotation = Vector3.Angle(Vector3.up, velocity);
      transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, -angleRotation + 90);
    }

    /// <summary>
    /// Выстрел
    /// </summary>
    /// <param name="tapStart"></param>
    /// <param name="tapEnd"></param>
    public override void Shot(Vector3 tapStart, Vector3 tapEnd) {

      RaycastHit2D hit = Physics2D.Raycast(new Vector3(tapEnd.x, tapEnd.y, -10), Vector3.forward * 100, 100, targetMask);

      if (hit.collider != null) {
        targetObject = hit.transform;
        Debug.Log(targetObject.transform.position);
      } else {
        gameObject.SetActive(false);
      }
    }

    /// <summary>
    /// Установка цели
    /// </summary>
    /// <param name="newTarget"></param>
    public void SetTarget(Transform newTarget) {
      targetObject = newTarget;
    }


    public override void OnTriggerEnter2D(Collider2D col) {

      if (LayerMask.LayerToName(col.gameObject.layer) == "Enemy" && col.gameObject == targetObject.gameObject) {

        DamageEnemy(col.gameObject);
        DamageRadius();

        BattleEventEffects.Instance.VisualEffect(BattleEffectsType.bazookaBom, targetObject.position, this);

        ParticleSystem.EmissionModule em1 = emis1.emission;
        em1.enabled = false;
        ParticleSystem.EmissionModule em2 = emis2.emission;
        em2.enabled = false;
        ParticleSystem.EmissionModule em3 = emis3.emission;
        em3.enabled = false;



        DeactiveThis(col.GetComponent<Enemy>());
      }
      DamageBallonsTry(col);

    }

    /// <summary>
    /// Дамаг в радиусе
    /// </summary>
    private void DamageRadius() {
      RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, damageRadius, Vector2.zero, 1, targetMask);
      foreach (RaycastHit2D hit in hits) {
        if (targetObject != null && hit.collider.gameObject == targetObject.gameObject)
          continue;
        float newDamageValue = (damageRadius - Vector2.Distance(transform.position, hit.transform.position)) / damageRadius * DamageValue;
        if (newDamageValue <= 0)
          continue;
        DamageBallonsTry(hit.collider);
        DamageEnemy(hit.collider.gameObject, newDamageValue);

      }


      GameObject grenadeInst = Instantiate(boomPrefabs, transform.position, Quaternion.identity);
      grenadeInst.SetActive(true);
      Destroy(grenadeInst, 2);
    }

    /// <summary>
    /// Событие соприкосновение с землей
    /// </summary>
    public override void OnGround() {
      targetObject = null;
      DamageRadius();
      base.OnGround();
    }

    private void OnDrawGizmos() {
      Gizmos.DrawWireSphere(transform.position, 4);
    }



    public override void GetConfig() {
      base.GetConfig();


      damageRadius = wep.param1.Value;
      speed = wep.param2.Value;
      vectorChangeSpeed = 1.5f;

      //damageRadius = float.Parse((string)config["RadiusDamage"]);
      //speed = float.Parse((string)config["Speed"]);
      //vectorChangeSpeed = float.Parse((string)config["SpeedChangeVector"]);

    }
  }


}