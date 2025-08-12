using System.Collections.Generic;
using it.Network.Rest;
using UnityEngine;
using System;

public class TableApi
{
	public static void GetTables(Action<ResultResponse<List<Table>>> onCallback)
	{
		string url = "/tables";

		it.Managers.NetworkManager.Request(url, (result) =>
		{
			it.Logger.Log("All tables " + result);

			//var data = (List<Table>)it.Helpers.ParserHelper.Parse(typeof(List<Table>), 	Leguar.TotalJSON.JSON.ParseString(result).GetJArray("data"));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<TablesResponse>(result).data;
			onCallback?.Invoke(new ResultResponse<List<Table>>(data));
		}, (error) =>
		{
			it.Logger.Log("GetTables error " + error);
			onCallback?.Invoke(new ResultResponse<List<Table>>(error));
		});
	}

	public static void CreateTable(CreateTableInfo CreateTableInfo, Action<ResultResponse<Table>> callbackSuccess)
	{
		string url = $"/tables/create";

		List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
		paramsList.Add(new KeyValuePair<string, object>("id", CreateTableInfo.id));
		paramsList.Add(new KeyValuePair<string, object>("level_id", CreateTableInfo.level_id));
		paramsList.Add(new KeyValuePair<string, object>("action_time", CreateTableInfo.action_time));
		paramsList.Add(new KeyValuePair<string, object>("players_count", CreateTableInfo.players_count));
		paramsList.Add(new KeyValuePair<string, object>("auto_start_players_count", CreateTableInfo.auto_start_players_count));
		paramsList.Add(new KeyValuePair<string, object>("time_bank", CreateTableInfo.time_bank));
		paramsList.Add(new KeyValuePair<string, object>("password", CreateTableInfo.password));

		it.Managers.NetworkManager.Request(url, paramsList, (result) =>
		{
			it.Logger.Log(result);
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<TableResponse>(result);
			//var data = (TableResponse)it.Helpers.ParserHelper.Parse(typeof(TableResponse),
			//Leguar.TotalJSON.JSON.ParseString(result));
			callbackSuccess?.Invoke(new ResultResponse<Table>(data.data));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse<Table>(error));
		});
	}

	public static void DealerChoiceSelect(ulong tableId, string rules, Action<ResultResponse> callbackSuccess)
	{
		string url = $"/tables/{tableId}/distributions/active/chooseRules";

		List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
		paramsList.Add(new KeyValuePair<string, object>("rules", rules));

		it.Managers.NetworkManager.Request(url, paramsList, (result) =>
		{
			it.Logger.Log("Diller choise response" + result);

			callbackSuccess?.Invoke(new ResultResponse(String.IsNullOrEmpty(result)));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse(error));
		});
	}

	public static void GetTable(ulong id, Action<ResultResponse<Table>> callbackSuccess)
	{
		string url = $"/tables/{id}";

		it.Managers.NetworkManager.Request(url, (result) =>
		{

			it.Logger.Log("Table: " + result);

			//var data = (TableResponse)it.Helpers.ParserHelper.Parse(typeof(TableResponse),	Leguar.TotalJSON.JSON.ParseString(result));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<TableResponse>(result);
			callbackSuccess?.Invoke(new ResultResponse<Table>(data.data));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse<Table>(error));
		});

	}
	public static void ReservatioCancelTable(ulong tableId, int place, string password, Action<ResultResponse> callbackSuccess)
	{
		string url = $"/tables/{tableId}/cancel_reservation";

		List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
		paramsList.Add(new KeyValuePair<string, object>("password", password));
		paramsList.Add(new KeyValuePair<string, object>("place", place));

		it.Managers.NetworkManager.Request(url, paramsList, (result) =>
		{
			//var data = (TableUpDownResponse)it.Helpers.ParserHelper.Parse(typeof(TableUpDownResponse),
			//Leguar.TotalJSON.JSON.ParseString(result));
			callbackSuccess?.Invoke(new ResultResponse(true));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse(error));
		});
	}
	public static void ReservatioTable(ulong tableId, int place, string password, Action<ResultResponse> callbackSuccess)
	{
		string url = $"/tables/{tableId}/make_reservation";

		List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
		paramsList.Add(new KeyValuePair<string, object>("password", password));
		paramsList.Add(new KeyValuePair<string, object>("place", place));

		it.Managers.NetworkManager.Request(url, paramsList, (result) =>
		{
			//var data = (TableUpDownResponse)it.Helpers.ParserHelper.Parse(typeof(TableUpDownResponse),
			//Leguar.TotalJSON.JSON.ParseString(result));
			callbackSuccess?.Invoke(new ResultResponse(true));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse(error));
		});
	}

	public static void SitDownTable(ulong id, SitdownInfo sitdownInfo, Action<ResultResponse<Table>> callbackSuccess)
	{
		string url = $"/tables/{id}/sit_down";

		List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
		paramsList.Add(new KeyValuePair<string, object>("buy_in", sitdownInfo.buy_in));
		paramsList.Add(new KeyValuePair<string, object>("password", sitdownInfo.password));
		paramsList.Add(new KeyValuePair<string, object>("place", sitdownInfo.place));

		it.Managers.NetworkManager.Request(url, paramsList, (result) =>
		{
			//var data = (TableUpDownResponse)it.Helpers.ParserHelper.Parse(typeof(TableUpDownResponse),	Leguar.TotalJSON.JSON.ParseString(result));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<TableUpDownResponse>(result);
			callbackSuccess?.Invoke(new ResultResponse<Table>(data.data.table));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse<Table>(error));
		});
	}

	public static void GetUpTable(ulong id, Action<ResultResponse<Table>> callbackSuccess)
	{
		string url = $"/tables/{id}/get_up";
		it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, (result) =>
	 {
		 //var data = (TableUpDownResponse)it.Helpers.ParserHelper.Parse(typeof(TableUpDownResponse), Leguar.TotalJSON.JSON.ParseString(result));
		 var data = Newtonsoft.Json.JsonConvert.DeserializeObject<TableUpDownResponse>(result);
		 callbackSuccess?.Invoke(new ResultResponse<Table>(data.data.table));
	 }, (error) =>
	 {
		 callbackSuccess?.Invoke(new ResultResponse<Table>(error));
	 });
	}

	public static void GetActiveDistributionsData(ulong id, Action<ResultResponse<DistributionDataResponse>> callbackSuccess)
	{
		string url = $"/tables/{id}/distributions/active";
		it.Managers.NetworkManager.Request(url, (result) =>
		{

			it.Logger.Log("Active distributions " + result);

			//var data = (DistributionDataResponse)it.Helpers.ParserHelper.Parse(typeof(DistributionDataResponse),	Leguar.TotalJSON.JSON.ParseString(result));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<DistributionDataResponse>(result);
			callbackSuccess?.Invoke(new ResultResponse<DistributionDataResponse>(data));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse<DistributionDataResponse>(error));
		});
	}

	public static void GetChinaActiveDistributionsData(ulong id, Action<ResultResponse<ChinaDistributionDataResponse>> callbackSuccess)
	{

		string url = $"/tables/{id}/distributions/active";

		it.Managers.NetworkManager.Request(url, (result) =>
		{
			//var data = (ChinaDistributionDataResponse)it.Helpers.ParserHelper.Parse(typeof(ChinaDistributionDataResponse),	Leguar.TotalJSON.JSON.ParseString(result));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ChinaDistributionDataResponse>(result);
			callbackSuccess?.Invoke(new ResultResponse<ChinaDistributionDataResponse>(data));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse<ChinaDistributionDataResponse>(error));
		});
	}

	public static void GetDistributionsData(ulong id, Action<ResultResponse<DistributionHistoryDataResponse>> callbackSuccess)
	{

		string url = $"/distributions/{id}";

		it.Managers.NetworkManager.Request(url, (result) =>
		{
			//var data = (DistributionHistoryDataResponse)it.Helpers.ParserHelper.Parse(typeof(DistributionHistoryDataResponse),	Leguar.TotalJSON.JSON.ParseString(result));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<DistributionHistoryDataResponse>(result);
			callbackSuccess?.Invoke(new ResultResponse<DistributionHistoryDataResponse>(data));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse<DistributionHistoryDataResponse>(error));
		});

	}

	public static void GetTableDistributionsData(Table table, Action<ResultResponse<DistributionTableHistoryResponse>> callbackSuccess)
	{
		string url = $"/tables/{table.id}/distributions/history";

		it.Managers.NetworkManager.Request(url, (result) =>
		{
			it.Logger.Log("GetTableDistributionsData: " + result);

			//var data = (DistributionTableHistoryResponse)it.Helpers.ParserHelper.Parse(typeof(DistributionTableHistoryResponse),	Leguar.TotalJSON.JSON.ParseString(result));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<DistributionTableHistoryResponse>(result);
			callbackSuccess?.Invoke(new ResultResponse<DistributionTableHistoryResponse>(data));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse<DistributionTableHistoryResponse>(error));
		});

	}

	public static void GetTableDistributionsDataByCustom(Table table, string from, string to, Action<ResultResponse<DistributionTableHistoryResponse>> callbackSuccess)
	{
		string url = $"/tables/{table.id}/distributions/history?from={from}&to={to}";

		it.Managers.NetworkManager.Request(url, (result) =>
		{
			it.Logger.Log("GetTableDistributionsData: " + result);

			//var data = (DistributionTableHistoryResponse)it.Helpers.ParserHelper.Parse(typeof(DistributionTableHistoryResponse),	Leguar.TotalJSON.JSON.ParseString(result));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<DistributionTableHistoryResponse>(result);
			callbackSuccess?.Invoke(new ResultResponse<DistributionTableHistoryResponse>(data));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse<DistributionTableHistoryResponse>(error));
		});

	}

	public static void FoldActiveCards(ulong id, Action<ResultResponse> callbackSuccess)
	{
		string url = $"/tables/{id}/distributions/active/fold";


		it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, (result) =>
		{
			//var data = (TableActionResponse)it.Helpers.ParserHelper.Parse(typeof(TableActionResponse),
			//	Leguar.TotalJSON.JSON.ParseString(result));
			callbackSuccess?.Invoke(new ResultResponse(true));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse(error));
		});

	}

	public static void CheckActiveDistribution(ulong id, Action<ResultResponse> callbackSuccess)
	{

		string url = $"/tables/{id}/distributions/active/check";


		it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, (result) =>
		{
			it.Logger.Log("Check Response " + result);
			//var data = (TableActionResponse)it.Helpers.ParserHelper.Parse(typeof(TableActionResponse),
			//Leguar.TotalJSON.JSON.ParseString(result));
			callbackSuccess?.Invoke(new ResultResponse(true));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse(error));
		});

	}

	public static void CallActiveDistribution(ulong id, Action<ResultResponse> callbackSuccess)
	{

		string url = $"/tables/{id}/distributions/active/call";


		it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, (result) =>
		{

			it.Logger.Log("Call Response " + result);
			//var data = (TableActionResponse)it.Helpers.ParserHelper.Parse(typeof(TableActionResponse),
			//Leguar.TotalJSON.JSON.ParseString(result));
			callbackSuccess?.Invoke(new ResultResponse(true));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse(error));
		});

	}

	public static void RaiseActiveDistributionBet(ulong id, RaiseInfo raiseInfo, Action<ResultResponse> callbackSuccess)
	{
		string url = $"/tables/{id}/distributions/active/raise";

		List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
		paramsList.Add(new KeyValuePair<string, object>("raise", raiseInfo.raise));

		it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, Newtonsoft.Json.JsonConvert.SerializeObject(raiseInfo), (result) =>
		{

			it.Logger.Log("Raise Response " + result);

			//var data = (TableActionResponse)it.Helpers.ParserHelper.Parse(typeof(TableActionResponse),
			//Leguar.TotalJSON.JSON.ParseString(result));
			callbackSuccess?.Invoke(new ResultResponse(true));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse(error));
		});

	}

	public static void AllinActiveDistribution(ulong id, Action<ResultResponse> callbackSuccess)
	{
		string url = $"/tables/{id}/distributions/active/allin";

		it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, (result) =>
		{

			it.Logger.Log("Allin Response " + result);
			callbackSuccess?.Invoke(new ResultResponse(true));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse(error));
		});

	}

	public static void GetTablePlayerSessionOptions(ulong id, Action<ResultResponse<PlayerSessionOptionsResponse>> callbackSuccess)
	{

		string url = $"/tables/{id}/options";

		it.Managers.NetworkManager.Request(url, (result) =>
		{
			//var data = (PlayerSessionOptionsResponse)it.Helpers.ParserHelper.Parse(typeof(PlayerSessionOptionsResponse),	Leguar.TotalJSON.JSON.ParseString(result));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<PlayerSessionOptionsResponse>(result);
			callbackSuccess?.Invoke(new ResultResponse<PlayerSessionOptionsResponse>(data));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse<PlayerSessionOptionsResponse>(error));
		});

	}

	public static void UpdateTablePlayerSessionOptions(ulong id, PlayerSessionOptions options, Action<ResultResponse<PlayerSessionOptionsResponse>> callbackSuccess)
	{

		string url = $"/tables/{id}/options";


		List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
		paramsList.Add(new KeyValuePair<string, object>("skip_distributions", options.skip_distributions));
		paramsList.Add(new KeyValuePair<string, object>("fold_to_any_bet", options.fold_to_any_bet));
		paramsList.Add(new KeyValuePair<string, object>("is_bb_accepted", options.is_bb_accepted));

		it.Managers.NetworkManager.Request(it.Managers.RequestType.put, url, paramsList, (result) =>
	 {
		 it.Logger.Log("Options = " + result);
		 //var data = (PlayerSessionOptionsResponse)it.Helpers.ParserHelper.Parse(typeof(PlayerSessionOptionsResponse),	 Leguar.TotalJSON.JSON.ParseString(result));
		 var data = Newtonsoft.Json.JsonConvert.DeserializeObject<PlayerSessionOptionsResponse>(result);
		 callbackSuccess?.Invoke(new ResultResponse<PlayerSessionOptionsResponse>(data));
	 }, (error) =>
	 {
		 it.Logger.Log("Options error = " + error);
		 callbackSuccess?.Invoke(new ResultResponse<PlayerSessionOptionsResponse>(error));
	 });

	}

	public static void OpenDistributionCard(ulong id, Action<ResultResponse> callbackSuccess)
	{


		string url = $"/distribution_player_cards/{id}/open";


		it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, (result) =>
		{
			//var data = (TableActionResponse)it.Helpers.ParserHelper.Parse(typeof(TableActionResponse), 	Leguar.TotalJSON.JSON.ParseString(result));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<TableActionResponse>(result);
			callbackSuccess?.Invoke(new ResultResponse(true));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse(error));
		});

	}

	public static void ObserveTable(ulong id, string password, Action<ResultResponse<TableObserverSession>> callbackSuccess)
	{

		string url = $"/tables/{id}/observe";

		List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
		paramsList.Add(new KeyValuePair<string, object>("password", password));

		it.Managers.NetworkManager.Request(url, paramsList, (result) =>
		{
			//var data = (TableObserverSessionResponse)it.Helpers.ParserHelper.Parse(typeof(TableObserverSessionResponse),	Leguar.TotalJSON.JSON.ParseString(result));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<TableObserverSessionResponse>(result);
			callbackSuccess?.Invoke(new ResultResponse<TableObserverSession>(data.data));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse<TableObserverSession>(error));
		});

	}
	public static void ObserveCancelTable(ulong id, Action<ResultResponse<TableObserverSession>> callbackSuccess)
	{

		string url = $"/tables/{id}/observe";

		//List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
		//paramsList.Add(new KeyValuePair<string, object>("password", password));

		it.Managers.NetworkManager.Request(it.Managers.RequestType.delete, url, (result) =>
		{
			//var data = (TableObserverSessionResponse)it.Helpers.ParserHelper.Parse(typeof(TableObserverSessionResponse),	Leguar.TotalJSON.JSON.ParseString(result));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<TableObserverSessionResponse>(result);
			callbackSuccess?.Invoke(new ResultResponse<TableObserverSession>(data.data));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse<TableObserverSession>(error));
		});

	}

	public static void WaitPlaceTable(ulong id, PasswordBody password, Action<ResultResponse<TableObserverSession>> callbackSuccess)
	{

		string url = $"/tables/{id}/wait";

		List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
		paramsList.Add(new KeyValuePair<string, object>("password", password.password));

		it.Managers.NetworkManager.Request(url, paramsList, (result) =>
		{
			//var data = (TableObserverSessionResponse)it.Helpers.ParserHelper.Parse(typeof(TableObserverSessionResponse),	Leguar.TotalJSON.JSON.ParseString(result));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<TableObserverSessionResponse>(result);
			callbackSuccess?.Invoke(new ResultResponse<TableObserverSession>(data.data));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse<TableObserverSession>(error));
		});

	}
	public static void GetAutoAddChipsOptions(ulong tableId, Action<ResultResponse<TablePlayerOptions>> callback)
	{
		string url = $"/tables/{tableId}/options";

		it.Managers.NetworkManager.Request(it.Managers.RequestType.get, url, (result) =>
		{
			it.Logger.Log("UpdateOptions = " + result);

			//var data = (TablePlayerOptions)it.Helpers.ParserHelper.Parse(typeof(TablePlayerOptions), Leguar.TotalJSON.JSON.ParseString(result).GetJSON("data"));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<TablePlayerOptionsResponse>(result).data;
			callback?.Invoke(new ResultResponse<TablePlayerOptions>(data));
		}, (error) =>
		{
			callback?.Invoke(new ResultResponse<TablePlayerOptions>(error));
		});
	}

	public static void UpdateAutoAddChipsOptions(ulong tableId, bool value, Action<ResultResponse> callback )
	{
		string url = $"/tables/{tableId}/options";
		List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
		paramsList.Add(new KeyValuePair<string, object>("auto_add_to_max_buy_in", value));

		it.Managers.NetworkManager.Request(it.Managers.RequestType.put, url, paramsList, (result) =>
		{
			it.Logger.Log("UpdateAutoAddChipsOptions = " + result);

			//var data = (TablePlayerOptions)it.Helpers.ParserHelper.Parse(typeof(TablePlayerOptions), Leguar.TotalJSON.JSON.ParseString(result).GetJSON("data"));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<TablePlayerOptionsResponse>(result).data;
			callback?.Invoke(new ResultResponse(true));
		}, (error) =>
		{
			callback?.Invoke(new ResultResponse(error));
		});
	}

	public static void FinishAfk(ulong id, Action<ResultResponse<TablePlayerSession>> callbackSuccess, Action<string> callbackFailed = null)
	{

		string url = $"/tables/{id}/finish_resting";

		it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, (result) =>
		{
			//var data = (TableAfkResponse)it.Helpers.ParserHelper.Parse(typeof(TableAfkResponse),	Leguar.TotalJSON.JSON.ParseString(result));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<TableAfkResponse>(result);
			callbackSuccess?.Invoke(new ResultResponse<TablePlayerSession>(data.data));
		}, (error) =>
		{
			callbackFailed?.Invoke(error);
		});

	}

	public static void Straddle(ulong tableId, Action<ResultResponse> callback)
	{
		string url = $"/tables/{tableId}/distributions/active/straddle";
		it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, (result) =>
		{
			it.Logger.Log("Straddle = " + result);
			callback?.Invoke(new ResultResponse(true));
		}, (error) =>
		{
			callback?.Invoke(new ResultResponse(error));
		});
	}

	public static void StartMyAfk(ulong tableId, Action<ResultResponse<TablePlayerOptions>> callback)
	{
		string url = $"/tables/{tableId}/options";
		List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
		paramsList.Add(new KeyValuePair<string, object>("skip_distributions", true));

		it.Managers.NetworkManager.Request(it.Managers.RequestType.put, url, paramsList, (result) =>
		{
			it.Logger.Log("StartMyAfk = " + result);

			//var data = (TablePlayerOptions)it.Helpers.ParserHelper.Parse(typeof(TablePlayerOptions), Leguar.TotalJSON.JSON.ParseString(result).GetJSON("data"));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<TablePlayerOptionsResponse>(result).data;
			callback?.Invoke(new ResultResponse<TablePlayerOptions>(data));
		}, (error) =>
		{
			callback?.Invoke(new ResultResponse<TablePlayerOptions>(error));
		});
	}
	public static void SkipStraddleOptions(ulong tableId, bool value, Action<ResultResponse<TablePlayerOptions>> callback)
	{
		string url = $"/tables/{tableId}/options";
		List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
		paramsList.Add(new KeyValuePair<string, object>("skip_straddle", value));

		it.Managers.NetworkManager.Request(it.Managers.RequestType.put, url, paramsList, (result) =>
		{
			it.Logger.Log("SkipStraddleOptions = " + result);
			//var data = (TablePlayerOptions)it.Helpers.ParserHelper.Parse(typeof(TablePlayerOptions), Leguar.TotalJSON.JSON.ParseString(result).GetJSON("data"));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<TablePlayerOptionsResponse>(result).data;
			callback?.Invoke(new ResultResponse<TablePlayerOptions>(data));
		}, (error) =>
		{
			callback?.Invoke(new ResultResponse<TablePlayerOptions>(error));
		});
	}
	public static void StopMyAfk(ulong tableId, Action<ResultResponse<TablePlayerOptions>> callback)
	{
		string url = $"/tables/{tableId}/options";
		List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
		paramsList.Add(new KeyValuePair<string, object>("skip_distributions", false));

		it.Managers.NetworkManager.Request(it.Managers.RequestType.put, url, paramsList, (result) =>
		{
			it.Logger.Log("StopMyAfk = " + result);
			//var data = (TablePlayerOptions)it.Helpers.ParserHelper.Parse(typeof(TablePlayerOptions), Leguar.TotalJSON.JSON.ParseString(result).GetJSON("data"));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<TablePlayerOptionsResponse>(result).data;
			callback?.Invoke(new ResultResponse<TablePlayerOptions>(data));
		}, (error) =>
		{
			callback?.Invoke(new ResultResponse<TablePlayerOptions>(error));
		});
	}
	public static void GetMyAfk(ulong tableId, Action<ResultResponse<TablePlayerOptions>> callback)
	{
		string url = $"/tables/{tableId}/options";

		it.Managers.NetworkManager.Request(it.Managers.RequestType.get, url, (result) =>
		{
			it.Logger.Log("GetMyAfk = " + result);
			//var data = (TablePlayerOptions)it.Helpers.ParserHelper.Parse(typeof(TablePlayerOptions), Leguar.TotalJSON.JSON.ParseString(result).GetJSON("data"));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<TablePlayerOptionsResponse>(result).data;
			callback?.Invoke(new ResultResponse<TablePlayerOptions>(data));
		}, (error) =>
		{
			callback?.Invoke(new ResultResponse<TablePlayerOptions>(error));
		});
	}

	public static void AddMoney(ulong id, MoneyBody money, Action<ResultResponse<TablePlayerSession>> callbackSuccess)
	{

		string url = $"/tables/{id}/add_funds";

		List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
		paramsList.Add(new KeyValuePair<string, object>("amount", money.amount));


		it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, paramsList, (result) =>
		{
			//var data = (TableActionResponse)it.Helpers.ParserHelper.Parse(typeof(TableActionResponse),	Leguar.TotalJSON.JSON.ParseString(result));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<TableActionResponse>(result);
			callbackSuccess?.Invoke(new ResultResponse<TablePlayerSession>(data.data));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse<TablePlayerSession>(error));
		});

	}

	public static void InitPayment(PaymentBody paymentBody, Action<ResultResponse<PaymentResponse>> callbackSuccess)
	{

		string url = $"/payments/replenishment";

		List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
		paramsList.Add(new KeyValuePair<string, object>("amount", paymentBody.amount));
		paramsList.Add(new KeyValuePair<string, object>("requisites", paymentBody.requisites));


		it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, paramsList, (result) =>
		{
			//var data = (PaymentResponse)it.Helpers.ParserHelper.Parse(typeof(PaymentResponse),Leguar.TotalJSON.JSON.ParseString(result));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<PaymentResponse>(result);
			callbackSuccess?.Invoke(new ResultResponse<PaymentResponse>(data));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse<PaymentResponse>(error));
		});

	}

	public static void ChinaMoveCard(ulong id, ChinaCardRequestBody chinaCardRequestBody, Action<ResultResponse<DistributionCard[]>> callbackSuccess)
	{

		string url = $"/tables/{id}/distributions/active/cp_move_card";

		List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
		paramsList.Add(new KeyValuePair<string, object>("player_card_id", chinaCardRequestBody.player_card_id));
		paramsList.Add(new KeyValuePair<string, object>("row", chinaCardRequestBody.row));
		paramsList.Add(new KeyValuePair<string, object>("position", chinaCardRequestBody.position));


		it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, paramsList, (result) =>
		{
			//var data = (ChinaChangeCardResponse)it.Helpers.ParserHelper.Parse(typeof(ChinaChangeCardResponse),Leguar.TotalJSON.JSON.ParseString(result));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ChinaChangeCardResponse>(result);
			callbackSuccess?.Invoke(new ResultResponse<DistributionCard[]>(data.data));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse<DistributionCard[]>(error));
		});

	}

	public static void ChinaApplyAllCard(ulong id, Action<ResultResponse<DistributionCard[]>> callbackSuccess)
	{

		string url = $"/tables/{id}/distributions/active/cp_apply_card_positions";


		it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, (result) =>
		{
			//var data = (ChinaChangeCardResponse)it.Helpers.ParserHelper.Parse(typeof(ChinaChangeCardResponse),	Leguar.TotalJSON.JSON.ParseString(result));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ChinaChangeCardResponse>(result);
			callbackSuccess?.Invoke(new ResultResponse<DistributionCard[]>(data.data));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse<DistributionCard[]>(error));
		});

	}

	public static void ChatTable(ulong id, Action<ResultResponse<TableChatGetResponse>> callbackSuccess)
	{

		string url = $"/tables/{id}/chat";


		it.Managers.NetworkManager.Request(it.Managers.RequestType.get, url, (result) =>
		{
			it.Logger.Log($"RESPONSE {url} " + result);

			//var data = (TableChatGetResponse)it.Helpers.ParserHelper.Parse(typeof(TableChatGetResponse),	Leguar.TotalJSON.JSON.ParseString(result));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<TableChatGetResponse>(result);
			callbackSuccess?.Invoke(new ResultResponse<TableChatGetResponse>(data));
		}, (error) =>
		{
			it.Logger.Log($"RESPONSE ERROR {url} " + error);
			callbackSuccess?.Invoke(new ResultResponse<TableChatGetResponse>(error));
		});

	}

	public static void PostChatTable(ulong id, TableChatPostMessage message, Action<ResultResponse<TableChatSendResponse>> callbackSuccess)
	{

		string url = $"/tables/{id}/chat";


		List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
		paramsList.Add(new KeyValuePair<string, object>("message", message.message));


		it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, paramsList, (result) =>
		{
			it.Logger.Log($"RESPONSE PostChatTable " + result);

			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<TableChatSendResponse>(result);
			callbackSuccess?.Invoke(new ResultResponse<TableChatSendResponse>(data));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse<TableChatSendResponse>(error));
		});

	}

	public static void DropSmie(ulong tableId, ulong userId, ulong smileId, Action<ResultResponse> callbackSuccess)
	{
		string url = $"/tables/{tableId}/distributions/active/send_smile";

		List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
		paramsList.Add(new KeyValuePair<string, object>("user_id", userId));
		paramsList.Add(new KeyValuePair<string, object>("smile_id", smileId));

		it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, paramsList, (result) =>
		{

			it.Logger.Log($"RESPONSE DropSmie " + result);

			callbackSuccess?.Invoke(new ResultResponse(true));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse(error));
		});
	}

	public static void GetWalletTransactions(IndexRequestMetaData request, Action<ResultResponse<UserWalletTransactionRespone>> callbackSuccess)
	{

		string url = $"/payments/user_wallet_transactions?" +
				$"page={request.page}&per_page={request.per_page}&order_by={request.order_by}&order_direction={request.order_direction}" +
				$"&search={request.search}&without_pagination={(request.without_pagination ? "1" : "0")}";

		it.Managers.NetworkManager.Request(it.Managers.RequestType.get, url, (result) =>
		{
			//var data = (UserWalletTransactionRespone)it.Helpers.ParserHelper.Parse(typeof(UserWalletTransactionRespone),	Leguar.TotalJSON.JSON.ParseString(result));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<UserWalletTransactionRespone>(result);
			callbackSuccess?.Invoke(new ResultResponse<UserWalletTransactionRespone>(data));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse<UserWalletTransactionRespone>(error));
		});

	}

	public static void GetObservers(Table table, Action<ResultResponse<UsersLimitedRespone>> callbackSuccess)
	{


		string url = $"/tables/{table.id}/observers";

		it.Managers.NetworkManager.Request(it.Managers.RequestType.get, url, (result) =>
		{

			it.Logger.Log("Get observe = " + result);


			//var data = (UsersLimitedRespone)it.Helpers.ParserHelper.Parse(typeof(UsersLimitedRespone),		Leguar.TotalJSON.JSON.ParseString(result));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<UsersLimitedRespone>(result);
			callbackSuccess?.Invoke(new ResultResponse<UsersLimitedRespone>(data));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse<UsersLimitedRespone>(error));
		});

	}

	public static void GetUser(ulong user_id, Action<ResultResponse<UserLimitedRespone>> callbackSuccess)
	{


		string url = $"/users/{user_id}";

		it.Managers.NetworkManager.Request(it.Managers.RequestType.get, url, (result) =>
		{
			it.Logger.Log("GetUser = " + result);
			//var data = (UserLimitedRespone)it.Helpers.ParserHelper.Parse(typeof(UserLimitedRespone),	Leguar.TotalJSON.JSON.ParseString(result));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<UserLimitedRespone>(result);
			callbackSuccess?.Invoke(new ResultResponse<UserLimitedRespone>(data));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse<UserLimitedRespone>(error));
		});

	}

	public static void ResetBingoGame(ulong id, Action<ResultResponse<BingoGame>> callbackSuccess)
	{
		string url = $"/tables/{id}/bingo_games/reset";

		it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, (result) =>
		{
			it.Logger.Log(result);

			//var data = (BingoGame)it.Helpers.ParserHelper.Parse(typeof(BingoGame),		Leguar.TotalJSON.JSON.ParseString(result).GetJSON("data"));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<BingoGameResponse>(result).data;
			callbackSuccess?.Invoke(new ResultResponse<BingoGame>(data));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse<BingoGame>(error));
		});

	}

	public static void AddActionTime(ulong id, Action<ResultResponse> callbackSuccess)
	{
		string url = $"/tables/{id}/distributions/active/timebank";


		it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, (result) =>
		{
			callbackSuccess?.Invoke(new ResultResponse(true));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse(error));
		});

	}

	class SendOpenCards
	{
		public List<ulong> card_id_list;
	}

	public static void ShowFoldCard(ulong tableId,ulong distribId,  List<ulong> cardIds, Action<ResultResponse> callbackSuccess)
	{
		string url = $"/tables/{tableId}/{distribId}/show_cards";

		SendOpenCards sendData = new SendOpenCards(){ card_id_list = cardIds };

		it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, Newtonsoft.Json.JsonConvert.SerializeObject(sendData),  (result) =>
		{
			it.Logger.Log("Show card response " + result);

			callbackSuccess?.Invoke(new ResultResponse(true));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse(error));
		});

	}

	public static void GetCombination(ulong tableId, Action<ResultResponse<GameCardCombinationInGame>> callbackSuccess)
	{
		string url = $"/tables/{tableId}/distributions/active/combination";

		it.Managers.NetworkManager.Request(it.Managers.RequestType.get, url, (result) =>
		{
			it.Logger.Log("GetCombination = " + result);

			//var data = (GameCardCombinationInGame)it.Helpers.ParserHelper.Parse(typeof(GameCardCombinationInGame),	Leguar.TotalJSON.JSON.ParseString(result).GetJSON("data"));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<GameCardCombinationInGameResponse>(result).data;

			callbackSuccess?.Invoke(new ResultResponse<GameCardCombinationInGame>(data));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse<GameCardCombinationInGame>(error));
		});
	}

	public static void ReadyToPlay(ulong tableId, Action<ResultResponse> callbackSuccess)
	{
		string url = $"/tables/{tableId}/ready_to_play";

		it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, (result) =>
		{

			//var data = (GameCardCombinationInGame)it.Helpers.ParserHelper.Parse(typeof(GameCardCombinationInGame),	Leguar.TotalJSON.JSON.ParseString(result).GetJSON("data"));
			//var data = Newtonsoft.Json.JsonConvert.DeserializeObject<GameCardCombinationInGameResponse>(result).data;

			callbackSuccess?.Invoke(new ResultResponse(true));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse(error));
		});
	}

	#region Chat

	public static void ChatBlockListGet(Action<ResultResponse<List<UserBlock>>> callbackSuccess)
	{
		string url = $"/tables/chat/blocked";

		it.Managers.NetworkManager.Request(it.Managers.RequestType.get, url, (result) =>
		{

			it.Logger.Log("[CHAT_BLOCK] GetChatBlockList = " + result);

			//var data = (List<UserBlock>)it.Helpers.ParserHelper.Parse(typeof(List<UserBlock>),	Leguar.TotalJSON.JSON.ParseString(result).GetJArray("data"));
			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<UserBlockResponse>(result).data;

			callbackSuccess?.Invoke(new ResultResponse<List<UserBlock>>(data));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse<List<UserBlock>>(error));
		});
	}
	public static void ChatBlockListBlockChange(ulong userId, bool isBlock, Action<ResultResponse> callbackSuccess)
	{
		string url = isBlock
		? $"/tables/chat/block/{userId}"
		: $"/tables/chat/unlock/{userId}";

		it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, (result) =>
		{
			it.Logger.Log("[CHAT_BLOCK] ChatBlockListBlockChange = " + result);

			callbackSuccess?.Invoke(new ResultResponse(true));
		}, (error) =>
		{
			callbackSuccess?.Invoke(new ResultResponse(error));
		});
	}

	#endregion



}
