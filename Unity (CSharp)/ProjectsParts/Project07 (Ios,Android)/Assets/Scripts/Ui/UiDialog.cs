using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UiDialog : UiPanel {
	
  protected Animator _animation;
	public static UiDialog lastOpenDialog;

  protected Animator animation {
    get {
      if (_animation == null)
        _animation = GetComponent<Animator>();
      return _animation;
    }
  }

	protected bool isOpened = false;

	

  protected override void OnEnable() {
    base.OnEnable();
		
	}

  protected override void OnDisable() {
    base.OnDisable();
		
	}

  public virtual void Show() {
		if (lastOpenDialog != null && lastOpenDialog == this && lastOpenDialog.gameObject.activeInHierarchy) return;
		if (lastOpenDialog != null && lastOpenDialog != this && lastOpenDialog.gameObject.activeInHierarchy) {
			lastOpenDialog.Hide(() => { PlayShow(); });
		} 
		else PlayShow();
		lastOpenDialog = this;


	}

	public virtual void PlayShow() {
		gameObject.SetActive(true);
    animation.SetTrigger("show");

	}

	public virtual void Hide(System.Action OnHideComplited = null) {
		UiPanel.OnHideComplited = OnHideComplited;

    animation.SetTrigger("hide");

  }

}
