using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategoryShopDialog : MonoBehaviour {

	public Action<IapType> OnClick;

	public bool _isActive;
	public IapType type;

	public GameObject activePage;
	public GameObject deactivePage;

	public Animation anim;

	public void SetActive(bool isActive) {
		this._isActive = isActive;

		if (this._isActive) {
			anim.Play("active");
			return;
		}

		anim.Play("deactive");

	}

	public void Click() {
		AudioManager.Instance.library.PlayClickAudio();
		if (OnClick != null) OnClick(type);
	}

}
