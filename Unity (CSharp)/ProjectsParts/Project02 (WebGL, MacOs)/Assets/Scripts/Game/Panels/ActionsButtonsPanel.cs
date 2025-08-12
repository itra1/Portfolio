using UnityEngine;
using UnityEngine.UI;
using TMPro;
using it.Network.Rest;
using System;
using it.Popups;


namespace Garilla.Games.UI
{
	public class ActionsButtonsPanel : MonoBehaviour
	{
		public UnityEngine.Events.UnityAction UpdateSlider = null;
		public GameObject SliderBlock;

		[SerializeField] private RectTransform _body;
		[SerializeField] private TMPro.TMP_InputField _inputRaise;
		[SerializeField] private Slider _slider;
		//#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
		[SerializeField] private RectTransform _riseExtendOptions;
		[SerializeField] private RectTransform _numberInputTablo;
		//#endif
		[SerializeField] private Selectable _foldButton;
		[SerializeField] private Selectable _callButton;
		[SerializeField] private Selectable _checkButton;
		[SerializeField] private Selectable _riseButton;
		[SerializeField] private Selectable _allInButton;
		[SerializeField] private Selectable _betButton;
		[SerializeField] private Selectable _x2Button;
		[SerializeField] private Selectable _x3Button;
		[SerializeField] private Selectable _x4Button;
		[SerializeField] private Selectable _maxButton;
		[SerializeField] private Selectable _x2PreflopHMButton;
		[SerializeField] private Selectable _x3PreflopHMButton;
		[SerializeField] private Selectable _x4PreflopHMButton;
		[SerializeField] private Selectable _x2PreflopButton;
		[SerializeField] private Selectable _maxPreflopButton;
		[SerializeField] private GameObject _groupButtons;
		[SerializeField] private GameObject _groupPreflopButtons;
		[SerializeField] private GameObject _groupPreflopHMButtons;
		[SerializeField] private TextMeshProUGUI[] _callLabel;
		[SerializeField] private TextMeshProUGUI[] _raiseLabel;
		[SerializeField] private TextMeshProUGUI[] _betLabel;
		[SerializeField] private TextMeshProUGUI[] _X2Label;
		[SerializeField] private TextMeshProUGUI[] _X3Label;
		[SerializeField] private TextMeshProUGUI[] _X4Label;
		[SerializeField] private TextMeshProUGUI[] _XAllLabel;

		private GameUIManager _gameManager;
		private decimal _currentValue;
		private float desiredValue = 1f;
		private float speedModifier;
		private float _stepSlider = 1f;
		private bool _isWhole = false;
		private DistributionSharedData _sharedData;
		private SocketEventDistributionUserData _userData;
		private DistributionSharedDataPlayer _myPlayer;
		private decimal _beforeBat;
		private float _beforeAction;
		private Table _currentTable;
		private decimal _minRiseValue;
		private decimal _maxRiseValue;
		private decimal _riseDiapasone;
		private decimal _allInValue;
		private decimal _currentRiseValue;
		private decimal _maxBetOnRizeInPlayers = 0;
		private bool _allIn;
		private bool _isBet;
		private bool _isRiseExtend = false;

		private decimal MaxCurrentValue => Math.Min(_allInValue, _maxRiseValue);

		public GameUIManager GameManager
		{
			get => _gameManager;
			set
			{
				_gameManager = value;
				if (_slider != null)
					_slider.onValueChanged.AddListener((value) =>
					{
						UpdateSlider?.Invoke();
					});
			}
		}

		private void Start()
		{
			_inputRaise.onValueChanged.RemoveAllListeners();
			_inputRaise.onDeselect.RemoveAllListeners();
			_inputRaise.onSubmit.RemoveAllListeners();
			_inputRaise.onValueChanged.AddListener(InRaise_onValueChange);
			_inputRaise.onDeselect.AddListener(InRaise_onSubmit);
			_inputRaise.onSubmit.AddListener(InRaise_onSubmit);
			if (_slider != null)
			{
				_slider.onValueChanged.RemoveAllListeners();
				_slider.onValueChanged.AddListener(Slider_OnValueChanged);
			}
		}

