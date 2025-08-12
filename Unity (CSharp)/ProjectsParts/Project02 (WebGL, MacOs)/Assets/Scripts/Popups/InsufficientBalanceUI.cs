using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
 
using it.Network.Rest;

namespace it.Popups
{
  public class InsufficientBalanceUI : PopupBase
  {
    [SerializeField] private TextMeshProUGUI MinimumAmount;
    [SerializeField] private it.UI.Elements.GraphicButtonUI _cashierButton;

		public override void Show(bool force = false)
    {
      base.Show(force);
      Table table = GameHelper.SelectTable;
      MinimumAmount.text = $"{"popup.insufficientBalance.minimumReq".Localized()}: $ {table.SmallBlindSize}";
      gameObject.SetActive(true);
      CashierVisibleChange();
		}

    public void CloseButton(){
      Hide();
		}

    public void CashierButton(){
      Hide();

      it.Main.PopupController.Instance.ShowPopup(PopupType.Cashier);
		}
		private void CashierVisibleChange()
		{
#if UNITY_IOS
			_cashierButton?.gameObject.SetActive(AppConfig.ActiveCashier);
#endif
		}

	}
}