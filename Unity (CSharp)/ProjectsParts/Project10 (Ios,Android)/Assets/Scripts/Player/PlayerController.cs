using Spine.Unity;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player.Jack {

  [System.Serializable]
  public class Boundary {
    public float xMin, xMax;
  }

  [System.Serializable]
  public enum DamagType {
    live, power
  }

  /// <summary>
  /// Основной контроллер игры
  /// </summary>
  public class PlayerController: PlayerComponent {

    public BoxCollider2D boxGroundCollider;
    public BoxCollider2D boxDamageCollider;
    public static PlayerController Instance;

    [HideInInspector]
    public bool isGround;

    private readonly string XAxis = "Horizontal";                        // Движение по горизонтале
    private readonly string JumpButton = "Jump";                         // Движение прыжка
    private readonly string FireButton = "Fire1";                        // Движение атакаи

    protected override void Awake() {
      base.Awake();
      Instance = this;
    }

    public Transform graundPoint;

    public GameObject graphic;                          // Ссылка на собственный объект графики

    public GameObject dust;                             // Пыль
    public Rigidbody[] HingeJointBodyes;
    private float lastMove;                                     // Последнее смещение, используется при управлении с клавиатуры

    [HideInInspector]
    public float graceTime = 3;                             // Время грейс

    public LayerMask groundLayer;                       // Слой для определения поверхности
    [HideInInspector]
    public RunnerPhase runnerPhase;                     // фаза раннера

    private float graceDamage;                                  // Грейсовое время после восстановления
    private float damageAnim;                                   // Время время анимации дамага

    [HideInInspector]
    public bool playerStart;

    [HideInInspector]
    public bool playerFall = false;
    [HideInInspector]
    public bool playerFallWait = false;

    public bool isMove;

    [HideInInspector]
    public float gravityNorm;

    public void SetClassicRigidbody() {
      rigidbody.gravityScale = 6;
      gravityNorm = rigidbody.gravityScale;
      rigidbody.angularDrag = 0;
      rigidbody.drag = 0;
      rigidbody.mass = 1;
    }

    private void SetFirstRigidbody() {
      rigidbody.gravityScale = 2;
      gravityNorm = rigidbody.gravityScale;
      rigidbody.angularDrag = 0;
      rigidbody.drag = 0;
      rigidbody.mass = 1;
    }

    public void SetJetPackRigidbody() {
      rigidbody.gravityScale = 2;
      gravityNorm = rigidbody.gravityScale;
      rigidbody.angularDrag = 0;
      rigidbody.drag = 0;
      rigidbody.mass = 1;
      //move.jumpSpeed /= 2;
    }

    public void SetStartParam() {
      SetFirstRigidbody();
    }

    public void SerRunParam() {
      if (GameManager.activeLevelData.gameFormat == GameMechanic.jetPack) {
        SetJetPackRigidbody();
      } else {
        SetClassicRigidbody();
      }
    }

    void OnEnable() {
      RunnerController.OnChangeRunnerPhase += ChangePhase;
      ChangePhase(RunnerController.Instance.runnerPhase);
      playerStart = true;

      //playerCheck.enabled = GameManager.gameMode != GameMode.survival;

      ControllerManager.OnHorizontal += MoveKey;
      ControllerManager.OnFire += FireKey;
      ControllerManager.OnJump += JumpKey;

      PlayerController.Instance.animation.onAnimEvent += AnimEvent;
      PlayerController.Instance.animation.onAnimEnd += AnimEnd;

      animation.ResetAnimation();

      RunnerController.SetPause += SetPause;
      UpdateValue();

      Helpers.Invoke(this, ActiveteBody, 0.2f);
      SerRunParam();

    }

    public void GraphicVector(bool isRight) {
      graphic.transform.localScale = new Vector3(1f * (isRight ? 1 : -1) * (GameManager.activeLevelData.moveVector == MoveVector.left ? -1 : 1), 1f, 1f);
    }

    private void ActiveteBody() {
      rigidbody.bodyType = RigidbodyType2D.Dynamic;
    }

    private void FixedUpdate() {
      isGround = (Physics2D.OverlapCircle(graundPoint.position, 0.1f, groundLayer) != null);

      if (playerFall && transform.position.y >= 8)
        playerFall = false;

      if (playerFall) {
        rigidbody.AddForce(new Vector2(rigidbody.velocity.x, move.jumpSpeed * 3), ForceMode2D.Force);
      }

    }

    void OnDisable() {
      ControllerManager.OnHorizontal -= MoveKey;
      ControllerManager.OnFire -= FireKey;
      ControllerManager.OnJump -= JumpKey;
      RunnerController.SetPause -= SetPause;
      RunnerController.OnChangeRunnerPhase -= ChangePhase;
      rigidbody.velocity = Vector2.zero;
      rigidbody.bodyType = RigidbodyType2D.Static;

      PlayerController.Instance.animation.onAnimEvent -= AnimEvent;
      PlayerController.Instance.animation.onAnimEnd -= AnimEnd;

    }

    private int defendStones;
    private int breackReturn;
    private int defendSpear;
    private int defendHending;

    void UpdateValue() {
      ClothesBonus defendbarrier = Config.GetActiveCloth(ClothesSets.defendBarrier);
      defendStones = 0 + (defendbarrier.head ? 1 : 0) + (defendbarrier.spine ? 1 : 0) + (defendbarrier.accessory ? 1 : 0);

      ClothesBonus beackClothes = Config.GetActiveCloth(ClothesSets.noBreack);
      breackReturn = 0 + (beackClothes.head ? 1 : 0) + (beackClothes.spine ? 1 : 0) + (beackClothes.accessory ? 1 : 0);

      ClothesBonus spearClothes = Config.GetActiveCloth(ClothesSets.noAirAttack);
      defendSpear = 0 + (spearClothes.head ? 1 : 0) + (spearClothes.spine ? 1 : 0) + (spearClothes.accessory ? 1 : 0);

      ClothesBonus hendingClothes = Config.GetActiveCloth(ClothesSets.noHendingBarrier);
      defendHending = 0 + (hendingClothes.head ? 1 : 0) + (hendingClothes.spine ? 1 : 0) + (hendingClothes.accessory ? 1 : 0);
    }

    void ChangePhase(RunnerPhase newPhase) {

      if (runnerPhase != RunnerPhase.start)
        SerRunParam();

      if (runnerPhase == RunnerPhase.postBoost && newPhase == RunnerPhase.run) {
        CreateDust();
        AudioManager.PlayEffect(boost.boosEndClip, AudioMixerTypes.runnerEffect, 1);
      }

      if (runnerPhase == RunnerPhase.run && newPhase == RunnerPhase.boost) {
        CreateDust();
      }

      if (runnerPhase == RunnerPhase.dead && newPhase == RunnerPhase.run) {
        if (transform.position.y < 0) {
          transform.position = new Vector3(CameraController.displayDiff.transform.position.x, 0, transform.position.z);
          playerFall = true;
          Questions.QuestionManager.ConfirmQuestion(Quest.breakToLive, 1);
        }
        SetGraceDamage();
      }

      if (newPhase == RunnerPhase.dead)
        GetComponent<ShadowController>().Dead();

      if (newPhase != RunnerPhase.roulette)
        animation.timeScale = 1f;

      runnerPhase = newPhase;

      check.InitParametrs();
    }

    bool audioInPause;
    void SetPause(bool pauseStatus) {
      if (pauseStatus) {

        GetComponent<AudioSource>().Pause();

        AudioSource[] allAudio = GetComponentsInChildren<AudioSource>();
        foreach (AudioSource one in allAudio)
          one.Pause();

        audioInPause = true;
      }

      if (!pauseStatus && audioInPause) {
        GetComponent<AudioSource>().UnPause();

        AudioSource[] allAudio = GetComponentsInChildren<AudioSource>();
        foreach (AudioSource one in allAudio)
          one.UnPause();
        audioInPause = false;
      }
    }

    public void CreateDust() {
      Instantiate(dust, transform.position + new Vector3(0, 0, -5), Quaternion.identity);
    }

    public void GetGem() {
      transform.localScale = new Vector3(-1, 1, 1);
      StartCoroutine(GemAnim());
    }

    IEnumerator GemAnim() {
      animation.SetAnimation(0, animation.gemGetAnim, false);
      yield return new WaitForSeconds(0.5f);
      animation.AddAnimation(1, animation.gemIdleAnim, true, 0);
    }
    public void GetGemEnd() {
      StartCoroutine(GemAnimEnd());
    }

    IEnumerator GemAnimEnd() {
      yield return new WaitForSeconds(0.05f);
      animation.ResetAnimation();
      animation.SetAnimation(0, animation.jumpIdleAnim, true);
    }

    private void Update() {

      /// Проверка работы магнита
      if (magnetActive)
        Magnet();

      /// Обработка работы щита
      if (shieldActive)
        Shield();

      float move = Input.GetAxis(XAxis);
      if (move != lastMove) {
        MoveKey(move);
        lastMove = move;
      }

      if (playerStart) {
        if (transform.position.x >= CameraController.displayDiff.transform.position.x)
          playerStart = false;
      }

      if (Input.GetButtonDown(JumpButton))
          JumpKey(true);
      
      if (Input.GetButton(FireButton))
        FireKey(true);

      if (transform.position.y <= -1 && runnerPhase != RunnerPhase.dead && runnerPhase != RunnerPhase.start)
        PlayerFall();
    }

    /// <summary>
    /// Получение урона
    /// </summary>
    /// <param name="weaponType"></param>
    /// <param name="damageType"></param>
    /// <param name="power"></param>
    /// <param name="from"></param>
    /// <param name="damageEffect"></param>
    public void ThisDamage(WeaponTypes weaponType, DamagType damageType, float power, Vector3 from, bool damageEffect = true) {
      if ((runnerPhase != RunnerPhase.run && runnerPhase != RunnerPhase.tutorial && runnerPhase != RunnerPhase.boss) && weaponType != WeaponTypes.gate)
        return;

      if (power <= 0)
        return;

      check.ResetJump();

      // Отсрочка
      if (graceDamage > Time.time && weaponType != WeaponTypes.gate)
        return;

      // Проверка работы щита
      if (shieldActive) {
        DamageShield();
        return;
      }

      //if (OnPlayerDamage != null)
      //	OnPlayerDamage();

      ExEvent.PlayerEvents.Damage.CallAsync();

      PlayDamageAudio(weaponType);

      if (weaponType == WeaponTypes.gate) {
        if (pet.Type != PetsTypes.none) {
          pet.PetDisconnect();
        }
      }

      // Если активен пет, сбрасываемся
      if (pet.Type != PetsTypes.none) {
        pet.PetDisconnect(true);
        SetGraceDamage(true);
        return;
      }

      if (weaponType == WeaponTypes.stone && defendStones > 0) {
        defendStones--;
        ExEvent.GameEvents.SetArmorPlayer.Call(ClothesSets.defendBarrier);
        //RunnerGamePlay.armorActiv(ClothesSets.defendBarrier);
        return;
      }

      if (defendSpear > 0 && weaponType == WeaponTypes.spear) {
        defendSpear--;
        ExEvent.GameEvents.SetArmorPlayer.Call(ClothesSets.noAirAttack);
        //RunnerGamePlay.armorActiv(ClothesSets.noAirAttack);
        return;
      }

      if (defendHending > 0 && weaponType == WeaponTypes.hendingBarrier) {
        defendHending--;
        ExEvent.GameEvents.SetArmorPlayer.Call(ClothesSets.noHendingBarrier);
        //RunnerGamePlay.armorActiv(ClothesSets.noHendingBarrier);
        return;
      }

      if (from.x <= transform.position.x && damageAnim <= Time.time)
        animation.AddAnimation(1, animation.leftDamageAnim);

      if (from.x > transform.position.x && damageAnim <= Time.time)
        animation.AddAnimation(1, animation.rightDamageAnim);

      // Эффект атаки
      if (damageEffect) {
        GameObject playerKick = Pooler.GetPooledObject("PlayerKick");
        playerKick.transform.position = Vector3.Lerp(transform.position + Vector3.up / 2, from, 0.3f);
        playerKick.SetActive(true);
      }

      if (runnerPhase == RunnerPhase.tutorial)
        return;

      if (weaponType != WeaponTypes.gate) {
        SetGraceDamage(true);
      }

      if (damageAnim <= Time.time)
        damageAnim = Time.time + 1f;

      // Жизни
      if (damageType == DamagType.live)
        RunnerController.Instance.PlayerDamager(power);

      // Силы
      if (damageType == DamagType.power)
        RunnerController.Instance.PowerDamage(power);
    }

    IEnumerator GraceActive() {

      SkeletonRenderer skelet = animation.skeletonAnimation.GetComponent<SkeletonRenderer>();
      float alpha = 1;
      float color = 1;

      while (graceDamage > Time.time) {
        yield return new WaitForSeconds(0.3f);
        alpha = (alpha > 0.5f ? 0.5f : 1);
        color = (alpha == 0.5f ? 2f : 1);
        foreach (Spine.Slot bon in skelet.skeleton.slots) {
          bon.A = alpha;
          bon.R = color;
          bon.G = color;
          bon.B = color;
        }
      }
      foreach (Spine.Slot bon in skelet.skeleton.slots) {
        bon.A = 1;
        bon.R = 1;
        bon.G = 1;
        bon.B = 1;
      }
    }

    private string _onTriggerLayer;

    void OnTriggerEnter2D(Collider2D other) {

      _onTriggerLayer = LayerMask.LayerToName(other.gameObject.layer);

      IOnPlayerTrigger player = other.GetComponent<IOnPlayerTrigger>();

      if (player != null) {
        player.OnTriggerPlayer(this);
      }

      // При контакте с монетой, добавляем
      //if (other.tag.Equals("Coins")) {
      //	other.GetComponent<Coin>().AddCouns();
      //}

      // Если столкнулись с камнем
      if (_onTriggerLayer == "Barrier") {
        if (other.tag.Equals("RollingStone")) {

          if ((!isGround && rigidbody.velocity.y <= 0) || (pet.Type == PetsTypes.spider && pet.playerMuveUp)) {
            if (pet.Type != PetsTypes.spider)
              move.MinJump();
            Questions.QuestionManager.ConfirmQuestion(Quest.jumpOnStone, 1, transform.position);
          } else {
            ThisDamage(WeaponTypes.stone, DamagType.live, (int)RunnerController.barrierDamage(other.tag, true), transform.position, false);
          }
          other.GetComponent<BarrierController>().DestroyThis();
        }
      }

      if (_onTriggerLayer == "Enemy") {
        Questions.QuestionManager.ConfirmQuestion(Quest.enemyToch, 1, transform.position);
      }

      if (_onTriggerLayer == "Spiders") {
        if (/*!isGround &&*/ rigidbody.velocity.y < 0) {
          move.MinJump();
          other.GetComponent<Spider>().PlayerDamage();
        }
      }
    }

    public void Wonderer() {
      animation.AddAnimation(1, animation.wonderAnim, false, 0);
    }


    #region События кнопок
    /// <summary>
    /// РЕакция на кнопку прыжка
    /// </summary>
    /// <param name="flag"></param>
    public void JumpKey(bool value) {
      if (!isMove) return;
      move.jumpKey = value;
    }

    /// <summary>
    /// Реакция на кнопку движения
    /// </summary>
    /// <param name="mov"></param>
    public void MoveKey(float value) {
      if (!isMove) return;
      move.horizontalKey = value;
    }
    /// <summary>
    /// Реакция на кнопку атаки
    /// </summary>
    public void FireKey(bool shoot) {
      //if(shoot == false) return;

      if ((runnerPhase == RunnerPhase.run || runnerPhase == RunnerPhase.tutorial || runnerPhase == RunnerPhase.boss) && RunnerController.Instance.PlayerCanShoot && pet.Type == PetsTypes.none) {
        animation.timeScale = 1;
        base.shoot.Shoot(RunnerController.Instance.playerWeapon);
      } else if (pet.Type == PetsTypes.dino) {

        animation.timeScale = 1;
        pet.Shoot();
      } else if ((runnerPhase == RunnerPhase.run || runnerPhase == RunnerPhase.tutorial || runnerPhase == RunnerPhase.boss) && (pet.Type != PetsTypes.bat) && graceDamage < Time.time) {
        animation.timeScale = 1;
        SpearDefender();
      }
    }

    [HideInInspector]
    public float spearDefenderTime;           // Время в пределах которого выполняется отбивание
    /// <summary>
    /// Отбивание копья
    /// </summary>
    public void SpearDefender() {
      if ((runnerPhase == RunnerPhase.run || runnerPhase == RunnerPhase.tutorial) && spearDefenderTime + 0.5f < Time.time) {
        spearDefenderTime = Time.time + 0.5f;
        animation.AddAnimation(1, animation.spearDefenderAnim, false, 0);
        audio.Play(audio.swordDefendClip, AudioMixerTypes.runnerEffect);
        Invoke("CheckDamageEnemySpear", 0.35f);
      }
    }

    #endregion

    //public LayerMask enemyLayer;
    public float SableJackPower;

    void CheckDamageEnemySpear() {
      Collider2D[] check = Physics2D.OverlapCircleAll(new Vector3(transform.position.x - 1f, transform.position.y, transform.position.z), 0.4f);
      if (check.Length > 0) {
        foreach (Collider2D one in check) {
          if (LayerMask.LayerToName(one.gameObject.layer) == "Enemy") {
            if (one.GetComponent<ClassicEnemy>())
              one.GetComponent<ClassicEnemy>().Damage(WeaponTypes.sablePlayer, SableJackPower, new Vector3(transform.position.x - 0.25f, transform.position.y, transform.position.z), DamagePowen.level1);
          }
        }
      }
    }

    public WeaponTypes GetActiveWeapon() {
      return shoot.GetActiveWeapon();
    }

    /// <summary>
    /// Шут аттак
    /// </summary>
    public void ShootPlayer() {
      RunnerController.Instance.PlayerShoot();
    }

    /// <summary>
    /// Событие анимации
    /// </summary>
    /// <param name="state"></param>
    /// <param name="trackIndex"></param>
    /// <param name="e"></param>
    void AnimEvent(Spine.AnimationState state, int trackIndex, Spine.Event e) {
      if (runnerPhase == RunnerPhase.run || runnerPhase == RunnerPhase.tutorial || runnerPhase == RunnerPhase.boss)
        shoot.ShootEvent();
    }

    /// <summary>
    /// Окончание анимации
    /// </summary>
    /// <param name="state"></param>
    /// <param name="trackIndex"></param>
    void AnimEnd(Spine.AnimationState state, int trackIndex) {
      if (runnerPhase == RunnerPhase.run && state.GetCurrent(trackIndex).ToString() == "Weapon_Attack_Brave")
        shoot.ShootEvent();
    }

    /// <summary>
    /// Событие падения в яму
    /// </summary>
    private void PlayerFall() {
      if (playerFall || playerFallWait) return;

      if (RunnerController.playerDistantion >= 1100 && RunnerController.playerDistantion <= 1500)
        Questions.QuestionManager.ConfirmQuestion(Quest.breackDown1100_1500, 1, transform.position);

      // выбрасываем активного пета
      if (pet.Type != PetsTypes.none)
        pet.PetDisconnect();

      //if (OnPlayerDown != null)
      //	OnPlayerDown();
      ExEvent.PlayerEvents.PitDown.CallAsync();

      if (runnerPhase == RunnerPhase.tutorial) {
        PlayerFallConfirm(true);
        return;
      }

      rigidbody.velocity = Vector2.zero;
      rigidbody.bodyType = RigidbodyType2D.Kinematic;

      Questions.QuestionManager.ConfirmQuestion(Quest.fallBreack, 1, transform.position);

      if (breackReturn > 0) {
        breackReturn--;
        ExEvent.GameEvents.SetArmorPlayer.Call(ClothesSets.noBreack);
        //RunnerGamePlay.armorActiv(ClothesSets.noBreack);
        PlayerFallConfirm(true);
        return;
      }

      audio.PlayEffect(audio.fallClip, AudioMixerTypes.runnerEffect);

      int savePerk = UserManager.Instance.savesPerk;

      if ((!useGarpun && savePerk > 0) || (!useVideoInGarpun && UnityAdsController.adsReady && GameManager.DayAfterStart(1))) {
        useGarpun = true;
        useVideoInGarpun = true;
        playerFallWait = true;
        RunnerController.Instance.PlayerFall();
      } else {
        PlayerFallConfirm(false);
      }

    }

    public void PlayerFallConfirm(bool isReturn) {
      Debug.Log("PlayerFallConfirm" + isReturn);

      if (!isReturn) {
        RunnerController.Instance.PlayerDead();
        playerFallWait = false;
        return;
      }

      Questions.QuestionManager.ConfirmQuestion(Quest.breakToLive, 1);
      playerFallWait = false;
      playerFall = true;
      rigidbody.velocity = Vector2.zero;
      rigidbody.bodyType = RigidbodyType2D.Dynamic;
      rigidbody.AddForce(new Vector2(0, 18), ForceMode2D.Impulse);
      if (runnerPhase != RunnerPhase.tutorial) {
        SetGraceDamage(true);
      }
      PlayJumpOutAudio();

    }

    bool useGarpun;
    bool useVideoInGarpun;

    /// <summary>
    /// Установка грейс периода
    /// </summary>
    /// <param name="animate"></param>
    public void SetGraceDamage(bool animate = false) {
      graceDamage = Time.time + graceTime;
      if (animate)
        StartCoroutine(GraceActive());
    }

    /// <summary>
    /// Мигание игрока в пределах грайс периода
    /// </summary>
    /// <returns></returns>
    IEnumerator Graceses() {
      int cnt = (int)(graceTime / 0.2f);

      for (int i = 0; i < cnt; i++) {
        graphic.SetActive(!graphic.activeInHierarchy);
        yield return new WaitForSeconds(0.2f);
      }
      graphic.SetActive(true);
    }

    /// <summary>
    /// Анимация магической атаки
    /// </summary>
    public void MagicAttack() {
      animation.AddAnimation(1, animation.weaponAttackMagicAnim, false, 0);
      PlayBravoAudio();
    }

    void PlayJumpOutAudio() {
      audio.PlayOneShot(audio.jumpOutClip, 1);
    }

    public void PlayBravoAudio() {
      audio.PlayOneShot(audio.bravoClip[Random.Range(0, audio.bravoClip.Length)], 1);
    }

    public void PlayDamageAudio(WeaponTypes damagType) {

      if (damagType != WeaponTypes.stone && damagType != WeaponTypes.airStone && damagType != WeaponTypes.gate) {
        audio.PlayOneShot(audio.damageEnemy, 1);
      } else {
        audio.PlayOneShot(audio.damageStone, 1);

      }
    }


    #region Тень

    #endregion

    #region Магнит

    public float magnetRadius;  // Радиус действия магнита
    [HideInInspector]
    public bool magnetActive; // флаг активности магнита
    float magnetTime; // Время до окончания работы магнита

    /// <summary>
    /// Делегат активации магнита
    /// </summary>
    /// <param name="flag">Флаг активности</param>
    public delegate void MagnetActive(bool flag);
    public static event MagnetActive MagnetEvent;

    void MagnetOnEnable() {
      ClothesBonus fuulMagnet = Config.GetActiveCloth(ClothesSets.magnet);
      if (fuulMagnet.full)
        Magnet(true, true);
    }

    /// <summary>
    /// Обработка работы магнита
    /// </summary>
    /// <param name="activate">Флаг активации магнита</param>
    /// <param name="noLimit">Флаг активации бесконечного магнита</param>
    public void Magnet(bool activate = false, bool noLimit = false) {

      // Активация при необходимости
      if (activate) {
        magnetTime = (magnetActive ? magnetTime : Time.time) + (noLimit ? 99999999999f : 10f);
        magnetActive = true;
        if (MagnetEvent != null)
          MagnetEvent(magnetActive);
        GetComponent<Animator>().SetBool("magnet", magnetActive);
        Questions.QuestionManager.ConfirmQuestion(Quest.getMagnet, 1, transform.position);
      }

      // Дальше не идем, если магнит не активен
      if (!magnetActive)
        return;

      // Проверка на наобходимость запершить работу магнита
      if (magnetTime < Time.time) {
        magnetActive = false;
        GetComponent<Animator>().SetBool("magnet", magnetActive);
        if (MagnetEvent != null)
          MagnetEvent(magnetActive);
      }

    }

    /// <summary>
    /// Событие анимации работы магнита. Воспроизводит звуковой эффект
    /// </summary>
    public void MagnetAuraEvent() {
      audio.PlayEffect(audio.magnetAuraClip, AudioMixerTypes.runnerEffect);
    }

    #endregion

    #region Щит
    public float shieldTime;  // Врея работы щита
    float shieldEndTime;  // Время окончания работы щита
    bool shieldActive;  // Статус активности щита

    /// <summary>
    /// Обработка работы щита
    /// </summary>
    /// <param name="init">Инициализация щита</param>
    public void Shield(bool init = false) {

      if (init) {
        shieldEndTime = Time.time + shieldTime;
        shieldActive = true;
        GetComponent<Animator>().SetBool("shield", shieldActive);
      }

      if (shieldActive && shieldEndTime < Time.time) {
        shieldActive = false;
        GetComponent<Animator>().SetBool("shield", shieldActive);
      }
    }

    /// <summary>
    /// Эффект получения дамага при рабочем щите
    /// </summary>
    void DamageShield() {
      GetComponent<Animator>().SetTrigger("shieldColor");
    }

    #endregion

  }
}