using System.Collections;
using System.Collections.Generic;
using ExEvent;
using UnityEngine;

namespace Game.Weapon {

  /// <summary>
  /// Контроллер снаряда
  /// </summary>
  public abstract class Bullet: EventBehaviour {

    public static System.Action<Bullet> OnCreate;
    public System.Action OnDestroyBullet;
    public System.Action<Enemy> OnDamageEnemy;
    public Rigidbody2D rig;

    public WeaponType weaponType;               // Тип оружия
    protected float _damageValue;               // Сила атаки
    public float DamageValue { get { return _damageValue * (1 + 0.1f * Game.User.UserManager.Instance.UserProgress.PowerLevel); } }

    protected float speedDelay;                 // Степень задержки при ударе
    protected float timeSpeedDelay;             // Время задержки при ударе
    [SerializeField]
    private SpanFloat rotationSpan;             // Диапазон скорости вращения
    protected float rotationSpeed;              // Текущая скорость вращения
    protected float groundLevel;                // Высота земли
    [HideInInspector]
    public Vector3 velocity;                    // Вектор движения

    protected float angleRotation;              // Угол вращения в полете
    protected Rigidbody2D rigidBody;            // Компонент твердого теля
    [SerializeField]
    protected bool catapultaStonePush;          // Флаг соударения с катапультным ядром
                                                //bool isDown;
    protected bool isActive;
    public float damageKoef = 1;
    public bool isDamage = true;                // Флаг разрешающий наносить урон
    public bool groundDestroyOnly;              // Разрушение только об землю
    public bool zoorbRebound;                   // Рикоше от зорба
    private bool isRebound;

    public GameObject graphic;

		public string killEnemySound;

    protected override void Awake() {
      base.Awake();
      GetConfig();
    }
    public virtual void Start() { }

    public virtual void OnEnable() {
      isRebound = false;
      isDamage = true;
      isActive = true;
      if(graphic != null)
      graphic.SetActive(true);
      damageKoef = 1;
      if (graphic != null)
        graphic.SetActive(true);
      try {
        GetComponent<Collider2D>().enabled = true;
      } catch { }

      if (OnCreate != null)
        OnCreate(this);

      rigidBody = GetComponent<Rigidbody2D>();
      rotationSpeed = Random.Range(rotationSpan.min, rotationSpan.max);
      velocity = Vector3.zero;
    }

    public virtual void Update() {
      if (!isActive)
        return;
      if (isRebound)
        ReboundMove();
      else
        Move();
      Rotation();
      if (transform.position.y <= groundLevel) {
        OnGround();
      }
      ///если оружие ушло вправо за экран дестроить
      if (transform.position.x > CameraController.rightPoint.x + 1f)
        DeactiveThis(null);
    }

    private Vector3 firstPoint;
    /// <summary>
    /// Бросок
    /// </summary>
    /// <param name="deltaTouch">Дельта косания пальца на экране</param>
    /// <param name="tapEnd">Точка старта тапа</param>
    /// <param name="tapStart">Точка конца тапа</param>
    public virtual void Shot(Vector3 tapStart, Vector3 tapEnd) {
      tapEnd.y = Mathf.Clamp(tapEnd.y, DecorationManager.Instance.loaderLocation.roadSize.min, DecorationManager.Instance.loaderLocation.roadSize.max);
      groundLevel = tapEnd.y;
      velocity = new Vector3(1, 1, 0).normalized;
      float distanceX = tapEnd.x - tapStart.x;
      firstPoint.x = tapStart.x + (distanceX / 2 * 1f);
      firstPoint.y = tapStart.y + distanceX / Mathf.Lerp(4f, 8f, (1 - distanceX / 23));

      targetPoint = tapEnd;
      forceDown = 0.06f;
    }

    [ExEvent.ExEventHandler(typeof(BattleEvents.StartBattle))]
    public void BattleStart(BattleEvents.StartBattle startEvent) {
      gameObject.SetActive(false);
    }

    private void OnDrawGizmos() {
      Gizmos.color = Color.yellow;
      Gizmos.DrawWireSphere(firstPoint, 0.5f);
    }

