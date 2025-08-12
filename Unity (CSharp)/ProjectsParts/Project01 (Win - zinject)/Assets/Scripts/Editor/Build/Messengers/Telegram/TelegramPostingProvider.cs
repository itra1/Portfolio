using System;
using BestHTTP;
using UnityEditor;
using UnityEngine;

namespace Editor.Build.Messengers.Telegram
{
	public class TelegramPostingProvider : ITelegramPostingProvider
	{
		public void Post(string message)
		{
			Post(TelegramChannelId.First, message);
			Post(TelegramChannelId.Second, message);
		}
		
		public void Post(string channelId, string message)
		{
			var httpRequest = new HTTPRequest(new Uri("https://api.telegram.org/bot1744139829:AAGdUcPAeo0-m0_8Xb02Kf65FKPA8j66lNU/sendMessage"), 
				HTTPMethods.Post, 
				true, 
				true, 
				(request, response) => 
				{
					EditorUtility.ClearProgressBar();
					
					switch (request.State)
					{
						// The request finished without any problem.
						case HTTPRequestStates.Finished:
							if (response.IsSuccess)
							{
								Debug.Log("OnComplete");
							}
							else
							{
								Debug.Log(string.Format("Request finished successfully, but the server sent an error. Status Code: {0}-{1}. Message: {2} {3}",
									response.StatusCode,
									response.Message,
									response.DataAsText,
									request.Uri.AbsoluteUri));
							}
							break;

						// The request finished with an unexpected error. The request's Exception property may contain more info about the error.
						case HTTPRequestStates.Error:
							var exception = request.Exception;
							Debug.LogError("Request finished with Error! " + (exception != null ? exception.Message + "\n" + exception.StackTrace : "No exception"));
							break;

						// The request aborted, initiated by the user.
						case HTTPRequestStates.Aborted:
							Debug.LogError("Request aborted!");
							break;

						// Connecting to the server is timed out.
						case HTTPRequestStates.ConnectionTimedOut:
							Debug.LogError("Connection timed out!");
							break;

						// The request didn't finished in the given time.
						case HTTPRequestStates.TimedOut:
							Debug.LogError("Processing the request timed out!");
							break;
					}
				});
			
			httpRequest.SetHeader("Content-Type", "application/json");
			httpRequest.AddField("chat_id", channelId);
			httpRequest.AddField("parse_mode", "HTML");
			httpRequest.AddField("text", message);
			
			httpRequest.Send();
		}
	}
}