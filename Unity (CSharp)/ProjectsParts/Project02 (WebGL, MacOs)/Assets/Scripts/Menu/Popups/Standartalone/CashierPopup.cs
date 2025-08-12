using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Popups
{
	public class CashierPopup : PopupBase
	{
		[SerializeField] private FiltrButton[] FiltrButtons;
		//[SerializeField] private it.Inputs.CurrencyLabel _amountLabel;

		[System.Serializable]
		public struct FiltrButton
		{
			public GameObject Panel;
			public it.UI.Elements.FilterSwitchButtonUI Button;

			public void CheckSelect(bool isSelect)
			{
				Button.IsSelect = isSelect;
			}
		}

		public override void Show(bool force = false)
		{
			//com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserBalanceUpdate, BalanceUpdate);

			base.Show(force);
		}

		public override void Hide()
		{
			//com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserBalanceUpdate, BalanceUpdate);

			base.Hide();
		}
		//private void BalanceUpdate(com.ootii.Messages.IMessage handler)
		//{
		//	_amountLabel.SetValue((float)UserController.User.UserWallet.Amount);
		//}

		protected override void EnableInit()
		{

			for (int i = 0; i < FiltrButtons.Length; i++)
			{
				int index = i;
				FiltrButtons[index].Button.OnClickPointer.RemoveAllListeners();
				FiltrButtons[index].Button.OnClickPointer.AddListener(() =>
				{
					SelectPage(index);
				});
			}

			base.EnableInit();
			SelectPage(0);
			//_amountLabel.SetValue((float)UserController.User.UserWallet.Amount);
		}

		public void SelectPage(int page)
		{
			for (int i = 0; i < FiltrButtons.Length; i++)
			{
				FiltrButtons[i].CheckSelect(page == i);
				FiltrButtons[i].Panel.SetActive(page == i);
				var c = FiltrButtons[i].Panel.GetComponent<CashierOperationsPage>();
				if (c != null)
					c.Back();
			}
		}

	}
}