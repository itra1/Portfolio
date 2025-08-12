using Game.Providers.Profile;
using Game.Providers.Telegram.Handlers;
using Game.Providers.Ui.Attributes;
using Game.Providers.Ui.Base;
using Game.Providers.Ui.Controllers;
using Game.Providers.Ui.Presenters.Base;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Presenters
{
	[UiController(WindowPresenterType.Popup, WindowPresenterNames.Withdrawal)]
	public class WithdrawalWindowPresenter : WindowPresenter
	{
		public UnityAction OnSend;

		[SerializeField] private TMP_InputField _fioInput;
		[SerializeField] private TMP_InputField _sexInput;
		[SerializeField] private TMP_InputField _ageInput;
		[SerializeField] private TMP_InputField _countryInput;
		[SerializeField] private TMP_InputField _cityInput;
		[SerializeField] private TMP_InputField _payPalInput;
		[SerializeField] private Toggle _confirmToggle;

		private TelegramHandler _telegramHandler;
		private IProfileProvider _profile;
		private IUiProvider _uiProvider;

		[Inject]
		public void Constructor(TelegramHandler telegramHandler, IProfileProvider profile, IUiProvider uiProvider)
		{
			_telegramHandler = telegramHandler;
			_profile = profile;
			_uiProvider = uiProvider;
		}

		public void CloseButtonTouch()
		{
			Hide();
		}

		public void SendButtonTouch()
		{
			var sendMessage =
			$"Withdrawal form:\r\nFull name: {_fioInput.text}\r\nSex: {_sexInput.text}\r\nAge: {_ageInput.text}\r\nCountry: {_countryInput.text}\r\nCity: {_cityInput.text}\r\nPayPal: {_payPalInput.text}\r\nBalance: {_profile.Dollar}";

			_telegramHandler.Send(sendMessage);

			var developController = _uiProvider.GetController<DevelopWindowPresenterController>();
			_ = developController.Show(null);
			developController.Presenter.SetTitle("Thanks!");
			developController.Presenter.SetDescription("We will respond to you after processing the information, usually it takes 1-2 weeks.");

			OnSend?.Invoke();
			CloseButtonTouch();
		}
	}
}
