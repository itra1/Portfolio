using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
 
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Leguar.TotalJSON;
using it.Network.Rest;

namespace it.Popups
{
	public class LoginBehaviour : PopupBase
	{
		Dictionary<int, string> errorTypes = new Dictionary<int, string>
		{
			[403] = "Неправильно введен пароль или почта",
			[422] = "Ошибка валидации",
			[2] = "Одно или более полей ввода пустое",
			[4] = "Неверный формат поля e-mail"
		};
		public static int errorType;

		[SerializeField] private Button btnLogin = null;
		[SerializeField] private TMP_InputField email = null;
		[SerializeField] private TMP_InputField password = null;
		[SerializeField] private Toggle rememberMe = null;
		[SerializeField] private TMP_Text errorText;
		[SerializeField] GameObject SplashScreen;

		private SessionSaver sessionSaver;

		private void Start()
		{
			btnLogin.interactable = true;
			sessionSaver = new SessionSaver();
			sessionSaver.LoadUser();

			if (AppConfig.TableExe == null)//run multiwindow on standalone
			{
				//SplashScreen.SetActive(true);
				var authManager = AuthManager.Instance;
				authManager.LoginByToken(SessionSaver.GetTokenForMultiWindow(), (res) =>
				{

					GameHelper.SaveUserInfo(res);
					StartGame();
				}, (err) =>
				{

				});
				return;
			}

			if (!string.IsNullOrWhiteSpace(sessionSaver.AuthKey) &&
					DateTime.Now < sessionSaver.RefreshDateTime &&
					(DateTime.Now - sessionSaver.FirstLoginDateTime).Days <= 30)
			{
				SplashScreen.SetActive(true);

				AuthManager.Instance.Refresh(sessionSaver.AuthKey, (userinfo) =>
				{

					if (userinfo == null)
					{
						SplashScreen.SetActive(false);
						return;
					}

				//sessionSaver.SaveAuth(userinfo, false);
				GameHelper.SaveUserInfo(userinfo);
					StartGame();
				}, null);


				//var userinfo = await authManager.Refresh(sessionSaver.AuthKey);
				//if (userinfo == null)
				//{
				//	SplashScreen.SetActive(false);
				//	return;
				//}

				////sessionSaver.SaveAuth(userinfo, false);
				//GameController.SaveUserInfo(userinfo);
				//StartGame();
			}
			email.ActivateInputField();
		}

		private void Update()
		{
			if (Input.GetKeyUp(KeyCode.Tab))
			{
				password.ActivateInputField();
			}

			if (Input.GetKeyUp(KeyCode.Return))
			{
				Login();
			}
		}

		public void Login()
		{
			btnLogin.interactable = false;
			if (CheckEmptyFields())
			{
				errorType = 2;
				OutputError();
				return;
			}
			if (Validator.Email(email.text) == -1)
			{
				errorType = 4;
				OutputError();
				return;
			}

			//LoginRequest();
			//it.Managers.AuthManager.Instance.Login(email.text, password.text);

			//var authManager = await AuthManager.GetInstanceAsync();
			//var loginInfo = new LoginInfo()
			//{
			//	email = email.text,
			//	password = password.text
			//};
			//var userinfo = await authManager.Login(loginInfo);
			//if (errorType != 0)
			//{
			//	OutputError();
			//	return;
			//}

			//if (userinfo == null)
			//	return;
			//if (rememberMe.isOn)
			//	sessionSaver.SaveAuth(userinfo, true);
			//else
			//	sessionSaver.ClearUser();

			//GameController.SaveUserInfo(userinfo);
			//StartGame();
		}

		private void LoginRequest()
		{
			List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();

			paramsList.Add(new KeyValuePair<string, object>("email", email.text));
			paramsList.Add(new KeyValuePair<string, object>("password", password.text));

			it.Managers.NetworkManager.Request("/auth/login", paramsList, (result) =>
			{
#if UNITY_EDITOR
			it.Logger.Log("LOGIN RESPONSE " + result);
#endif

			//var userinfo = (AuthData)it.Helpers.ParserHelper.Parse(typeof(AuthData), JSON.ParseString(result));
				var userinfo = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthData>(result);
				if (errorType != 0)
				{
					OutputError();
					return;
				}

			//it.Managers.NetworkManager.Token = userinfo.

			//if (userinfo == null)
			//	return;

			//if (rememberMe.isOn)
			//	sessionSaver.SaveAuth(userinfo, true);
			//else
			//	sessionSaver.ClearUser();
			//GameController.SaveUserInfo(userinfo);
			//StartGame();

		},
		 (error) =>
		 {
			 it.Logger.LogError("Login error " + error + " | Request: " + "/auth/login");
			 OutputError();
			 return;
		 });
		}

		private void RequestMe()
		{

		}

		private void OutputError()
		{
			SplashScreen.SetActive(false);
			btnLogin.interactable = true;
			errorText.text = $"{(errorTypes.ContainsKey(errorType) ? errorTypes[errorType] : errorTypes[403])}";
		}

		private void StartGame()
		{

			//SocketClient.Instance.Init();
			SceneLoader.LoadScene(SceneType.Tables);
		}

		private bool CheckEmptyFields()
		{
			return string.IsNullOrWhiteSpace(email.text) || string.IsNullOrWhiteSpace(password.text);
		}
		public void ResetPassword()
		{
			Application.OpenURL("https://www.google.com/");
		}
	}
}