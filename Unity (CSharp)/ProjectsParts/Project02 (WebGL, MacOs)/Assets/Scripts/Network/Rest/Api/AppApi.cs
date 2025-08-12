using it.Managers;
using it.Network.Rest;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace it.Api
{
	public static class AppApi
	{
		public static void GetJackpot(Action<ResultResponse<JackpotInfoResponse>> callbackSuccess)
		{
			string url = "/jackpot";

			it.Managers.NetworkManager.Request(url, (result) =>
			{
				it.Logger.Log(System.Reflection.MethodBase.GetCurrentMethod() + " RESPONSE " + result);

				//var data = (JackpotResponece)it.Helpers.ParserHelper.Parse(typeof(JackpotResponece),	Leguar.TotalJSON.JSON.ParseString(result));
				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<JackpotResponece>(result);
				callbackSuccess?.Invoke(new ResultResponse<JackpotInfoResponse>(data.data));
			}, (error) => {
				callbackSuccess?.Invoke(new ResultResponse<JackpotInfoResponse>(error)); });
		}
		public static void GetBadBeat(Action<ResultResponse<JackpotResponece>> callbackSuccess)
		{
			string url = "/bad_beat_jackpot";

			it.Managers.NetworkManager.Request(url, (result) =>
			{
				it.Logger.Log(System.Reflection.MethodBase.GetCurrentMethod() + " RESPONSE " + result);

				//var data = (JackpotResponece)it.Helpers.ParserHelper.Parse(typeof(JackpotResponece),	Leguar.TotalJSON.JSON.ParseString(result));
				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<JackpotResponece>(result);
				callbackSuccess?.Invoke(new ResultResponse<JackpotResponece>(data));
			}, (error) => {
				callbackSuccess?.Invoke(new ResultResponse<JackpotResponece>(error));
			});
		}


		public static void GetCreateTableOptions(Action<ResultResponse<Dictionary<string, TableOptions>>> callback)
		{
			string url = "/tables/create/options";

			it.Managers.NetworkManager.Request(url, (result) =>
			{
				it.Logger.Log(System.Reflection.MethodBase.GetCurrentMethod() + " RESPONSE " + result);

				//var dat = (TableOptionsResponse)it.Helpers.ParserHelper.Parse(typeof(TableOptionsResponse), Leguar.TotalJSON.JSON.ParseString(result));
				var dat = Newtonsoft.Json.JsonConvert.DeserializeObject<TableOptionsResponse>(result);

				callback?.Invoke(new ResultResponse<Dictionary<string, TableOptions>>(dat.options));
			},
		 (error) =>
		 {
			 it.Logger.LogError("VipCreateTableOptions data error " + error + " | Request: " + url);
			 callback?.Invoke(new ResultResponse<Dictionary<string, TableOptions>>(error));
			 return;
		 });

		}

		public static void GetLeaderboard(string type, Action<ResultResponse<LeaderBoardResponse>> callback)
		{
			string url = "/leader_board";


			List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
			paramsList.Add(new KeyValuePair<string, object>("type", type));


			it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, paramsList, (result) =>
			{
				it.Logger.Log("GetLeaderboard = " + result);
				//var data = (LeaderBoardResponse)it.Helpers.ParserHelper.Parse(typeof(LeaderBoardResponse),		Leguar.TotalJSON.JSON.ParseString(result));
				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<LeaderBoardResponse>(result);
				callback?.Invoke(new ResultResponse<LeaderBoardResponse>(data));
			}, (error) =>
			{
				callback.Invoke(new ResultResponse<LeaderBoardResponse>(error));
			});
		}

		public static void GetLeaderboardLastWinner(string type, Action<ResultResponse<LeaderBoardResponseData>> callback)
		{
			string url = "/leader_board/winner_prev_week";


			List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
			paramsList.Add(new KeyValuePair<string, object>("type", type));

			it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, paramsList, (result) =>
			{
				it.Logger.Log("GetLeaderboardLastWinner = " + result);
				LeaderBoardResponseData data =
				string.IsNullOrEmpty(result)
				? null
				//: (LeaderBoardResponseData)it.Helpers.ParserHelper.Parse(typeof(LeaderBoardResponseData),	Leguar.TotalJSON.JSON.ParseString(result).GetJSON("data"));
				: Newtonsoft.Json.JsonConvert.DeserializeObject<LeaderBoardResponseDataResponse>(result).data;
				callback?.Invoke(new ResultResponse<LeaderBoardResponseData>(data));
			}, (error) =>
			{
				callback.Invoke(new ResultResponse<LeaderBoardResponseData>(error));
			});
		}
	}
}