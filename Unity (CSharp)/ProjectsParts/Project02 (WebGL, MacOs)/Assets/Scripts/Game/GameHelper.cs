using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using UnityEngine;
using it.Network.Rest;

public class GameHelper
{
	private static string USER_INFO_KEY = "USER_INFO";

	public static Boolean IsMobile
	{
		get
		{
#if UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
			return true;
#else
			return false;
#endif
		}
	}

	public static User UserInfo => UserController.User;

	public static string ServerTime
	{
		get
		{
			if (serverTime == null) serverTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);
			return serverTime;
		}

		set
		{
			serverTime = value;
			var time = DateTime.Parse(serverTime);
			var diff = (time - DateTime.Now).TotalMilliseconds;
			timeDifferences = (float)diff;
		}
	}

	private static float timeDifferences = 0;

	public static DateTime NowTime => DateTime.Now.AddMilliseconds(timeDifferences);

	public static Table SelectTable = null;

	private static List<GameType> selectedGameTypes = new List<GameType>() { GameType.None };
	private static string serverTime = null;
	public static UserProfile UserProfile = null;

	public static void SaveUserInfo(AuthData info)
	{
		SaveUserInfo(info.UserData);
	}

	public static void SaveUserInfo(User info)
	{
		// userInfo = info;
	}

	//public AppGameRules AppGameRuleByRuleId(ulong ruleId){
	
	//}

	public static void GetUserProfile(Action<UserProfile> callbackSuccess,
			Action<string> callbackFailed = null)
	{
		it.Api.UserApi.GetUsetProfile((result) =>
		{
			if (result.IsSuccess)
			{
				UserProfile = result.Result.data;
				callbackSuccess?.Invoke(result.Result.data);
			}
			else
				callbackFailed?.Invoke(result.ErrorMessage);

		});


		//var tableListManager = await TableApiManager.GetInstanceAsync();
		//var result = await tableListManager.GetUsetProfile();
		//if (result.IsSuccess)
		//{
		//	UserProfile = result.Result.data;
		//	callbackSuccess?.Invoke(result.Result.data);
		//}
		//else
		//	callbackFailed?.Invoke(result.ErrorMessage);
	}

	public static void PostUserProfile(UserProfilePost profilePost, Action<UserProfile> callbackSuccess,
			Action<string> callbackFailed = null)
	{
		it.Api.UserApi.PostUserProfile(profilePost, (result) =>
		{
			if (result.IsSuccess)
			{
				UserProfile = result.Result.data;
				callbackSuccess?.Invoke(result.Result.data);
			}
			else
				callbackFailed?.Invoke(result.ErrorMessage);
		});

		//	var tableListManager = await TableApiManager.GetInstanceAsync();
		//	var result = await tableListManager.PostUsetProfile(profilePost);
		//	if (result.IsSuccess)
		//	{
		//		UserProfile = result.Result.data;
		//		callbackSuccess?.Invoke(result.Result.data);
		//	}
		//	else
		//		callbackFailed?.Invoke(result.ErrorMessage);
	}

	public static void LogOut()
	{
		PlayerPrefs.SetString(USER_INFO_KEY, null);
		//userInfo = null;
	}

	public static void SelectGame(List<GameType> type)
	{
		selectedGameTypes.Clear();
		selectedGameTypes.AddRange(type);
	}

	public static List<GameType> GetSelectGame()
	{
		return selectedGameTypes;
	}

	public static void SitDownTable(Table table, SitdownInfo sitdownInfo, Action<Table> callbackSuccess,
			Action<string> callbackFailed = null)
	{
		TableApi.SitDownTable(table.id, sitdownInfo, (result) =>
		{
			if (result.IsSuccess)
			{
				SelectTable = result.Result;
				callbackSuccess?.Invoke(result.Result);
			}
			else
			{
				callbackFailed?.Invoke(result.ErrorMessage);
			}

		});


	}

	public static void GetUpTable(Table table, Action<Table> callbackSuccess,
			Action<string> callbackFailed = null, bool isLeaveChanel = true)
	{

		TableApi.GetUpTable(table.id, (result) =>
		{
			if (result.IsSuccess)
			{
				callbackSuccess?.Invoke(result.Result);
				if (isLeaveChanel) SocketClient.Instance.LeaveTableChanel(table.id);
			}
			else
			{
				callbackFailed?.Invoke(result.ErrorMessage);
			}

		});


		//var tableListManager = await TableApiManager.GetInstanceAsync();
		//var result = await tableListManager.GetUpTable(table.Id);

		//if (result.IsSuccess)
		//{
		//	callbackSuccess?.Invoke(result.Result);
		//	if (isLeaveChanel) WebsocketClient.Instance.LeaveTableChanel(table.Id);
		//}
		//else
		//{
		//	callbackFailed?.Invoke(result.ErrorMessage);
		//}
	}

	public static void OpenTable(Table table)
	{
		SelectTable = table;
	}

	public static void CloseTable()
	{
		SceneLoader.LoadScene(SceneType.Tables);
	}

	//public static async void ChatTableOpen(Table table, Action<TableChatMessage[]> callbackSuccess)
	//{
	//	var tableListManager = await TableApiManager.GetInstanceAsync();
	//	var result = await tableListManager.ChatTable(table.Id);
	//	callbackSuccess?.Invoke(result.Result.data);
	//}
	public static void ChatTableOpen(Table table, Action<List<TableChatMessage>> callbackSuccess)
	{
		TableApi.ChatTable(table.id, (result) =>
		{
			if (result.IsSuccess)
				callbackSuccess?.Invoke(result.Result.data);
		});



		//string url = $"/tables/{table.Id}/chat";

		//it.Managers.NetworkManager.Request(url, (result) =>
		//{
		//	it.Logger.Log("Open chat " + result);
		//	var data = (TableChatMessage[])it.Helpers.ParserHelper.Parse(typeof(TableChatMessage[]), Leguar.TotalJSON.JSON.ParseString(result).GetJArray("data"));
		//	callbackSuccess?.Invoke(data);
		//	//var data = (TableChatMessage)it.Helpers.ParserHelper.Parse(typeof(TableChatMessage), Leguar.TotalJSON.JSON.ParseString(result).GetJSON("data"));
		//	//callbackSucces?.Invoke(data, false);
		//}, (err) =>
		//{
		//	//callbackFailed?.Invoke(err);

		//});
	}

	//public static async void ChatTablePost(Table table, TableChatPostMessage message, Action<TableChatMessage, bool> callbackSucces = null, Action<string> callbackFailed = null)
	//{
	//	var tableListManager = await TableApiManager.GetInstanceAsync();
	//	var result = await tableListManager.PostChatTable(table.Id, message);
	//	if (result.IsSuccess)
	//	{
	//		callbackSucces?.Invoke(result.Result.data, false);
	//	}
	//	else
	//	{
	//		callbackFailed?.Invoke(result.ErrorMessage);
	//	}
	//}
	public static void ChatTablePost(Table table, TableChatPostMessage message, Action<TableChatMessage, bool, bool> callbackSucces = null, Action<string> callbackFailed = null)
	{

		TableApi.PostChatTable(table.id, message, (result) =>
		{

			if (result.IsSuccess)
			{
				callbackSucces?.Invoke(result.Result.data, true, false);
			}
			else
			{
				callbackFailed?.Invoke(result.ErrorMessage);
			}
		});




		//string url = $"/tables/{table.Id}/chat";


		//List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();

		//paramsList.Add(new KeyValuePair<string, object>("message", message.message));

		//it.Managers.NetworkManager.Request(url, paramsList, (result) =>
		//{
		//	var data = (TableChatMessage)it.Helpers.ParserHelper.Parse(typeof(TableChatMessage), Leguar.TotalJSON.JSON.ParseString(result).GetJSON("data"));
		//	callbackSucces?.Invoke(data, false);
		//}, (err) =>
		//{
		//	callbackFailed?.Invoke(err);

		//});
	}

	public static void GetTable(Table table, Action<Table> callbackSuccess, Action<string> callbackFailed = null)
	{
		GetTable(table.id, callbackSuccess, callbackFailed);
	}

	public static void GetTable(ulong id, Action<Table> callbackSuccess, Action<string> callbackFailed = null)
	{
		TableApi.GetTable(id, (result) =>
		{

			if (result.IsSuccess)
			{
				callbackSuccess?.Invoke(result.Result);
			}
			else
			{
				callbackFailed?.Invoke(result.ErrorMessage);
			}

		});
	}

	public static void FoldActiveCards(Table table, Action<bool> callbackSuccess)
	{
		TableApi.FoldActiveCards(table.id, (result) =>
			{
				callbackSuccess.Invoke(result.IsSuccess);
			}
		);

	}

	public static void OpenDistributionCard(Action<bool> callbackSuccess)
	{
		TableApi.OpenDistributionCard(UserController.User.id, (result) =>
		{
			callbackSuccess.Invoke(result.IsSuccess);
		}
		);
	}

	public static void RaiseActiveDistributionBet(Table table, RaiseInfo raiseInfo, Action<bool> callbackSuccess)
	{

		TableApi.RaiseActiveDistributionBet(table.id, raiseInfo, (result) =>
		{
			callbackSuccess.Invoke(result.IsSuccess);
		}
		);

	}

	public static void AllinActiveDistributionBet(Table table, Action<bool> callbackSuccess)
	{
		TableApi.AllinActiveDistribution(table.id, (result) =>
		{
			callbackSuccess.Invoke(result.IsSuccess);
		}
		);

	}

	public static void UpdateTablePlayerSessionOptions(Table table, PlayerSessionOptions options,
			Action<bool> callbackSuccess)
	{
		TableApi.UpdateTablePlayerSessionOptions(table.id, options, (result) =>
		{
			callbackSuccess?.Invoke(result.IsSuccess);
		});
	}

	public static void GetTablePlayerSessionOptions(Table table, Action<PlayerSessionOptions> callbackSuccess,
			Action<string> callbackFailed = null)
	{

		TableApi.GetTablePlayerSessionOptions(table.id, (result) =>
		{

			if (result.IsSuccess)
			{
				callbackSuccess?.Invoke(result.Result.data);
			}
			else
			{
				callbackFailed?.Invoke(result.ErrorMessage);
			}
		});
	}

	public static void GetActiveDistributionsData(Table table, Action<DistributionDataResponse> callbackSuccess,
			Action<string> callbackFailed = null)
	{
		TableApi.GetActiveDistributionsData(table.id, (result) =>
		{

			if (result.IsSuccess)
			{
				callbackSuccess?.Invoke(result.Result);
			}
			else
			{
				callbackFailed?.Invoke(result.ErrorMessage);
			}
		});

	}

	public static void GetChinaActiveDistributionsData(Table table, Action<ChinaDistributionDataResponse> callbackSuccess,
			Action<string> callbackFailed = null)
	{
		TableApi.GetChinaActiveDistributionsData(table.id, (result) =>
		{

			if (result.IsSuccess)
			{
				callbackSuccess?.Invoke(result.Result);
			}
			else
			{
				callbackFailed?.Invoke(result.ErrorMessage);
			}
		});

	}

	public static void GetDistributionsHistoryData(ulong idDis, Action<DistributionHistoryDataResponse> callbackSuccess,
			Action<string> callbackFailed = null)
	{
		TableApi.GetDistributionsData(idDis, (result) =>
		{

			if (result.IsSuccess)
			{
				callbackSuccess?.Invoke(result.Result);
			}
			else
			{
				callbackFailed?.Invoke(result.ErrorMessage);
			}
		});
	}


	public static void ObserveTable(Table table, string password,
			Action<TableObserverSession> callbackSuccess, Action<string> callbackFailed = null)
	{
		TableApi.ObserveTable(table.id, password, (result) =>
		{
			if (result.IsSuccess)
			{
				SocketClient.Instance.EnterTableChanel(table.id);
				callbackSuccess?.Invoke(result.Result);
			}
			else callbackFailed?.Invoke(result.ErrorMessage);
		});
	}

	public static void WaitPlaceTable(Table table, PasswordBody passwordBody,
			Action<TableObserverSession> callbackSuccess, Action<string> callbackFailed = null)
	{
		TableApi.WaitPlaceTable(table.id, passwordBody, (result) =>
		{
			if (result.IsSuccess) callbackSuccess?.Invoke(result.Result);
			else callbackFailed?.Invoke(result.ErrorMessage);

		});
	}

	public static void FinishAfk(Table table, Action<TablePlayerSession> callbackSuccess,
			Action<string> callbackFailed = null)
	{
		TableApi.FinishAfk(table.id, (result) =>
		{
			if (result.IsSuccess) callbackSuccess?.Invoke(result.Result);
			else callbackFailed?.Invoke(result.ErrorMessage);
		});
	}

	public static void AddMoney(Table table, MoneyBody money, Action<TablePlayerSession> callbackSuccess,
			Action<string> callbackFailed = null)
	{
		TableApi.AddMoney(table.id, money, (result) =>
		{
			if (result.IsSuccess) callbackSuccess?.Invoke(result.Result);
			else callbackFailed?.Invoke(result.ErrorMessage);
		});
	}

	public static void InitPayment(PaymentBody paymentBody, Action<PaymentResponse> callbackSuccess,
			Action<string> callbackFailed = null)
	{
		TableApi.InitPayment(paymentBody, (result) =>
		{
			if (result.IsSuccess) callbackSuccess?.Invoke(result.Result);
			else callbackFailed?.Invoke(result.ErrorMessage);
		});
	}

	public static void ChinaApplyAllCard(ulong id, Action<DistributionCard[]> callbackSuccess,
			Action<string> callbackFailed = null)
	{
		TableApi.ChinaApplyAllCard(id, (result) =>
		{
			if (result.IsSuccess) callbackSuccess?.Invoke(result.Result);
			else callbackFailed?.Invoke(result.ErrorMessage);
		});
	}

	public static void ChinaMoveCard(ulong id, ChinaCardRequestBody body, Action<DistributionCard[]> callbackSuccess,
			Action<string> callbackFailed = null)
	{
		TableApi.ChinaMoveCard(id, body, (result) =>
		{
			if (result.IsSuccess) callbackSuccess?.Invoke(result.Result);
			else callbackFailed?.Invoke(result.ErrorMessage);
		});
	}


	public static void GetWalletTransactions(IndexRequestMetaData request, Action<UserWalletTransactionRespone> callbackSuccess,
			Action<string> callbackFailed = null, bool isLeaveChanel = true)
	{
		TableApi.GetWalletTransactions(request, (result) =>
		{
			if (result.IsSuccess)
			{
				callbackSuccess?.Invoke(result.Result);
				//if (isLeaveChanel) WebsocketClient.Instance.LeaveTableChanel(table.id);
			}
			else
			{
				callbackFailed?.Invoke(result.ErrorMessage);
			}
		});
	}

	public static void GetObservers(Table table, Action<UsersLimitedRespone> callbackSuccess,
			Action<string> callbackFailed = null)
	{

		TableApi.GetObservers(table, (result) =>
		{
			if (result.IsSuccess)
			{
				callbackSuccess?.Invoke(result.Result);
			}
			else
			{
				callbackFailed?.Invoke(result.ErrorMessage);
			}
		});
	}

	public static void GetUser(ulong user_id, Action<UserLimitedRespone> callbackSuccess,
			Action<string> callbackFailed = null)
	{

		TableApi.GetUser(user_id, (result) =>
		{
			if (result.IsSuccess)
			{
				callbackSuccess?.Invoke(result.Result);
			}
			else
			{
				callbackFailed?.Invoke(result.ErrorMessage);
			}
		});
	}
}