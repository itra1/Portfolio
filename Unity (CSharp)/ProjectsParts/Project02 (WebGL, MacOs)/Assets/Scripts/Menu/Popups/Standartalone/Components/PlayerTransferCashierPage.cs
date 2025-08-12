using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using it.Api;
using UnityEngine.UI;
using DG.Tweening;
 

namespace it.Popups
{
	public class PlayerTransferCashierPage : TransfersCashierPage
	{
		[SerializeField] protected TextMeshProUGUI _resultLabel;

		protected override void Awake()
		{
			base.Awake();
			ClearError();
		}

		protected override void ClearError()
		{
			base.ClearError();
			_resultLabel.gameObject.SetActive(false);
		}


		protected override void FindUser(string nickname)
		{
			base.FindUser(nickname);
			UserApi.FindUser(nickname, (result) =>
			{
				if (result.IsSuccess)
					SpawnLiad(result.Result);

			});

		}

		public override void TransferButtonTouch()
		{
			base.TransferButtonTouch();
			if (_findUser == null) return;

			double amount = double.Parse(_amountField.text);
			_applyButton.interactable = false;
			UserApi.Transfer(amount, _findUser.id, (result) =>
			{
				_applyButton.interactable = true;
				it.Main.PopupController.Instance.ShowPopup<it.Popups.InfoPopup>(PopupType.Info)
				.SetDescriptionString("message.transform.process".Localized());
				_resultLabel.gameObject.SetActive(true);
				_resultLabel.text = "<color=#57A53C>" + "popup.cashier.transfers.completeTransfer".Localized();

			}, (error) =>
			{
				_applyButton.interactable = true;
				_resultLabel.gameObject.SetActive(true);
				_resultLabel.text = "<color=#D03333>" + "popup.cashier.transfers.errorTransfer".Localized();
			});
		}
	}
}