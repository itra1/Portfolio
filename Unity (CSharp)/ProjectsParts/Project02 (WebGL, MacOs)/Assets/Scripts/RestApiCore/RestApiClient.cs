using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using it.Api;

public class RestApiClient : Singleton<RestApiClient>
{
	private const string AuthHeader = "AUTHORIZATION";

	private readonly TableApi _tableApi;
	private readonly SupportChatUserApi _supportChatUserApi;

	private bool _isAuth;

	public static TableApi TableApi => Instance._tableApi;

	public static SupportChatUserApi SupportChatUserApi => Instance._supportChatUserApi;

	private static TaskCompletionSource<string> _authTcs = new TaskCompletionSource<string>();

	public static Task<string> AuthTask = _authTcs.Task;

	private string _token;

	public RestApiClient()
	{

		_tableApi = new TableApi();
		_supportChatUserApi = new SupportChatUserApi();
	}

	public void SetToken(string token)
	{
		_token = token;
		SetHttpHeader(AuthHeader, $"Bearer {token}");
		MarkAsAuth(token);
	}


	public string GetToken()
	{
		return _token;
	}

	private void SetLanguage(string lang)
	{
		SetHttpHeader("lang", lang);
	}

	private void SetHttpHeader(string key, string value)
	{
		//RestClient.DefaultRequestHeaders.Remove(key);
		//RestClient.DefaultRequestHeaders.Add(key, value);
	}

	private void MarkAsAuth(string token)
	{
		if (_isAuth)
			return;
		_isAuth = true;
		if (!_authTcs.TrySetResult(token))
		{
			_authTcs = new TaskCompletionSource<string>();
			_authTcs.SetResult(token);
		}
	}
}