using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial {
  public class Tutorial: ExEvent.EventBehaviour {

    public Type type;

    public string text;

    public bool isShowing = false;

    private float timeOpen;

    private string playerPefsKey {
      get {
        return "tutorial" + type;
      }
    }
    TutorialUi tutorialUi;
    protected override void Awake() {
      base.Awake();
      isShowing = false;
    }

    public virtual void Show() {
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

      tutorialUi = UiController.GetUi<TutorialUi>();

      tutorialUi.SetData(this);
      tutorialUi.Show();

    }

    public void Complete() {

      Time.timeScale = 1;
      TutorialManager.Instance.Complete();
      if (tutorialUi != null) tutorialUi.CloseButton();
    }

    public void Save() {
      PlayerPrefs.SetString(playerPefsKey, isShowing.ToString());
    }

    public void Load() {

      if (!PlayerPrefs.HasKey(playerPefsKey)) {
        isShowing = false;
      } else {
        isShowing = bool.Parse(PlayerPrefs.GetString(playerPefsKey));
      }

    }

  }


  public enum Type {
    intro,
    tap,
    medicine,
    weapon1,
    newWeapon1,
    complete,
    weapon2,
    newWeapon2,
    enemyStats
  }

}