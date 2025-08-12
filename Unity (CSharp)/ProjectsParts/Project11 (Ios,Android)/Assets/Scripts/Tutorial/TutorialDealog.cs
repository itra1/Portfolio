using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExEvent;
using System;

public class TutorialDealog : UiPanel {

	public GameObject dialog;
	
	[ExEvent.ExEventHandler(typeof(TutorialEvents.LevelLoad))]
	private void OnLevelLoad(TutorialEvents.LevelLoad level) {
		if (dialog.gameObject.activeInHierarchy) return;
		dialog.gameObject.SetActive(true);
		dialog.GetComponent<AnimationHelper>().OnEvent1 = () => {
			dialog.gameObject.SetActive(false);
			GameWorld word = (GameWorld)WorldManager.Instance.GetWorld(WorldType.game);
			word.OnShow();
		};
	}

	public void ClickDialog() {
		dialog.GetComponent<Animation>().Play("hide");
	}

	[ExEvent.ExEventHandler(typeof(TutorialEvents.TutorialEnd))]
	private void OnLevelLoad(TutorialEvents.TutorialEnd level) {
		gameObject.SetActive(false);
	}

	public override void ManagerClose() {
		if (isClosing) return;
		isClosing = true;
		ClickDialog();
	}
}