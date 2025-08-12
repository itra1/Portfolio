using System;
using ExEvent;
using UnityEngine;

public abstract class UiPanel : EventBehaviour {

	public bool isChangeAnimated { get; set; }

	public bool isDialog;

	public Action OnShow;
	public Action OnHide;
	public Action OnDeactive;

	protected bool isClosing;
	protected bool isOpening;

	protected virtual void OnEnable() {
		isClosing = false;
		if(isDialog)
			AudioManager.Instance.SetLowPass(true);
    if(UIManager.Instance != null)
		UIManager.Instance.VisibleDialog(this);
	}

	protected virtual void OnDisable() {
		isClosing = false;

		if (isDialog)
			AudioManager.Instance.SetLowPass(false);
    if (UIManager.Instance != null)
      UIManager.Instance.HiddenDialog(this);

		if (OnDeactive != null) OnDeactive();
	}

	public abstract void ManagerClose();
	
	public virtual void Show(Action OnShow = null) {
		this.OnShow = OnShow;
	}

	public virtual void ShowComplited() {
		if (OnShow != null) OnShow();
		OnShow = null;
	}

	public virtual void Hide(Action OnHide = null) {
		this.OnHide = OnHide;
	}

	public virtual void HideComplited() {
		if (OnHide != null) OnHide();

		OnShow = null;
	}

}
