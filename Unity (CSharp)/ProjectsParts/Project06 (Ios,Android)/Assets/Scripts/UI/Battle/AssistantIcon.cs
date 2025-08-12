using UnityEngine;
using Game.Weapon;

namespace Game.UI.Battle {
  public class AssistantIcon: WeaponIconBase
  {
    [System.Serializable]
    public struct Icons {
      public WeaponType type;
      public GameObject icon;
    }

    [SerializeField]
    private System.Collections.Generic.List<Icons> _icons;

    public override void SetData(int num, WeaponManager weaponManager) {
      base.SetData(num, weaponManager);

      if(weaponManager == null)
        return;

      _icons.ForEach(x => x.icon.SetActive(x.type == _weapon.weaponType));

    }
  }
}
