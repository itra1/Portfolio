using System;
using System.Collections.Generic;
using System.Text;
using Core.Network.Http.Data;
using Core.Network.Http.Utils;
using Core.Options;
using Cysharp.Threading.Tasks;
using Leguar.TotalJSON;
using UnityEngine;
using UnityEngine.Networking;
using Debug = Core.Logging.Debug;

namespace Core.Network.Http
{
	public class UnityWebRequestProvider : IHttpRequest, IDisposable
	{
		private readonly IApplicationOptions _options;
		private readonly IHttpBaseUrl _baseUrl;
		private readonly IDictionary<UnityWebRequest, (Action<DownloadHandler>, Action<string>)> _callbacksByRequest;

		public UnityWebRequestProvider(IApplicationOptions options, IHttpBaseUrl baseUrl)
		{
			_options = options;
			_baseUrl = baseUrl;
			_callbacksByRequest = new Dictionary<UnityWebRequest, (Action<DownloadHandler>, Action<string>)>();
		}

		public void Request(string url,
	Action<string> onCompleted = null,
	Action<string> onFailure = null) =>
	Request(url, HttpMethodType.Get, default(string), onCompleted, onFailure);

		public void Request(string url,
			IList<(string, object)> parameters,
			Action<string> onCompleted = null,
			Action<string> onFailure = null) =>
			Request(url, HttpMethodType.Post, parameters, onCompleted, onFailure);

		public void Request(string url,
			string rawData,
			Action<string> onCompleted = null,
			Action<string> onFailure = null) =>
			Request(url, HttpMethodType.Put, rawData, onCompleted, onFailure);

		public void Request(string url,
			HttpMethodType methodType,
			Action<string> onCompleted = null,
			Action<string> onFailure = null) =>
			Request(url, methodType, default(string), onCompleted, onFailure);

		public void Request(string url,
			HttpMethodType methodType,
			IList<(string, object)> parameters,
			Action<string> onCompleted = null,
			Action<string> onFailure = null)
		{
			var json = new JSON();

			if (parameters is { Count: > 0 })
			{
				foreach (var (key, value) in parameters)
					json.Add(key, value);
			}

			Request(url, methodType, json.CreateString(), onCompleted, onFailure);
		}

		public void Request(string url,
			HttpMethodType methodType,
			string rawData,
			Action<string> onCompleted = null,
			Action<string> onFailure = null)
		{
			var request = CreateRequest(_baseUrl.ServerApi + url,
				methodType,
				response => onCompleted?.Invoke(response.text),
				onFailure);

			if (request == null)
				return;

			if (_options.IsManagersLogEnabled)
				Debug.Log($"Request: {request.uri.AbsoluteUri}" + (!string.IsNullOrEmpty(rawData) ? $". Data: {rawData}" : string.Empty));

			request.SetRequestHeader("Content-Type", "application/json");

			if (!string.IsNullOrEmpty(_options.ServerToken))
				request.SetRequestHeader("Authorization", $"Bearer {_options.ServerToken}");

			if (!string.IsNullOrEmpty(rawData))
				request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(rawData));

			request.downloadHandler = new DownloadHandlerBuffer();

			SendRequestAsync(request).Forget();
		}

#if UNITY_EDITOR
		public void Request(Uri uri,
			HttpMethodType methodType,
			IList<(string, object)> parameters,
			string token,
			Action<string> onCompleted = null,
			Action<string> onFailure = null)
		{
			var isManagersLogEnabled = _options.IsManagersLogEnabled;

			var request = CreateRequest(uri.AbsoluteUri,
				methodType,
				response => onCompleted?.Invoke(response.text),
				onFailure);

			if (request == null)
				return;

			if (isManagersLogEnabled)
				Debug.Log($"Request: {request.uri.AbsoluteUri}");

			request.SetRequestHeader("Content-Type", "application/json");

			if (!string.IsNullOrEmpty(token))
				request.SetRequestHeader("Authorization", $"Bearer {token}");

			if (parameters is { Count: > 0 })
			{
				var json = new JSON();

				foreach (var (key, value) in parameters)
					json.Add(key, value);

				var rawData = json.CreateString();

				if (!string.IsNullOrEmpty(rawData))
					request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(rawData));
			}

			request.downloadHandler = new DownloadHandlerBuffer();

			SendRequestAsync(request).Forget();
		}
