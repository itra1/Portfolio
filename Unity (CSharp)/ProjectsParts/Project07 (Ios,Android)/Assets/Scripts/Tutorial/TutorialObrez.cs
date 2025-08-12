using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial {

  public class TutorialObrez: Tutorial {

    public ChestProduct chest;

    public override void Show() {

      if (isShowing) {
        TutorialManager.Instance.Complete();
        return;
      }
      Load();
      if (isShowing) {
        TutorialManager.Instance.Complete();
        return;
      }

      Time.timeScale = 0;
      isShowing = true;
      Save();

      chest.onShowComplete = () =>
      {

        WeaponGamePlayIcon icon = UiController.GetUi<GamePlay>().iconsList
          .Find(x => x.weapon.uuid == "d281772c-2892-4b6f-b068-fd95b7f98d08");

        TutorialManager.Instance.Show(Type.newWeapon1, () => { }, icon);
        Complete();
      };

      Shop.Instance.BuyProduct(chest);

    }

  }

}