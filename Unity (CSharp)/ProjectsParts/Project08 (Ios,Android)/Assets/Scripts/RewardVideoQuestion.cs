using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardVideoQuestion : UiPanel {

	public Action OnCancel;
	public Action OnOk;

	public LocalizationUiText nameHintText;

	public Animation animComponent;

	public GiftElement gift;


	protected override void OnEnable() {
		base.OnEnable();
		gift.gameObject.SetActive(false);
		Show(() => {

		});
	}

	public void CancelButton() {
		isClosing = true;
		Hide(() => {
			isClosing = false;
			if (OnCancel != null) OnCancel();
			gameObject.SetActive(false);
		});
	}

	public override void Hide(Action OnHide = null) {
		base.Hide(OnHide);
		animComponent.Play("hide");
	}

	public override void Show(Action OnShow = null) {
		base.Show(OnShow);
		animComponent.Play("show");
	}

	public void OkButton() {
		if (OnOk != null) OnOk();
		//CancelButton();
	}

	public override void ManagerClose() {
		if (isClosing) return;
		CancelButton();
	}
	public void SetHint(GiftElement.Type hint) {

		switch (hint) {
			case GiftElement.Type.hintAnyletter:
				nameHintText.SetCode("shop.page.HintAnyLetterDesctiption");
				break;
			case GiftElement.Type.hintLetter:
				nameHintText.SetCode("shop.page.HintfirstLetterDesctiption");
				break;
			case GiftElement.Type.hintWord:
				nameHintText.SetCode("shop.page.HintFirstWordDesctiption");
				break;
		}

	}

	public void ShowGift(GiftElement.GiftElementParam giftParam, Action OnConplete) {

		RewardVideoHint gp = (RewardVideoHint)UIManager.Instance.GetPanel(UiType.revardVideoGift);

		gp.gameObject.SetActive(false);
		gp.ShowGift(giftParam, OnConplete);

		//PlayGamePlay gp = (PlayGamePlay)UIManager.Instance.GetPanel(UiType.game);
		//gift.Show(giftParam.type, gp, gp);
		//gift.gameObject.SetActive(true);

		//gift.MoveComplete = () => {
		//	OnConplete();
		//};


	}

}
