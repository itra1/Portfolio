using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour {

	void Start () {
    SetLocalization();
  }

  private void SetLocalization() {

    switch (Application.systemLanguage) {
      case SystemLanguage.Russian:
      case SystemLanguage.Belarusian:
      case SystemLanguage.Ukrainian:
        //I2.Loc.LocalizationManager.CurrentLanguage = "English";
        I2.Loc.LocalizationManager.CurrentLanguage = "Russian";
        break;
      default:
        I2.Loc.LocalizationManager.CurrentLanguage = "English";
        break;
    }

  }
}
