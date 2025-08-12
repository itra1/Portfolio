using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PushDialog : PanelUi {
  	
	protected override void OnDisable() {
		base.OnDisable();
		if (OnClose != null) OnClose();
	}

	public void CloseThis() {
		UiController.ClickButtonAudio();
		GetComponent<Animator>().SetTrigger("close");
		StartCoroutine(waitAndDisable());
	}

	IEnumerator waitAndDisable() {
		yield return new WaitForSeconds(0.5f);
		Destroy(gameObject);
	}

	public void ButtonYes() {
		UiController.ClickButtonAudio();
#if PLUGIN_VOXELBUSTERS
		Apikbs.Instance.SubscribePush();
#endif
		UserManager.pushRegister = true;
		CloseThis();
	}

	public override void BackButton() {
		CloseThis();
	}
}
