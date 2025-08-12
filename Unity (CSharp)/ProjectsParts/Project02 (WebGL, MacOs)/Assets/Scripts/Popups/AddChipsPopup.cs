using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using it.Network.Rest;
using Sett = it.Settings;

namespace it.Popups
{
	public class AddChipsPopup : PopupBase
	{
		[SerializeField] private TextMeshProUGUI _titleLabel;
		[SerializeField] private TextMeshProUGUI _balanceLabel;
		[SerializeField] private TextMeshProUGUI _minButtonValueLavel;
		[SerializeField] private TextMeshProUGUI _maxButtonValueLavel;
		[SerializeField] private TMPro.TMP_InputField _amountValueInput;
		[SerializeField] private Slider _sliderValue;
		[SerializeField] private Toggle _alawysMax;
		[SerializeField] private TextMeshProUGUI _timerLabel;

		//public UnityEngine.Events.UnityAction<bool> OnAlwaysToggleChange;

		private Action<decimal,bool> callback;
		public Action OnTimerOut;

		private decimal PayValue;
		private it.Network.Rest.Table _useTable;

		private decimal _amountMin;
		private decimal _amountMax;
		private decimal _currentValue;
		private decimal _diapasone;

		//private float AmountMax =>Mathf.Max(0,Mathf.Min((float)UserController.User.UserWallet.Amount, 
		// Mathf.Min(_useTable.BuyInMaxEURO, Mathf.Min(_useTable.BuyInMaxEURO,
		//(int)(_useTable.BuyInMaxEURO / _useTable.SmallBlindSize) * _useTable.SmallBlindSize) - _userAmount - _userAmountBuffer)));
		//private float AmountMin => 0;
		private decimal _userAmount;
		private decimal _userAmountBuffer;
		private bool _allwaysMaxValue;

		public void Init(Table table, TablePlayerSession user, DistributionSharedDataPlayer player, Action<decimal, bool> callback, string endTime = null, bool autoMaxValue = false)
		{
			_userAmount = player != null ? player.amount : user.amount;
			_userAmountBuffer = user.amountBuffer;

			_useTable = table;
			_allwaysMaxValue = autoMaxValue;
			_alawysMax.isOn = _allwaysMaxValue;
			_alawysMax.onValueChanged.RemoveAllListeners();
			_alawysMax.onValueChanged.AddListener((val) =>
			{
				_allwaysMaxValue = val;
				//OnAlwaysToggleChange?.Invoke(_allwaysMaxValue);
			});

			this.callback = callback;
			if(string.IsNullOrEmpty(endTime))
			{
				_timerLabel.gameObject.SetActive(false);
			}else{
				var targetEndTime = DateTime.Parse(endTime);
				var timeWait = targetEndTime - GameHelper.NowTime;
				_timerLabel.gameObject.SetActive(true);
				StartCoroutine(Timer((int)timeWait.TotalSeconds));
			}

#if UNITY_STANDALONE
			if (_useTable.ante != null && _useTable.ante > 0)
			{
				_titleLabel.text = $"{Helpers.Currency.String(_useTable.ante)} (A{Helpers.Currency.String(_useTable.ante)})  <b><color=#C68C43>{Sett.GameSettings.GetBlock(_useTable).Name}";
			}
			else
			{
				_titleLabel.text = $"{Helpers.Currency.String(_useTable.SmallBlindSize)} / {Helpers.Currency.String(_useTable.big_blind_size)}  <b><color=#C68C43>{Sett.GameSettings.GetBlock(_useTable).Name}";
			}
#else

			if (_useTable.ante != null && _useTable.ante > 0)
			{
				_titleLabel.text = $"<b><color=#C68C43>{Sett.GameSettings.GetBlock(_useTable).Name}</color></b>\n{Helpers.Currency.String(_useTable.ante)} (A{Helpers.Currency.String(_useTable.ante)})";
			}
			else
			{
				_titleLabel.text = $"<b><color=#C68C43>{Sett.GameSettings.GetBlock(_useTable).Name}</color></b>\n{Helpers.Currency.String(_useTable.SmallBlindSize)} / {Helpers.Currency.String(_useTable.big_blind_size)}";
			}

#endif

			_balanceLabel.text = it.Helpers.Currency.String(UserController.User.user_wallet.amount);

			_amountMin = _useTable.BuyInMinEURO;
			_amountMax = Math.Min((decimal)UserController.User.user_wallet.amount, _useTable.BuyInMaxEURO - _userAmount - _userAmountBuffer);
			_amountMax = (decimal)Math.Round(_amountMax, 2);

			if (_amountMax < 0)
				_amountMax = 0;

			if (_amountMax < _amountMin)
				_amountMin = _amountMax;
			_diapasone = _amountMax - _amountMin;

			_minButtonValueLavel.text = $"{I2.Loc.LocalizationManager.GetTermTranslation("popup.addChips.min")}\n<color=#C68C43>{Helpers.Currency.String(_amountMin)}";
			_maxButtonValueLavel.text = $"{I2.Loc.LocalizationManager.GetTermTranslation("popup.addChips.max")}\n<color=#C68C43>{Helpers.Currency.String(_amountMax)}";

			_sliderValue.minValue = 0;
			_sliderValue.maxValue = 1;
			_currentValue = _amountMax;

			ValueChange(_amountMax, true);

			decimal speedScroll = _amountMax - _useTable.BuyInMinEURO;
			if (speedScroll == 0)
				speedScroll = 1;

			var c = _sliderValue.GetComponent<it.UI.Elements.SliderScrollHandler>();
			c.Speed = (float)(_useTable.SmallBlindSize / speedScroll);

			_amountValueInput.onValueChanged.RemoveAllListeners();
			_amountValueInput.onDeselect.RemoveAllListeners();
			_amountValueInput.onSubmit.RemoveAllListeners();

			_amountValueInput.onDeselect.AddListener((str) =>
			{
				decimal value = decimal.Parse(str);
				ValueChange(value, true);
			});
			_amountValueInput.onSubmit.AddListener((str) =>
			{
				decimal value = decimal.Parse(str);
				ValueChange(value, true);
			});
			_amountValueInput.interactable = _currentValue != 0;

			_sliderValue.onValueChanged.RemoveAllListeners();
			_sliderValue.onValueChanged.AddListener((value) =>
			{
				ValueChange((decimal)value * _diapasone + _amountMin);
			});

			gameObject.SetActive(true);
		}

