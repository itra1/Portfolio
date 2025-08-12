using UnityEngine;
using System.Collections;
using Spine.Unity;

public enum EnenyWeaponEnum { noWeapon, spear, boomerang, body, head, enemy, run, underwear, runfull, pillow }
public enum DamagePowen { level1, level2, level3 }
/// <summary>
/// Контроллер врага
/// </summary>
public class EnemyController : MonoBehaviour {

  public delegate void EventDelegate();
  public event EventDelegate OnDead;

  public EnemyTypes enemyType;

  public int enemyNum;
  public GameObject graphic;                                              // Ссылка на графику

  EnemyShoot shoot;                                                       // Ссылка на компонент атаки
  EnemyMove play;                                                         // Ссылка на компонент забега
  [SerializeField]
  SkeletonAnimation skeletonAnimation;                   // Анимация Скелета
  public GameObject shadowObject;                     // Объект тени
  GameObject shadowInstance;                          // Экземпляр тени

  /// <summary>
  /// Количество монет выдаваемых за смерть
  /// <para>Назначается контроллером</para>
  /// </summary>
  [HideInInspector]
  public int deadCoins;

  public RunnerPhase runnerPhase;                                         // Фаза раннера
  public float damagePowerPlayerForContact;                               // При положительном значении повреждает плеера при контакте

  #region Live
  public int live;                                                        // Максимальное значение жизней
  float liveNow;                                                          // Текущее состояние жизней
                                                                          //[HideInInspector] public int deadType;                                  // Тип смерти
  [HideInInspector]
  public bool head = true;                              // Флаг наличия головы
  #endregion

  [SerializeField]
  PlayerDamage playerDamag;                                               // Структура, для атаки по игроку
  float lastDamage;                                                       // Время последней атаки
  public bool damageFromStone;

  #region Weapon
  public EnenyWeaponEnum weaponType;                                      // Используемый тип оружия
  public float timeShootMin;                                              // Минимальное время выстрела
  public float timeShootMax;                                              // Максимальное время выстрела
  float shootNextTime;                                                    // Время средующего выстрела
  GameObject MyEnemy;
  [Range(0,1)]
  public float probilityShoot;
  [HideInInspector]
  public float lastDistanceShoot;
  #endregion

  #region Toss
  public bool tossEnable;                                                 // Разрешение бросания
  [HideInInspector]
  public bool thisToss;                                 // Флаг выполнения выстрела
  bool tossConnect;
  #endregion

  #region Player Detect
  public LayerMask playerLayer;                                            // Слой игрока
  public float playerRadius;                                              // Радиус определения игрока
  #endregion

  #region Animation
  string currentAnimation;                                                // Текущая анимация
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string attackAnim = "";                                          // Анимация атаки
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string damageAnim = "";                                          // Анимация повреждения
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string firesAnim = "";                                          // Анимация повреждения
  #endregion

  #region Fire 
  [HideInInspector]
  public bool thisFire;                                 // Флаг горения
  public GameObject fireObject;                                           // Объект огня
  public float fireDamage;
  float firelastDamage;
  public float fireDamageTime;
  float timeFires;
  GameObject fireObj;                                                     // Ссылка на объект огня
  #endregion

  #region LiveLine
  public GameObject livelinePrefab;   // Ссылка на префаб линии жизней 
  public Vector3 liveLineDiff;        // Смещение полосы жизней относительно позиции врага
  [HideInInspector]
  public GameObject livelineInst;            // Ссылка на созданную линию жизней
  #endregion

  #region BodyElements
  public GameObject[] bodyElements;
  public GameObject[] specialBodyElements;
  public float bodyHeight;
  #endregion

  #region audio
  [Header("Audio")]
  AudioSource audioComp;                  // Компонент аудио
  public AudioClip[] enemyIdleAudio;      // Звуки при беге
  public FloatSpan audioTime;             // Время между звучанием
  float nextIdleAudio;                    // Время следуюзего звучания
  public AudioClip[] enemyDamageAudio;     // Звук Аудио
  public AudioClip[] deadClip;
  public AudioClip[] shotAttack;
  public AudioClip[] attackClip;
  #endregion
  /*
  bool darkColor;
  float needColor;
  float thisColor;
  */
  public float playerJumpDamage;

  void Start() {
    //ResetAnimation();
  }

  void OnEnable() {

    play = GetComponent<EnemyMove>();
    shoot = GetComponent<EnemyShoot>();
    audioComp = GetComponent<AudioSource>();

    RunnerController.OnChangeRunnerPhase += ChangePhase;
    liveNow = live;
    head = true;

    MyEnemy = null;
    tossConnect = false;
    Toss();

    // Тень
    InitShadow();
    ResetAnimation();
  }


