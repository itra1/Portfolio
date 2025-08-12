using Providers.Network.Common;
using Providers.Network.Materials;

using TMPro;

using UGui.Screens.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Zenject;
using UGui.Screens.Components;
using Cysharp.Threading.Tasks;
using Providers.User;
using UGui.Screens.Elements;
using Providers.SystemMessage.Common;
using Common;

namespace UGui.Screens.Pages
{
	public class RegistrationPage : MonoBehaviour, IZInjection
	{
		[SerializeField] private Toggle _phoneToggle;
		[SerializeField] private Toggle _emailToggle;
		[SerializeField] private TMP_InputField _loginInput;
		[SerializeField] private TMP_InputField _passwordInput;
		[SerializeField] private TMP_InputField _promoInput;
		[SerializeField] private CountryDropdown _countryDropdown;
		[SerializeField] private TMP_Dropdown _currencyDropdown;
		[SerializeField] private Toggle _old18YearToggle;
		[SerializeField] private Button _applyButton;

		private INetworkApi _api;
		private IUserProvider _userProvider;
		private ISystemMessageVisible _meassage;
		private CurrencyData[] _currencies;
		private List<CountryData> _countries;

		[Inject]
		private void Initialize(IUserProvider userProvider, INetworkApi api,
		ISystemMessageVisible message)
		{
			_api = api;
			_userProvider = userProvider;
			_meassage = message;
		}

		private void OnEnable()
		{
			ClearForm();
			FillPage();
		}

		private void FillPage()
		{
			FillCurrencys();
			FillCountryes();
		}

		private void ClearForm()
		{
			_loginInput.text = "";
			_passwordInput.text = "";
			_promoInput.text = "";
			_old18YearToggle.isOn = false;
		}

		private async void FillCurrencys()
		{

			if (_currencies == null)
			{
				(bool result, object response) = await _api.GetCurrency();
				if (!result)
				{
				}
				_currencies = (CurrencyData[])response;
			}

			_currencyDropdown.ClearOptions();

			List<TMP_Dropdown.OptionData> _options = new();

			int recomended = -1;

			for (int i = 0; i < _currencies.Length; i++)
			{
				_options.Add(new() { text = _currencies[i].code });
				if (_currencies[i].recommended)
					recomended = i;
			}

			_currencyDropdown.AddOptions(_options);
			if (recomended >= 0)
				_currencyDropdown.value = recomended;
		}

		private async void FillCountryes()
		{

			if (_countries == null)
			{
				(bool result, object response) = await _api.GetCountries();

				if (!result)
				{

				}

				_countries = (List<CountryData>)response;
				_countryDropdown.SetCoutryList(_countries);
			}

		}

		private bool ValidateForm(){

			var login = _loginInput.text;
			var password = _passwordInput.text;

			var isEmail = _emailToggle.isOn;
			var isPhone = _phoneToggle.isOn;

			if (string.IsNullOrEmpty(login))
			{
				_meassage.SetMessage(ErrorMessages.UserNameEmpty);
				return false;
			}

			if (string.IsNullOrEmpty(password))
			{
				_meassage.SetMessage(ErrorMessages.PasswordEmpty);
				return false;
			}

			isEmail = isEmail && InputValidate.IsEmail(login);
			isPhone = isPhone && InputValidate.IsPhone(login);

			if (!isEmail && !isPhone)
			{
				_meassage.SetMessage(ErrorMessages.UserNameErrorFormat);
				return false;
			}

			var passwordError = InputValidate.Password(password);
			switch (passwordError)
			{
				case InputValidate.ErrorFormat:
					{
						_meassage.SetMessage(ErrorMessages.PasswordErrorFormat);
						return false;
					};
				case InputValidate.ErrorLenght:
					{
						_meassage.SetMessage(ErrorMessages.PasswordLenght);
						return false;
					};
			};


			var country = _countryDropdown.GetValue();

			if(country == null)
			{
				_meassage.SetMessage(ErrorMessages.CountryNoSelect);
				return false;
			}
			bool is18 = _old18YearToggle.isOn;

			if (!is18)
			{
				_meassage.SetMessage(ErrorMessages.AgeNoConfirm);
				return false;
			}

			return true;
		}

		private async UniTaskVoid RegistrationProcess()
		{
			if (!ValidateForm()) return;

			Dictionary<string, object> properties = new(){
			 {"password", _passwordInput.text },
			 {"password_confirmation", _passwordInput.text},
			 {"country", _countryDropdown.GetValue()},
			 {"currency", _currencies[_currencyDropdown.value] },
			 {"confirm", _old18YearToggle.isOn}
			};

			var isEmail = _emailToggle.isOn;
			var isPhone = _phoneToggle.isOn;

			if (isEmail)
			{
				properties.Add("email", _loginInput.text);
			}

			if (isPhone)
			{
				properties.Add("phone", "7" + _loginInput.text);
			}

			(bool result, object response) = await _api.Registration(properties);

			if (!result)
			{
				Debug.Log($"Error {result} {response}");
				return;
			}
			var authorization = response as UserData;
			if (isEmail)
				GetComponentInParent<AuthorizationScreen>().OnRegistrationEmail(authorization);
			if (isPhone)
				GetComponentInParent<AuthorizationScreen>().OnRegistrationPhone(authorization);

			//_userProvider.SetUserData(authorization);

			//GetComponentInParent<Screens.Base.Screen>().Hide();
		}

		public void RegistrationButtonTouch()
		{
			RegistrationProcess().Forget();
		}

	}
}
