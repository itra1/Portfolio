using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace it.Inputs
{
  public class CurrencyLabel : MonoBehaviour
  {
    [SerializeField] private string _pattern = "{0} / {1}";
    [SerializeField] private bool _visibleValuteSymbol = true;

    private float[] _floatValues;
    private double[] _doubleValues;
    private decimal[] _decimalValues;

    private void OnEnable()
		{
      com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.CurrencyChange, CurrencyChange);
		}

		private void OnDisable()
    {
      com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.CurrencyChange, CurrencyChange);
    }

    public void SerPattern(string pattern){
      _pattern = pattern;
    }

    private void CurrencyChange(com.ootii.Messages.IMessage handel){
      ConfirmValueFloat();
    }
    public void SetValue(string pattern, params decimal[] values)
    {
      this._pattern = pattern;
      SetValue(values);
    }

    public void SetValue(string pattern, params float[] values)
    {
      this._pattern = pattern;
      SetValue(values);
    }
    public void SetValue(string pattern, params double[] values)
    {
      this._pattern = pattern;
      SetValue(values);
    }
    public void SetValue(params float[] values)
    {
      _floatValues = values;
      ConfirmValueFloat();
    }
    public void SetValue(params double[] values)
    {
      _doubleValues = values;
      ConfirmValueDouble();
    }
    public void SetValue(params decimal[] values)
    {
			_decimalValues = values;
			ConfirmValueDecimal();
		}
		public void SetValue<T>(params T[] values)
		{
			T[] vals = values;
			ConfirmValues<T>(vals);
		}
		private void ConfirmValues<T>(T[] vals)
		{
			object[] args = new object[vals.Length];

			for (int i = 0; i < vals.Length; i++)
			{
				args[i] = it.Helpers.Currency.String<T>(vals[i], _visibleValuteSymbol);
			}

			TextMeshProUGUI textTMLabel = GetComponent<TextMeshProUGUI>();

			if (textTMLabel != null)
				textTMLabel.text = string.Format(_pattern, args);
		}

		private void ConfirmValueFloat()
    {
      object[] args = new object[_floatValues.Length];

      for(int i = 0; i < _floatValues.Length;i++){
        args[i] = it.Helpers.Currency.String(_floatValues[i], _visibleValuteSymbol);
      }

      TextMeshProUGUI textTMLabel = GetComponent<TextMeshProUGUI>();

      if (textTMLabel != null)
        textTMLabel.text = string.Format(_pattern, args);
    }

    private void ConfirmValueDouble()
    {
      object[] args = new object[_doubleValues.Length];

      for (int i = 0; i < _doubleValues.Length; i++)
      {
        args[i] = it.Helpers.Currency.String(_doubleValues[i], _visibleValuteSymbol);
      }

      TextMeshProUGUI textTMLabel = GetComponent<TextMeshProUGUI>();

      if (textTMLabel != null)
        textTMLabel.text = string.Format(_pattern, args);
    }
    private void ConfirmValueDecimal()
    {
      object[] args = new object[_decimalValues.Length];

      for (int i = 0; i < _decimalValues.Length; i++)
      {
        args[i] = it.Helpers.Currency.String(_decimalValues[i], _visibleValuteSymbol);
      }

      TextMeshProUGUI textTMLabel = GetComponent<TextMeshProUGUI>();

      if (textTMLabel != null)
        textTMLabel.text = string.Format(_pattern, args);
    }

  }
}