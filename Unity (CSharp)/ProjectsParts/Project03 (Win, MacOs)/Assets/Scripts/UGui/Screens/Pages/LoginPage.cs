using Common;
using Cysharp.Threading.Tasks;
using Providers.Network.Common;
using Providers.Network.Materials;
using Providers.SystemMessage.Common;
using Providers.User;
using TMPro;
using UGui.Screens.Common;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UGui.Screens.Pages
{
	public class LoginPage : MonoBehaviour, IZInjection
	{
		public UnityEngine.Events.UnityAction OnForgot;
		public UnityEngine.Events.UnityAction OnEnter;

		[SerializeField] private Button _applyButton;
		[SerializeField] private TMP_InputField _loginInput;
		[SerializeField] private TMP_InputField _passwordInput;
		[SerializeField] private Toggle _rememberToggle;
		[SerializeField] private Button _forgotButton;
		[SerializeField] private Button _enterButton;

		private IUserProvider _userProvider;
		private IUserProviderRequests _userProviderRequests;
		private IUserAuth _userAuth;
		private INetworkApi _api;
		private ISystemMessageVisible _meassage;

		[Inject]
		private void Initiate(IUserProvider userProvider,
		IUserAuth userAuth,
		INetworkApi api,
		IUserProviderRequests userProviderRequests,
		ISystemMessageVisible message)
		{
			_userProvider = userProvider;
			_userAuth = userAuth;
			_meassage = message;
			_userProviderRequests = userProviderRequests;
			_api = api;
		}

		private void OnEnable()
		{
			ClearFields();
			_applyButton.interactable = true;
		}

		private void ClearFields()
		{
			(var user, var password) = _userAuth.GetAuthData();

			_loginInput.text = user == null ? "" : user;
			_passwordInput.text = password == null ? "" : password;
			_rememberToggle.isOn = user != null;
		}

		private bool ValidateForm()
		{
			var login = _loginInput.text;
			var password = _passwordInput.text;

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

			bool isEmail = InputValidate.IsEmail(login);
			bool isPhone = InputValidate.IsPhone(login);

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

			return true;

		}

		private async UniTaskVoid Login()
		{

			var login = _loginInput.text;
			var password = _passwordInput.text;
			bool remember = _rememberToggle.isOn;

			if (!ValidateForm()) return;

			Debug.Log($"Try auth {login} {password}");

			_applyButton.interactable = false;

			(bool result, object response) = await _userProviderRequests.AuthorizationRequest(login, password);
			_applyButton.interactable = true;

			if (!result)
			{
				try
				{
					ErrorData errorData = (ErrorData)response;
					if (errorData.message == "Unauthenticated.")
						_meassage.SetMessage(ErrorMessages.Unauthenticated);
				}
				catch
				{
					_meassage.SetMessage(ErrorMessages.ServerError);
				}
				return;
			}

			if (remember)
				_userAuth.SetAuthData(login, password);
			else
				_userAuth.ClearAuthData();

			var authorization = response as UserData;
			_userProvider.SetUserData(authorization);

			GetComponentInParent<Screens.Base.Screen>().Hide();
		}

		#region ButtonsEvents

		public void ForgotButtonTouch()
		{
			Debug.Log("ForgotButtonTouch");
			OnForgot?.Invoke();
		}

		public void EnterButtonTouch()
		{
			Login().Forget();
			OnEnter?.Invoke();
		}

		#endregion
	}
}
