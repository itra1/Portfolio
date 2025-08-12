using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShareDialog : PanelUi {

	protected override void OnEnable() {
		base.OnEnable();
	}

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

#if PLUGIN_FACEBOOK
		UiController.ClickButtonAudio();
		FBController.ShareResult();
#endif

		CloseThis();
	}

	public override void BackButton() {
		CloseThis();
	}
}
