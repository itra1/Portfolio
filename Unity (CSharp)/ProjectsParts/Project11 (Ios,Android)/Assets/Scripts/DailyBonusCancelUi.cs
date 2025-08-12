using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyBonusCancelUi : UiPanel {

	public Action OnCancel;
	public Action OnExit;

	public Animation animComponent;

	protected override void OnEnable() {
		base.OnEnable();
		Show(() => {
			
		});
	}

	public void CancelButton() {
		isClosing = true;
		Hide(() => {
			isClosing = false;
			if (OnCancel != null) OnCancel();
			gameObject.SetActive(false);
		});
	}

	public override void Hide(Action OnHide = null) {
		base.Hide(OnHide);
		animComponent.Play("hide");
	}

	public override void Show(Action OnShow = null) {
		base.Show(OnShow);
		animComponent.Play("show");
	}

	public void ExitButton() {
		if (OnExit != null) OnExit();
	}

	public override void ManagerClose() {
		if (isClosing) return;
		CancelButton();
	}
}