  void OnDisable() {

    if(shadowInstance) shadowInstance.SetActive(false);
    //if (shadowInstance) Destroy(shadowInstance);
    if(livelineInst) Destroy(livelineInst);
    if(fireObj) Destroy(fireObj);

    RunnerController.OnChangeRunnerPhase -= this.ChangePhase;
    runnerPhase = RunnerPhase.run;
    play.inBoundary = false;
  }

  #region Shadow

  void InitShadow() {
    if(shadowInstance && !shadowInstance.activeInHierarchy)
      shadowInstance.SetActive(true);
      shadowInstance.transform.parent = transform;
      shadowInstance.transform.position = Vector3.zero;
      shadowInstance.GetComponent<ShadowBehaviour>().matherObject = gameObject.transform;
      shadowInstance.GetComponent<ShadowBehaviour>().diff = 0;
      shadowInstance.GetComponent<ShadowBehaviour>().SetDeff();
    
  }


  public void ShadowFixed(bool flag) {
    if(shadowInstance)
      shadowInstance.GetComponent<ShadowBehaviour>().fixedsize = flag;
  }

  public void ShadowSetDiff(Vector3 newPos, Vector3 newScale) {
    if(shadowInstance)
      shadowInstance.GetComponent<ShadowBehaviour>().SetDiff(newPos, newScale);
  }

  public void ShadowSetDeff() {
    if(shadowInstance)
      shadowInstance.GetComponent<ShadowBehaviour>().SetDeff();
  }

  #endregion

  #region Audio

  // Воспроизведение стандартного аудио бега
  public void PlayIdleAudio() {
    if(enemyIdleAudio.Length > 0 & Time.time > EnemySpawner.Instance.lastEnemyIdlePlay) {
      audioComp.PlayOneShot(enemyIdleAudio[Random.Range(0, enemyIdleAudio.Length)], 1);
      EnemySpawner.Instance.lastEnemyIdlePlay = Time.time + 1f;
    }
  }

  public void PlayDistAttackAudio() {
    if(shotAttack.Length > 0) {
      audioComp.PlayOneShot(shotAttack[Random.Range(0, shotAttack.Length)], 1);
    }
  }

  public void PlayAttackAudio() {
    if(attackClip.Length > 0) {
      AudioManager.PlayEffect(attackClip[Random.Range(0, attackClip.Length)], AudioMixerTypes.runnerEffect, 1);
    }
  }

  // Воспроизведение аудио атаки
  void PlayDamageAudio() {
    if(enemyDamageAudio.Length > 0) {
      AudioManager.PlayEffect(enemyDamageAudio[Random.Range(0, enemyDamageAudio.Length)], AudioMixerTypes.runnerEffect);
    }
  }
  void PlayDeadAudio() {
    if(deadClip.Length > 0) {
      AudioManager.PlayEffect(deadClip[Random.Range(0, deadClip.Length)], AudioMixerTypes.runnerEffect, 0.75f);
    }
  }

  #endregion

  public void ChangePhase(RunnerPhase newPhase) {
    if(runnerPhase == RunnerPhase.boost & newPhase != runnerPhase) return;

    runnerPhase = newPhase;
  }

  void Update() {
    // Анимация горения
    if(fireObj) {
      fireObj.transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, -0.1f);
      AddAnimation(2, firesAnim, true, 0);
    }

    // Отключение анимации горения
    if(thisFire && timeFires <= Time.time) {
      Destroy(fireObj);
      thisFire = false;
      ResetAnimation();
    }
    // Повреждения наносимые огнем
    if(thisFire && firelastDamage + fireDamageTime < Time.time) {
      firelastDamage = Time.time;
      Enemy1Damage(WeaponTypes.fire, ref fireDamage, Vector3.zero, DamagePowen.level1);
    }

    checkPlayer();

    if(runnerPhase == RunnerPhase.run && play.inBoundary) {

      if(lastDistanceShoot + 5 <= RunnerController.playerDistantion) {
        lastDistanceShoot = RunnerController.playerDistantion;

        if(probilityShoot >= Random.value && !thisFire && !(weaponType == EnenyWeaponEnum.head && !head) && weaponType != EnenyWeaponEnum.enemy) {
          shoot.Shoot(weaponType);
        }

        if(probilityShoot >= Random.value && !thisFire && weaponType == EnenyWeaponEnum.enemy && MyEnemy == null) {
          MyEnemy = EnemySpawner.Instance.GenEnemyForGigant();
        }
      }
    }

