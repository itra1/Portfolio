using System.Collections.Generic;
using Game.User;
using UnityEngine;

namespace Game.Weapon {

  [System.Serializable]
  public struct DamageObg {
    public int objId;
    public float lastDamageTime;
  }
  public enum WeaponCategory {
    weapon,
    abilities,
    asisstant
  }

  public enum WeaponType {
    none = 0,
    tomato = 1,
    bottle = 2,
    brick = 3,
    cat = 4,
    axe = 5,
    molotov = 6,
    molotovFire = 7,
    holyGrenade = 8,
    bazooka = 9,
    obrez = 10,
    tomatGun = 11,
    chainLightning = 12,
    hunter = 13,
    bear = 14,
    varganich = 15,
    alien = 16,
    eagle = 17,
    radiator = 18,
    iscander = 19,
    medicineChest = 20,
    automat = 21
  };

  /// <summary>
  /// Оружие плеера
  /// </summary>
  public abstract class WeaponManager: ExEvent.EventBehaviour, ISave {

    public static event System.Action<WeaponManager> OnWeaponChange;          // Изменение менеджера
    public System.Action shootComplited;

    public static bool UnlimBullet = false;

    [UUID]
    public string uuid;

    //#if UNITY_EDITOR
    public string title;
    //#endif

    public string titleTranslate;

    public PoolObjectCache[] poolerCaches;

    public int orderId;                                                 // Сортировка
    [HideInInspector]
    protected bool panentPlayerActive;                                  // Флаг, что родительский плеер активный
    public bool changeQueue;                                            // Необходимо выполнить изменение очереди после выстрела
    public WeaponCategory category;
    public WeaponType weaponType;                                       // Тип используемого оружия

    private float _shootSpeed;
    protected float TimeBetweenShoot {
      get { return 1f / _shootSpeed; }
    }

    [HideInInspector]
    public bool isSelected;                                             // Выбран

    public bool IsActive { get; set; } = false;
    [HideInInspector]
    public float _timeReload;
    [SerializeField]
    private Sprite _icon;                                                // Иконка
    [SerializeField]
    private Sprite _iconActive;                                          // Активная иконка
    public Sprite Icon { get { return _icon; } }
    public Sprite IconActive { get { return _iconActive != null ? _iconActive : _icon; } }
    [SerializeField]
    protected GameObject bulletPrefab;                                     // Снаряд
    public float TimeReaload {
      get {
        if (category != WeaponCategory.weapon)
          return _timeReload;
        else {
          try {
            return _timeReload;
          } catch {
            return _timeReload;
          }
        }
      }
    }

    public virtual int BulletCount {
      get {
        return 1;
      }
      set { }
    }

    // Время на перезарядку
    protected Vector3 targetPosition;                                   // Текущее положение указателя
    private Vector3 lastTargetPositionCalc;                             // Последняя позиция указателя испльзуемая в рассчетах
    protected Vector3 shootStartPosition;                               // Стартовая позиция броска
    private Vector3 velocityShoot;                                      // Вектор броска

    protected bool isInstance = false;

    private bool _isGet;
    /// <summary>
    /// Получен
    /// </summary>
    public bool IsGet {
      get {
        if (!isInstance)
          Load();
        return _isGet;
      }
      set {
        if (_isGet == true && _isGet == value)
          return;
        _isGet = value;
        Save();
      }
    }

    [HideInInspector]
    public float startReloadTime = -1000;

    protected float shootTime = 0;

    public string playerStartAnim;
    public string playerIdleAnim;
    public string playerEndAnim;

    [System.Serializable]
    public struct LevelAdded {
      public int group;
      public int level;
    }

    protected override void Awake() {
      base.Awake();
      WeaponGenerator.WeaponSelected += WeaponSelected;
    }

    protected virtual void Start() {

      isInstance = true;

      InitPrefabs();

      startReloadTime = -100;
      shootStartPosition = PlayerController.Instance.ShootPoint;
      EmitEventChange();
    }


    protected override void OnDestroy() {
      base.OnDestroy();
      WeaponGenerator.WeaponSelected -= WeaponSelected;
    }

