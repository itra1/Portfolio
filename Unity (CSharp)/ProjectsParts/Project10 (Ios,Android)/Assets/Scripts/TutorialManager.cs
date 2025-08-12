using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager> {

  private bool? _isTutorial;
  public bool Istutorial {
    set {
      _isTutorial = value;
      PlayerPrefs.SetString("tutorial", _isTutorial.Value.ToString());
    }
    get {
      if (_isTutorial == null)
        _isTutorial = bool.Parse(PlayerPrefs.GetString("tutorial","true"));
      return _isTutorial.Value;
    }
  }
   
}
