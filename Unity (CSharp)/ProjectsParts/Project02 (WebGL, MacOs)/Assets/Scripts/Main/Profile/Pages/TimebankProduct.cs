using System.Collections;
using UnityEngine;
using TMPro;
using it.Main.SinglePages;
 

namespace it.Main
{
	public class TimebankProduct : MonoBehaviour
	{
		public UnityEngine.Events.UnityAction<string> OnBuy;

		[SerializeField] private it.Inputs.CurrencyLabel _priceLabel;

		private string _productName;

		public void BuyTouch(){
			OnBuy?.Invoke(_productName);
		}

		public void SetData(string name, double price){
			_productName = name;
			_priceLabel.SetValue("singlePage.timeBank.buyPrice".Localized(), price+0.00000001d );
		}

	}
}