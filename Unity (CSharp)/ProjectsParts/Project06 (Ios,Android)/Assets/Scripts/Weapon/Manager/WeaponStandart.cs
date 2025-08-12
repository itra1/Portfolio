using Game.User;
using UnityEngine;

namespace Game.Weapon {

  public abstract class WeaponStandart: WeaponManager {
    
    public int magazinMax = 1;                                          // Размер магазина
    public int Magazin { get; set; }                                    // Актуальное состояние магазина

    private int _bulletCount;
    /// <summary>
    /// Количество снарядов
    /// </summary>
    public override int BulletCount {
      get {
        if (!isInstance)
          Load();
        return _bulletCount;
      }
      set {
        _bulletCount = value;
        Save();
      }
    }

    protected override void Start() {
      base.Start();
      InitMagazin();
    }


    /// <summary>
    /// Инициализация оружейного магазина
    /// </summary>
    protected void InitMagazin() {
      Magazin = Mathf.Min(magazinMax, BulletCount);
    }

    protected override void OnReloadStart() {
      base.OnReloadStart();
      InitMagazin();
    }

    /// <summary>
    /// Увеличение количества снарядов
    /// </summary>
    /// <param name="incrementCount"></param>
    public void IncrementBulletCount(int incrementCount) {
      BulletCount += incrementCount;
      EmitEventChange();
    }


    protected void BulletDecrement() {
      Magazin--;
      if (!ZbCatScene.CatSceneManager.Instance.isSpecLevel) {
        BulletCount--;
      }
      EmitEventChange();
    }

    public override bool CheckReadyShoot(Vector3 tapStart, Vector3 tapEnd) {

      if (!base.CheckReadyShoot(tapStart, tapEnd))
        return false;

      if (Magazin <= 0 || (BulletCount <= 0 && !ZbCatScene.CatSceneManager.Instance.isSpecLevel))
      {
        PlayEmptyAudioBlock();
        return false;
      }

      return true;
    }

    protected virtual void PlayEmptyAudioBlock() { }

    protected override void OnShootComplited(bool isReload = true) {
      if (IsActive != false)
        CompliteShootAudio();

      SetActiveStatus(false);

      if (Magazin <= 0) {
        PlayReload();
      }

      if (shootComplited != null)
        shootComplited();

      WeaponGenerator.Instance.WeaponOnShoot(this, changeQueue);
    }

    public override void GetConfig() {
      base.GetConfig();

      if (IsOpened) {
        BulletCount = (int)wepConfig.startBullet;
        UserWeapon.Instance.AddWeapon(weaponType, BulletCount);
      }
      magazinMax = (int)wepConfig.magazinSize.Value;

    }
    public override void Save() {
      base.Save();
      PlayerPrefs.SetInt(weaponType.ToString() + " bullet", _bulletCount);
    }

    public override void Load() {
      base.Load();
      if (!PlayerPrefs.HasKey(weaponType.ToString() + " bullet")) {
        _bulletCount = 0;
      } else {
        _bulletCount = PlayerPrefs.GetInt(weaponType.ToString() + " bullet", 0);
      }
    }

    protected virtual void ByeWeapon() {
      if (!ZbCatScene.CatSceneManager.Instance.isSpecLevel && BulletCount <= 0) {
        WeaponGenerator.Instance.BuyWeapon(this, false);
      }
    }

    protected void ShowCatScana() {
      if (Game.User.UserManager.Instance.ActiveBattleInfo.Mode != PointMode.survival && BulletCount <= 0 && !ZbCatScene.CatSceneManager.Instance.isSpecLevel)
        ZbCatScene.CatSceneManager.Instance.ShowCatScene(10, () => { });
    }

  }

}