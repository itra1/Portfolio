using UnityEngine;
using BestHTTP;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Providers.Network.Base;
using System.Collections.Generic;
using System;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace Providers.Network
{
	public partial class NetworkProvider
	{

		private async UniTask<(bool, string)> Request(RequestType requestType, string url, string json = null, List<KeyValuePair<string, string>> addHeaders = null)
		{

			string targetUrl = (url.Contains("https://") || url.Contains("http://"))
			? url
			: ServerApi + url;

			var met = requestType switch
			{
				RequestType.post => HTTPMethods.Post,
				RequestType.put => HTTPMethods.Put,
				RequestType.delete => HTTPMethods.Delete,
				RequestType.patch => HTTPMethods.Patch,
				_ => HTTPMethods.Get,
			};
			HTTPRequest request = new(new Uri(targetUrl), met, false, null);

			Debug.Log("Request " + request.Uri.AbsoluteUri + " Send " + json);

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
					if (!string.IsNullOrEmpty(addHeaders[i].Value))
						request.AddHeader(addHeaders[i].Key, addHeaders[i].Value);
				}
			}

			if (json != null)
				request.RawData = Encoding.UTF8.GetBytes(json);
			request.Timeout = TimeSpan.FromSeconds(1200);
			bool isBackground = Thread.CurrentThread.IsBackground;
			await request.Send();

			if (isBackground && !Thread.CurrentThread.IsBackground)
				await UniTask.SwitchToThreadPool();

			var response = request.Response;
			bool resultStatus = false;
			string resultString = "";

			switch (request.State)
			{
				case HTTPRequestStates.Finished:
					if (request.Response.IsSuccess)
					{
						resultStatus = true;
						resultString = response.DataAsText;
						//OnFinish?.Invoke(resultStatus, resultString);
					}
					else
					{
						if (request.Exception != null)
						{
							resultStatus = false;
							resultString = request.Exception.Message;
							//OnFinish?.Invoke(resultStatus, resultString);
						}
						else if (request.Response != null)
						{
							resultStatus = false;
							resultString = response.DataAsText;
							//OnFinish?.Invoke(resultStatus, resultString);
						}
						Debug.Log(string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2} {3}",
							response.StatusCode,
							response.Message,
							response.DataAsText,
							request.Uri.AbsoluteUri));
					}
					break;

				case HTTPRequestStates.Error:
					resultStatus = false;
					resultString = request.Exception.Message;
					//OnFinish?.Invoke(resultStatus, resultString);
					Debug.LogError("Request Finished with Error! " + (request.CurrentUri) + " " + (request.Exception != null ? (request.Exception.Message + "\n" + request.Exception.StackTrace) : "No Exception"));
					break;

				case HTTPRequestStates.Aborted:
					resultStatus = false;
					resultString = "Request Aborted!";
					//OnFinish?.Invoke(resultStatus, resultString);
					Debug.Log(resultString);
					break;

				case HTTPRequestStates.ConnectionTimedOut:
					resultStatus = false;
					resultString = "Connection Timed Out!";
					//OnFinish?.Invoke(resultStatus, resultString);
					Debug.Log(resultString);
					break;

				case HTTPRequestStates.TimedOut:
					resultStatus = false;
					resultString = "Processing the request Timed Out!";
					//OnFinish?.Invoke(resultStatus, resultString);
					Debug.Log(resultString);
					break;
			}

			request?.Dispose();
			return (resultStatus, resultString);

		}
	}
}