    /// <summary>
    /// Событие при ударе о землю
    /// </summary>
    public virtual void OnGround() {
      //PlayDamageAudio();
      SpawnGroundSfx();
      DeactiveThis(null, true);
    }
    /// <summary>
    /// Деактивация при ударе о землю
    /// </summary>
    protected virtual void DeactiveThis(Enemy enemy, bool isGround = false) {

      if (groundDestroyOnly && !isGround)
        return;

      BattleEventEffects.Instance.DestroyBullet(weaponType, enemy, transform.position);

      PlayDestroyAudio();
      //GroundParticel();

      isActive = false;
      if(graphic != null)
      graphic.SetActive(false);

      if (OnDestroyBullet != null)
        OnDestroyBullet();

      if (gameObject.activeInHierarchy)
        StartCoroutine(DeactiveCoroutine());
    }

    private IEnumerator DeactiveCoroutine() {
      if (graphic != null)
        graphic.SetActive(false);
      try {
        GetComponent<Collider2D>().enabled = false;
      } catch { }
      yield return new WaitForSeconds(2);
      gameObject.SetActive(false);
    }

    //protected virtual void GroundParticel() {

    //	if (groundParticle != "") {
    //		GameObject grdPrt = PoolerManager.Spawn(groundParticle);
    //		grdPrt.transform.position = transform.position;
    //		grdPrt.SetActive(true);
    //	}
    //}

    private Vector3 targetPoint;
    protected float speed = 25f;
    private float forceDown;
    private float tmpForceDown;
    /// <summary>
    /// Движение
    /// </summary>
    public virtual void Move() {

      if (transform.position.x < firstPoint.x) {
        velocity += (firstPoint - transform.position).normalized * speed * Time.deltaTime;
      } else {

        if (firstPoint.y > transform.position.y) {
          tmpForceDown = ((firstPoint.y - transform.position.y) / (firstPoint.y - targetPoint.y)) * 2f;
          forceDown = Mathf.Max(forceDown, tmpForceDown);
        }

        if ((targetPoint - transform.position).x < 0)
          velocity += Vector3.down * speed * forceDown * 45 * Time.deltaTime;
        else
          velocity += (targetPoint - transform.position).normalized * speed * forceDown * 45 * Time.deltaTime;
      }
      transform.position += velocity.normalized * speed * Time.deltaTime;
      //rigidBody.MovePosition(transform.position + velocity.normalized * speed * Time.deltaTime);

      if (transform.position.x > targetPoint.x)
        transform.position = new Vector3(targetPoint.x, transform.position.y, transform.position.z);

    }

    private void ReboundMove() {
      velocity.y -= 9 * Time.deltaTime;
      if (rigidBody != null)
        rigidBody.MovePosition(transform.position + velocity.normalized * speed * Time.deltaTime);
    }

    protected void SetRebound(Collider2D sourceCollider) {
      isRebound = true;

      Vector3 one = transform.position - sourceCollider.transform.position;
      float angle = -Vector3.Angle(-velocity, one) * 2;
      float anglRadian = angle * (Mathf.PI / 180);

      velocity = -velocity.normalized;

      velocity = new Vector3(velocity.x * Mathf.Cos(anglRadian) - velocity.y * Mathf.Sin(anglRadian),
                          velocity.x * Mathf.Sin(anglRadian) + velocity.y * Mathf.Cos(anglRadian),
                          0).normalized;

    }


    /// <summary>
    /// Поворот объевта против движения
    /// </summary>
    public virtual void Rotation() {

      angleRotation += rotationSpeed * Time.deltaTime;
      if(rigidBody != null)
      rigidBody.MoveRotation(angleRotation);
    }

    public virtual void OnTriggerEnter2D(Collider2D col) {
      OnHit(col);

    }

    public void OnHit(Collider2D col) {
      if (!isActive || !isDamage)
        return;

      if (catapultaStonePush && col.tag == "CatapultStone")
        CatapultStonePush(col.gameObject);
      if (LayerMask.LayerToName(col.gameObject.layer) == "Enemy") {

        if (zoorbRebound && col.GetComponent<DiskoZorb>() != null) {
          col.GetComponent<DiskoZorb>().SetBound(gameObject);
          SetRebound(col);
          return;
        }

        DamageEnemy(col.gameObject);
        DamageObject(col);
        PlayDamageAudio();
        DeactiveThis(col.GetComponent<Enemy>());
      }
      if (LayerMask.LayerToName(col.gameObject.layer) == "Bonuses") {
        if (col.GetComponent<PostController>()) {
          col.GetComponent<PostController>().DamagePlayer();
          DamageObject(col);
          PlayDamageAudio();
          DeactiveThis(null);
        }
      }
      DamageBallonsTry(col);
      //if (col.gameObject.tag == "Ballon") {
      //	try {
      //		col.GetComponent<CupidonBallonsHelper>().BalloneDestroy();
      //		PlayDamageAudio();
      //		DeactiveThis();
      //	}
      //	catch { }
      //}
    }