		private void OnDisable()
		{
			it.Main.PopupController.Instance.ClosePopup(PopupType.FoldActionTable);
		}

		private void InRaise_onValueChange(string str)
		{
			decimal value = _minRiseValue;
			try
			{
				value = decimal.Parse(str) /*- (_beforeBat + _sharedData.CallCount)*/;
			}
			catch
			{
				return;
			}
			_currentRiseValue = Math.Clamp((decimal)value, (decimal)_minRiseValue, _maxRiseValue);
			if (value < MaxCurrentValue)
				_currentRiseValue = (decimal)Math.Round(_currentRiseValue, 2);
			//_currentRiseValue = (int)(_currentRiseValue / _currentTable.SmallBlindSize) * _currentTable.SmallBlindSize;
			ConfirmRiseValue();
			//_amountValueInput.text = intValue.ToString();
			//_sliderValue.value = (intValue - _useTable.buyInMinEURO) / (AmountMax - _useTable.buyInMinEURO);
		}

		private void InRaise_onSubmit(string str)
		{
			decimal value = _minRiseValue;
			//decimal value = decimal.Parse(str) /*- (_beforeBat + _sharedData.CallCount)*/;
			bool isParce = decimal.TryParse(str, out value);

			if (!isParce){
				it.Main.PopupController.Instance.ShowPopup<InfoPopup>(PopupType.Info).SetDescriptionString("game.error.noCorrectRaiseValue".Localized());
				return;
			}

			_currentRiseValue = Math.Clamp(value, _minRiseValue, _maxRiseValue);
			if (value < MaxCurrentValue)
			{
				//_currentRiseValue = (int)(_currentRiseValue / _currentTable.SmallBlindSize) * _currentTable.SmallBlindSize;
				_currentRiseValue = (decimal)Math.Round(_currentRiseValue, 2);
				if (!IsVisibleNumberTable)
					_inputRaise.text = (_currentRiseValue).ToString("F2");
			}
			else
			{
				if (!IsVisibleNumberTable)
					_inputRaise.text = MaxCurrentValue.ToString("F2");
			}
			if (_slider != null)
				_slider.value = (float)((_currentRiseValue - _minRiseValue) / (_maxRiseValue - _minRiseValue));

			ConfirmRiseValue();
		}
		private void Slider_OnValueChanged(float value)
		{
			_currentRiseValue = _minRiseValue + ((_maxRiseValue - _minRiseValue) * (decimal)value);
			if (value < 1 && value > 0)
				_currentRiseValue = (decimal)Math.Round(_currentRiseValue, 2);
			//_currentRiseValue = (int)(_currentRiseValue / _currentTable.SmallBlindSize) * _currentTable.SmallBlindSize;

			var outValue = _currentRiseValue;
			_inputRaise.text = _currentRiseValue.ToString("F2");
			ConfirmRiseValue();
		}

		private void ConfirmRiseValue()
		{
			if (_sharedData.IsFirstBetInRound && !_sharedData.IsPreFlop)
			{
				_allInButton.gameObject.SetActive(false);
				_betButton.gameObject.SetActive(true);
				//_betButton.gameObject.SetActive(_sharedData.CanRaise);
				_riseButton.gameObject.SetActive(false);
				SliderBlock.gameObject.SetActive(true);
				//SliderBlock.gameObject.SetActive(_sharedData.CanRaise);
			}

			if (_currentRiseValue >= _allInValue)
			{
				_allInButton.gameObject.SetActive(_sharedData.CanAllin);
				//_allInButton.gameObject.SetActive(_sharedData.CanAllin);
				_riseButton.gameObject.SetActive(false);
				_betButton.gameObject.SetActive(false);
				SliderBlock.gameObject.SetActive(_minRiseValue < _allInValue);
				SetRiseValue(_currentRiseValue);
			}
			else
			{
				SliderBlock.gameObject.SetActive(true);
				//SliderBlock.gameObject.SetActive(_sharedData.CanRaise);
				_allInButton.gameObject.SetActive(false);
				_riseButton.gameObject.SetActive(!_isBet);
				_betButton.gameObject.SetActive(_isBet);
				SetRiseValue(_currentRiseValue);
			}

		}

