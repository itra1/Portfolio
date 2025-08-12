using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRecordUiGamePlay : MonoBehaviour {

	public GameObject panel;
	public Animation anim;

	private void OnEnable() {
		OnEnableNewDecord();
	}

	private void OnDisable() {
		OnDisableNewRecord();
	}
	
	void OnEnableNewDecord() {
		BestDist.newRecord += NewRecord;
		
	}

	void OnDisableNewRecord() {
		BestDist.newRecord -= NewRecord;
	}

	public void NewRecord() {
		panel.SetActive(true);
		anim.Play("show");
	}
	

}
