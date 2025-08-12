using System.Collections;
using UnityEngine;

public class WeaponBehaviour: MonoBehaviour {

#if UNITY_EDITOR
  public string titleEditor;
#endif

  [Tooltip("Получить только в туторе")]
  [SerializeField]
  private bool _isTutorOnly;

  public bool IsTutorOnly {
    get {
      return _isTutorOnly;
    }
  }

  [UUID]
  public string uuid;
  public WeaponGroup.GroupType groupType;
  public string title;
  public bool wishBullets;
  public bool isExpendable = true;

  [SerializeField]
  private int _sortIndex;

  public int SortIndex {
    get { return _sortIndex; }
  }

  public WeaponManager.WeaponData weaponData;

  [SerializeField]
  [ResourcePath]
  private string _weaponIconSource;
  private Sprite _weaponIcon;

  public Sprite WeaponIcon {
    get {
      if (_weaponIcon == null)
        _weaponIcon = Resources.Load<Sprite>(_weaponIconSource);
      return _weaponIcon;
    }
  }

  private WeaponGroup _group;
  public WeaponGroup Group {
    get {
      if (_group == null)
        _group = WeaponManager.Instance.groupList.Find(x => x.type == groupType);
      return _group;
    }
  }

  [SerializeField]
  private long damageDefault;    // Стандартное значение дамага
  public long Damage {
    get {
      return damageDefault;
    }
  }

  public float BulletCount {
    get {
      return Group.bulletCount;
    }
  }

  public float reloadDefault;   // Стандартное значение времени перезарядки

  public bool unlimBullets;     // Безлимитные ли патроны
  public bool massDMG;          // Урон по всем

  public bool isStartReady;     // Доступно на страте

  private float timeNextShootReady = -100;

  public WeaponUi uiGui;

  public WeaponUi UiGuiInstance { get; set; }

  private bool _isSelected;
  public bool IsSelected {
    get {
      return _isSelected;
    }
    set {
      if (_isSelected == value)
        return;
      _isSelected = value;
      OnChengeWeapon();
    }
  }

  private bool IsReloading {
    get {
      return timeNextShootReady > Time.time;
    }
  }

  /// <summary>
  /// Разрешение сделать выстрел
  /// </summary>
  public bool IsShootReady {
    get {
      return IsSelected
          && (BulletCount > 0 || unlimBullets)
          && !IsReloading;
    }
  }

  /// <summary>
  /// Необходимо перезарядиться
  /// </summary>
  private bool NeedReload {
    get {
      return BulletCount > 0 && !IsReloading;
    }
  }

  private void Start() { }

  public void OnChengeWeapon() {
    ExEvent.BattleEvents.WeaponChange.Call(this);
  }

  private void OnDestroy() {
    StopAllCoroutines();
  }

  public void SetSelected(string uuid) {
    IsSelected = this.uuid == uuid;
  }

  public void Shoot(Enemy targetEnemy) {

    if (!IsShootReady)
      return;
    PlayShootAudio();

    if (!unlimBullets) {
      Group.bulletCount--;
    }

    timeNextShootReady = Time.time + reloadDefault;
    ExEvent.BattleEvents.WeaponShoot.Call(this, targetEnemy);

    if (!NeedReload) {
      //todo Невозможно перезарядиться
      return;
    }

  }

  private IEnumerator Reload() {

    if (!NeedReload)
      yield break;

  }

  private void PlayShootAudio() {

    switch (groupType) {
      case WeaponGroup.GroupType.automat:
        DarkTonic.MasterAudio.MasterAudio.PlaySound("Weapon",1,1,0, "Automat");
        break;
      case WeaponGroup.GroupType.usi:
        DarkTonic.MasterAudio.MasterAudio.PlaySound("Weapon", 1, 1, 0, "Uzi");
        break;
      case WeaponGroup.GroupType.pistol:
        DarkTonic.MasterAudio.MasterAudio.PlaySound("Weapon", 1, 1, 0, "Pistol");
        break;
      case WeaponGroup.GroupType.obrez:
        DarkTonic.MasterAudio.MasterAudio.PlaySound("Weapon", 1, 1, 0, "Shotgun");
        break;
      case WeaponGroup.GroupType.others:
        DarkTonic.MasterAudio.MasterAudio.PlaySound("Weapon", 1, 1, 0, "Knite");
        break;
    }

  }

  public AudioClipData shoopAudio;

}