		private void SetRiseValue(decimal value)
		{
			if (!_isBet)
			{
				foreach (var elem in _raiseLabel)
					elem.text = $"{"game.panels.actionsButtons.raise".Localized()}\n{it.Helpers.Currency.String(value)}";
			}
			else
			{
				foreach (var elem in _betLabel)
					elem.text = $"{"game.panels.actionsButtons.bet".Localized()}\n{it.Helpers.Currency.String(value)}";
			}
		}

		public void Distribution(Table selectTable, DistributionSharedData sharedData, SocketEventDistributionUserData userData)
		{
			_sharedData = null;
			_sharedData = sharedData;
			_userData = null;
			_userData = userData;
			_currentTable = null;
			_currentTable = selectTable;
			_allIn = false;
			_isBet = _sharedData.IsFirstBetInRound && !_sharedData.IsPreFlop;

#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
			SetExtendVisible(false);
#endif

			if (_sharedData.active_event_meta_data == null) return;

			it.Main.PopupController.Instance.ClosePopup(PopupType.FoldActionTable);
			if (_groupPreflopHMButtons != null)
				_groupPreflopHMButtons.gameObject.SetActive(sharedData.IsPreFlop && ((GameType)_currentTable.game_rule_id == GameType.Holdem || (GameType)_currentTable.game_rule_id == GameType.Holdem3) && (_sharedData.CanRaise || _sharedData.CanBet));
			if (_groupPreflopButtons != null)
				_groupPreflopButtons.gameObject.SetActive(sharedData.IsPreFlop && !((GameType)_currentTable.game_rule_id == GameType.Holdem || (GameType)_currentTable.game_rule_id == GameType.Holdem3) && (_sharedData.CanRaise || _sharedData.CanBet));
			_groupButtons.gameObject.SetActive(!sharedData.IsPreFlop && (_sharedData.CanRaise || _sharedData.CanBet));

			for (int i = 0; i < sharedData.players.Count; i++)
				if (sharedData.players[i].user.id == UserController.User.id)
					_myPlayer = sharedData.players[i];

			for (int i = 0; i < sharedData.banks[0].sources.Length; i++)
				if (_beforeAction < sharedData.banks[0].sources[i].amount)
					_beforeAction = sharedData.banks[0].sources[i].amount;

			for (int i = 0; i < sharedData.players.Count; i++)
				if (_beforeBat < sharedData.players[0].bet)
					_beforeBat = sharedData.players[0].bet;

			_beforeBat = sharedData.IsPreFlop ? _myPlayer.bet : _myPlayer.BetInRound;
			_allInValue = _sharedData.MePlayer.amount + _beforeBat;

			if ((_sharedData.active_event_meta_data.min_raise == 0 && _sharedData.active_event_meta_data.max_raise == 0) || _sharedData.active_event_meta_data.min_raise == 0)
			{
				_minRiseValue = _sharedData.MePlayer.amount + _beforeBat;
				_maxRiseValue = _sharedData.MePlayer.amount + _beforeBat;
				_allIn = true;
			}
			else
			{
				_minRiseValue = (decimal)_sharedData.active_event_meta_data.min_raise;
				_maxRiseValue = Math.Min((decimal)_sharedData.active_event_meta_data.max_raise,
											Math.Max((decimal)_sharedData.active_event_meta_data.min_raise, _sharedData.MePlayer.amount + (decimal)_sharedData.active_event_meta_data.max_raise - _sharedData.CallCount));
			}
			_maxBetOnRizeInPlayers = _sharedData.MaxBetInPlayers();
			_currentRiseValue = _minRiseValue;
			_riseDiapasone = _maxRiseValue - _minRiseValue;

			if (_riseDiapasone <= 0)
				_allIn = false;

			if (_slider != null)
				_slider.value = 0;
			Slider_OnValueChanged(0);

			if (_sharedData.IsFirstBetInRound && !_sharedData.IsPreFlop)
				_currentRiseValue = _currentTable.big_blind_size;

			if (_slider != null)
			{
				var c = _slider.GetComponent<it.UI.Elements.SliderScrollHandler>();
				//c.Speed = (_minRiseValue != _maxRiseValue) ? selectTable.SmallBlindSize / (_maxRiseValue - _minRiseValue) : 1;
				c.Speed = (float)((_minRiseValue != _maxRiseValue) ? 0.01m / (_maxRiseValue - _minRiseValue) : 1);
			}
			var isMeActive = sharedData.IsMeActive;
			SetStateButtons(isMeActive);
			if (isMeActive)
			{

				if (!selectTable.is_all_or_nothing)
					CheckAndSetActionButtons();

				ConfirmRiseValue();

				if (!(bool)sharedData.active_event_meta_data.can_raise && (bool)sharedData.active_event_meta_data.can_allin)
					SliderBlock.gameObject.SetActive(false);

				ConfirmXButtons();
				//bool isFistWord = _sharedData.IsFirstWord();

				var betInfo = sharedData.BetInfo;
				var isMeAllIn = betInfo.IsAllIn;

#if !UNITY_ANDROID && !UNITY_WEBGL && !UNITY_IOS
				//if (!selectTable.is_all_or_nothing) RaiseBtn.gameObject.SetActive(!isMeAllIn && callInfo.IsMe && currentSharedData.CanRaise);
				// if (!selectTable.is_all_or_nothing) RaiseBtn.gameObject.SetActive(currentSharedData.CanRaise);
				_x2Button.interactable = !isMeAllIn && betInfo.IsMe;
				_x3Button.interactable = !isMeAllIn && betInfo.IsMe;
				_x4Button.interactable = !isMeAllIn && betInfo.IsMe;
				_maxButton.interactable = !isMeAllIn && betInfo.IsMe;
				_x2PreflopHMButton.interactable = !isMeAllIn && betInfo.IsMe;
				_x3PreflopHMButton.interactable = !isMeAllIn && betInfo.IsMe;
				_x4PreflopHMButton.interactable = !isMeAllIn && betInfo.IsMe;
				_x2PreflopButton.interactable = !isMeAllIn && betInfo.IsMe;
				_maxPreflopButton.interactable = !isMeAllIn && betInfo.IsMe;
				SetEnabledSlider(!isMeAllIn && betInfo.IsMe);
#endif
#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
				//if(selectTable.is_all_or_nothing == false) RaiseBtn.gameObject.SetActive(!isMeAllIn && callInfo.IsMe && currentSharedData.CanRaise);
				_x2Button.gameObject.SetActive(!isMeAllIn && betInfo.IsMe);
				_x3Button.gameObject.SetActive(!isMeAllIn && betInfo.IsMe);
				_x4Button.gameObject.SetActive(!isMeAllIn && betInfo.IsMe);
				_maxButton.gameObject.SetActive(!isMeAllIn && betInfo.IsMe);
				_x2PreflopHMButton.gameObject.SetActive(!isMeAllIn && betInfo.IsMe);
				_x3PreflopHMButton.gameObject.SetActive(!isMeAllIn && betInfo.IsMe);
				_x4PreflopHMButton.gameObject.SetActive(!isMeAllIn && betInfo.IsMe);
				_x2PreflopButton.gameObject.SetActive(!isMeAllIn && betInfo.IsMe);
				_maxPreflopButton.gameObject.SetActive(!isMeAllIn && betInfo.IsMe);
				SetEnabledSlider(!isMeAllIn && betInfo.IsMe);
#endif
			}

		}

