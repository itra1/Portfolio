using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternetNotFoundGui : UiPanel {

	public Action OnCancel;
	public Action OnRepeat;

	public Animation animComponent;
	
	public override void Hide(Action OnHide = null) {
		base.Hide(OnHide);
		animComponent.Play("hide");
	}

	public override void Show(Action OnShow = null) {
		base.Show(OnShow);
		animComponent.Play("show");
	}

	public void Repeat() {

		Hide(() => {
			gameObject.SetActive(false);
			if (OnRepeat != null) OnRepeat();
		});

	}

	public void Cancel() {

		Hide(() => {
			gameObject.SetActive(false);
			if (OnCancel != null) OnCancel();
		});
	}

	public override void ManagerClose() {
		Cancel();
	}
}