    [ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.BattlePhaseChange))]
    public void PhaseChange(ExEvent.BattleEvents.BattlePhaseChange phase) {
      if (phase.phase == BattlePhase.start) {
        startReloadTime = -1000;
        SetActiveStatus(false);
        OnShootComplited(false);
        //Inicialized();
        EmitEventChange();
      }
    }

    protected void SetActiveStatus(bool isActive) {
      IsActive = isActive;
      EmitEventChange();
    }

    public virtual void InitPrefabs() {
      foreach (var poolerCachesElement in poolerCaches) {
        PoolerManager.AddPool(poolerCachesElement);
      }
      if (bulletPrefab != null)
        PoolerManager.AddPool(bulletPrefab, 0, 3);
    }

    private void WeaponSelected(WeaponType newWeaponType) {

      if (newWeaponType == weaponType && isSelected)
        return;

      bool isSelectLast = isSelected;

      isSelected = (newWeaponType == weaponType);

      if (!isSelected && isSelectLast != isSelected) {
        DeInicialized();
      }

      if (isSelected)
        Inicialized();
      EmitEventChange();
    }

    public void SetPanentPlayerActive(bool parentActive) {
      panentPlayerActive = parentActive;
    }

    protected virtual void DeInicialized() {
    }

    public virtual void Inicialized() {
      PlayActivationAudio();
      //PlayerController.Instance.ChangeWeaponInHand(weaponType);
    }

    protected bool isTap;
    protected Vector3 pointerDown;
    protected Vector3 pointerUp;


    public virtual void OnPointerDown(Vector3 position) {
      if (!isSelected)
        return;

      isTap = true;
      pointerDown = position;
      pointerUp = pointerDown;
      OnClickDown(pointerDown);
    }

    public virtual void OnPointerUp(Vector3 position) {
      isTap = false;
      pointerUp = position;
      OnClickUp(pointerUp);
    }
    public void OnPointerDrag(Vector3 position, Vector3 delta) {
      if (!isSelected || !isTap)
        return;
      OnClickDrag(position, delta);
    }

    protected virtual void OnClickDown(Vector3 position) { }
    protected virtual void OnClickUp(Vector3 position) { }

    protected virtual void OnClickDrag(Vector3 position, Vector3 delta) { }
    protected virtual void Update() { }

    public virtual void StartShooting(Vector3 tapStart, Vector3 tapEnd) {
      if (!CheckReadyShoot(tapStart, tapEnd))
        return;
      StartShootAnimation();
    }

    public List<string> playerShootAnim;
    protected virtual void StartShootAnimation() {
      PlayerController.Instance.playerAnimation.SetAnimation(0, playerShootAnim[Random.Range(0, playerShootAnim.Count)], false);
    }

    /// <summary>
    /// Проверка на возможность выстрела
    /// </summary>
    /// <returns></returns>
    public virtual bool CheckReadyShoot(Vector3 tapStart, Vector3 tapEnd) {
      if (IsReloading || !isSelected || IsActive)
        return false;

      return true;
    }

    /// <summary>
    /// Перезаряжается
    /// </summary>
    public bool IsReloading {
      get { return startReloadTime + TimeReaload > Time.time; }
    }

    public void ShootAnimPlayerEvent(Vector3 tapStart, Vector3 tapEnd) {
      Shoot(tapStart, tapEnd);
      OnShootAnimEvent();
    }

    protected virtual void OnShootAnimEvent() { }

    /// <summary>
    /// Осуществление выстрела
    /// </summary>
    public virtual void Shoot(Vector3 tapStart, Vector3 tapEnd) {


      if (!CheckReadyShoot(tapStart, tapEnd))
        return;

      OnShoot(tapStart, tapEnd);
      OnShootComplited();
    }

    protected virtual void OnShoot(Vector3 tapStart, Vector3 tapEnd) {
      OnShoot();
    }


    protected void EmitEventChange() {
      if (OnWeaponChange != null)
        OnWeaponChange(this);
    }

    protected virtual void OnShoot() {
      PlayShootAudio();

      BattleManager.Instance.AddUseWeapon(weaponType);
      EmitEventChange();
    }

    public virtual void Overcharge() {
      PlayReload();
    }

    protected void PlayReload() {

      OnReloadStart();

      startReloadTime = Time.time;
      if (TimeReaload > 0) {
        //WeaponGenerator.instance.StartWeaponReload(this, timeReaload, 1, 0);
        EmitEventChange();
      }
    }

    protected virtual void OnReloadStart() { }
    protected virtual void OnReloadEnd() { }

    protected virtual void OnShootComplited(bool isReload = true) {
      if (IsActive != false)
        CompliteShootAudio();

      SetActiveStatus(false);

      //if (Magazin <= 0) {
      //  PlayReload();
      //}

      if (shootComplited != null)
        shootComplited();

      WeaponGenerator.Instance.WeaponOnShoot(this, changeQueue);
    }

    #region Настройки

    protected Configuration.Weapon wepConfig;
    public virtual void GetConfig() {
      try {

        wepConfig = GameDesign.Instance.allConfig.weapon.Find(x => x.id == (int)weaponType);

        _timeReload = wepConfig.timeReload.Value;

        _shootSpeed = wepConfig.shootSpeed == null || wepConfig.shootSpeed.Value == 0 ? 999 : wepConfig.shootSpeed.Value;

        startReloadTime = -1000;
      } catch {
        Debug.Log(gameObject.name);
      }
    }

    protected bool IsOpened {
      get {
        
        int? openNum = GameDesign.Instance.allConfig.levels.Find(x =>
          x.chapter == UserManager.Instance.ActiveBattleInfo.Group &&
          x.level == UserManager.Instance.ActiveBattleInfo.Level).openWeapon;

        if (openNum == null || openNum == 0 || openNum.Value == 0)
          return false;

        WeaponType wep = (WeaponType)openNum.Value;
        Debug.Log("is opened");
        return wep == weaponType;
      }
    }

    #endregion

    #region Звуки

    public AudioBlock shootAudioBlock;
    public AudioBlock activationAudioBlock;
    public AudioBlock compliteAudioBlock;

    protected virtual void PlayShootAudio() {
      shootAudioBlock.PlayRandom(PlayerController.Instance);
    }

    protected virtual void PlayActivationAudio() {
      activationAudioBlock.PlayRandom(PlayerController.Instance);
    }

    protected virtual void CompliteShootAudio() {
      compliteAudioBlock.PlayRandom(PlayerController.Instance);
    }

    #endregion

    public virtual void Save() {
      PlayerPrefs.SetString(weaponType.ToString() + " isGet", _isGet.ToString());
    }

    public virtual void Load() {
      _isGet = bool.Parse(PlayerPrefs.GetString(weaponType.ToString() + " isGet","false"));

    }


  }

}