		private void ConfirmXButtons()
		{

			bool isFirstBet = _sharedData.IsFirstBetInRound && !_sharedData.IsPreFlop;
			foreach (var elem in _X2Label)
				elem.text = !isFirstBet ? "x2" : UserController.User.user_profile.betting.button1.normal_bet_size;
			foreach (var elem in _X3Label)
				elem.text = !isFirstBet ? "x3" : UserController.User.user_profile.betting.button2.normal_bet_size;
			foreach (var elem in _X4Label)
				elem.text = !isFirstBet ? "x4" : UserController.User.user_profile.betting.button3.normal_bet_size;
			foreach (var elem in _XAllLabel)
				elem.text = !isFirstBet ? "game.panels.actionsButtons.bank".Localized() : "100%";
		}

		public void SliderIncButtonTouch()
		{
			InRaise_onSubmit((_currentRiseValue + _currentTable.SmallBlindSize).ToString());
		}

		public void SliderDeckButtonTouch()
		{
			InRaise_onSubmit((_currentRiseValue - _currentTable.SmallBlindSize).ToString());
		}

		public void UpdateRaiseCount()
		{
			_allInButton.gameObject.SetActive(GameManager._currentSharedData.CanAllin && !GameManager._currentSharedData.CanRaise);
			_betButton.gameObject.SetActive(false);
			_riseButton.gameObject.SetActive(GameManager._currentSharedData.CanRaise);

			if (_sharedData == null) return;
			_isBet = _sharedData.IsFirstBetInRound && !_sharedData.IsPreFlop;
			var bet = _isBet ? "Bet" : "Raise";
			if ((decimal)GetSliderValue() + _sharedData.CallCount >= _sharedData.MePlayer.amount)
			{
				_allInButton.gameObject.SetActive(_sharedData.CanAllin);
				//_allInButton.gameObject.SetActive(_sharedData.CanAllin);
			}
			else
			{
				if (bet == "Bet")
				{
					_betButton.gameObject.SetActive(true);
					_allInButton.gameObject.SetActive(false);
					//_betButton.gameObject.SetActive(_sharedData.CanRaise);
				}
				else
				{
					//_riseButton.gameObject.SetActive(true);
					_riseButton.gameObject.SetActive(_sharedData.CanRaise);
					_allInButton.gameObject.SetActive(false);
				}
			}
		}

