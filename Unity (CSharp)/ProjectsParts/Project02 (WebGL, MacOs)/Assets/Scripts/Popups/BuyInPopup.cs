using it.Network.Rest;
using it.UI.Elements;

using System;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using Sett = it.Settings;

namespace it.Popups
{
	public class BuyInPopup : PopupBase
	{

		[SerializeField] private TextMeshProUGUI _titleLabel;
		[SerializeField] private TextMeshProUGUI _balanceLabel;
		[SerializeField] private TextMeshProUGUI _minButtonValueLavel;
		[SerializeField] private TextMeshProUGUI _maxButtonValueLavel;
		[SerializeField] private TextMeshProUGUI _timerLabel;
		[SerializeField] private TMPro.TMP_InputField _amountValueInput;
		[SerializeField] private Slider _sliderValue;
		[SerializeField] private GraphicButtonUI _okButton;

		private Action<decimal> _callback;
		private it.Network.Rest.Table _useTable;
		private DateTime _target;
		private bool _isActiveTime;

		private decimal _amountMin;
		private decimal _amountMax;
		private decimal _currentValue;
		private decimal _diapasone;

		public void Init(it.Network.Rest.Table table, Action<decimal> callback)
		{
			_useTable = table;
			this._callback = callback;

			ClearData();
			Lock(true);
			TableManager.Instance.LoadTableRecord(_useTable.id, (t) =>
			{
				_useTable = t;
				ConfirmData();
				Lock(false);
			});

		}

		private void ConfirmData()
		{

			if (_useTable.can_join_only_with_exit_amount)
			{
				_amountMin = _useTable.exit_amount.Value;
				_amountMax = _useTable.exit_amount.Value;
			}
			else
			{
				_amountMin = _useTable.BuyInMinEURO;
				_amountMax = Math.Max(_amountMin, Math.Min((decimal)UserController.User.user_wallet.amount, _useTable.BuyInMaxEURO));
			}

			_diapasone = _amountMax - _amountMin;

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
			_minButtonValueLavel.text = $"{I2.Loc.LocalizationManager.GetTermTranslation("popup.buyIn.min")}\n<color=#C68C43>{Helpers.Currency.String(_amountMin)}";
			_maxButtonValueLavel.text = $"{I2.Loc.LocalizationManager.GetTermTranslation("popup.buyIn.max")}\n<color=#C68C43>{Helpers.Currency.String(_amountMax)}";

			_currentValue = _amountMax;

			_sliderValue.minValue = 0;
			_sliderValue.maxValue = 1;

			decimal speedScroll = _amountMax - _amountMin;
			if (speedScroll == 0)
				speedScroll = 1;

			var c = _sliderValue.GetComponent<it.UI.Elements.SliderScrollHandler>();
			c.Speed = _diapasone == 0
				? 1
				: (float)(_useTable.SmallBlindSize / speedScroll);
			//c.Speed = 1;

			_target = DateTime.Now.AddSeconds(60);
			_isActiveTime = true;

			ValueChange(_currentValue, true);
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

			_sliderValue.onValueChanged.RemoveAllListeners();
			_sliderValue.onValueChanged.AddListener((value) =>
			{
				ValueChange((decimal)value * _diapasone + _amountMin);
			});

			gameObject.SetActive(true);

			_okButton.interactable = _amountMin <= UserController.User.user_wallet.amount;

		}

		private void ClearData()
		{
			_minButtonValueLavel.text = $"{I2.Loc.LocalizationManager.GetTermTranslation("popup.buyIn.min")}\n<color=#C68C43>-";
			_maxButtonValueLavel.text = $"{I2.Loc.LocalizationManager.GetTermTranslation("popup.buyIn.max")}\n<color=#C68C43>-";
		}

		private void ValueChange(decimal newValue, bool slide = false)
		{
			if (newValue >= _amountMax)
				_currentValue = _amountMax;
			else if (newValue <= _amountMin)
				_currentValue = _amountMin;
			else
			{
				_currentValue = newValue;
			}
			_amountValueInput.text = _currentValue.ToString("F2");
			if (slide || _diapasone == 0)
				_sliderValue.value = _diapasone == 0
				? 1
				: (float)((_currentValue - _amountMin) / _diapasone);
		}


		private void Update()
		{
			if (!_isActiveTime) return;
			TimeSpan ts = _target - DateTime.Now;
			_timerLabel.text = $"<color=\"red\">{ts.ToString("ss")}</color> {I2.Loc.LocalizationManager.GetTermTranslation("popup.buyIn.secondsLeft")}";
			if (ts.TotalSeconds <= 0)
			{
				_isActiveTime = false;
				it.Main.PopupController.Instance.ShowPopup(PopupType.SitTimeOut);
				CancelButton();
			}
		}

		public void MaxButtonTouch()
		{
			ValueChange(_amountMax, true);
		}

		public void MinbuttonTouch()
		{
			ValueChange(_amountMin, true);
		}

		public void OkButton()
		{
			Hide();
			_callback(Math.Max(Math.Round(_currentValue, 2), _amountMin));
		}

		public void CancelButton()
		{

			Hide();
			//gameObject.SetActive(false);
			_callback(-1);
		}

	}
}