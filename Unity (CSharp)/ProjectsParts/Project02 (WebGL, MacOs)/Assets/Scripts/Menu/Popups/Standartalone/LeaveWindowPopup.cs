using UnityEngine;
using TMPro;
using it.Network.Rest;
using it.Inputs;
using it.UI;
using Garilla.Games;
using it.Settings;

namespace it.Popups
{
	public class LeaveWindowPopup : PopupBase
	{
		public UnityEngine.Events.UnityAction OnOk;

		[SerializeField] private TextMeshProUGUI _titleLabel;
		[SerializeField] private TextMeshProUGUI _tableLabel;
		[SerializeField] private CurrencyLabel _dataLabel;
		[SerializeField] private TextMeshProUGUI CloseSumText;

		public void Init(Table table, GamePanel panel, decimal amount = -1)
		{
			var gb = GameSettings.GetBlock(table);
			var gm = GameSettings.GameNames.Find(x => x.GameType == (GameType)table.game_rule_id);
			if (_titleLabel != null) _titleLabel.text = gb.Name;
			if (_tableLabel != null) _tableLabel.text = table.name;
			if (_dataLabel != null)
			{
#if UNITY_STANDALONE
				_dataLabel.SetValue(it.Settings.GameSettings.GetFullVisibleName(table) + " - {0} / {1}", table.SmallBlindSize, table.big_blind_size);
#else
				_dataLabel.SetValue(it.Settings.GameSettings.GetFullVisibleName(table) + " - {0} / {1}", table.SmallBlindSize, table.big_blind_size);
#endif
			}
			decimal amountTxt = amount;

			if (amountTxt < 0)
			{
				amountTxt = 0;
				foreach (var elem in panel.CurrentGameUIManager.Players)
					if (elem.Value.UserId == UserController.User.id)
						amountTxt = elem.Value.TablePlayerSession.amount;
			}
			//decimal amountTxt = amount != -1 ? amount : table.MePlayer.Amount;
			if (CloseSumText != null)
				CloseSumText.text = $"<b>{I2.Loc.LocalizationManager.GetTranslation("popup.exitGame.cashOutAmount")}:</b> <color=#C68C43>{it.Helpers.Currency.String(amountTxt)}";
			gameObject.SetActive(true);
		}

		public void CloseButtonTouch()
		{
			//OnOk?.Invoke();
			Hide();
		}
		public void OnButtonTouch()
		{
			OnOk?.Invoke();
			Hide();
		}

		public void CashierButton()
		{
			it.Main.PopupController.Instance.ShowPopup(PopupType.Cashier);
			Hide();
		}

	}
}