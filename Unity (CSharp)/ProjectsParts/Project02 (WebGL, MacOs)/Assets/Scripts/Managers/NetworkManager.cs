//#if UNITY_WEBGL
//#define UNITY_WebRequest
//#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leguar.TotalJSON;
using System.Text;
using System;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Sett = it.Settings;
using it.Network.Rest;
using UnityEngine.UI;
using BestHTTP;

#if !UNITY_WebRequest

#endif

namespace it.Managers
{

	public enum RequestType
	{
		get,
		post,
		put,
		delete,
		patch,
		head
	}
	public struct AuthResult
	{
		public string token;
	}

	public class NetworkManager : Singleton<NetworkManager>
	{
		private string _token;
		public static string Token
		{
			get => Instance._token;
			set
			{
				Instance._token = value;
				RestApiClient.Instance.SetToken(value);
			}
		}
#if !BESTHTTP_DISABLE_CACHING
		private static bool allDownloadedFromLocalCache = true;
#endif

		public static bool ReadyQueue => _requestQueue.Count > 0;
		public static void ClearQueue()
		{
			_requestQueue.Clear();
		}

		Dictionary<string, Texture2D> _cacheTexture = new Dictionary<string, Texture2D>();

		static private Queue<RequestsQueue> _requestQueue = new Queue<RequestsQueue>();

		// WebRequestBestHttp(RequestType requestType, string url, string json, System.Action<string> OnComplete = null, System.Action<string> OnFalse = null)

		class RequestsQueue
		{
			public RequestType RequestType;
			public string Url;
			public string Json;
			public List<KeyValuePair<string, string>> Addheaders;
			public System.Action<string> OnComplete;
			public System.Action<string> OnFalse;
		}

		public static void UpdateToken()
		{
			Instance.StartCoroutine(Instance.QueueCor());
		}

		IEnumerator QueueCor()
		{
			while (_requestQueue.Count > 0)
			{
				var itm = _requestQueue.Dequeue();
				//WebRequestBestHttp(itm.RequestType, itm.Url, itm.Json, itm.OnComplete, itm.OnFalse);

#if UNITY_WebRequest
				WebRequestUnity(itm.RequestType, itm.Url, itm.Json, itm.OnComplete, itm.OnFalse);
#else
				WebRequestBestHttp(itm.RequestType, itm.Url, null, itm.Json, itm.Addheaders, itm.OnComplete, itm.OnFalse);
#endif

				yield return new WaitForSeconds(0.1f);
			}
		}