		public void SetSlider(float step, decimal min, decimal max, bool isWhole)
		{
			return;
			this._isWhole = isWhole;
			_stepSlider = step;
			_minRiseValue = (decimal)min;
			_maxRiseValue = (decimal)max;
			_maxRiseValue = Math.Min(_maxRiseValue, _sharedData.MePlayer.amount);

			SetEnabledSlider(min != max);
			//SetRaiseCount(min);
		}

		public void SetEnabledSlider(bool isEnable)
		{
			if (_slider != null)
				_slider.interactable = isEnable;
		}

		public void X2Button()
		{
			if (_allIn) return;

			var value = float.Parse(UserController.User.user_profile.betting.button1.normal_bet_size.Replace("%", "")) / 100;
			decimal count = !(_sharedData.IsFirstBetInRound && !_sharedData.IsPreFlop)
					? _maxBetOnRizeInPlayers * 2 //  _currentTable.BigBlindSize * 2
					: (GameManager._currentSharedData.GetTotalBank() * (decimal)value);
			count = (decimal)Math.Round(count, 2);

			InRaise_onSubmit(count.ToString());
		}

		public void X3Button()
		{
			if (_allIn) return;

			var value = float.Parse(UserController.User.user_profile.betting.button2.normal_bet_size.Replace("%", "")) / 100;
			decimal count = !(_sharedData.IsFirstBetInRound && !_sharedData.IsPreFlop)
					? _maxBetOnRizeInPlayers * 3 // _currentTable.BigBlindSize * 3
					: (GameManager._currentSharedData.GetTotalBank() * (decimal)value);
			count = (decimal)Math.Round(count, 2);
			InRaise_onSubmit(count.ToString());
		}