#endif

		public void Request(string url,
			HttpMethodType methodType,
			byte[] rawData,
			Action<string> onCompleted = null,
			Action<string> onFailure = null)
		{
			var request = CreateRequest(_baseUrl.ServerApi + url,
				methodType,
				response => onCompleted?.Invoke(response.text),
				onFailure);

			if (request == null)
				return;

			if (_options.IsManagersLogEnabled)
				Debug.Log($"Request: {request.uri.AbsoluteUri}");

			request.SetRequestHeader("Content-Type", "application/octet-stream");

			if (!string.IsNullOrEmpty(_options.ServerToken))
				request.SetRequestHeader("Authorization", $"Bearer {_options.ServerToken}");

			if (rawData is { Length: > 0 })
				request.uploadHandler = new UploadHandlerRaw(rawData);

			request.downloadHandler = new DownloadHandlerBuffer();

			SendRequestAsync(request).Forget();
		}

		public void RequestBytes(string url,
			HttpMethodType methodType,
			Action<byte[]> onCompleted = null,
			Action<string> onFailure = null)
		{
			var request = CreateRequest(_baseUrl.ServerApi + url,
				methodType,
				response => onCompleted?.Invoke(response.data),
				onFailure);

			if (request == null)
				return;

			if (_options.IsManagersLogEnabled)
				Debug.Log($"Request: {request.uri.AbsoluteUri}");

			request.SetRequestHeader("Content-Type", "application/octet-stream");

			if (!string.IsNullOrEmpty(_options.ServerToken))
				request.SetRequestHeader("Authorization", $"Bearer {_options.ServerToken}");

			request.downloadHandler = new DownloadHandlerBuffer();

			SendRequestAsync(request).Forget();
		}

		public void RequestTexture2D(string url,
			HttpMethodType methodType,
			bool readable,
			Action<Texture2D> onCompleted = null,
			Action<string> onFailure = null)
		{
			var request = CreateRequest(_baseUrl.ServerApi + url,
				methodType,
				response => onCompleted?.Invoke(((DownloadHandlerTexture) response).texture),
				onFailure);

			if (request == null)
				return;

			if (_options.IsManagersLogEnabled)
				Debug.Log($"Request: {request.uri.AbsoluteUri}");

			request.SetRequestHeader("Content-Type", "application/octet-stream");

			if (!string.IsNullOrEmpty(_options.ServerToken))
				request.SetRequestHeader("Authorization", $"Bearer {_options.ServerToken}");

			request.downloadHandler = new DownloadHandlerTexture(readable);

			SendRequestAsync(request).Forget();
		}

		public void Dispose()
		{
			foreach (var request in _callbacksByRequest.Keys)
			{
				request.Abort();
				request.Dispose();
			}

			_callbacksByRequest.Clear();
		}

		private UnityWebRequest CreateRequest(string url,
			HttpMethodType methodType,
			Action<DownloadHandler> onCompleted = null,
			Action<string> onFailure = null)
		{
			var method = methodType.Stringify();

			if (method == null)
			{
				Debug.LogError($"Unknown HTTP method type detected: {methodType}");
				return null;
			}

			var request = new UnityWebRequest(url, method);

			request.timeout = (int) TimeSpan.FromMinutes(10.0).TotalSeconds;

			request.disposeCertificateHandlerOnDispose = true;
			request.disposeDownloadHandlerOnDispose = true;
			request.disposeUploadHandlerOnDispose = true;

			if (_options.IsLocalSslIgnored)
				request.certificateHandler = new ForceAcceptAllCertificates();

			_callbacksByRequest.Add(request, (onCompleted, onFailure));

			return request;
		}

		private async UniTaskVoid SendRequestAsync(UnityWebRequest request)
		{
			var operation = request.SendWebRequest();

			try
			{
				await UniTask.WaitUntil(() => operation.isDone);
			}
			catch
			{
				// ignored
			}

			HandleRequestResult(request);
		}

		private void HandleRequestResult(UnityWebRequest request)
		{
			if (!_callbacksByRequest.Remove(request, out var callbacks))
				return;

			var (onCompleted, onFailure) = callbacks;

			var response = request.downloadHandler;

			try
			{
				switch (request.result)
				{
					case UnityWebRequest.Result.Success:
						{
							if (response.isDone)
							{
								if (_options.IsManagersLogEnabled)
								{
									if (response is DownloadHandlerTexture)
									{
										Debug.Log("Response: [texture2d]");
									}
									else
									{
										var bytes = response.data;

										if (bytes is { Length: > 0 })
										{
											Debug.Log("Response: [bytes]");
										}
										else
										{
											var text = response.text;

											if (!string.IsNullOrEmpty(text))
												Debug.Log($"Response: {text}");
										}
									}
								}

								onCompleted?.Invoke(response);
							}
							else
							{
								if (request.error != null)
									onFailure?.Invoke(request.error);

								Debug.LogError($"Request is completed successfully, but the server sent an error. Message: {request.error}. Status code: {request.responseCode}. URL: {request.uri.AbsoluteUri}");
							}

							break;
						}
					case UnityWebRequest.Result.DataProcessingError:
						{
							onFailure?.Invoke(request.error);
							Debug.LogError($"Request is completed with data processing error: {request.error}. Status code: {request.responseCode}. URL: {request.uri.AbsoluteUri}");
							break;
						}
					case UnityWebRequest.Result.ProtocolError:
						{
							onFailure?.Invoke(request.error);
							Debug.LogError($"Request is completed with protocol error: {request.error}. Status code: {request.responseCode}. URL: {request.uri.AbsoluteUri}");
							break;
						}
					case UnityWebRequest.Result.ConnectionError:
						{
							onFailure?.Invoke(request.error);
							Debug.LogError($"Request is completed with connection error: {request.error}. Status code: {request.responseCode}. URL: {request.uri.AbsoluteUri}");
							break;
						}
					default:
						{
							Debug.LogError($"Unknown request result. Status code: {request.responseCode}. URL: {request.uri.AbsoluteUri}");
							break;
						}
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}

			request.Dispose();
		}
	}
}