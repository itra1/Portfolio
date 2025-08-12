using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Weapon;

namespace Game.UI.Battle {

  /// <summary>
  /// Иконка оружия на панели
  /// </summary>
  public class WeaponGamePlayIcon: WeaponIconBase {

    private WeaponUiMagazin _magazinUi;
    private WeaponUiMagazin MagazinUi {
      get {
        if (_magazinUi == null)
          _magazinUi = GetComponent<WeaponUiMagazin>();
        return _magazinUi;
      }
    }

    private Animation _animationComponent;

    private Animation animationComponent {
      set { _animationComponent = value; }
      get {
        if (_animationComponent == null) {
          _animationComponent = GetComponent<Animation>();
        }
        return _animationComponent;
      }
    }

    public GameObject addIcon;
    public GameObject countIcon;

    [System.Serializable]
    public struct Icons {
      public WeaponType type;
      public GameObject icons;
    }

    public List<Icons> icons;

    public WeaponType weaponType;
    private WeaponManager weaponManager;

    public Image iconActive;
    public Image iconDisactive;
    private float timeReload;

    private void OnEnable() {
      WeaponManager.OnWeaponChange += OnWeaponChange;
      OnWeaponChange(weaponManager);
    }

    private void OnDisable() {
      WeaponManager.OnWeaponChange -= OnWeaponChange;
    }

    public void OnClick() {
      WeaponGenerator.Instance.SetActiveWeaponManager(weaponType, WeaponGenerator.WeaponSelectType.iconClick);
      PlayClickAudio();
    }

    public AudioBlock clickAudioBlock;

    protected virtual void PlayClickAudio() {
      if (clickAudioBlock != null)
        clickAudioBlock.PlayRandom(this);
    }

    public void SetData(WeaponManager newWeaponManager) {
      weaponManager = newWeaponManager;
      weaponType = weaponManager.weaponType;
      if (iconDisactive != null)
        iconDisactive.sprite = weaponManager.Icon;
      if (iconActive != null)
        iconActive.sprite = weaponManager.IconActive;

      if (weaponManager.category == WeaponCategory.asisstant) {
        icons.ForEach(x => x.icons.SetActive(x.type == weaponType));
      }

      SetActivate(weaponManager.isSelected, true);
      if (Status != StatusType.reloading) {
        if (weaponManager.BulletCount > 0) {
          SetStatus(StatusType.active);
        } else
          SetStatus(StatusType.noBullets);
      }

      if (countIcon != null)
        countIcon.SetActive(!Game.Weapon.WeaponManager.UnlimBullet);
      if (addIcon != null)
        addIcon.SetActive(!Game.Weapon.WeaponManager.UnlimBullet);
    }

    private void OnWeaponChange(WeaponManager target) {

      if (weaponManager != null)
        SetActivate((weaponManager.isSelected), true);

      if (target.weaponType != weaponType)
        return;

      if (target.IsActive) {
        SetStatus(StatusType.locked);
        return;
      }


      if ((ZbCatScene.CatSceneManager.Instance.isSpecLevel || weaponManager.BulletCount > 0) && weaponManager.startReloadTime + weaponManager.TimeReaload > Time.time) {
        StartReload();
      } else if (weaponManager.BulletCount > 0 && weaponManager.startReloadTime + weaponManager.TimeReaload < Time.time) {
        ClearTimer();
        if (weaponManager.category != WeaponCategory.asisstant)
        {

          MagazinUi.SetBullet((target as Game.Weapon.WeaponStandart).magazinMax
            , (target as Game.Weapon.WeaponStandart).Magazin);
        }
      }


      if (target.weaponType == weaponType)
        ChanceCountBullet();
      if (Status != StatusType.reloading) {
        if (weaponManager.BulletCount > 0) {
          if (Status != StatusType.reloading)
            SetStatus(StatusType.active);
        } else
          SetStatus(StatusType.noBullets);
      }
    }

    public TextUGUIScale textCount;

    private void ChanceCountBullet() {

      if (countIcon == null)
        return;

      textCount.SetText(weaponManager.BulletCount.ToString());
    }
    
    private void SetActivate(bool nowActive, bool force = false) {
      if (!force && nowActive == isActive)
        return;
      isActive = nowActive;
      ChanceCountBullet();
      transform.localScale = isActive ? Vector3.one * 1.2f : Vector3.one;
    }

    private void SetStatus(StatusType newStatus, bool force = false) {
      if (!force && newStatus == Status)
        return;
      Status = newStatus;

      switch (Status) {
        case StatusType.active:
          TimerUi.timer.fillAmount = 0;
          break;
        case StatusType.reloading:
        case StatusType.locked:
          if (addIcon != null)
            addIcon.SetActive(false);
          if (countIcon != null)
            countIcon.SetActive(true);
          TimerUi.timer.fillAmount = 1;
          break;
        case StatusType.noBullets:
          if (addIcon != null)
            addIcon.SetActive(true);
          if (countIcon != null)
            countIcon.SetActive(false);
          break;
      }


      if (countIcon != null)
        countIcon.SetActive(!Game.Weapon.WeaponManager.UnlimBullet);
      if (addIcon != null)
        addIcon.SetActive(!Game.Weapon.WeaponManager.UnlimBullet);

    }

    #region Таймер

    public void StartReload() {
      if (Status == StatusType.reloading)
        return;
      if (!ZbCatScene.CatSceneManager.Instance.isSpecLevel && weaponManager.BulletCount == 0)
        return;
      SetStatus(StatusType.reloading);

      float timereload = (weaponManager.startReloadTime + weaponManager.TimeReaload) - Time.time;
      bool isAnimate = false;

      if (timereload > 0.5f) {
        isAnimate = true;
        animationComponent.Play("hide");
      }

      TimerUi.StartTimer(timereload, () => {

        if (isAnimate) {
          animationComponent.Play("show");
        }

        if (weaponManager.BulletCount == 0)
          SetStatus(StatusType.noBullets);
        else
          SetStatus(StatusType.active);
      });
    }

    public void ClearTimer() {
      if (weaponManager.BulletCount == 0)
        return;
      SetStatus(StatusType.active);
      TimerUi.ClearTimer();
    }

    #endregion

  }

}