		public static void Request(string url, List<KeyValuePair<string, object>> paramsData, System.Action<string> OnComplete = null, System.Action<string> OnFalse = null)
		{
			WebRequestBestHttp(RequestType.post, url, paramsData, "", null, OnComplete, OnFalse);
		}
		public static void Request(string url, System.Action<string> OnComplete = null, System.Action<string> OnFalse = null)
		{
			WebRequestBestHttp(RequestType.get, url, null, "", null, OnComplete, OnFalse);
		}
		public static void Request(string url, List<KeyValuePair<string, string>> addHeaders, System.Action<string> OnComplete = null, System.Action<string> OnFalse = null)
		{
			WebRequestBestHttp(RequestType.get, url, null, "", addHeaders, OnComplete, OnFalse);
		}
		public static void Request(string url, string stringData, System.Action<string> OnComplete = null, System.Action<string> OnFalse = null)
		{
			WebRequestBestHttp(RequestType.put, url, null, stringData, null, OnComplete, OnFalse);
		}
		public static void Request(RequestType type, string url, System.Action<string> OnComplete = null, System.Action<string> OnFalse = null)
		{
			WebRequestBestHttp(type, url, null, null, OnComplete, OnFalse);
		}
		public static void Request(RequestType type, string url, JValue json, System.Action<string> OnComplete = null, System.Action<string> OnFalse = null)
		{
			//WebRequestBestHttp(type, url, json.CreateString(), OnComplete, OnFalse);
#if UNITY_WebRequest
			Instance.WebRequestUnity(type, url, json.CreateString(), OnComplete, OnFalse);
#else
			WebRequestBestHttp(type, url, json.CreateString(), null, OnComplete, OnFalse);
#endif
		}
		public static void Request(RequestType type, string url, string json, System.Action<string> OnComplete = null, System.Action<string> OnFalse = null)
		{
			//WebRequestBestHttp(type, url, json, OnComplete, OnFalse);

#if UNITY_WebRequest
			Instance.WebRequestUnity(type, url, json, OnComplete, OnFalse);
#else
			WebRequestBestHttp(type, url, json, null, OnComplete, OnFalse);
#endif
		}
		public static void Request(RequestType type, string url, Leguar.TotalJSON.JSON json, System.Action<string> OnComplete = null, System.Action<string> OnFalse = null)
		{
			//WebRequestBestHttp(type, url, json.CreateString(), OnComplete, OnFalse);
#if UNITY_WebRequest
			Instance.WebRequestUnity(type, url, json.CreateString(), OnComplete, OnFalse);
#else
			WebRequestBestHttp(type, url, json.CreateString(), null, OnComplete, OnFalse);
#endif
		}
		public static void Request(RequestType type, string url, List<KeyValuePair<string, object>> paramsData, System.Action<string> OnComplete = null, System.Action<string> OnFalse = null)
		{
			WebRequestBestHttp(type, url, paramsData, null, null, OnComplete, OnFalse);
		}
		public static void Request(RequestType type, string url, List<KeyValuePair<string, object>> paramsData, string stringData, System.Action<string> OnComplete = null, System.Action<string> OnFalse = null)
		{
			WebRequestBestHttp(type, url, paramsData, stringData, null, OnComplete, OnFalse);
		}
		private static void WebRequestBestHttp(RequestType requestType, string url, List<KeyValuePair<string, object>> paramsData, string stringData, List<KeyValuePair<string, string>> addHeaders = null, System.Action<string> OnComplete = null, System.Action<string> OnFalse = null)
		{

			JSON sendData = new JSON();

			if (paramsData != null && paramsData.Count > 0)
			{
				foreach (var itm in paramsData)
				{
					sendData.Add(itm.Key, itm.Value);
				}
			}

#if UNITY_WebRequest
			Instance.WebRequestUnity(requestType, url, sendData.CreateString(), OnComplete, OnFalse);
#else
			WebRequestBestHttp(requestType, url, sendData.CreateString(), addHeaders, OnComplete, OnFalse);
#endif

		}
		public void WebRequestUnity(RequestType requestType, string url, string json, System.Action<string> OnComplete = null, System.Action<string> OnFalse = null)
		{
			StartCoroutine(WebRequestUnityCor(requestType, url, json, OnComplete, OnFalse));
		}
		private IEnumerator WebRequestUnityCor(RequestType requestType, string url, string json, System.Action<string> OnComplete = null, System.Action<string> OnFalse = null)
		{

			UnityWebRequest request = null;

			string requestUrl = url.Contains("https://") ? url : ServerManager.ServerApi + url;

			var rawData = Encoding.UTF8.GetBytes(json);

			switch (requestType)
			{
				case RequestType.put:
					request = UnityWebRequest.Put(requestUrl, rawData);
					break;
				case RequestType.patch:
					request = UnityWebRequest.Head(requestUrl);
					break;
				case RequestType.delete:
					request = UnityWebRequest.Delete(requestUrl);
					break;
				case RequestType.post:
					request = UnityWebRequest.Post(requestUrl, json);
					var array = Encoding.UTF8.GetBytes(json);
					request.uploadHandler = new UploadHandlerRaw(array);
					request.uploadHandler.contentType = "application/json";
					break;
				case RequestType.get:
				default:
					request = UnityWebRequest.Get(requestUrl);
					break;
			}
			//request.SetRequestHeader("Access-Control-Allow-Headers", "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
			//request.SetRequestHeader("Access-Control-Allow-Credentials", "true");
			//request.SetRequestHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
			//request.SetRequestHeader("Access-Control-Allow-Origin", "*");
			request.SetRequestHeader("Content-Type", "application/json");
			//request.SetRequestHeader("Origin", "https://game.garillapoker.com");

			if (!string.IsNullOrEmpty(Token))
				request.SetRequestHeader("Authorization", "Bearer " + Token);

			it.Logger.Log("Request " + requestUrl + " Send " + json + (!string.IsNullOrEmpty(Token) ? ("Token =" + Token) : ""));
			yield return request.SendWebRequest();

			var resultCode = request.responseCode;

			switch (request.result)
			{
				case UnityWebRequest.Result.Success:
					switch (resultCode)
					{
						case 200:
							OnComplete?.Invoke(request.downloadHandler.text);
							break;
						case 401:

							if (!string.IsNullOrEmpty(Token))
							{
								//var errors = it.Helpers.ParserHelper.Parse<ErrorHttp>(JSON.ParseString(request.error));
								var errors = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorHttp>(request.error);

								// ошибка уникальной сессии
								if (errors.error == "single_session_error")
								{
									UserController.Instance.AnotherPlayerAuthorization();
									yield break;
								}

								bool submitAuth = _requestQueue.Count <= 0;
								_requestQueue.Enqueue(new RequestsQueue()
								{
									RequestType = requestType,
									Url = url,
									Json = json,
									OnComplete = OnComplete,
									OnFalse = OnFalse
								});
								if (submitAuth)
								{
									DG.Tweening.DOVirtual.DelayedCall(0.05f, () =>
									{
										it.Logger.Log("UserController.Instance = " + UserController.Instance == null);
										UserController.Instance.SessionLoss();
									});
								}
							}
							break;
						default:
							it.Logger.LogError("Request Finished with Error! " + (!string.IsNullOrEmpty(request.error) ? request.error : "No Exception"));
							OnFalse?.Invoke(request.error);
							break;
					}
					break;
				case UnityWebRequest.Result.ConnectionError:
					it.Logger.Log("Request ConnectionError! " + requestUrl);
					OnFalse?.Invoke("Request ConnectionError!");
					break;
				case UnityWebRequest.Result.ProtocolError:
					it.Logger.Log("Request ProtocolError! " + requestUrl + " : " + request.error);
					OnFalse?.Invoke("Request ProtocolError!");
					break;
				case UnityWebRequest.Result.DataProcessingError:
					it.Logger.Log("Request DataProcessingError! " + requestUrl);
					OnFalse?.Invoke("Request DataProcessingError!");
					break;
			}

			//if (!string.IsNullOrEmpty(request.error))
			//{
			//	it.Logger.LogError("Request Finished with Error! " + (!string.IsNullOrEmpty(request.error) ? request.error : "No Exception"));
			//	OnFalse?.Invoke(request.error);
			//}
			request.Dispose();
		}

#if !UNITY_WebRequest

