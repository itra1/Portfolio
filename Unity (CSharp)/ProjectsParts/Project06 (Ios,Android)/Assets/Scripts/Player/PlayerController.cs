using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ExEvent;
using Spine;
using Spine.Unity;
using Random = UnityEngine.Random;
using Game.Weapon;

[System.Serializable]
public struct WeaponStructIconForHand {
  public WeaponType type;
  public GameObject iconForHand;
}


/// <summary>
/// Игрок
/// </summary>
public class PlayerController: Singleton<PlayerController>, IPlayerDamage {

  public PlayerAnimator playerAnimation;
  private WeaponManager _activeWeapon;

  [SerializeField]
  private PlayerVectorBoneRotation _boneRotation;

  public WeaponManager ActiveWeapon { get { return _activeWeapon; } }

  public static event System.Action<float> hitTarget;

  public WeaponStructIconForHand[] iconForHandPref;

  [SerializeField]
  private Transform _shootPoint;
  public Vector3 ShootPoint {
    get {
      return _shootPoint.position;
    }
  }
  public Transform forwardSmoke;
  public Transform backSmoke;
  
  public CharacterSize characterSize;

  protected Vector3 startTap;     // Начало смаха
  protected Vector3 endTap;       // Конец смаха
  public bool isTap;
  protected float timeShootWait;

  public string idleAnim;

  public ParticleSystem forwardSmokeParticle;
  public ParticleSystem backSmokeParticle;

  public bool DamageReady {
    get { return true; }
  }

  public virtual void Update() {
    ChangeLineEnergy();
  }

  public void OnEnable() {
    characterSize.size = GameDesign.Instance.allConfig.summary.Find(x => x.name == "PlayerSize").param1;
    if (characterSize.useSize)
      transform.localScale = new Vector3(characterSize.size, characterSize.size, characterSize.size);

    StartCoroutine(InitAnimation());
    //SetAnimation(0, _activeWeapon.playerIdleAnim,true);
  }

