using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace it.Popups
{
	public class CreditCardForm : MonoBehaviour
	{
		[SerializeField] private TMP_InputField _cardNumberInput;
		[SerializeField] private TMP_InputField _monthInput;
		[SerializeField] private TMP_InputField _yearInput;
		[SerializeField] private TMP_InputField _ownerInput;
		[SerializeField] private TMP_InputField _cvv;

		public string CardNumber => _cardNumberInput.text;
		public string Owner => _ownerInput.text;
		public string CVV => _cvv.text;
		public string Month
		{
			get
			{
				if (_monthInput == null)
					return null;
				else
					return _monthInput.text;
			}
		}
		public string Year
		{
			get
			{
				if (_yearInput == null)
					return null;
				else
					return _yearInput.text;
			}
		}

		private void Awake()
		{
			CompleteForm();
		}

		private void OnEnable()
		{
			ClearForm();
		}


		private void CompleteForm()
		{
			//List<TMP_Dropdown.OptionData> _options = new List<TMP_Dropdown.OptionData>();
			//if (_monthDropDown != null)
			//{
			//	_monthDropDown.ClearOptions();

			//	_options = new List<TMP_Dropdown.OptionData>();
			//	_options.Add(new TMP_Dropdown.OptionData() { text = "" });
			//	for (int i = 1; i <= 12; i++)
			//		_options.Add(new TMP_Dropdown.OptionData() { text = i.ToString() });

			//	_monthDropDown.AddOptions(_options);
			//}

			//if (_yearDropDown != null)
			//{
			//	_yearDropDown.ClearOptions();

			//	_options = new List<TMP_Dropdown.OptionData>();
			//	_options.Add(new TMP_Dropdown.OptionData() { text = "" });

			//	for (int i = System.DateTime.Now.Year; i <= System.DateTime.Now.AddYears(7).Year; i++)
			//		_options.Add(new TMP_Dropdown.OptionData() { text = i.ToString() });

			//	_yearDropDown.AddOptions(_options);
			//}

		}

		private void ClearForm()
		{
			if (_cardNumberInput != null)
				_cardNumberInput.text = "";
			if (_ownerInput != null)
				_ownerInput.text = "";
			if (_cvv != null)
				_cvv.text = "";
			if (_monthInput != null)
				_monthInput.text = "";
			if (_yearInput != null)
				_yearInput.text = "";
		}

	}
}