		private static void WebRequestBestHttp(RequestType requestType, string url, string json, List<KeyValuePair<string, string>> addHeaders = null, System.Action<string> OnComplete = null, System.Action<string> OnFalse = null)
		{

			HTTPMethods met = HTTPMethods.Get;

			switch (requestType)
			{
				case RequestType.post:
					met = HTTPMethods.Post;
					break;
				case RequestType.put:
					met = HTTPMethods.Put;
					break;
				case RequestType.delete:
					met = HTTPMethods.Delete;
					break;
				case RequestType.patch:
					met = HTTPMethods.Patch;
					break;
				case RequestType.get:
				default:
					met = HTTPMethods.Get;
					break;
			}

			if (!(url.Contains("https://") || url.Contains("http://")) && !ServerManager.ExistsServers)
			{
				OnFalse?.Invoke("no server");
				return;
			}

			string requestUrl = (url.Contains("https://") || url.Contains("http://")) ? url : ServerManager.ServerApi + url;

			//if (!string.IsNullOrEmpty(Token))
			//{
			//	requestUrl += "?token=" + Token;
			//}

			Uri serverUrl = new Uri(requestUrl);

			var request = new HTTPRequest(serverUrl, met, false, (req, resp) =>
			{
				switch (req.State)
				{
					// The request finished without any problem.
					case HTTPRequestStates.Finished:
						if (resp.IsSuccess)
						{
							OnComplete?.Invoke(resp.DataAsText);
						}
						else
						{
							if (req.Exception != null)
							{
								OnFalse?.Invoke(req.Exception.Message);
							}
							else if (req.Response != null)
								it.Logger.Log(string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2} {3} ({4})",
									resp.StatusCode,
									resp.Message,
									resp.DataAsText,
									req.Uri.AbsoluteUri,
									met));
							OnFalse?.Invoke(req.Response.DataAsText);

							switch (resp.StatusCode)
							{
								case 401:
									{
										//var errors = it.Helpers.ParserHelper.Parse<ErrorHttp>(JSON.ParseString(resp.DataAsText));
										var errors = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorHttp>(resp.DataAsText);

										// ошибка уникальной сессии
										if (errors.error == "single_session_error")
										{
											UserController.Instance.AnotherPlayerAuthorization();
											return;
										}

										//var errorsRest = it.Helpers.ParserHelper.Parse<ErrorRest>(JSON.ParseString(resp.DataAsText));
										var errorsRest = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(resp.DataAsText);
										if (errorsRest.errors[0].id == "unauthorized")
										{
											bool existsToken = !string.IsNullOrEmpty(Token);
											if (string.IsNullOrEmpty(Token))
											{
												//UserController.Instance?.ClearTokenData();
												com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.UserUnauthorized);
												return;
											}

											bool submitAuth = _requestQueue.Count <= 0;
											_requestQueue.Enqueue(new RequestsQueue()
											{
												RequestType = requestType,
												Url = url,
												Json = json,
												OnComplete = OnComplete,
												OnFalse = OnFalse
											});
											if (submitAuth)
											{
												DG.Tweening.DOVirtual.DelayedCall(0.05f, () =>
												{
													it.Logger.Log("UserController.Instance = " + UserController.Instance == null);
													UserController.Instance.SessionLoss();
												});
											}

											return;
										}


										break;
									}
								case 403:
									{

										if (!string.IsNullOrEmpty(Token))
										{
											//var errors = it.Helpers.ParserHelper.Parse<ErrorRest>(JSON.ParseString(resp.DataAsText));
											var errors = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(resp.DataAsText);
											if (errors.errors[0].id == "unactive_account")
											{
												UserController.Instance.Logout();
												return;
											}
										}

										break;
									}
							}


						}
						break;

					// The request finished with an unexpected error. The request's Exception property may contain more info about the error.
					case HTTPRequestStates.Error:
						it.Logger.LogError("Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));
						OnFalse?.Invoke(req.Exception.Message);
						break;

					// The request aborted, initiated by the user.
					case HTTPRequestStates.Aborted:
						it.Logger.Log("Request Aborted!");
						OnFalse?.Invoke("Request Aborted!");
						break;

					// Connecting to the server is timed out.
					case HTTPRequestStates.ConnectionTimedOut:
						it.Logger.LogError("Connection Timed Out!");
						OnFalse?.Invoke("Connection Timed Out!");
						break;

					// The request didn't finished in the given time.
					case HTTPRequestStates.TimedOut:
						it.Logger.LogError("Processing the request Timed Out!");
						OnFalse?.Invoke("Processing the request Timed Out!");
						break;
					default:

						it.Logger.Log("State = " + req.State);
						break;
				}

			});
			it.Logger.Log("Request " + request.Uri.AbsoluteUri + " Send " + json + (!string.IsNullOrEmpty(Token) ? ("Token =" + Token) : ""));

