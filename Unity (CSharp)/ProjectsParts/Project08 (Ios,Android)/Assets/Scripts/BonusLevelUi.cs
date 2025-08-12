using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusLevelUi : UiPanel {

	public Action OnPlay;

	public Animation animComp;

	public void CloseButton() {
		isClosing = true;
		Hide(() => {
			isClosing = true;
			gameObject.SetActive(false);
		});
	}

	public void PlayButton() {
		if (OnPlay != null) OnPlay();
	}

	public override void Show(Action OnShow = null) {
		base.Show(OnShow);
		PlayClickAudio();
		animComp.Play("show");
	}

	public override void Hide(Action OnHide = null) {
		base.Hide(OnHide);
		animComp.Play("hide");
	}

	public AudioClipData openAudio;

	public void PlayClickAudio() {
		AudioManager.PlayEffects(openAudio, AudioMixerTypes.effectUi);
	}

	public override void ManagerClose() {
		if (isClosing) return;
		CloseButton();
	}
}
