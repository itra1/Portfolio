using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashUi : UiPanel {
	public override void ManagerClose() {
		//ToGame();
	}

	private void Start() {
		GetComponent<Animation>().Play("play");
	}

	protected override void OnDisable() {
	}

	IEnumerator CheckReady() {
		
		while(PlayerManager.Instance.company.saveCompanies.Count == 0 || PlayerManager.Instance.company.saveCompanies[0].levels.Count == 0)
			yield return new WaitForSeconds(0.5f);

		if(!Tutorial.Instance.isComplete && !Tutorial.Instance.isTutorial)
			yield return new WaitForSeconds(0.5f);

		ToGame();
	}

	public void ToGame() {
		GameManager.Instance.AfterSplash();
	}

	public void AnimComplete() {
		StartCoroutine(CheckReady());
	}
	
}