			request.AddHeader("Content-Type", "application/json");


			if (!string.IsNullOrEmpty(Token))
			{
				request.AddHeader("Authorization", "Bearer " + Token);
			}
			if (addHeaders != null && addHeaders.Count > 0)
			{
				for (int i = 0; i < addHeaders.Count; i++)
				{
					if (request.HasHeader(addHeaders[i].Key))
						request.RemoveHeader(addHeaders[i].Key);
					request.AddHeader(addHeaders[i].Key, addHeaders[i].Value);
				}
			}
			//else

			//request.AddHeader("Authorization", "Basic MEwyY1VPN29QUlhBOTdkdjo5cWF0ejgzRUp6cXlwaW9K");

			//request.Context.Add ("body", json);
			if (json != null)
				request.RawData = Encoding.UTF8.GetBytes(json);

			//request.UseAlternateSSL = true;

			request.Send();

		}


		public static void WebDownloadBestHttp(string url, System.Action<string> OnComplete = null, System.Action<byte[]> OnDownload = null, System.Action<string> OnFalse = null)
		{

			var request = new HTTPRequest(new Uri(ServerManager.ServerApi + url), (req, resp) =>
			{

				switch (req.State)
				{
					// The request finished without any problem.
					case HTTPRequestStates.Finished:
						if (resp.IsSuccess)
						{
							OnComplete?.Invoke(resp.DataAsText);
						}
						else
						{
							OnFalse?.Invoke(req.Exception.Message);
							it.Logger.Log(string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2} {3}",
								resp.StatusCode,
								resp.Message,
								resp.DataAsText,
								req.Uri.AbsoluteUri));
						}
						break;

					// The request finished with an unexpected error. The request's Exception property may contain more info about the error.
					case HTTPRequestStates.Error:
						OnFalse?.Invoke(req.Exception.Message);
						it.Logger.LogError("Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));
						break;

					// The request aborted, initiated by the user.
					case HTTPRequestStates.Aborted:
						OnFalse?.Invoke("Request Aborted!");
						it.Logger.Log("Request Aborted!");
						break;

					// Connecting to the server is timed out.
					case HTTPRequestStates.ConnectionTimedOut:
						OnFalse?.Invoke("Connection Timed Out!");
						it.Logger.LogError("Connection Timed Out!");
						break;

					// The request didn't finished in the given time.
					case HTTPRequestStates.TimedOut:
						OnFalse?.Invoke("Processing the request Timed Out!");
						it.Logger.LogError("Processing the request Timed Out!");
						break;
				}

			});
			request.OnStreamingData += (req, res, dataFragment, dataFragmentLength) =>
			{

				OnDownload?.Invoke(dataFragment);
				return false;
			};
			it.Logger.Log("Request " + request.Uri.AbsoluteUri);

			request.AddHeader("Content-Type", "application/json");

			if (!string.IsNullOrEmpty(Token))
			{
				request.AddHeader("Authorization", "Bearer " + Token);
			}

			request.Send();

		}
