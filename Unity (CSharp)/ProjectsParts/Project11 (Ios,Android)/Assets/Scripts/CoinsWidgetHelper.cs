using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsWidgetHelper : MonoBehaviour {

  public System.Action OnBound;

	public void BaundComplete() {
    if (OnBound != null) OnBound();
  }
}
