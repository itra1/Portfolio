using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Battle {
  public class WeaponIcon: WeaponIconBase {
    [SerializeField]
    private TMPro.TextMeshProUGUI _EfText;
    [SerializeField]
    private GameObject _iconDown;
    [SerializeField]
    private GameObject _addIcon;
    [SerializeField]
    private TMPro.TextMeshProUGUI _bulletCountText;
    [SerializeField]
    private Image _icone;

    private WeaponUiMagazin _magazinUi;
    private WeaponUiMagazin MagazinUi {
      get {
        if (_magazinUi == null)
          _magazinUi = GetComponent<WeaponUiMagazin>();
        return _magazinUi;
      }
    }

    protected override void OnReloadComplete() {
      MagazinUi.SetBullet((_weapon as Game.Weapon.WeaponStandart).magazinMax, (_weapon as Game.Weapon.WeaponStandart).Magazin);
    }

    protected override void ChanceCountBullet() {
      _bulletCountText.SetText(_weapon.BulletCount.ToString());
    }

    protected override void OnChengeStatus()
    {
      base.OnChengeStatus();
      //switch (Status) {
      //  case StatusType.reloading:
      //  case StatusType.locked:
      //    break;
      //  case StatusType.noBullets:
      //    break;
      //}

      if (_weapon) {
        _addIcon.SetActive(_weapon.BulletCount <= 0);
        _bulletCountText.gameObject.SetActive(_weapon.BulletCount > 0);
      }

    }

    public override void SetData(int num, Game.Weapon.WeaponManager weaponManager)
    {
      _EfText.text = "F" + num;
      _iconDown.SetActive(weaponManager != null);

      if (weaponManager == null)
        return;

      base.SetData(num, weaponManager);
    }

    protected override void ChangeActiveState()
    {
      base.ChangeActiveState();
      _icone.sprite = isActive ? _weapon.IconActive : _weapon.Icon;
      _icone.color = Color.white;
      _icone.GetComponent<AspectRatioFitter>().aspectRatio = _icone.sprite.rect.width / _icone.sprite.rect.height;
    }

  }
}