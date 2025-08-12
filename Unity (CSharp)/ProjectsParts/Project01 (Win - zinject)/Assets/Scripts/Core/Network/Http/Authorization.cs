using System;
using System.Collections.Generic;
using System.Threading;
using com.ootii.Messages;
using Core.Configs;
using Core.Configs.Consts;
using Core.Materials.Parsing;
using Core.Messages;
using Core.Network.Api.Consts;
using Core.Network.Http.Consts;
using Core.Options;
using Core.User;
using Cysharp.Threading.Tasks;
using Leguar.TotalJSON;
using Debug = Core.Logging.Debug;

namespace Core.Network.Http
{
	/// <summary>
	/// Устаревшее название - "RestManager"
	/// </summary>
	public class Authorization : IAuthorization, IDisposable
	{
		private readonly IConfig _config;
		private readonly IApplicationOptions _options;
		private readonly IApplicationOptionsSetter _optionsSetter;
		private readonly IUserProfileSetter _userProfileSetter;
		private readonly IHttpBaseUrl _baseUrl;
		private readonly IHttpRequest _request;
		private readonly IMaterialDataParsingHelper _parsingHelper;
		private readonly CancellationTokenSource _disposeCancellationTokenSource;

		private string _login;
		private string _password;
		private string _role;
		
		public Authorization(IConfig config, 
			IApplicationOptions options, 
			IApplicationOptionsSetter optionsSetter, 
			IUserProfileSetter userProfileSetter, 
			IHttpBaseUrl baseUrl, 
			IHttpRequest request, 
			IMaterialDataParsingHelper parsingHelper)
		{
			_config = config;
			_options = options;
			_optionsSetter = optionsSetter;
			_userProfileSetter = userProfileSetter;
			_baseUrl = baseUrl;
			_request = request;
			_parsingHelper = parsingHelper;
			_disposeCancellationTokenSource = new CancellationTokenSource();
		}
		
		public void Authorize(string login = null, string password = null, string role = null)
		{
			if (_disposeCancellationTokenSource.IsCancellationRequested)
				return;
			
			_login = !string.IsNullOrEmpty(login) ? login : _config.GetValue(ConfigKey.Login);
			_password = !string.IsNullOrEmpty(password) ? password : _config.GetValue(ConfigKey.Password);
			_role = !string.IsNullOrEmpty(role) ? role : "VideoWall";
			
			var parameters = new List<(string, object)>
			{
				(OutgoingAuthDataKey.Username, _login),
				(OutgoingAuthDataKey.Password, _password),
				(OutgoingAuthDataKey.Role, _role)
			};
			
			_request.Request(RestApiUrl.Login, 
				parameters, 
				result => OnAuthorizationCompleteAsync(result).Forget(),
				error => OnAuthorizationFailureAsync(error).Forget());
		}
		
		public void Dispose()
		{
			if (!_disposeCancellationTokenSource.IsCancellationRequested)
			{
				_disposeCancellationTokenSource.Cancel();
				_disposeCancellationTokenSource.Dispose();
			}
			
			_login = null;
			_password = null;
			_role = null;
		}
		
		private async UniTaskVoid OnAuthorizationCompleteAsync(string result)
		{
			try
			{
				var cancellationToken = _disposeCancellationTokenSource.Token;
				
				if (_options.IsManagersLogEnabled)
					Debug.Log($"Network auth data: {result}");
				
				await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
				{
					await UniTask.SwitchToThreadPool();
					
					cancellationToken.ThrowIfCancellationRequested();
					
					var json = JSON.ParseString(result);
					var token = json.GetString(IncomingAuthDataKey.Token);
					var user = json.GetJSON(IncomingAuthDataKey.User);
					
					_optionsSetter.ServerToken = token;
					
					_userProfileSetter.Id = user.GetInt(IncomingAuthDataKey.Id);
					_userProfileSetter.FirstName = user.GetString(IncomingAuthDataKey.Firstname);
					_userProfileSetter.LastName = user.GetString(IncomingAuthDataKey.Lastname);
					_userProfileSetter.AvatarUrl = user.GetString(IncomingAuthDataKey.Avatar);
				}
				
				MessageDispatcher.SendMessage(MessageType.AuthorizationConfirm);
			}
			catch (Exception exception) when (exception is not OperationCanceledException)
			{
				Debug.LogException(exception);
			}
			finally
			{
				if (Thread.CurrentThread.IsBackground)
					await UniTask.SwitchToMainThread();
			}
		}
		
		private async UniTaskVoid OnAuthorizationFailureAsync(string error)
		{
			try
			{
				var cancellationToken = _disposeCancellationTokenSource.Token;
				
				cancellationToken.ThrowIfCancellationRequested();
				
				Debug.LogError($"Login error: {error}. Request: {_baseUrl.ServerApi}{RestApiUrl.Login}");
				
				if (error.Contains("Could not resolve host"))
				{
					MessageDispatcher.SendMessageData(MessageType.NetworkError, NetworkErrorStatus.ServerDoesNotExist);
				}
				else
				{
					var data = _parsingHelper.Parse<NetworkError>(JSON.ParseString(error));
					var status = data.Status;
					
					switch (status)
					{
						case NetworkErrorStatus.AuthorizationError:
						case NetworkErrorStatus.UserIsLocked:
						case NetworkErrorStatus.ServerDoesNotExist:
							MessageDispatcher.SendMessageData(MessageType.NetworkError, status);
							break;
						default:
							await UniTask.Delay(TimeSpan.FromSeconds(2.0), cancellationToken: cancellationToken);
							Authorize(_login, _password, _role);
							break;
					}
				}
			}
			catch (Exception exception) when (exception is not OperationCanceledException)
			{
				Debug.LogException(exception);
			}
		}
	}
}