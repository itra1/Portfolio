using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecorAchiveUi : UiPanel {

	public System.Action<int> OnTake;

	private int activeAchive;
	public Image icon;

	public Animation animComp;

	public TextUiScale descr;

	protected override void OnEnable() {
		base.OnEnable();
		Show();
		PlayGiftIdleAudio();

	}

	public override void Show(Action OnShow = null) {
		base.Show(OnShow);
		animComp.Play("show");

		//descr.SetText(PlayerManager.Instance.company.GetActualLocation().title + " " + LanguageManager.GetTranslate("decor.dialog.description"), LanguageManager.Instance.activeLanuage.code);
		descr.SetText(LanguageManager.GetTranslate("decor.dialog.description"), LanguageManager.Instance.activeLanuage.code);
	}

	public override void Hide(Action OnHide = null) {
		base.Hide(OnHide);
		animComp.Play("hide");
	}

	public void SetAchive(int numAchive) {
		activeAchive = numAchive;

		//var gp = GraphicManager.Instance.link.achiveList[numAchive];
		//icon.sprite = gp.mini;
		ScalingImage();
	}

	public void TakeButton() {
		OnTake(activeAchive);
	}

	private void ScalingImage() {
		icon.rectTransform.sizeDelta = new Vector2(icon.sprite.rect.width, icon.sprite.rect.height);
		/*
		if (icon.sprite.rect.width > icon.sprite.rect.height) {
			icon.rectTransform.sizeDelta = new Vector2(240,240/ icon.sprite.rect.width* icon.sprite.rect.height);
		}
		else {
			icon.rectTransform.sizeDelta = new Vector2(240 / icon.sprite.rect.height * icon.sprite.rect.width, 240);
		}
		*/
	}

	private void Close() {
		isClosing = true;

		giftIdleAudioPoint.Stop(0.5f);
		Hide(() => {
			isClosing = false;
			gameObject.SetActive(false);
		});
	}

	public override void ManagerClose() {
		if (isClosing) return;
		Close();
	}

	public AudioClipData giftIdleAudio;
	private AudioPoint giftIdleAudioPoint;

	public void PlayGiftIdleAudio() {
		giftIdleAudioPoint = AudioManager.PlayEffects(giftIdleAudio, AudioMixerTypes.effectUi);
	}

}