#endif
		IEnumerator WebRequest(RequestType request, string url, WWWForm data, string stringData, System.Action<string> OnComplete = null, System.Action<string> OnFalse = null)
		{

			UnityWebRequest www = null;

			switch (request)
			{
				case RequestType.post:
					www = UnityWebRequest.Post(ServerManager.ServerApi + url, data);
					break;
				case RequestType.put:
					www = UnityWebRequest.Put(ServerManager.ServerApi + url, stringData);
					break;
				case RequestType.delete:
					www = UnityWebRequest.Delete(ServerManager.ServerApi + url);
					break;
				case RequestType.get:
				default:
					www = UnityWebRequest.Get(ServerManager.ServerApi + url);
					break;
			}

			it.Logger.Log("Request " + www.url);

			www.SetRequestHeader("Content-Type", "application/json");

			if (!string.IsNullOrEmpty(Token))
			{
				www.SetRequestHeader("Authorization", "Bearer " + Token);
			}

			yield return www.SendWebRequest();

			if (www.result == UnityWebRequest.Result.ConnectionError)
			{
				it.Logger.Log("Request err" + www.error);
				OnFalse?.Invoke(www.error);
			}
			else
			{
				if (www != null && www.downloadHandler != null)
				{
					it.Logger.Log("Request ok" + www.downloadHandler.text);
					OnComplete?.Invoke(www.downloadHandler.text);
				}
			}
		}
		public void CheckExistsFile(string url, System.Action OnComplete = null, System.Action<string> OnFalse = null)
		{
			StartCoroutine(CheckExistsFileRequest(url, OnComplete, OnFalse));
		}
		IEnumerator CheckExistsFileRequest(string url, System.Action OnComplete = null, System.Action<string> OnFalse = null)
		{
			var www = UnityWebRequest.Head(url);
			www.timeout = 1200;

			it.Logger.Log("Request " + www.url);

			www.SetRequestHeader("Content-Type", "application/json");

			if (!string.IsNullOrEmpty(Token))
			{
				www.SetRequestHeader("Authorization", "Bearer " + Token);
			}

			yield return www.SendWebRequest();

			if (www.result == UnityWebRequest.Result.ConnectionError)
			{
				it.Logger.Log("Request err" + www.error);
				OnFalse?.Invoke(www.error);
			}
			else
			{
				if (www != null && www.downloadHandler != null)
				{
					it.Logger.Log("Request ok" + www.downloadHandler.text);
					OnComplete?.Invoke();
				}
			}
		}
		public void RequestAudioClip(string url, System.Action<AudioClip> OnComplete = null, System.Action<string> OnFalse = null)
		{
			StartCoroutine(WebRequestAudioclip(url, (www) =>
			{
				var clip = DownloadHandlerAudioClip.GetContent(www);
				OnComplete?.Invoke(clip);
			}, OnFalse));
		}
		public void RequestFileByte(string url, System.Action<byte[]> OnComplete = null, System.Action<string> OnFalse = null)
		{
			StartCoroutine(WebRequestAudioclip(url, (www) =>
			{

				var clip = DownloadHandlerAudioClip.GetContent(www);
				OnComplete?.Invoke(www.downloadHandler.data);
			}, OnFalse));
		}
		IEnumerator WebRequestAudioclip(string url, System.Action<UnityWebRequest> OnComplete = null, System.Action<string> OnFalse = null)
		{

			UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(ServerManager.ServerApi + url, AudioType.AUDIOQUEUE);
			//www.SetRequestHeader("Access-Control-Allow-Credentials", "true");
			//www.SetRequestHeader("Access-Control-Allow-Headers", "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
			//www.SetRequestHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
			//www.SetRequestHeader("Access-Control-Allow-Origin", "*");
			//www.SetRequestHeader("Origin", "https://game.garillapoker.com");

			if (!string.IsNullOrEmpty(Token))
			{
				www.SetRequestHeader("Authorization", "Bearer " + Token);
			}

			yield return www.SendWebRequest();

			if (www.result == UnityWebRequest.Result.ConnectionError)
			{
				OnFalse?.Invoke(www.error);
			}
			else
			{
				try
				{
					OnComplete?.Invoke(www);
				}
				catch { }
			}
		}

		//public async Texture2D RequestTextureAsync(string url){
		//	var result = WebRequestImageAsync(url);
		//	return result.Result;
		//}

		//public async Task<Texture2D> WebRequestImageAsync(string url){

		//}


