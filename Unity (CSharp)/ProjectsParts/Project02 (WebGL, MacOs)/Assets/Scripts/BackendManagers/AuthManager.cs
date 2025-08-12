using UnityEngine;
using Leguar.TotalJSON;
using System.Collections.Generic;
using System;
using it.Api;

public class AuthManager : Singleton<AuthManager>
{

	public void Login(string email, string password, UnityEngine.Events.UnityAction<AuthData> OnComplete, UnityEngine.Events.UnityAction<string> OnError)
	{

		List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();

		paramsList.Add(new KeyValuePair<string, object>("email", email));
		paramsList.Add(new KeyValuePair<string, object>("password", password));

		it.Managers.NetworkManager.Request("/auth/login", paramsList, (result) =>
		{
#if UNITY_EDITOR
			it.Logger.Log("LOGIN RESPONSE " + result);
#endif

			//var userinfo = (AuthData)it.Helpers.ParserHelper.Parse(typeof(AuthData), JSON.ParseString(result));
			var userinfo = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthData>(result);

			OnComplete?.Invoke(userinfo);
		},
	 (error) =>
	 {
		 it.Logger.LogError("Login error " + error + " | Request: " + "/auth/login");
		 OnError?.Invoke(error);
		 return;
	 });
	}

	//public async Task<AuthData> Login(LoginInfo info)
	//{
	//	RestApiClient.Instance.SetToken("");
	//	var authInfo = await RestApiClient.UserApi.Login(info);
	//	if (authInfo != null) RestApiClient.Instance.SetToken(authInfo.access_token);

	//	var userData = await RestApiClient.UserApi.GetUserData();
	//	if (authInfo != null && userData != null)
	//	{
	//		RefreshTokenDelay(authInfo.refresh_in - 120);
	//		return new AuthData(authInfo, userData.user);
	//	}

	//	if (authInfo != null) Refresh(authInfo.access_token);
	//	return null;
	//}

	public void Refresh(string authToken, UnityEngine.Events.UnityAction<AuthData> OnComplete, UnityEngine.Events.UnityAction<string> OnError)
	{
		it.Managers.NetworkManager.Token = authToken;

		it.Managers.NetworkManager.Request("/auth/refresh", (result) =>
		{
#if UNITY_EDITOR
			it.Logger.Log("REFRESH RESPONSE " + result);
#endif
			//var userinfo = (AuthData)it.Helpers.ParserHelper.Parse(typeof(AuthData), JSON.ParseString(result));
			var userinfo = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthData>(result);

			OnComplete?.Invoke(userinfo);
		},
	 (error) =>
	 {
		 it.Logger.LogError("Login error " + error + " | Request: " + "/auth/login");
		 OnError?.Invoke(error);
		 return;
	 });
	}

	public static void Refresh(string authToken, Action<AuthData> callbackSuccess, Action<string> callbackFailed = null)
	{
		RestApiClient.Instance.SetToken(authToken);

		UserApi.Refresh((authInfo) =>
		{

			UserApi.GetUserData((userData) =>
			{

				if (authInfo != null && userData != null)
				{
					RestApiClient.Instance.SetToken(authInfo.Result.access_token);
					AuthManager.Instance.RefreshTokenDelay(authInfo.Result.refresh_in - 120);
					callbackSuccess?.Invoke(new AuthData(authInfo.Result, userData.user));
				}

			}, null);


		});


		//var authInfo = await RestApiClient.UserApi.Refresh();
		//if (authInfo != null) RestApiClient.Instance.SetToken(authInfo.AccessToken);

		//var userData = await RestApiClient.UserApi.GetUserData();

		//if (authInfo != null && userData != null)
		//{
		//	RestApiClient.Instance.SetToken(authInfo.AccessToken);
		//	RefreshTokenDelay(authInfo.RefreshIn - 120);
		//	return new AuthData(authInfo, userData.user);
		//}

		//return null;
	}

	public void LoginByToken(string authToken, UnityEngine.Events.UnityAction<AuthData> OnComplete, UnityEngine.Events.UnityAction<string> OnError)
	{
		it.Managers.NetworkManager.Token = authToken;

		it.Managers.NetworkManager.Request("/auth/me", (result) =>
		{
#if UNITY_EDITOR
			it.Logger.Log("LoginByToken RESPONSE " + result);
#endif
			//var userinfo = (AuthData)it.Helpers.ParserHelper.Parse(typeof(AuthData), JSON.ParseString(result));
			var userinfo = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthData>(result);

			//var userResponse = (UserResponse)it.Helpers.ParserHelper.Parse(typeof(UserResponse), JSON.ParseString(result));
			var userResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<UserResponse>(result);

			OnComplete?.Invoke(userinfo);
		},
	 (error) =>
	 {
		 it.Logger.LogError("LoginByToken error " + error + " | Request: " + "/auth/login");
		 OnError?.Invoke(error);
		 return;
	 });

	}

	public void LoginByToken(string authToken, Action<UserResponse> callbackSuccess)
	{
		RestApiClient.Instance.SetToken(authToken);

		UserApi.GetUserData((result)=> {
			callbackSuccess?.Invoke(result);
		});
	}

	public void RefreshTokenDelay(int timeRefresh)//in sec
	{
		if (timeRefresh < 0) timeRefresh = 10;

		DG.Tweening.DOVirtual.DelayedCall(1, () =>
		{
			RefreshToken();

		});

		//await Task.Delay(timeRefresh * 1000);
		//RefreshToken();
	}

	public void RefreshToken()
	{
		UserApi.Refresh((authInfo) =>
		{
			RestApiClient.Instance.SetToken(authInfo.Result.access_token);
			it.Logger.Log("new token:\n" + authInfo.Result.access_token);
			RefreshTokenDelay(authInfo.Result.refresh_in - 120);

		});


		//var authInfo = await RestApiClient.UserApi.Refresh();
		//if (authInfo != null)
		//{
		//	RestApiClient.Instance.SetToken(authInfo.AccessToken);
		//	it.Logger.Log("new token:\n" + authInfo.AccessToken);
		//	RefreshTokenDelay(authInfo.RefreshIn - 120);
		//};
	}

	public void Logout(Action<bool> callbackSuccess)
	{
		UserApi.Logout(callbackSuccess);
	}
}