		public void X4button()
		{
			if (_allIn) return;

			var value = decimal.Parse(UserController.User.user_profile.betting.button3.normal_bet_size.Replace("%", "")) / 100;
			decimal count = !(_sharedData.IsFirstBetInRound && !_sharedData.IsPreFlop)
					? _maxBetOnRizeInPlayers * 4 // _currentTable.BigBlindSize * 4
					: (GameManager._currentSharedData.GetTotalBank() * (decimal)value);

			count = (decimal)Math.Round(count, 2);

			InRaise_onSubmit(count.ToString());
		}

		public void TotalBankButton()
		{
			if (_allIn) return;

			decimal count = GameManager._currentSharedData.GetTotalBank();
			if (!(_sharedData.IsFirstBetInRound && !_sharedData.IsPreFlop))
			//if (!GameManager.CurrentSharedData.IsFirstWord())
			{
				//float maxBet = GameManager.CurrentSharedData.GetMaxBet();
				count = _maxBetOnRizeInPlayers * 3 + count;
			}
			count = (decimal)Math.Round(count, 2);

			InRaise_onSubmit(count.ToString());

		}

		#region Визуальная клавиатура

		private string vk_inputString;
		private bool IsVisibleNumberTable =>
#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
		_numberInputTablo != null && _numberInputTablo.gameObject.activeSelf;
#else
		false;
#endif

		public void ShowNumberTablo()
		{
#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
			_numberInputTablo.gameObject.SetActive(true);
			vk_inputString = _inputRaise.textComponent.text;
#endif
		}
		/// <summary>
		/// Ввод с визуальной клавиатуры на мобильной и WebGL платформе
		/// </summary>
		/// <param name="symbol"></param>
		public void SetVisibleKeyboardInput(string symbol)
		{
			vk_inputString += symbol;
			ConfirmVkInput();
			//InRaise_onSubmit(vk_inputString);
		}
		public void SetVisibleKeyboardBackspace()
		{
			if (vk_inputString.Length == 0) return;
			vk_inputString = _inputRaise.text.Substring(0, _inputRaise.text.Length - 1);
			ConfirmVkInput();
		}

		private void ConfirmVkInput()
		{
			decimal vk_customValue = 0;

			try
			{
				vk_customValue = decimal.Parse(vk_inputString);
			}
			catch
			{
				_inputRaise.text = vk_inputString;
				_inputRaise.caretPosition = vk_inputString.Length;
				return;
			}

			if (vk_customValue > _maxRiseValue)
				vk_customValue = _maxRiseValue;
			if (vk_customValue < _minRiseValue)
				vk_customValue = _minRiseValue;

			if (_slider != null)
				_slider.value = (float)((vk_customValue - _minRiseValue) / (_maxRiseValue - _minRiseValue));
			//SetRiseValue(vk_customValue);
			InRaise_onSubmit(vk_customValue.ToString());
			_inputRaise.text = vk_inputString;
			_inputRaise.caretPosition = vk_inputString.Length;
			//if (vk_customValue == _maxRiseValue)
			//{
			//	_inputRaise.text = vk_inputString;
			//}
		}

		/// <summary>
		/// Вызывается кнопкой интерфейся
		/// </summary>
		public void CancelExtendRize()
		{
			SetExtendVisible(false);
			Slider_OnValueChanged(0);

			if (_slider != null)
				_slider.value = 0;
		}
		public void SetExtendVisible(bool isVisible)
		{
			_isRiseExtend = isVisible;

			_checkButton.gameObject.SetActive(_sharedData.CanCheck && !_isRiseExtend);
			_callButton.gameObject.SetActive(_sharedData.CanCall && !_isRiseExtend);
			_foldButton.gameObject.SetActive(!_isRiseExtend);
#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
			_numberInputTablo.gameObject.SetActive(false);
			_riseExtendOptions.gameObject.SetActive(_isRiseExtend);
#endif
		}


		#endregion

		public void SetRaiseCount(decimal count)
		{
			if (count > (decimal)_slider.maxValue) count = (decimal)_slider.maxValue;
			_currentValue = _isWhole && _slider.value != _slider.maxValue ? (decimal)count : count;
			_slider.value = (float)_currentValue;

		}