    // Двигаем полосу жизней
    if(livelineInst != null) {
      livelineInst.transform.position = transform.position + liveLineDiff;
    }

    if(transform.position.y <= 0
        || transform.position.x <= CameraController.displayDiff.transform.position.x + CameraController.displayDiff.left * 2f
        || transform.position.y > CameraController.displayDiff.transform.position.y + CameraController.displayDiff.top * 2f
        || transform.position.x > CameraController.displayDiff.transform.position.x + CameraController.displayDiff.right * 2f)
      gameObject.SetActive(false);
  }

  public void ShootNow() {
    shoot.Shoot(weaponType);
  }

  #region Animation
  /* ***************************
   * Применение анимации
   * ***************************/
  public void SetAnimation(string anim, bool loop) {
    if(currentAnimation != anim) {
      if(currentAnimation == shoot.distAttackAnim && weaponType == EnenyWeaponEnum.enemy && MyEnemy != null) {
        MyEnemy.SetActive(false);
        MyEnemy = null;
      }

      skeletonAnimation.state.SetAnimation(0, anim, loop);
      currentAnimation = anim;
    }
  }

  /// <summary>
  /// Повторная инициализация анимации
  /// </summary>
  public void ResetAnimation() {

    skeletonAnimation.Initialize(true);

    if(weaponType == EnenyWeaponEnum.enemy && MyEnemy != null) {
      MyEnemy = null;
    }

    skeletonAnimation.state.Event += this.AnimEvent;
    skeletonAnimation.state.End += this.AnimEnd;
    currentAnimation = null;
  }

  public void deleteEventAnimation() {
    skeletonAnimation.state.Event -= AnimEvent;
    skeletonAnimation.state.End -= this.AnimEnd;
  }

  /* ***************************
   * Добавленная анимация
   * ***************************/
  public void AddAnimation(int index, string animName, bool loop, float delay) {
    skeletonAnimation.state.AddAnimation(index, animName, loop, delay);
  }

  /* ***************************
   * Установка скорости
   * ***************************/
  public void SpeedAnimation(float speed) {
    skeletonAnimation.timeScale = speed;
  }
  #endregion

  bool playerChecker;
  /* ********************
   * Функция определения приближения Игрока
   * ********************/
  bool checkPlayer() {
    playerChecker = Physics.CheckCapsule(transform.position, transform.position + Vector3.up * 5, playerRadius, playerLayer);
    if(playerChecker && lastDamage <= Time.time) {
      // Если не началась телесная атака выполняем анимацию и отменяем телесную
      if(shoot.bodyStep <= 1 && !thisToss && runnerPhase == RunnerPhase.run) {
        AddAnimation(2, attackAnim, false, 0);
        PlayAttackAudio();
        // Если выполняется атака с забегом вперед, отключаем
        if(shoot.bodyAttack) shoot.bodyAttack = false;
        shoot.bodyStep = 0;
        lastDamage = Time.time + Random.Range(playerDamag.damageTime.min, playerDamag.damageTime.max);
        return true;
      }
    }
    return false;
  }


  void OnTriggerEnter2D(Collider2D oth) {
    if(runnerPhase == RunnerPhase.boost || runnerPhase == RunnerPhase.preBoost || runnerPhase == RunnerPhase.postBoost) {
      if(oth.tag == "Player") Enemy1Damage(WeaponTypes.none, 100000, Vector3.zero, 0);
    } else {
      if(damagePowerPlayerForContact > 0 && oth.tag == "Player" && !shoot.runFullAttackBack && shoot.bodyStep != 3) {
        oth.GetComponent<Player.Jack.PlayerController>().ThisDamage(WeaponTypes.none, Player.Jack.DamagType.power, damagePowerPlayerForContact, transform.position);
      } else if(LayerMask.LayerToName(oth.gameObject.layer) == "Player" & (thisToss || shoot.bodyAttack) & shoot.bodyStep != 3) {
        Player.Jack.PlayerController playerCont = oth.GetComponent<Player.Jack.PlayerController>();
        if(playerCont) playerCont.ThisDamage(WeaponTypes.none, playerDamag.type, playerDamag.damagePower, transform.position);
      } else if(LayerMask.LayerToName(oth.gameObject.layer) == "Player" && shoot.bodyStep == 3) {
        AddAnimation(1, shoot.failAnim, false, 0);
        shoot.PlayBodyAttackAudio();
        Enemy1Damage(WeaponTypes.none, playerJumpDamage, oth.transform.position, DamagePowen.level1, 0, false);
        oth.GetComponent<Player.Jack.PlayerMove>().MinJump();
      }
    }

    if(LayerMask.LayerToName(oth.gameObject.layer) == "Barrier") {
      if(oth.tag == "RollingStone") {
        Enemy1Damage(WeaponTypes.none, (int)RunnerController.barrierDamage(oth.tag, false), transform.position, DamagePowen.level1, 0, true);
        oth.GetComponent<BarrierController>().DestroyThis();
      }
    }

    // Атака человечком
    if(runnerPhase == RunnerPhase.run && weaponType == EnenyWeaponEnum.enemy && MyEnemy != null && !tossConnect) {
      if(oth.gameObject == MyEnemy) {
        MyEnemy.GetComponent<EnemyController>().thisToss = true;
        shoot.Shoot(weaponType, MyEnemy.gameObject);
        tossConnect = true;
        MyEnemy = null;
      }
    }

    if(MyEnemy != null && !MyEnemy.activeInHierarchy) {
      MyEnemy = null;
      tossConnect = false;
    }

    if(runnerPhase == RunnerPhase.dead && oth.tag == "Player") {
      EnemySpawner.Instance.CreateDeadCloud();
      gameObject.SetActive(false);
    }
  }

  /* *********************************
   * Повреждение врага
   * 
   * ref float power - входящий и остаточный урон
   * *********************************/

  public void Enemy1Damage(WeaponTypes weaponType, float power, Vector3 damagePosition, DamagePowen powerDamage, float fire = 0, bool stoneDam = false) {

    float newPower = power;
    Enemy1Damage(weaponType, ref newPower, damagePosition, DamagePowen.level1, fire, stoneDam);
  }

  public void Enemy1Damage(WeaponTypes weaponType, ref float power, Vector3 damagePosition, DamagePowen powerDamage, float fire = 0, bool stoneDam = false) {

    // Сабля может повредить только простых скелетов
    //if (weaponType == WeaponTypes.sablePlayer && ( enemyType != enemyTypes.aztec && enemyType != enemyTypes.aztecForward )) return;

    // Сбрасываем анимацию атаки
    shoot.runAttack = false;

    // Запрет повреждать врага при туториале саблей
    if(runnerPhase == RunnerPhase.tutorial && weaponType == WeaponTypes.sablePlayer) return;

    if(enemyType == EnemyTypes.bigMama) return;

    // не получаем повлеждение от камня
    if(stoneDam & !damageFromStone) {
      if(runnerPhase == RunnerPhase.run || runnerPhase == RunnerPhase.tutorial) PlayDamageAudio();
      if((runnerPhase == RunnerPhase.run || runnerPhase == RunnerPhase.tutorial) & Random.value <= 0.3f) PlayIdleAudio();
      return;
    }

    // Определяем дамаг, который получит враг
    float damageSum = (liveNow > power ? power : liveNow);

    liveNow -= power;

    power -= damageSum;



    // Генерируем эффект получения урона
    Vector3 boomPos = Vector3.zero;
    if(damagePosition != Vector3.zero) {

      boomPos = Vector3.Lerp(new Vector3(transform.position.x, damagePosition.y, -0.1f),
                                     new Vector3(damagePosition.x, damagePosition.y, -0.1f),
                                     -0.3f);
    } else {
      boomPos = new Vector3(transform.position.x, transform.position.y + 1.5f, -0.3f);
    }
    GameObject bonesBum = Pooler.GetPooledObject("BoneBoom");
    bonesBum.transform.position = boomPos;
    bonesBum.SetActive(true);

    if(liveNow <= 0) {
      PlayDeadAudio();

      if(OnDead != null) OnDead();
      OnDead = null;


      if(deadCoins > 0) {
				CoinsSpawner.GenOneMonetToPlayer(transform.position, deadCoins);
      }

      bool fly = !play.isGround;
      bool breaks = true;

      RaycastHit[] hits = Physics.RaycastAll(transform.position + Vector3.up, Vector3.down, 10, play.groundMask);

      for(int i = 0; i < hits.Length; i++) {
        if(LayerMask.LayerToName(hits[i].transform.gameObject.layer) == "Ground") {
          breaks = false;
        }
      }

      Questions.QuestionManager.AddEnemyDead(enemyType, weaponType, transform.position, fly, breaks);

      GenBones(powerDamage);

      if(!stoneDam)
        RunnerController.Instance.enemiesDeadCount[enemyNum]++;

      EnemySpawner.Instance.EnemyDead(gameObject);
      gameObject.SetActive(false);
    } else {

      if(runnerPhase == RunnerPhase.run || runnerPhase == RunnerPhase.tutorial) PlayDamageAudio();

      if((runnerPhase == RunnerPhase.run || runnerPhase == RunnerPhase.tutorial) & Random.value <= 0.3f) PlayIdleAudio();

      // Отображаем полосу жизней
      if(livelineInst == null) {
        livelineInst = Instantiate(livelinePrefab, transform.position + liveLineDiff, Quaternion.identity) as GameObject;
        livelineInst.transform.parent = transform;
      }

      // Отображаем на полосе жизней текущее состояние
      if(livelineInst) livelineInst.GetComponent<LiveLineController>().SetSize(live, liveNow);

      AddAnimation(1, damageAnim, false, 0);

      if(fire > Random.value & fireObj == null)
        EnemyFire();
    }
  }

  void EnemyFire() {
    fireObj = Instantiate(fireObject, transform.position, Quaternion.identity) as GameObject;

    if(weaponType == EnenyWeaponEnum.enemy)
      fireObj.transform.position = new Vector3(0.3f, 0.3f, 0.3f);
    else if(weaponType == EnenyWeaponEnum.body)
      fireObj.transform.position = new Vector3(0.2f, 0.2f, 0.2f);
    else
      fireObj.transform.position = new Vector3(0.15f, 0.15f, 0.15f);

    thisFire = true;
    timeFires = Time.time + Random.Range(3, 7);
  }

  /// <summary>
  /// Обработка события анимации
  /// </summary>
  /// <param name="state"></param>
  /// <param name="trackIndex"></param>
  /// <param name="e"></param>
  void AnimEvent(Spine.AnimationState state, int trackIndex, Spine.Event e) {
    if(play.thisJump) return;
    shoot.runAttack = false;

    // Выполняется атака головой, обнуляем влаг головы
    if(state.GetCurrent(trackIndex).ToString() == "Attack_Head_Back")
      head = false;

    if(state.GetCurrent(trackIndex).ToString() == attackAnim) {
      bool isPlayer = Physics.CheckSphere(transform.position, playerRadius, playerLayer);
      if(isPlayer) {
        Player.Jack.PlayerController.Instance.ThisDamage(WeaponTypes.none, Player.Jack.DamagType.live, playerDamag.damagePower, transform.position);
      } else {
        Questions.QuestionManager.ConfirmQuestion(Quest.enemyShotButMiss, 1, transform.position + Vector3.up);
      }
    }

    if(state.GetCurrent(trackIndex).ToString() == shoot.distAttackAnim) {

      if(Random.value <= 0.9f && shotAttack.Length > 0)
        PlayDistAttackAudio();
      else
        AudioManager.PlayEffect(enemyIdleAudio[Random.Range(0, enemyIdleAudio.Length)], AudioMixerTypes.runnerEffect, 1);

      shoot.SpawnWeapon(weaponType);
    }
  }

  void AnimEnd(Spine.AnimationState state, int trackIndex) {
    if(state.GetCurrent(trackIndex).ToString() == shoot.distAttackAnim) {
      ResetAnimation();
    }
  }

  /// <summary>
  /// Сгенерировал для броска
  /// </summary>
  public void Toss() {
    if(!tossEnable) return;

    play.Toss();
  }

  public void GenBones(DamagePowen powerDamage, bool group = false) {

    float koefDamage;

    switch(powerDamage) {
      case DamagePowen.level2:
        koefDamage = 30f;
        break;
      case DamagePowen.level3:
        koefDamage = 20f;
        break;
      default:
        koefDamage = 10f;
        break;
    }

    // Генерируем разлетающиеся частицы
    if(bodyElements.Length > 0) {
      foreach(GameObject bodyElem in bodyElements) {
        GameObject inst = Instantiate(bodyElem, bodyElem.transform.position, Quaternion.identity) as GameObject;
        inst.GetComponent<BodyParticle>().SetStartCoef(koefDamage);
      }
    }

    //Генерируем выпадание спецефичных частей
    if(specialBodyElements.Length > 0 & head) {
      foreach(GameObject bodyElem in specialBodyElements) {
        GameObject inst = Instantiate(bodyElem, bodyElem.transform.position , Quaternion.identity) as GameObject;
        inst.GetComponent<BodyParticle>().SetStartCoef(koefDamage);
      }
    }
  }
}
