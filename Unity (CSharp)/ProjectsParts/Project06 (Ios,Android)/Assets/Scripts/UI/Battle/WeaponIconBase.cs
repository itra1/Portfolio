using System.Collections.Generic;
using UnityEngine;
using Game.Weapon;

namespace Game.UI.Battle {
  public class WeaponIconBase: MonoBehaviour {

    protected Game.Weapon.WeaponManager _weapon;
    [SerializeField]
    protected Animator _animator;
    protected bool isActive;

    [SerializeField]
    private GameObject _lockIcone;

    [System.Flags]
    protected enum StatusType
    {
      deactive = 1
      , active = 2
      , reloading = 4
      , noBullets = 6
      , locked = 16
    }
    private StatusType _status = StatusType.deactive;

    [SerializeField]
    protected StatusType Status
    {
      get { return _status; }
      set{
        _status = value;
        OnChengeStatus();
      }
    }

    private WeaponUiTimer _timerUi;
    protected WeaponUiTimer TimerUi {
      get {
        if (_timerUi == null)
          _timerUi = GetComponent<WeaponUiTimer>();
        return _timerUi;
      }
    }

    public void OnClick() {
      if(_weapon == null)
        return;
      WeaponGenerator.Instance.SetActiveWeaponManager(_weapon, WeaponGenerator.WeaponSelectType.iconClick);
      PlayClickAudio();
    }

    public AudioBlock clickAudioBlock;

    protected virtual void PlayClickAudio() {
      if (clickAudioBlock != null)
        clickAudioBlock.PlayRandom(this);
    }

    private void OnEnable() {
      Game.Weapon.WeaponManager.OnWeaponChange += OnWeaponChange;
      OnWeaponChange(_weapon);
      Status = _status;
    }

    private void OnDisable() {
      Game.Weapon.WeaponManager.OnWeaponChange -= OnWeaponChange;
    }

    protected virtual void OnWeaponChange(Game.Weapon.WeaponManager target) {

      if(_weapon == null)
        return;

      if (_weapon != null)
        SetActivate((_weapon.isSelected), true);

      if (target != _weapon)
        return;

      if (target.IsActive) {
        SetStatus(StatusType.locked);
        return;
      }

      if ((ZbCatScene.CatSceneManager.Instance.isSpecLevel || _weapon.BulletCount > 0) && _weapon.startReloadTime + _weapon.TimeReaload > Time.time) {
        StartReload();
      } else if (_weapon.BulletCount > 0 && _weapon.startReloadTime + _weapon.TimeReaload < Time.time) {
        ClearTimer();
        OnReloadComplete();
      }

      if (target == _weapon)
        ChanceCountBullet();
      if (Status != StatusType.reloading) {
        if (_weapon.BulletCount > 0) {
          if (Status != StatusType.reloading)
            SetStatus(StatusType.active);
        } else
          SetStatus(StatusType.noBullets);
      }
    }

    public virtual void SetData(int num, Game.Weapon.WeaponManager weaponManager) {
      _weapon = weaponManager;

      _lockIcone.SetActive(weaponManager == null);

      if(_weapon == null)
        return;

      SetActivate(_weapon.isSelected, true);
      if (Status != StatusType.reloading) {
        if (_weapon.BulletCount > 0) {
          SetStatus(StatusType.active);
        } else
          SetStatus(StatusType.noBullets);
      }

    }

    private void SetActivate(bool isActive, bool force = false) {
      if (!force && this.isActive == isActive)
        return;
      this.isActive = isActive;
      ChangeActiveState();
    }

    protected virtual void ChangeActiveState()
    {
      ChanceCountBullet();
    }

    private void SetStatus(StatusType newStatus, bool force = false) {
      //if (!force && newStatus == Status)
      //  return;
      Status = newStatus;

      if (_weapon)
        _animator.SetBool("active", _weapon.isSelected);

    }

    protected virtual void OnReloadComplete() { }

    protected virtual void OnChengeStatus()
    {
      switch (Status) {
        case StatusType.active:
          TimerUi.timer.fillAmount = 0;
          break;
        case StatusType.reloading:
        case StatusType.locked:
          TimerUi.timer.fillAmount = 1;
          break;
        case StatusType.noBullets:
          break;
      }

      _animator.SetBool("visible", (Status & (StatusType.active | StatusType.reloading | StatusType.locked)) > 0);
    }

    protected virtual void ChanceCountBullet()
    {}

    #region Таймер

    public void StartReload() {
      if (Status == StatusType.reloading)
        return;
      if (!ZbCatScene.CatSceneManager.Instance.isSpecLevel && _weapon.BulletCount == 0)
        return;
      SetStatus(StatusType.reloading);

      float timereload = (_weapon.startReloadTime + _weapon.TimeReaload) - Time.time;
      bool isAnimate = false;

      if (timereload > 0.5f) {
        isAnimate = true;
        //animationComponent.Play("hide");
      }

      TimerUi.StartTimer(timereload, () => {

        if (isAnimate) {
          //animationComponent.Play("show");
        }

        if (_weapon.BulletCount == 0)
          SetStatus(StatusType.noBullets);
        else
          SetStatus(StatusType.active);
      });
    }

    public void ClearTimer() {
      if (_weapon.BulletCount == 0)
        return;
      SetStatus(StatusType.active);
      TimerUi.ClearTimer();
    }

    #endregion

  }
}