		public void FoldButton()
		{
			if (_sharedData.CanCheck)
			{
				var panel = it.Main.PopupController.Instance.ShowPopup<FoldActionPopup>(PopupType.FoldActionTable);
				panel.OnOk = () =>
				{
					FoldConfirm();
				};
			}
			else
				FoldConfirm();
		}

		private void FoldConfirm()
		{
			ulong? eventId = GameManager._currentSharedData.active_event.id;
			SetStateButtons(false);

#if UNITY_ANDROID
			Handheld.Vibrate();
#endif

			gameObject.SetActive(false);
			TableApi.FoldActiveCards(GameManager.SelectTable.id, (result) =>
			{
				//if (result.IsSuccess)
				//	if (gameObject.activeSelf && (GameManager._currentSharedData == null || GameManager._currentSharedData.active_event == null || GameManager._currentSharedData.active_event.id == eventId))
				//		gameObject.SetActive(false);
				if (!result.IsSuccess)
				{
					SetStateButtons(true);
					gameObject.SetActive(true);
				};
			}
			);
		}

		public void CallButton()
		{
			ulong? eventId = GameManager._currentSharedData.active_event.id;
			SetStateButtons(false);

			it.Logger.Log("Can call = " + GameManager._currentSharedData.CanCall);

#if UNITY_ANDROID
			Handheld.Vibrate();
#endif
			gameObject.SetActive(false);
			TableApi.CallActiveDistribution(GameManager.SelectTable.id, (result) =>
			{

				//SetStateButtons(true);
				//if (result.IsSuccess)
				//	if (gameObject.activeSelf && (GameManager._currentSharedData == null || GameManager._currentSharedData.active_event == null || GameManager._currentSharedData.active_event.id == eventId))
				//		gameObject.SetActive(false);

				if (!result.IsSuccess)
				{
					SetStateButtons(true);
					gameObject.SetActive(true);
				}

			}
			);
		}

		public void CheckButton()
		{
			ulong? eventId = GameManager._currentSharedData.active_event.id;
			SetStateButtons(false);
			it.Logger.Log("Can check = " + GameManager._currentSharedData.CanCheck);

#if UNITY_ANDROID
			Handheld.Vibrate();
#endif
			gameObject.SetActive(false);
			TableApi.CheckActiveDistribution(GameManager.SelectTable.id, (result) =>
			{
				//SetStateButtons(true);
				//if (result.IsSuccess)
				//	if (gameObject.activeSelf && (GameManager._currentSharedData == null || GameManager._currentSharedData.active_event == null || GameManager._currentSharedData.active_event.id == eventId))
				//		gameObject.SetActive(false);
				if (!result.IsSuccess)
				{
					SetStateButtons(true);
					gameObject.SetActive(true);
				}
			}
			);
		}

		public void AllinButton()
		{
			ulong? eventId = GameManager._currentSharedData.active_event.id;
			SetStateButtons(false);
			it.Logger.Log("Can allin = " + GameManager._currentSharedData.CanAllin);

#if UNITY_ANDROID
			Handheld.Vibrate();
#endif
			gameObject.SetActive(false);
			TableApi.AllinActiveDistribution(GameManager.SelectTable.id, (result) =>
			{
				//SetStateButtons(true);
				//if (result.IsSuccess)
				//	if (gameObject.activeSelf && (GameManager._currentSharedData == null || GameManager._currentSharedData.active_event == null || GameManager._currentSharedData.active_event.id == eventId))
				//		gameObject.SetActive(false);
				if (!result.IsSuccess)
				{
					SetStateButtons(true);
					gameObject.SetActive(true);
				}
			}
			);
		}

		public void RaiseButton()
		{
#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
			if (!_isRiseExtend)
			{
				SetExtendVisible(true);
				return;
			}
			SetExtendVisible(false);
#endif

			Raise(((_currentRiseValue * 100)) / 100);
		}
		public void BetButton()
		{
#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
			if (!_isRiseExtend)
			{
				SetExtendVisible(true);
				return;
			}
			SetExtendVisible(false);
#endif
			//Raise(_minRiseValue);
			Raise(((_currentRiseValue * 100)) / 100);
		}

