using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardVideoHint : UiPanel {

	public Animation animComponent;

	public GiftElement gift;

	public override void ManagerClose() {

	}
	
	public void ShowGift(GiftElement.GiftElementParam giftParam, Action OnConplete) {

		PlayGamePlay gp = UIManager.Instance.GetPanel<PlayGamePlay>();
		gift.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		gift.Show(giftParam.type, gp, gp);
		gift.gameObject.SetActive(true);

		gift.MoveComplete = () => {
			OnConplete();
		};
	}

}
