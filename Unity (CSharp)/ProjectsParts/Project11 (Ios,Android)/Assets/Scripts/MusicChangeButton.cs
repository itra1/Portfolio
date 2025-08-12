using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChangeButton : ExEvent.EventBehaviour {

	public GameObject closeIcon;

	private void OnEnable() {
		ChangeParam(PlayerManager.Instance.audioMisic);
	}
	
	public void Click() {
		PlayerManager.Instance.audioMisic = !PlayerManager.Instance.audioMisic;
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnChangeMusic))]
	private void OnChangeMusic(ExEvent.GameEvents.OnChangeMusic param) {
		ChangeParam(param.isActive);
	}

	public void ChangeParam(bool isOn) {
		closeIcon.SetActive(!isOn);
	}

}
