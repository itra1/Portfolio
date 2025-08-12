using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsChangeButton : ExEvent.EventBehaviour {

	public GameObject closeIcon;

	private void OnEnable() {
		ChangeParam(PlayerManager.Instance.audioEffects);
	}

	public void Click() {
		PlayerManager.Instance.audioEffects = !PlayerManager.Instance.audioEffects;
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnChangeEffects))]
	private void OnChangeEffects(ExEvent.GameEvents.OnChangeEffects param) {
		ChangeParam(param.isActive);
	}

	public void ChangeParam(bool isOn) {
    if(closeIcon != null)
		  closeIcon.SetActive(!isOn);
	}

}