		private IEnumerator Timer(int seconds)
		{
			_timerLabel.text = $"{seconds} s.";
			while (seconds > 0)
			{
				yield return new WaitForSeconds(1);
				seconds--;
				_timerLabel.text = $"{seconds} s.";
			}
			_timerLabel.text = $"{seconds} s.";
			Hide();
			OnTimerOut?.Invoke();
		}


		private void ValueChange(decimal newValue, bool slide = false)
		{
			if (newValue >= _amountMax)
				_currentValue = _amountMax;
			else if (newValue <= _amountMin)
				_currentValue = _amountMin;
			else
			{
				_currentValue = Math.Round(newValue, 2);
			}
			_amountValueInput.text = _currentValue.ToString("F2");
			if (slide || _diapasone == 0)
			{
				if (_diapasone == 0)
					_sliderValue.value = 1;
				else
					_sliderValue.value = (float)((_currentValue - _amountMin) / _diapasone);
			}
		}

		public void CloseButtonTouch()
		{
			base.Hide();
			OnTimerOut?.Invoke();
			//gameObject.SetActive(false);
			callback(-1, _allwaysMaxValue);
		}

		public void SubmitButtonTouch()
		{
			callback(Math.Max(Math.Round(_currentValue, 2), _amountMin), _allwaysMaxValue);
			base.Hide();
		}
		public void MaxButtonTouch()
		{
			ValueChange(_amountMax, true);
			//	_currentValue = _amountMax;
			//_sliderValue.value = _currentValue;
		}

		public void MinbuttonTouch()
		{
			ValueChange(_amountMin, true);

			//_currentValue = _amountMin;
			//_sliderValue.value = _currentValue;
		}

		//public void SetValuePaySlider(float vl)
		//{
		//	_sliderValue.value = vl;
		//	float value = AmountMin + (AmountMax - AmountMin) * vl;
		//	_amountValueInput.text = ((int)value).ToString();
		//}

	}
}