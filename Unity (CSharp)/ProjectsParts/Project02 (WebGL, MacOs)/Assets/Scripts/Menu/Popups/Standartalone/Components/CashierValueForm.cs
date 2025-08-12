using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace it.Popups
{
	public class CashierValueForm : MonoBehaviour
	{
		[SerializeField] private TMP_InputField _valueInput;

		public decimal Value
		{
			get
			{
				if (_valueInput == null) return 0;
				return decimal.Parse(_valueInput.text);
			}
		}
		public decimal MinValue { get => _minValue; set => _minValue = value; }
		public decimal MaxValue { get => _maxValue; set => _maxValue = value; }

		private decimal _minValue = 0;
		private decimal _maxValue = 1200;

		private void OnEnable()
		{
			ClearForm();
			_valueInput.onSubmit.RemoveAllListeners();
			_valueInput.onSubmit.AddListener(x =>
			{
				SetValue(double.Parse(x));
			});
		}

		private void ClearForm()
		{
			if (_valueInput != null)
				SetValue(0);
		}

		public void SetValue(double value)
		{
			_valueInput.text = System.Math.Clamp((decimal)value, _minValue, _maxValue).ToString();
		}

		public void Set50(){
			SetValue(50);
		}

		public void Set100()
		{
			SetValue(100);
		}

		public void Set500()
		{
			SetValue(500);
		}

		public void Set1000()
		{
			SetValue(1000);
		}

	}
}