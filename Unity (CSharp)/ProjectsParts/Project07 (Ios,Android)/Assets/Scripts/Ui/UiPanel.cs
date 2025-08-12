using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiPanel : ExEvent.EventBehaviour {

	public static System.Action OnHideComplited;

  protected virtual void OnEnable() {

  }

  protected virtual void OnDisable() {

	}

	public virtual void ShowComplete() {
	}

	public virtual void HideComplete() {
		gameObject.SetActive(false);

    var func = OnHideComplited;
    OnHideComplited = null;
    if (func != null) func();
  }

	


}
