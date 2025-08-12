using it.Network.Rest;
using System.Collections;
using TMPro;
using UnityEngine;

namespace it.Popups
{
	public class CashierOperationsPage : CashierPage
	{
		[SerializeField] private CashierOperationNavigations _navigations;
		[SerializeField] private CashierDetailsForm _form;
		[SerializeField] private CashierDetailsForm _formCardFull;
		[SerializeField] private CashierDetailsForm _formPhone;
		[SerializeField] private CashierDetailsForm _formWallet;
		[SerializeField] private CashierDetailsForm _formTelegram;
		[SerializeField] private CashierDetailsForm _formCrypto;
		//[SerializeField] private CashierDetailsForm _formUmonay;

		private ICashierMethod _method;

		[System.Serializable]
		public struct OperationsType
		{
			public string Name;
			public CashierDetailsForm Form;
		}

		private void OnEnable()
		{
			Back();
			_navigations.OnSelect = SetOperation;
		}

		public void SetOperation(SelectMethod method)
		{
			_navigations.gameObject.SetActive(false);

			CashierDetailsForm targetForm = _form;
			if (method.Type == TypeCashier.Withdrawal && method.Method.Slug.Contains("paycos") && !method.Method.Slug.Contains("qiwi"))
				targetForm = _formCardFull;
			if (method.Type == TypeCashier.Withdrawal && (method.Method.Slug.Contains("qiwi") || method.Method.Slug.Contains("yoomoney") || method.Method.Slug == "kauri"))
				targetForm = _formWallet;
			if (method.Type == TypeCashier.Deposit && method.Method.Slug == "kauri")
				targetForm = _formCrypto;
			if (method.Method.Slug.Contains("telegram"))
				targetForm = _formTelegram;

			targetForm.gameObject.SetActive(true);

			_method = method.Method;

			targetForm.Set(method);
			targetForm.OnBack = Back;

		}

		public void Back()
		{
			if (_navigations != null)	_navigations.gameObject.SetActive(true);
			if (_form != null) _form.gameObject.SetActive(false);
			if (_formCardFull != null) _formCardFull.gameObject.SetActive(false);
			if (_formPhone != null) _formPhone.gameObject.SetActive(false);
			if (_formTelegram != null) _formTelegram.gameObject.SetActive(false);
			if (_formWallet != null) _formWallet.gameObject.SetActive(false);
			if (_formCrypto != null) _formCrypto.gameObject.SetActive(false);
			//if (_formUmonay != null) _formUmonay.gameObject.SetActive(false);
		}

	}
}