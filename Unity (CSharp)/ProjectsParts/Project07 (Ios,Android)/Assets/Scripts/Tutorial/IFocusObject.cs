using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial {
  public interface IFocusObject {

    void Focus(bool isFocus, System.Action OnClick = null);

  }
}