#if !UNITY_WebRequest
		public void RequestTexture(string url, System.Action<Texture2D, byte[]> OnComplete = null, System.Action<string> OnFalse = null)
		{
#if !BESTHTTP_DISABLE_CACHING
			allDownloadedFromLocalCache = true;
#endif
			if (_cacheTexture.ContainsKey(url))
			{
				OnComplete?.Invoke(_cacheTexture[url], null);
				return;
			}
			WebDownloadImageHttp(url, (pic, b) =>
			{
				if (!_cacheTexture.ContainsKey(url))
					_cacheTexture.Add(url, pic);
				OnComplete?.Invoke(pic, null);

			}, OnFalse);
		}
#else
		public void RequestTexture(string url, System.Action<Texture2D, byte[]> OnComplete = null, System.Action<string> OnFalse = null)
		{
			StartCoroutine(WebRequestImage(url, OnComplete, OnFalse));
		}
#endif

		public static void WebDownloadImageHttp(string url, System.Action<Texture2D, byte[]> OnComplete = null, System.Action<string> OnFalse = null)
		{
			string targetUrl = url;
			var request = new HTTPRequest(new Uri(targetUrl, false), (req, resp) =>
			{

				switch (req.State)
				{
					// The request finished without any problem.
					case HTTPRequestStates.Finished:
						if (resp.IsSuccess)
						{
							// The target RawImage reference is stored in the Tag property
							OnComplete?.Invoke(resp.DataAsTexture2D, resp.Data);

#if !BESTHTTP_DISABLE_CACHING
							// Update the cache-info variable
							allDownloadedFromLocalCache = allDownloadedFromLocalCache && resp.IsFromCache;
#endif
						}
						else
						{
							it.Logger.LogWarning(string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}",
																							resp.StatusCode,
																							resp.Message,
																							resp.DataAsText));
						}
						break;

					// The request finished with an unexpected error. The request's Exception property may contain more info about the error.
					case HTTPRequestStates.Error:
						it.Logger.LogError("Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));
						break;

					// The request aborted, initiated by the user.
					case HTTPRequestStates.Aborted:
						it.Logger.LogWarning("Request Aborted!");
						break;

					// Connecting to the server is timed out.
					case HTTPRequestStates.ConnectionTimedOut:
						it.Logger.LogError("Connection Timed Out!");
						break;

					// The request didn't finished in the given time.
					case HTTPRequestStates.TimedOut:
						it.Logger.LogError("Processing the request Timed Out!");
						break;
				}

				//				activeRequests.Remove(req);
				//				if (activeRequests.Count == 0)
				//				{
				//#if !BESTHTTP_DISABLE_CACHING
				//					if (allDownloadedFromLocalCache)
				//						this._cacheLabel.text = "All images loaded from local cache!";
				//					else
				//#endif
				//						this._cacheLabel.text = string.Empty;
				//				}
			});
			//request.AddHeader("Access-Control-Allow-Origin", "*");
			//request.AddHeader("Access-Control-Allow-Headers", "origin, x-requested-with, content-type");
			//request.AddHeader("Access-Control-Allow-Methods", "PUT, GET, POST, DELETE, OPTIONS");
			//request.AddHeader("Access-Control-Allow-Credentials", "true");
			//request.AddHeader("Origin", "https://game.garillapoker.com");

			request.Send();
		}


		public void RequestSprite(string url, System.Action<Sprite, byte[]> OnComplete = null, System.Action<string> OnFalse = null)
		{
			string targetUrl = ServerManager.ServerApi + url;

			Sprite sp = WWWCachier.Instance.GetSpriteFromCashe(targetUrl);
			if (sp != null)
			{
				OnComplete?.Invoke(sp, new byte[0]);
				return;
			}

			StartCoroutine(WebRequestImage(url, (t, b) =>
			{
				Sprite sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(t.width / 2, t.height / 2));
				WWWCachier.Instance.AddSpriteToCashe(targetUrl, sprite);
				OnComplete?.Invoke(sprite, b);

			}, OnFalse));
		}
		IEnumerator WebRequestImage(string url, System.Action<Texture2D, byte[]> OnComplete = null, System.Action<string> OnFalse = null)
		{
			var urlfile = (url.Contains("https://") || url.Contains("http://"))
				? url
				: ServerManager.ServerApi + url;
			if (WWWCachier.Instance.CasheExistsTexture(urlfile))
			{
				///downloadfile
				var texture = WWWCachier.Instance.LoadFromCasheTexture(urlfile);
				//it.Logger.Log("Download");
				OnComplete?.Invoke(texture, null);
				yield break;
			}
#if UNITY_STANDALONE
			else if (WWWCachier.Instance.FileExist(urlfile))
			{
				///downloadfile
				var texture = WWWCachier.Instance.LoadTexture(urlfile);
				//it.Logger.Log("Download");
				OnComplete?.Invoke(texture, null);
				yield break;
			}
#endif
			else
			{
				UnityWebRequest www = UnityWebRequestTexture.GetTexture(urlfile);

				//www.SetRequestHeader("Access-Control-Allow-Credentials", "true");
				//www.SetRequestHeader("Access-Control-Allow-Headers", "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
				//www.SetRequestHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
				//www.SetRequestHeader("Access-Control-Allow-Origin", "*");

				if (!string.IsNullOrEmpty(Token))
				{
					www.SetRequestHeader("Authorization", "Bearer " + Token);
				}

				it.Logger.Log("Image request " + urlfile);
				yield return www.SendWebRequest();

				if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
				{
					OnFalse?.Invoke(www.error);
				}
				else
				{
					var texture = DownloadHandlerTexture.GetContent(www);
					WWWCachier.Instance.SaveByte(www.downloadHandler.data, urlfile);
					WWWCachier.Instance.SaveTextureCashe(texture, urlfile);
					OnComplete?.Invoke(texture, www.downloadHandler.data);


					// скорее всего большая картинка, грузим дата
					//yield return WebRequestBytes(url, (bytes) =>
					//{
					//	OnComplete?.Invoke(texture, bytes);
					//}, OnFalse);
				}
			}
		}
		public void RequestAudioClip(string url, AudioType audioType, System.Action<AudioClip> OnComplete = null, System.Action<string> OnFalse = null)
		{
			StartCoroutine(WebRequestAudioClip(url, audioType, OnComplete, OnFalse));
		}
		IEnumerator WebRequestAudioClip(string url, AudioType audioType, System.Action<AudioClip> OnComplete = null, System.Action<string> OnFalse = null)
		{
			WWW mp3WWW = new WWW(ServerManager.ServerApi + url);

			it.Logger.Log("Request audioClip " + mp3WWW.url);
			yield return mp3WWW;

			var myclip = mp3WWW.GetAudioClip();
			OnComplete?.Invoke(myclip);

		}
		public void RequestBytes(string url, System.Action<byte[]> OnComplete = null, System.Action<string> OnFalse = null)
		{
			StartCoroutine(WebRequestBytes(url, OnComplete, OnFalse));
		}
		IEnumerator WebRequestBytes(string url, System.Action<byte[]> OnComplete = null, System.Action<string> OnFalse = null)
		{
			var urlfile = (url.Contains("https://") || url.Contains("http://"))
					? url
					: ServerManager.ServerApi + url;
			if (WWWCachier.Instance.CasheExistsBytes(urlfile))
			{
				///downloadfile
				var byt = WWWCachier.Instance.LoadFromCasheBytes(urlfile);
				//it.Logger.Log("Download");
				OnComplete?.Invoke(byt);
				yield break;
			}
#if UNITY_STANDALONE_WIN
			else if (WWWCachier.Instance.FileExist(urlfile))
			{

				///downloadfile
				var data = WWWCachier.Instance.LoadByte(urlfile);
				//it.Logger.Log("Download");
				OnComplete?.Invoke(data);
				yield break;
			}
#endif
			else
			{
				it.Logger.Log("Byte request " + urlfile);
				UnityWebRequest www = UnityWebRequest.Get(urlfile);
				//www.SetRequestHeader("Access-Control-Allow-Credentials", "true");
				//www.SetRequestHeader("Access-Control-Allow-Headers", "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
				//www.SetRequestHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
				//www.SetRequestHeader("Access-Control-Allow-Origin", "*");

				if (!string.IsNullOrEmpty(Token))
				{
					www.SetRequestHeader("Authorization", "Bearer " + Token);
				}

				yield return www.SendWebRequest();

				if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
				{
					OnFalse?.Invoke(www.error);
				}
				else
				{
					//var texture = DownloadHandlerTexture.GetContent(www);
					WWWCachier.Instance.SaveByteCashe(www.downloadHandler.data, urlfile);
					WWWCachier.Instance.SaveByte(www.downloadHandler.data, urlfile);
					OnComplete?.Invoke(www.downloadHandler.data);
				}
			}
		}


		//private void Authorization()
		//{
		//	List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();

		//	paramsList.Add(new KeyValuePair<string, object>("username", _login));
		//	paramsList.Add(new KeyValuePair<string, object>("password", _password));

		//	paramsList.Add(new KeyValuePair<string, object>("role", "VideoWall"));

		//	Request("/auth/login", paramsList, (result) =>
		//	{
		//		it.Logger.Log("NEST auth data " + result);
		//		JSON data = JSON.ParseString(result);
		//		//var res = JsonUtility.FromJson<AuthResult>(result);
		//		GameManager.ServerToken = data.GetString("token");
		//		User.Token = GameManager.ServerToken;
		//		User.Id = data.GetJSON("user").GetInt("id");
		//		it.Logger.Log("Auth token NEST " + GameManager.ServerToken);
		//		com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.NetApiLigin, 0);
		//	},
		//	(error) =>
		//	{
		//		it.Logger.LogError("Login error " + error + " | Request: " + ServerApi + "/auth/login");
		//		DG.Tweening.DOVirtual.DelayedCall(2, () =>
		//		{
		//			Authorization();
		//		});
		//	});
		//}

	}
}