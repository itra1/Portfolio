using System.Threading.Tasks;
using UnityEngine;
using System;

public class SupportChatUserApi
{


	public static void GetSupportMessages(string queryStr, Action<MessageRequest> callbackSuccess, Action<string> callbackFailed = null)
	{

		string url = "/support/chat_messages" + queryStr;


		it.Managers.NetworkManager.Request(it.Managers.RequestType.get, url, (result) =>
		{
			//var data = (MessageRequest)it.Helpers.ParserHelper.Parse(typeof(MessageRequest), 	Leguar.TotalJSON.JSON.ParseString(result));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<MessageRequest>(result);
			callbackSuccess?.Invoke(data);
		}, (error) =>
		{
			callbackFailed?.Invoke(error);
		});



		//var tcs = new TaskCompletionSource<MessageRequest>();
		//RestClient.Get<MessageRequest>(Config.BasePath + "support/chat_messages" + queryStr) //"https://gorillapoker.goldapp.ru/api/v1/support/chat_messages"
		//		.Then(res => tcs.SetResult(res))
		//		.Catch(err =>
		//		{
		//			var requestException = (RequestException)err;
		//			var errorMessage = requestException.StatusCode == 401
		//					? "Неавторизован"
		//					: $"Непредвиденная ошибка; {requestException.StatusCode}";
		//			it.Logger.LogError(errorMessage);
		//			tcs.SetResult(null);
		//		});
		//return await tcs.Task;
	}

	public static void PostSupportMessage(SupportMessage mes, Action<MessageRequest> callbackSuccess, Action<string> callbackFailed = null)
{

		string url = "/support/chat_messages";


		it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, Newtonsoft.Json.JsonConvert.SerializeObject(mes), (result) =>
		{
			//var data = (MessageRequest)it.Helpers.ParserHelper.Parse(typeof(MessageRequest),	Leguar.TotalJSON.JSON.ParseString(result));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<MessageRequest>(result);
			callbackSuccess?.Invoke(data);
		}, (error) =>
		{
			callbackFailed?.Invoke(error);
		});


		//var tcs = new TaskCompletionSource<MessageRequest>();
		//LoginBehaviour.errorType = 0;
		//RestClient.Post<MessageRequest>(Config.BasePath + "support/chat_messages", mes)
		//		.Then(res => tcs.SetResult(res))
		//		.Catch(err =>
		//		{
		//			var requestException = (RequestException)err;
		//			LoginBehaviour.errorType = (int)requestException.StatusCode;
		//			tcs.SetResult(null);

		//		});
		//return await tcs.Task;
	}

}


public class SupportChatManager : BaseGameManager<SupportChatManager>, IInitializable
{
	public override Task Setup()
	{
		return Task.CompletedTask;
	}


	public void GetSupportMessages(string messageQuery, Action<MessageRequest> callbackSuccess)
	{
		SupportChatUserApi.GetSupportMessages(messageQuery, callbackSuccess);
	}

	public void PostSupportMessage(SupportMessage mes, Action<MessageRequest> callbackSuccess)
	{
		SupportChatUserApi.PostSupportMessage(mes, callbackSuccess);
	}

}