		public void Raise(decimal count)
		{

			SetStateButtons(false);
			gameObject.SetActive(false);
			RaiseInfo raiseInfo = new RaiseInfo(count);

#if UNITY_ANDROID
			Handheld.Vibrate();
#endif
			GameHelper.RaiseActiveDistributionBet(GameManager.SelectTable, raiseInfo, (isSuccess) =>
			{
				if (!isSuccess)
				{
					SetStateButtons(true);
					gameObject.SetActive(true);
				}
			});
		}

		public float GetSliderValue()
		{
			if (_isWhole && _slider.value != _slider.maxValue) return (int)_slider.value;
			return _slider.value;
		}
		public void SetSliderValue(decimal value)
		{
			_currentValue = _isWhole && _slider.value != _slider.maxValue ? (decimal)value : value;
			_slider.value = (float)_currentValue;
		}

		public void SetStateButtons(bool isActiveState)
		{


			_checkButton.interactable = isActiveState;
			_foldButton.interactable = isActiveState;
			_callButton.interactable = isActiveState;
			_betButton.interactable = isActiveState;
			if (_x2Button != null)
				_x2Button.interactable = isActiveState;
			if (_x3Button != null)
				_x3Button.interactable = isActiveState;
			if (_x4Button != null)
				_x4Button.interactable = isActiveState;
			if (_maxButton != null)
				_maxButton.interactable = isActiveState;
			if (_x2PreflopHMButton != null)
				_x2PreflopHMButton.interactable = isActiveState;
			if (_x3PreflopHMButton != null)
				_x3PreflopHMButton.interactable = isActiveState;
			if (_x4PreflopHMButton != null)
				_x4PreflopHMButton.interactable = isActiveState;
			if (_x2PreflopButton != null)
				_x2PreflopButton.interactable = isActiveState;
			if (_maxPreflopButton != null)
				_maxPreflopButton.interactable = isActiveState;
			if (_slider != null)
				_slider.interactable = isActiveState;
			_riseButton.interactable = isActiveState;
			//SliderBlock.gameObject.SetActive(_riseButton.gameObject.activeInHierarchy);
			_allInButton.interactable = isActiveState;
			SetEnabledSlider(isActiveState);
			if (!isActiveState)
				it.Main.PopupController.Instance.ClosePopup(PopupType.FoldActionTable);

			//#if UNITY_STANDALONE_WIN || UNITY_EDITOR
			//			if (isActiveState) GameMultiWindowManager.FocusWindow(true);
			//#endif
		}


		private void CheckAndSetActionButtons()
		{
			_checkButton.gameObject.SetActive(_sharedData.CanCheck && !_isRiseExtend);
			_callButton.gameObject.SetActive(_sharedData.CanCall && !_isRiseExtend);
			_foldButton.gameObject.SetActive(!_isRiseExtend);
			_riseButton.gameObject.SetActive(_sharedData.CanRaise);
			_betButton.gameObject.SetActive(_sharedData.CanBet);

			//_allInButton.gameObject.SetActive(_sharedData.CanAllin);
			_allInButton.gameObject.SetActive(_sharedData.CanAllin /*&& !GameManager._currentSharedData.CanCall*/ && !_sharedData.CanRaise);
			SetEnabledSlider(_sharedData.CanRaise);
			//_groupButtons.SetActive(GameManager._currentSharedData.CanRaise);
			if (_sharedData.CanCall)
				foreach (var elem in _callLabel)
					elem.text = _sharedData.CallCount > 0 ? $"{("game.panels.actionsButtons.call".Localized())}\n {it.Helpers.Currency.String((float)Math.Round(_sharedData.CallCount + _beforeBat, 2))}" : ("game.panels.actionsButtons.call".Localized());

			//SliderBlock.gameObject.SetActive(_riseButton.gameObject.activeInHierarchy);
			if (!_sharedData.CanRaise)
			{
				SetSliderValue(0);
			}
		}


	}
}