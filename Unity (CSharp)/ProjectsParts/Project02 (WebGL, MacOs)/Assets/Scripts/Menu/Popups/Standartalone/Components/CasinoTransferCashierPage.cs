using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using it.Api;
using UnityEngine.UI;
using DG.Tweening;
 

namespace it.Popups
{
	public class CasinoTransferCashierPage : TransfersCashierPage
	{
		[SerializeField] private TextMeshProUGUI _convertField;

		private List<it.Network.Rest.CurrencyConversionBlock> _conversion;

		protected override void OnEnable()
		{
			base.OnEnable();
			UserApi.GetTransferExchangeOut((ResultResponse<List<it.Network.Rest.CurrencyConversionBlock>> response) =>
			{
				_conversion = response.Result;
			});
		}

		protected override void FindUser(string nickname)
		{
			base.FindUser(nickname);
			UserApi.FindCasinoUser(nickname, (result) =>
			{
				if (result.IsSuccess)
				{
					List<FindUser> users = new List<FindUser>() { new FindUser() { nickname = nickname } };
					SpawnLiad(users);
				}

			});

		}

		protected override void AmountChange(string val)
		{
			if (_convertField == null) return;
			base.AmountChange(val);

			if (_conversion == null) return;

			var res = _conversion.Find(x => x.currencyFrom.abbreviation == "EUR" && x.currencyTo.abbreviation == "RUB");

			if (res == null) return;
			decimal res1;
			if (decimal.TryParse(val, out res1))
			{
			if(_convertField != null)
				_convertField.text = (res1 * (decimal)res.exchange).ToString();
			}


		}

		// "tifijex499@kixotic.com"
		public override void TransferButtonTouch()
		{
			base.TransferButtonTouch();
			if (_findUser == null) return;

			double amount = double.Parse(_amountField.text);
			_applyButton.interactable = false;

			UserApi.TransferCasino(_findUser.nickname, amount, (result) =>
			{
				_applyButton.interactable = true;
				it.Main.PopupController.Instance.ShowPopup<it.Popups.InfoPopup>(PopupType.Info)
				.SetDescriptionString("popup.cashier.casino.completeTransfer".Localized());

			}, (error) =>
			{
				_applyButton.interactable = true;
				it.Main.PopupController.Instance.ShowPopup<it.Popups.InfoPopup>(PopupType.Info).SetDescriptionString("popup.cashier.casino.errorTransfer".Localized());
			});


		}
	}
}