    [SerializeField]
    private string groundParticle;              // Название партиклов при уничтожении о землю в PoolerManager
    protected virtual void SpawnGroundSfx() {

      if (groundParticle != "") {
        GameObject grdPrt = PoolerManager.Spawn(groundParticle);
        grdPrt.transform.position = transform.position;
        grdPrt.SetActive(true);
      }
    }

    [SerializeField]
    private string enemyParticle;              // Название партиклов при уничтожении о землю в PoolerManager
    protected virtual void SpawnEnemyDamageSfx(GameObject enemy) {
      if (enemyParticle != "") {
        GameObject grdPrt = PoolerManager.Spawn(enemyParticle);
        grdPrt.transform.position = enemy.transform.position + Vector3.up * Random.Range(0.9f, 1.2f);
        try {
          grdPrt.GetComponent<ISpriteOrder>().SetSpriteOrder(enemy.GetComponent<ISpriteOrder>().spriteOrderValue + 1);
        } catch {
        }
        grdPrt.SetActive(true);
      }
    }

    protected void DamageBallonsTry(Collider2D collision) {
      if (collision.gameObject.tag == "Ballon") {
        try {
          collision.GetComponent<CupidonBallonsHelper>().BalloneDestroy();
          PlayDamageAudio();
          DeactiveThis(null);
        } catch { }
      }
    }

    protected virtual void DamageObject(Collider2D col) {

    }

    /// <summary>
    /// Удар о ядро катапульты
    /// </summary>
    /// <param name="obj"></param>
    public virtual void CatapultStonePush(GameObject obj) {

      PlayDestroyAudio();
      obj.GetComponent<CatapultStone>().OnRepuls(transform.position);
      CatapultStoneDamage(obj);

    }
    /// <summary>
    /// Реакция на удар о ядро катапульты
    /// </summary>
    /// <param name="obj"></param>
    public virtual void CatapultStoneDamage(GameObject obj) {
      Vector3 repils = (transform.position - obj.transform.position) * 15;
      repils.z = 0;
      repils.y = Mathf.Min(1, repils.y);
      velocity += repils;
    }

    /// <summary>
    /// Нанесение урона врагу
    /// </summary>
    /// <param name="enemy"></param>
    public virtual void DamageEnemy(GameObject enemy, float needDamage) {
      Enemy enemyController = enemy.GetComponent<Enemy>();
      if (enemyController != null) {
        if (OnDamageEnemy != null)
          OnDamageEnemy(enemyController);
        enemyController.Damage(gameObject, (needDamage != 0 ? needDamage : DamageValue) * damageKoef * (BattleManager.isLastPower ? 2 : 1),
          speedDelay, timeSpeedDelay);
      }
    }

    protected virtual void DamageEnemy(GameObject enemy) {
      SpawnEnemyDamageSfx(enemy);
      DamageEnemy(enemy, 0);
    }

    #region Настройки

    protected Configuration.Weapon wep;

    public virtual void GetConfig() {

      wep = GameDesign.Instance.allConfig.weapon.Find(x => x.id == (int)weaponType);

      _damageValue = wep.damage.Value;
      timeSpeedDelay = wep.stunTime.HasValue ? wep.stunTime.Value : 0;
      speedDelay = wep.stunValue.HasValue ? wep.stunValue.Value : 0;

    }

    #endregion

    #region Звуки

    public List<AudioClipData> damageAudio;
    public AudioBlock damageAudioBlock;

    protected virtual void PlayDamageAudio() {
      if (damageAudioBlock != null)
        damageAudioBlock.PlayRandom(this);
      //if (damageAudio.Count == 0)
      //	return;
      //damageAudioBlock.
      //AudioManager.PlayEffects(damageAudio[Random.Range(0, damageAudio.Count)], AudioMixerTypes.effectPlay);
    }

    public List<AudioClipData> destroyAudio;
    public AudioBlock destroyAudioBlock;

    protected virtual void PlayDestroyAudio() {
      if (destroyAudioBlock != null)
        destroyAudioBlock.PlayRandom(this);
      //if (destroyAudio.Count == 0)
      //	return;
      //AudioManager.PlayEffects(destroyAudio[Random.Range(0, destroyAudio.Count)], AudioMixerTypes.effectPlay);
    }


    #endregion
  }


}