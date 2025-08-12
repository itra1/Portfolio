using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial
{
  public class NewWeapon2 : Tutorial {
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

      chest.onShowComplete = () => {

        TutorialManager.Instance.Show(Type.newWeapon2, () => { });
        Complete();
      };

      Shop.Instance.BuyProduct(chest);

    }
  }
}