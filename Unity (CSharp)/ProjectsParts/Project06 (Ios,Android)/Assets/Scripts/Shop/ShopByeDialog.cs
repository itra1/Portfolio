using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopByeDialog : MonoBehaviour {

	public Action<ShopProductBehaviour, int> OnConfirm;
	private ShopProductBehaviour _product;
	public Image icon;

	public Text title;

	private int _maxValue;
	private int _byeValue;
  [SerializeField]
  private TMPro.TextMeshProUGUI countValue;
	
	public void SetData(ShopProductBehaviour product) {

		string _title = "Подтверждение покупки";

		_product = product;
		icon.sprite = _product.Icon;

		title.text = _title;

	  _maxValue = Game.User.UserManager.Instance.silverCoins.Value / product.price;
		_byeValue = Mathf.Min(1, _maxValue);
		ConfirmValue();
	}

	private void ConfirmValue() {
    countValue.text = _byeValue.ToString();
	}

	public void IncButton() {
		UIController.ClickPlay();
		_byeValue = Mathf.Min(_maxValue, _byeValue + 1);
		ConfirmValue();
	}

	public void DecButton() {
		UIController.ClickPlay();
		_byeValue = Mathf.Max(0, _byeValue - 1);
		ConfirmValue();
	}

	public void CancelButton() {
		UIController.ClickPlay();
		StartClose();
	}

	public void ConfirmButton() {
		UIController.ClickPlay();
		if (OnConfirm != null) OnConfirm(_product, _byeValue);
		StartClose();
	}

	public void StartClose() {
		gameObject.SetActive(false);
	}

}
