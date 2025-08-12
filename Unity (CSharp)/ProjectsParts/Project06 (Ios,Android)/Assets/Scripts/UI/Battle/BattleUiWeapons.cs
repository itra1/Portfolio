using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Battle {
  public class BattleUiWeapons: MonoBehaviour {
    [SerializeField]
    private WeaponIconBase[] weaponArray;

    public void WeaponsListChange(List<Game.Weapon.WeaponManager> weaponManagersList) {
      //for (int i = 0; i < weaponManagersList.Count; i++) {
      //  weaponArray[i].gameObject.SetActive(true);
      //  weaponArray[i].SetData(i+1, weaponManagersList[i]);
      //}

      for (int i = 0; i < weaponArray.Length; i++) {
        weaponArray[i].gameObject.SetActive(true);
        if(weaponManagersList.Count > i)
          weaponArray[i].SetData(i + 1, weaponManagersList[i]);
        else
          weaponArray[i].SetData(i + 1, null);
      }


    }

    //public void ChangeOneIcon(int numIcon, WeaponIcon[] iconsArr, WeaponManager weaponManager)
    //{
    //  if (weaponManager.BulletCount > 0)
    //  {
    //    iconsArr[numIcon].addIcon.SetActive(false);
    //    iconsArr[numIcon].countIcon.SetActive(true);
    //    iconsArr[numIcon].weaponIcon.color = new Color(1f, 1f, 1f, 1);
    //    iconsArr[numIcon].countText.text = weaponManager.BulletCount.ToString();
    //  } else
    //  {
    //    iconsArr[numIcon].addIcon.SetActive(true);
    //    iconsArr[numIcon].countIcon.SetActive(false);
    //    iconsArr[numIcon].weaponIcon.color = new Color(0.5f, 0.5f, 0.5f, 1);
    //  }

    //}

  }
}
