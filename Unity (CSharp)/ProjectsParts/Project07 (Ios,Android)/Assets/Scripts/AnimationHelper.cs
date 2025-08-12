using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHelper: MonoBehaviour {

  public System.Action Event1;

  public void OnEvent1() {
    if (Event1 != null) Event1();
  }

}