  IEnumerator InitAnimation() {

    yield return new WaitForSeconds(0.3f);

    playerAnimation.OnAnimComplited += AnimComplited;
    playerAnimation.OnAnimEvent += AnimEvent;
    playerAnimation.OnAnimInterrupt += AnimInterrupt;
    playerAnimation.OnAnimStart += AnimStart;
    playerAnimation.ResetAnimation();
    yield return new WaitForSeconds(0.01f);
    playerAnimation.SetAnimation(0, "idle", true);
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.ScreenEvents.PointerDown))]
  public void OnPointerDown(ExEvent.ScreenEvents.PointerDown eventData) {
    isTap = true;

    Vector3 targetPoint = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, 10));

    UpdateBoneVector(targetPoint);

    if (_activeWeapon != null)
    _activeWeapon.OnPointerDown(targetPoint);

    //OnDisplayTap(hand.position, Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, 10)));
  }
  [ExEvent.ExEventHandler(typeof(ExEvent.ScreenEvents.PointerUp))]
  public void OnPointerUp(ExEvent.ScreenEvents.PointerUp eventData) {

    isTap = false;
    Vector3 targetPoint = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, 10));

    if (_activeWeapon != null)
      _activeWeapon.OnPointerUp(targetPoint);
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.ScreenEvents.PointerDrag))]
  public void OnPointerDrag(ExEvent.ScreenEvents.PointerDrag eventData) {

    isTap = false;
    Vector3 targetPoint = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, 10));

    UpdateBoneVector(targetPoint);

    if (_activeWeapon != null)
      _activeWeapon.OnPointerDrag(targetPoint, eventData.delta);
  }

  private void UpdateBoneVector(Vector3 targetPoint)
  {
    float angle = Vector3.Angle(Vector3.right, targetPoint - ShootPoint);

    _boneRotation.SetAngleVector(angle * (targetPoint.y > ShootPoint.y ? 1 : -1));
  }


  public void FakeShoot(Vector3 wordPosition) {
    if (_activeWeapon != null)
      _activeWeapon.OnPointerDown(Camera.main.ScreenToWorldPoint(new Vector3(wordPosition.x, wordPosition.y, 10)));
   // OnDisplayTap(hand.position, wordPosition, true);
  }
  public void FakeShoot(Vector2 tapStart) {
    if (_activeWeapon != null)
      _activeWeapon.OnPointerDown(Camera.main.ScreenToWorldPoint(new Vector3(tapStart.x, tapStart.y, 10)));
    //OnDisplayTap(hand.position, Camera.main.ScreenToWorldPoint(new Vector3(tapStart.x, tapStart.y, 10)), true);
  }

  public virtual void SetNewWeapon(WeaponManager newWeaponManager) {

    if (_activeWeapon != null) {
      _activeWeapon.shootComplited = null;
    }

    bool _animRemove = false;

    if (_activeWeapon != null && !string.IsNullOrEmpty(_activeWeapon.playerEndAnim)) {
      _animRemove = true;
      playerAnimation.SetAnimation(0, _activeWeapon.playerEndAnim, false);
    }

    _activeWeapon = newWeaponManager;

    if (!_animRemove)
      ChangeWeaponInHand();

    _activeWeapon.shootComplited = () => {
      //playerAnimation.SetAnimation(0, _activeWeapon.playerIdleAnim, true);
    };

    if (_animRemove) return;

    if (!string.IsNullOrEmpty(_activeWeapon.playerStartAnim)) {
      playerAnimation.SetAnimation(0, _activeWeapon.playerStartAnim, false);
    } else {
      if (playerAnimation.currentAnimation != _activeWeapon.playerIdleAnim)
        playerAnimation.SetAnimation(0, _activeWeapon.playerIdleAnim, true);
    }

  }

  public void Damage(float damage) {
    if (hitTarget != null) hitTarget(damage);
  }

  /// <summary>
  /// Реакция на менеджмент оружия
  /// </summary>
  /// <param name="steck"></param>
  public void ChangeWeaponInHand() {
    SetWeaponInHand(_activeWeapon.weaponType);
  }

  /// <summary>
  /// Событие выстрела
  /// </summary>
  public void OnShoot() {
    if (startTap.x + 0.5f > endTap.x)
      endTap.x = startTap.x + 0.5f;

    _activeWeapon.ShootAnimPlayerEvent(startTap, endTap);
  }

  public virtual WeaponManager GetWeaponManager() {
    return _activeWeapon;
  }

  /// <summary>
  /// УСтановка оружия в руке кузмича
  /// </summary>
  /// <param name="weaponType">Тип оружия</param>
  void SetWeaponInHand(WeaponType weaponType) {

    for (int i = 0; i < iconForHandPref.Length; i++) {
      iconForHandPref[i].iconForHand.SetActive(iconForHandPref[i].type == weaponType);
    }
  }


  /// <summary>
  /// Подписываемся на смах по экрану
  /// </summary>
  /// <param name="startTarget">Начало движения пальца по экрану</param>
  /// <param name="endTarget">Начало движения пальца по экрану</param>
  public void OnDisplayTap(Vector3 startTarget, Vector3 endTarget, bool necessary = false) {
    if (!necessary && BattleManager.battlePhase != BattlePhase.battle) return;

    startTap = startTarget;
    endTap = endTarget;

  }


  #region lineEnergyWeapon

  public GameObject lineEnergyWeapon;
  public SpriteRenderer lineEnergyWeaponValue;

  public void ChangeLineEnergy() {
    if (lineEnergyWeapon == null) return;
    deltaShootTime = (Time.time - timeStartWait > timeMaxWait ? timeMaxWait : Time.time - timeStartWait);
    //deltaShootTime = Time.time - timeStartWait;

    if (deltaShootTime == timeMaxWait) {
      lineEnergyWeaponValue.color = Color.green;
    }
    lineEnergyWeaponValue.transform.localScale = new Vector3(lineEnergyWeaponValue.transform.localScale.x, (deltaShootTime / timeMaxWait), lineEnergyWeaponValue.transform.localScale.z);
  }

  protected float deltaShootTime;
  protected float timeStartWait = 1;
  protected float timeMaxWait = 1;

  public void SetEnergyLine(bool showLine, float newTimeStartWait = 1, float NewTimeMaxWait = 1) {
    if (!showLine) {
      lineEnergyWeapon.SetActive(false);
      return;
    }

    lineEnergyWeaponValue.color = Color.red;
    lineEnergyWeapon.SetActive(true);
    timeStartWait = newTimeStartWait;
    timeMaxWait = NewTimeMaxWait;

  }

  #endregion

  private bool _startAnimAttack = false;

  private bool startAnimAttack {
    get { return _startAnimAttack; }
    set {
      _startAnimAttack = value;
    }
  }

  public virtual void AnimComplited(string trackEntry) {
    if (trackEntry == _activeWeapon.playerStartAnim) {
      playerAnimation.SetAnimation(0, _activeWeapon.playerIdleAnim, true);
    }

    if (_activeWeapon != null && _activeWeapon.playerShootAnim.Contains(trackEntry)) {
      playerAnimation.SetAnimation(0, _activeWeapon.playerIdleAnim, true);
    }


    if (trackEntry.Contains("remove")) {
      ChangeWeaponInHand();
      if (!string.IsNullOrEmpty(_activeWeapon.playerStartAnim)) {
        playerAnimation.SetAnimation(0, _activeWeapon.playerStartAnim, false);
      } else {
        playerAnimation.SetAnimation(0, _activeWeapon.playerIdleAnim, true);
      }
    }
  }

  public virtual void AnimStart(string trackEntry) {
    if (trackEntry == "throw2" || trackEntry == "throw") {
      startAnimAttack = true;
      playerAnimation.animationTimeScale = 3;
    }
  }

  public virtual void AnimInterrupt(string trackEntry) {
    if (startAnimAttack) {
      startAnimAttack = false;
      playerAnimation.animationTimeScale = 1;
      OnShoot();

      //playerAnimation.SetAnimation(0, idleAnim, true);
    }
  }

  public void AnimEvent(string eventName) {
    if (eventName == "hit") {
      OnShootHit();
    }
  }

  void OnShootHit() {
    playerAnimation.animationTimeScale = 1;
    OnShoot();

    if (_activeWeapon.weaponType == WeaponType.bazooka) {
      forwardSmokeParticle.Play();
      backSmokeParticle.Play();
    }

    startAnimAttack = false;
  }

  #region Звуки

  public List<AudioClipData> hangShotAudio;

  public virtual void PlayHandShootAudio() {
    if (hangShotAudio.Count == 0) return;
    AudioManager.PlayEffects(hangShotAudio[Random.Range(0, hangShotAudio.Count)], AudioMixerTypes.effectPlay);
  }


  #endregion


}
