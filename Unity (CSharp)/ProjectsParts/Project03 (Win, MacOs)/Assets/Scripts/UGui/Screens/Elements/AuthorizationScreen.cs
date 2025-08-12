using Base.Attributes;
using UnityEngine;
using UGui.Screens.Base;
using UGui.Screens.Pages;
using Screen = UGui.Screens.Base.Screen;
using UGui.Screens.Components;
using Zenject;
using Providers.Network.Materials;

namespace UGui.Screens.Elements
{
	[PrefabName(ScreenTypes.Authorization)]
	public class AuthorizationScreen : Screen
	{
		[SerializeField] private AuthorizationBaseMenu _baseMenu;
		[SerializeField] private RegistrationPage _registrationPage;
		[SerializeField] private LoginPage _loginPage;
		[SerializeField] private RegistrationEmailConfirmPage _registrationEmailConfirmPage;
		[SerializeField] private RegistrationCompletePage _registrationCompletePage;
		[SerializeField] private RegistrationErrorPage _registrationErrorPage;

		private IScreenProvider _screenProvider;

		[Inject]
		public void Initialize(IScreenProvider screenProvider){
			_screenProvider = screenProvider;
		}

		private void Awake()
		{
			SubscribeEvents();

			_loginPage.OnForgot = () =>
			{
				_screenProvider.OpenWindow(ScreenTypes.ForgotPassword);
			};

		}

		private void SubscribeEvents()
		{
			_baseMenu.OnClose = () =>
			{
				Hide();
			};

			_baseMenu.OnLogin.AddListener(OnLogin);
			_baseMenu.OnRegistration.AddListener(OnRegistration);
		}

		private void CloaseAllPage()
		{
			_loginPage.gameObject.SetActive(false);
			_registrationPage.gameObject.SetActive(false);
			_registrationEmailConfirmPage.gameObject.SetActive(false);
			_registrationCompletePage.gameObject.SetActive(false);
			_registrationErrorPage.gameObject.SetActive(false);
		}

		private void OnLogin()
		{
			CloaseAllPage();
			_loginPage.gameObject.SetActive(true);
		}

		private void OnRegistration()
		{
			CloaseAllPage();
			_registrationPage.gameObject.SetActive(true);
		}

		public void OnRegistrationEmail(UserData userData)
		{
			_registrationPage.gameObject.SetActive(false);
			_registrationEmailConfirmPage.gameObject.SetActive(true);
			_registrationEmailConfirmPage.SetData(userData);
		}

		public void OnRegistrationPhone(UserData userData)
		{
			_registrationPage.gameObject.SetActive(false);
			_registrationCompletePage.gameObject.SetActive(true);
			_registrationEmailConfirmPage.SetData(userData);
		}

		public void OnRegistrationError()
		{
			_registrationPage.gameObject.SetActive(false);
			_registrationErrorPage.gameObject.SetActive(true);
		}

		public void OnRegistrationComplete()
		{
			_registrationCompletePage.gameObject.SetActive(false);
			_registrationEmailConfirmPage.gameObject.SetActive(false);
			OnLogin();
		}

		public void OnRegistrationErrorComplete()
		{
			_registrationErrorPage.gameObject.SetActive(false);
			OnRegistration();
		}

	}
}
