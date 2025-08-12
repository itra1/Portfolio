using it.Managers;
using it.Network.Rest;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Leguar.TotalJSON;
using System.Reflection;

namespace it.Api
{
	public static class UserApi
	{
		public static void Login(LoginInfo info, Action<AuthResponse> callbackSuccess, Action<string> callbackFailed = null)
		{
			string url = "/auth/login";

			it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, Newtonsoft.Json.JsonConvert.SerializeObject(info), (result) =>
			{
				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthResponse>(result);
				callbackSuccess?.Invoke(data);
			}, (error) =>
			{
				callbackFailed?.Invoke(error);
			});

		}

		public static void Login(string email, string password, Action<ResultResponse<AuthUserResponse>> callbackSuccess)
		{
			string url = "/auth/login";
			string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
			List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();

			paramsList.Add(new KeyValuePair<string, object>("email", email));
			paramsList.Add(new KeyValuePair<string, object>("password", password));

			it.Managers.NetworkManager.Request(url, paramsList, (result) =>
			{
				it.Logger.Log($"{methodName} {result}");

				AuthUserResponse data = new AuthUserResponse();

				data.User = Newtonsoft.Json.JsonConvert.DeserializeObject<UserResponse>(result).user;
				data.Token = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenRest>(result);
				callbackSuccess?.Invoke(new ResultResponse<AuthUserResponse>(data));
			}, (error) =>
			{
				it.Logger.Log("Login error " + error + " | Request: " + url);
				callbackSuccess?.Invoke(new ResultResponse<AuthUserResponse>(error));
			});

		}

		public static void NicknameChange(string nickname, Action<ResultResponse<User>> callbackSuccess)
		{
			string url = "/auth/change_nickname";
			string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
			List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
			paramsList.Add(new KeyValuePair<string, object>(key: "nickname", nickname));

			it.Managers.NetworkManager.Request(url, paramsList, (result) =>
			{
				it.Logger.Log($"{methodName} {result}");

				var user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserResponse>(result).user;

				callbackSuccess?.Invoke(new ResultResponse<User>(user));
			}, (error) =>
			{
				it.Logger.Log("Nickname change error " + error + " | Request: " + url);
				callbackSuccess?.Invoke(new ResultResponse<User>(error));
			});

		}

		public static void ResetPassword(string email, string code, Action<ResultResponse> callbackSuccess)
		{
			string url = "/auth/check_password_reset_code";
			string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

			List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();

			paramsList.Add(new KeyValuePair<string, object>("email", email));
			paramsList.Add(new KeyValuePair<string, object>("token", code));


			it.Managers.NetworkManager.Request(url, paramsList, (result) =>
			{
				it.Logger.Log($"{methodName} {result}");
				callbackSuccess?.Invoke(new ResultResponse(true));

			}, (error) =>
			 {
				 it.Logger.Log("ResetPassword error " + error + " | Request: " + url);
				 callbackSuccess?.Invoke(new ResultResponse(error));
				 //OutputError();
				 return;
			 });
		}

		public static void ConfirmCode(string email, string code, Action<ResultResponse<AuthUserResponse>> callbackSuccess)
		{
			string url = $"/auth/confirm_code";
			string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

			List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
			paramsList.Add(new KeyValuePair<string, object>("email", email));
			paramsList.Add(new KeyValuePair<string, object>("code", code));

			it.Managers.NetworkManager.Request(url, paramsList, (result) =>
			{
				it.Logger.Log($"{methodName} {result}");

				AuthUserResponse data = new AuthUserResponse();

				data.User = Newtonsoft.Json.JsonConvert.DeserializeObject<UserResponse>(result).user;
				data.Token = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenRest>(result);
				callbackSuccess?.Invoke(new ResultResponse<AuthUserResponse>(data));
			}, (error) =>
			{
				it.Logger.Log("ConfirmCode error " + error + " | Request: " + url);
				callbackSuccess?.Invoke(new ResultResponse<AuthUserResponse>(error));
			});


		}


		public static void GetReferenceData(Action<ResultResponse<ReferenceDataResponse>> callbackSuccess)
		{
			string url = $"/reference";
			string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

			it.Managers.NetworkManager.Request(it.Managers.RequestType.get, url, (result) =>
			{
				it.Logger.Log($"{methodName} {result}");

				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ReferenceDataResponse>(result);
				callbackSuccess?.Invoke(new ResultResponse<ReferenceDataResponse>(data));
			}, (error) =>
			{
				it.Logger.Log("GetReferences error " + error + " | Request: " + url);
				callbackSuccess?.Invoke(new ResultResponse<ReferenceDataResponse>(error));
			});

		}


		public static void Refresh(Action<ResultResponse<TokenRest>> callbackSuccess)
		{
			string url = "/auth/refresh";
			string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

			it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, (result) =>
			{
				it.Logger.Log($"{methodName} {result}");

				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenRest>(result);
				callbackSuccess?.Invoke(new ResultResponse<TokenRest>(data));
			}, (error) =>
			{
				it.Logger.Log("Refresh error " + error + " | Request: " + url);
				callbackSuccess?.Invoke(new ResultResponse<TokenRest>(error));
			});

		}

		public static void GetUserData(Action<UserResponse> callbackSuccess, Action<string> callbackFailed = null)
		{
			string url = "/auth/me";
			string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

			it.Managers.NetworkManager.Request(it.Managers.RequestType.get, url, (result) =>
			{
				it.Logger.Log($"{methodName} {result}");

				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<UserResponse>(result);
				callbackSuccess?.Invoke(data);
			}, (error) =>
			{
				//LoginBehaviour.errorType = (int)requestException.StatusCode;
				it.Logger.Log(methodName + " error " + error + " | Request: " + url);
				callbackFailed?.Invoke(error);
			});

		}
		public static void Logout(Action<bool> callbackSuccess)
		{
			string url = "/auth/logout";

			it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, (result) =>
			{
				//var data = (UserResponse)it.Helpers.ParserHelper.Parse(typeof(UserResponse),
				//Leguar.TotalJSON.JSON.ParseString(result));
				callbackSuccess?.Invoke(true);
			}, (error) =>
			{
				it.Logger.Log("Logout error " + error + " | Request: " + "/auth/login");
				//LoginBehaviour.errorType = (int)requestException.StatusCode;
				callbackSuccess?.Invoke(false);
			});

		}

		#region Payment

		public static void Deposite(Replenishment replenishment,
		UnityEngine.Events.UnityAction<string> onComplete,
		UnityEngine.Events.UnityAction<string> onError)
		{

			string url = "/payments/replenishment";

			it.Managers.NetworkManager.Request(RequestType.post, url,
				Newtonsoft.Json.JsonConvert.SerializeObject(replenishment), (result) =>
			{
				it.Logger.Log("Deposite " + result);

				onComplete?.Invoke(result);

			},
		 (error) =>
		 {
			 it.Logger.Log("GetReplenishmentMethod error " + error + " | Request: " + url);
			 onError?.Invoke(error);
			 //onError?.Invoke(error);
			 //OutputError();
			 return;
		 });
		}

		public static void Withdrawal(Replenishment replenishment, Action<ResultResponse> onComplete)
		{

			string url = "/payments/payout";

			it.Managers.NetworkManager.Request(RequestType.post, url, Newtonsoft.Json.JsonConvert.SerializeObject(replenishment), (result) =>
			{
				it.Logger.Log("Withdrawal " + result);

				onComplete?.Invoke(new ResultResponse(true));

			},
		 (error) =>
		 {
			 it.Logger.Log("GetReplenishmentMethod error " + error + " | Request: " + url);
			 // onError?.Invoke(error);
			 onComplete?.Invoke(new ResultResponse(error));
			 //onError?.Invoke(error);
			 //OutputError();
			 return;
		 });
		}

		public static void Registration(string email, string password, string nickname, string phone, ulong countryId, string promo, string referal, Action<ResultResponse> onComplete)
		{

			List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();

			paramsList.Add(new KeyValuePair<string, object>("email", email));
			paramsList.Add(new KeyValuePair<string, object>("password", password));
			paramsList.Add(new KeyValuePair<string, object>("phone", phone));
			paramsList.Add(new KeyValuePair<string, object>("nickname", nickname));
			paramsList.Add(new KeyValuePair<string, object>("country_id", countryId));
			paramsList.Add(new KeyValuePair<string, object>("promo", promo));
			paramsList.Add(new KeyValuePair<string, object>("language_id", UserController.Languages.Find(x => x.slug == I2.Loc.LocalizationManager.CurrentLanguageCode.ToLower()).id));
			if (!string.IsNullOrEmpty(referal))
				paramsList.Add(new KeyValuePair<string, object>("gp_ref", referal));

			string url = "/auth/reg_user";

			it.Managers.NetworkManager.Request(url, paramsList, (result) =>
			{
				it.Logger.Log("REGISTR RESPONSE " + result);

				onComplete?.Invoke(new ResultResponse(true));

			},
		 (error) =>
		 {
			 it.Logger.Log("Login error " + error + " | Request: " + url);
			 onComplete?.Invoke(new ResultResponse(error));
			 //OutputError();
			 return;
		 });

		}

		public static void GetLanguages(Action<ResultResponse<List<Language>>> onComplete)
		{
			string url = "/reference/languages";

			it.Managers.NetworkManager.Request(url, (result) =>
			{
				it.Logger.Log("LANGUAGES RESPONSE " + result);
				var responce = Newtonsoft.Json.JsonConvert.DeserializeObject<LanguageResponse>(result);

				onComplete?.Invoke(new ResultResponse<List<Language>>(responce.languages));

			},
		 (error) =>
		 {
			 it.Logger.Log("Login error " + error + " | Request: " + url);
			 onComplete?.Invoke(new ResultResponse<List<Language>>(error));
			 //OutputError();
			 return;
		 });

		}

		public static void GetReplenishmentMethod()
		{
			string url = "/payments/replenishmentMethods";

			it.Managers.NetworkManager.Request(RequestType.get, url, (result) =>
			{
				it.Logger.Log("GetReplenishmentMethod " + url);

			},
		 (error) =>
		 {
			 it.Logger.Log("Logout error " + error + " | Request: " + url);
			 //onError?.Invoke(error);
			 //OutputError();
			 return;
		 });
		}


		public static void GetPaymentTransactions(string options, UnityAction<UserWalletTransactionRespone> onComplete, UnityAction<string> onError)
		{

			string url = "/payments/user_wallet_transactions" + (!string.IsNullOrEmpty(options) ? $"?{options}" : "");

			it.Managers.NetworkManager.Request(RequestType.get, url, (result) =>
			{

				it.Logger.Log("GetTransactions " + result);

				//UserWalletTransactionRespone responce = (UserWalletTransactionRespone)it.Helpers.ParserHelper.Parse(typeof(UserWalletTransactionRespone), result);
				UserWalletTransactionRespone responce = Newtonsoft.Json.JsonConvert.DeserializeObject<UserWalletTransactionRespone>(result);
				onComplete?.Invoke(responce);

			},
		 (error) =>
		 {
			 it.Logger.Log("GetReplenishmentMethod error " + error + " | Request: " + url);
			 onError?.Invoke(error);
			 return;
		 });
		}

		public static void GetPaymentRequests(UnityAction<List<UserRequestTransaction>> onComplete, UnityAction<string> onError)
		{

			string url = "/payments/payoutRequest";

			it.Managers.NetworkManager.Request(RequestType.get, url, (result) =>
			{

				it.Logger.Log("GetPaymentRequests " + result);

				//UserWalletRequestRespone responce = (UserWalletRequestRespone)it.Helpers.ParserHelper.Parse(typeof(UserWalletRequestRespone), result);
				UserWalletRequestRespone responce = Newtonsoft.Json.JsonConvert.DeserializeObject<UserWalletRequestRespone>(result);
				onComplete?.Invoke(responce.data);

			},
		 (error) =>
		 {
			 it.Logger.Log("GetPaymentRequests error " + error + " | Request: " + url);
			 onError?.Invoke(error);
			 return;
		 });
		}


		public static void GetPaymentRequestSeens(List<string> requests, UnityAction onComplete, UnityAction<string> onError)
		{
			string requestStr = "";

			foreach (var elem in requests)
			{
				if (requestStr.Length > 0)
					requestStr += ",";
				requestStr += elem;
			};
			string url = $"/payments/payoutRequest/seen?request_id_list={requestStr}";

			List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
			paramsList.Add(new KeyValuePair<string, object>("request_id_list", requestStr));

			it.Managers.NetworkManager.Request(RequestType.post, url, paramsList, (result) =>
			{

				it.Logger.Log("GetPaymentRequests " + result);

				//UserWalletTransactionRespone responce = (UserWalletTransactionRespone)it.Helpers.ParserHelper.Parse(typeof(UserWalletTransactionRespone), result);
				onComplete?.Invoke();

			},
		 (error) =>
		 {
			 it.Logger.Log("GetPaymentRequests error " + error + " | Request: " + url);
			 onError?.Invoke(error);
			 return;
		 });
		}



		public static void GetPaymentRequestsUnseenCount(UnityAction<int> onComplete, UnityAction<string> onError)
		{

			string url = "/payments/payoutRequest/unseen";
			string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

			it.Managers.NetworkManager.Request(RequestType.get, url, (result) =>
			{
				it.Logger.Log(methodName + " " + result);

				//UserWalletTransactionRespone responce = (UserWalletTransactionRespone)it.Helpers.ParserHelper.Parse(typeof(UserWalletTransactionRespone), result);
				//onComplete?.Invoke(responce);

			},
		 (error) =>
		 {
			 it.Logger.Log("GetPaymentRequestsUnseenCount error " + error + " | Request: " + url);
			 onError?.Invoke(error);
			 return;
		 });
		}

		public static void GetPaymentRequestsUnseenCount(string id, UnityAction<int> onComplete, UnityAction<string> onError)
		{

			string url = "/payments/payoutRequest/" + id;
			string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

			it.Managers.NetworkManager.Request(RequestType.delete, url, (result) =>
			{

				it.Logger.Log(methodName + " " + result);

				//UserWalletTransactionRespone responce = (UserWalletTransactionRespone)it.Helpers.ParserHelper.Parse(typeof(UserWalletTransactionRespone), result);
				//onComplete?.Invoke(responce);

			},
		 (error) =>
		 {
			 it.Logger.Log("GetPaymentRequestsUnseenCount error " + error + " | Request: " + url);
			 onError?.Invoke(error);
			 return;
		 });
		}

		#endregion

		#region Rank

		public static void GetRanks(UnityAction<List<RankItemResponse>> onComplete, UnityAction<string> onError)
		{

			string url = "/ranks";
			string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

			it.Managers.NetworkManager.Request(RequestType.get, url, (result) =>
			{

				it.Logger.Log(methodName + " " + result);

				RanksResponce responce = Newtonsoft.Json.JsonConvert.DeserializeObject<RanksResponce>(result);
				onComplete?.Invoke(responce.data);

			},
		 (error) =>
		 {
			 it.Logger.Log(methodName + " error " + error + " | Request: " + url);
			 onError?.Invoke(error);
			 return;
		 });

		}

		public static void GetMyRank(UnityAction<RankUser> onComplete, UnityAction<string> onError)
		{
			string url = "/ranks/info";
			string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

			it.Managers.NetworkManager.Request(RequestType.get, url, (result) =>
			{
				it.Logger.Log(methodName + " " + result);

				UserRankResponce responce = Newtonsoft.Json.JsonConvert.DeserializeObject<UserRankResponce>(result);
				onComplete?.Invoke(responce.data);

			},
		 (error) =>
		 {
			 it.Logger.Log(methodName + " error " + error + " | Request: " + url);
			 onError?.Invoke(error);
			 return;
		 });

		}

		public static void GetMyRankHistory(UnityAction<RankHistoryStruct> onComplete, UnityAction<string> onError)
		{

			string url = "/ranks/history";
			string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

			it.Managers.NetworkManager.Request(RequestType.get, url, (result) =>
			{
				it.Logger.Log(methodName + " " + result);

				RankHistoryResponce responce = Newtonsoft.Json.JsonConvert.DeserializeObject<RankHistoryResponce>(result);
				onComplete?.Invoke(responce.data);

			},
		 (error) =>
		 {
			 it.Logger.Log(methodName + " error " + error + " | Request: " + url);
			 onError?.Invoke(error);
			 return;
		 });

		}

		#endregion

		public static void GetVerifiedToken(UnityAction<string> onComplete, UnityAction<string> onError)
		{

			string url = "/profile/verificationToken";

			it.Managers.NetworkManager.Request(RequestType.get, url, (result) =>
			{
				it.Logger.Log("Get Auth Token " + result);

				//var data = (AuthToken)it.Helpers.ParserHelper.Parse(typeof(AuthToken), result);
				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthToken>(result);

				it.Logger.Log("token " + data.data);
				onComplete?.Invoke(data.data);

			},
		 (error) =>
		 {
			 it.Logger.Log("VerifiedToken error " + error + " | Request: " + url);
			 //onError?.Invoke(error);
			 //OutputError();
			 return;
		 });
		}

		public static void WelcomeBonusList(Action<WelcomeBonnusListItem[]> callbackSuccess, Action<string> callbackFailed = null)
		{
			string url = "/welcome_bonus/list";

			it.Managers.NetworkManager.Request(url, (result) =>
			{
				it.Logger.Log("WelcomeBonusList responce " + result);

				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<WelcomeBonnusListResponce>(result);
				callbackSuccess?.Invoke(data.data);
			}, (error) =>
			{
				callbackFailed?.Invoke(error);
			});
		}

		public static void WelcomeBonus(Action<WelcomeBonusData> callbackSuccess, Action<string> callbackFailed = null)
		{
			string url = "/welcome_bonus";

			it.Managers.NetworkManager.Request(url, (result) =>
			{
				it.Logger.Log("WelcomeBonus responce " + result);

				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<WelcomeBonnusResponce>(result);
				callbackSuccess?.Invoke(data.data);
			}, (error) =>
			{
				callbackFailed?.Invoke(error);
			});
		}


		public static void GetMyApi(Action<string> callbackSuccess, Action<string> callbackFailed = null)
		{
			string url = "https://api.ipify.org";

			it.Managers.NetworkManager.Request(url, (result) =>
			{
				callbackSuccess?.Invoke(result);
			}, (error) =>
			{
				callbackFailed?.Invoke(error);
			});
		}

		public static void GetTimabankPrice(Action<Dictionary<string, double>> callbackSuccess, Action<string> callbackFailed = null)
		{
			string url = "/payments/timebank/prices";

			it.Managers.NetworkManager.Request(url, (result) =>
			{
				it.Logger.Log("GetTimabankPrice responce " + result);
				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<TimebankPriceResponce>(result);
				callbackSuccess?.Invoke(data.data);
			}, (error) =>
			{
				callbackFailed?.Invoke(error);
			});
		}

		public static void CashierMethods(Action<ResultResponse<CashierMethods>> onCallback)
		{
			string url = "/payments/cashierMethods";

			it.Managers.NetworkManager.Request(RequestType.get, url, (result) =>
			{
				it.Logger.Log("CashierMethods " + result);
				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<CashierMethods>(result);
				onCallback?.Invoke(new ResultResponse<CashierMethods>(data));
			}, (error) =>
			{
				onCallback?.Invoke(new ResultResponse<CashierMethods>(error));
			});
		}

		public static void BuyTimabank(float amount, Action<UserResponse> callbackSuccess, Action<string> callbackFailed = null)
		{
			string url = "/payments/timebank/buy";
			List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
			paramsList.Add(new KeyValuePair<string, object>("amount", amount));

			it.Managers.NetworkManager.Request(RequestType.post, url, paramsList, (result) =>
			{
				it.Logger.Log("BuyTimabank responce " + result);
				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<UserResponse>(result);
				callbackSuccess?.Invoke(data);
			}, (error) =>
			{
				callbackFailed?.Invoke(error);
			});
		}

		public static void RequestConfirmationCode(string email, Action<ResultResponse> onCallback)
		{
			string url = "/auth/request_confirmation_code";

			List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
			paramsList.Add(new KeyValuePair<string, object>("email", email));

			it.Managers.NetworkManager.Request(RequestType.post, url, paramsList, (result) =>
			{
				it.Logger.Log("RequestConfirmationCode responce " + result);
				onCallback?.Invoke(new ResultResponse(true));
			}, (error) =>
			{
				it.Logger.Log("RequestConfirmationCode error " + error + " | Request: " + "/auth/login");
				onCallback?.Invoke(new ResultResponse(error));
			});
		}
		public static void RequestRecoveryPassword(string email, Action<string> callbackSuccess, Action<string> callbackFailed = null)
		{
			string url = "/auth/create_password_reset_token ";

			List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
			paramsList.Add(new KeyValuePair<string, object>("email", email));

			it.Managers.NetworkManager.Request(RequestType.post, url, paramsList, (result) =>
			{
				it.Logger.Log("RequestConfirmationCode responce " + result);
				callbackSuccess?.Invoke(result);
			}, (error) =>
			{
				it.Logger.Log("RequestConfirmationCode error " + error + " | Request: " + "/auth/login");
				callbackFailed?.Invoke(error);
			});
		}

		public static void GetCountries(string langName, Action<ResultResponse<List<Network.Rest.Region>>> callbackSuccess)
		{
			string url = "/reference/gp/countries";
			if (!string.IsNullOrEmpty(langName))
				url += "?lang=" + langName;


			it.Managers.NetworkManager.Request(url, (result) =>
			{
				it.Logger.Log("Regions RESPONSE " + result);
				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<RegionResponse>(result).data;

				callbackSuccess?.Invoke(new ResultResponse<List<Network.Rest.Region>>(data));
			}, (error) =>
			{
				it.Logger.Log("RequestConfirmationCode error " + error + " | Request: " + "/auth/login");
				callbackSuccess?.Invoke(new ResultResponse<List<Network.Rest.Region>>(error));
			});

		}

		/// <summary>
		/// ������ ������������ �� ��������
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="callbackSuccess"></param>
		/// <param name="callbackFailed"></param>
		public static void FindUser(string userName, Action<ResultResponse<List<FindUser>>> callbackSuccess)
		{
			string url = $"/transfer/user?nickname={userName}";

			List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
			paramsList.Add(new KeyValuePair<string, object>("nickname", userName));

			it.Managers.NetworkManager.Request(RequestType.get, url, (result) =>
			{
				it.Logger.Log("FindUser responce " + result);
				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<FindUserResponse>(result);
				callbackSuccess?.Invoke(new ResultResponse<List<FindUser>>(data.data));
			}, (error) =>
			{
				callbackSuccess?.Invoke(new ResultResponse<List<FindUser>>(error));
			});
		}

		public static void Transfer(double amount, ulong userId, Action<bool> callbackSuccess, Action<string> callbackFailed = null)
		{
			string url = $"/transfer";

			List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
			paramsList.Add(new KeyValuePair<string, object>("user_id", userId));
			paramsList.Add(new KeyValuePair<string, object>("amount", amount));

			it.Managers.NetworkManager.Request(RequestType.post, url, paramsList, (result) =>
			{
				callbackSuccess?.Invoke(true);
			}, (error) =>
			{
				callbackFailed?.Invoke(error);
			});
		}

		public static void CancelRequest(ulong requestId, Action<bool> callbackSuccess, Action<string> callbackFailed = null)
		{
			string url = $"/payments/payoutRequest/{requestId}";

			it.Managers.NetworkManager.Request(RequestType.delete, url, (result) =>
			{
				it.Logger.Log("CancelRequest complete " + result);
				callbackSuccess?.Invoke(true);
			}, (error) =>
			{
				it.Logger.Log("CancelRequest error " + error);
				callbackFailed?.Invoke(error);
			});
		}
		public static void FindCasinoUser(string userName, Action<ResultResponse> callbackSuccess)
		{
			//string url = $"https://dev.garillacasino.com/api/v1/integration/transfer-out/check-login";
			string url = $"/integration/transferOut/checkLogin";

			List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
			paramsList.Add(new KeyValuePair<string, object>("login", userName));

			it.Managers.NetworkManager.Request(RequestType.post, url, paramsList, (result) =>
			{
				it.Logger.Log("FindCasinoUser responce " + result);
				callbackSuccess?.Invoke(new ResultResponse(true));
			}, (error) =>
			{
				callbackSuccess?.Invoke(new ResultResponse(error));
			});
		}

		public static void TransferCasino(string login, double amount, Action<bool> callbackSuccess, Action<string> callbackFailed = null)
		{
			//string url = $"https://dev.garillacasino.com/api/v1/integration/transfer-out/send-money";
			string url = $"/integration/transferOut/sendMoney";

			List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
			paramsList.Add(new KeyValuePair<string, object>("login", login));
			paramsList.Add(new KeyValuePair<string, object>("amount", amount.ToString()));

			it.Managers.NetworkManager.Request(RequestType.post, url, paramsList, (result) =>
			{
				callbackSuccess?.Invoke(true);
			}, (error) =>
			{
				callbackFailed?.Invoke(error);
			});
		}
		public static void UserOnlineCount(Action<int> callbackSuccess, Action<string> callbackFailed = null)
		{
			string url = $"/users/count_active";

			it.Managers.NetworkManager.Request(RequestType.get, url, (result) =>
			{
				it.Logger.Log("UserOnlineCount responce " + result);

				callbackSuccess?.Invoke(Leguar.TotalJSON.JSON.ParseString(result).GetInt("count"));

			}, (error) =>
			{
				callbackFailed?.Invoke(error);
			});
		}


		public static void GetUsetProfile(Action<ResultResponse<UserProfileResponse>> callbackSuccess)
		{
			string url = $"/profile";

			it.Managers.NetworkManager.Request(it.Managers.RequestType.get, url, (result) =>
			{
				it.Logger.Log("Profile = " + result);

				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfileResponse>(result);
				callbackSuccess?.Invoke(new ResultResponse<UserProfileResponse>(data));
			}, (error) =>
			{
				callbackSuccess?.Invoke(new ResultResponse<UserProfileResponse>(error));
			});

		}

		public static void PostUserProfile(UserProfilePost profilePost, Action<ResultResponse<UserProfileRespone>> callbackSuccess)
		{
			string url = $"/profile";

			it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, Newtonsoft.Json.JsonConvert.SerializeObject(profilePost), (result) =>
			{
				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfileRespone>(result);
				callbackSuccess?.Invoke(new ResultResponse<UserProfileRespone>(data));
			}, (error) =>
			{
				callbackSuccess?.Invoke(new ResultResponse<UserProfileRespone>(error));
			});

		}

		public static void ChangePassword(string passwordCurrent, string passwordNew, Action<ResultResponse> callbackSuccess)
		{
			string url = $"/auth/change_password";

			List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
			paramsList.Add(new KeyValuePair<string, object>("currentPassword", passwordCurrent));
			paramsList.Add(new KeyValuePair<string, object>("newPassword", passwordNew));

			it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, paramsList, (result) =>
			{
				it.Logger.Log("Password change " + result);
				callbackSuccess?.Invoke(new ResultResponse(true));
			}, (error) =>
			{
				callbackSuccess?.Invoke(new ResultResponse(error));
			});

		}

		public static void RestorePassword(string email, string token, string passwordNew, Action<ResultResponse> callbackSuccess)
		{
			string url = $"/auth/restore_password";

			List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
			paramsList.Add(new KeyValuePair<string, object>("email", email));
			paramsList.Add(new KeyValuePair<string, object>("token", token));
			paramsList.Add(new KeyValuePair<string, object>("newPassword", passwordNew));

			it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, paramsList, (result) =>
			{
				callbackSuccess?.Invoke(new ResultResponse(true));
			}, (error) =>
			{
				callbackSuccess?.Invoke(new ResultResponse(error));
			});

		}
		public static void PhoneUpdate(string phone, Action<ResultResponse> callbackSuccess)
		{
			string url = $"/auth/change_phone";

			List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
			paramsList.Add(new KeyValuePair<string, object>("phone", phone));

			it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, paramsList, (result) =>
			{
				UserController.Instance.GetUserData();
				callbackSuccess?.Invoke(new ResultResponse(true));
			}, (error) =>
			{
				callbackSuccess?.Invoke(new ResultResponse(error));
			});

		}

		public static void GetPokerStatistic(string game, Action<ResultResponse<PokerStatistic>> callbackSuccess)
		{
			string url = $"/users/statistics/win_lose?section={game}";

			it.Managers.NetworkManager.Request(it.Managers.RequestType.get, url, (result) =>
			{
				it.Logger.Log("GetBalanceStatistic = " + result);

				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<PokerStatisticResponse>(result).data;

				callbackSuccess?.Invoke(new ResultResponse<PokerStatistic>(data));
			}, (error) =>
			{
				callbackSuccess?.Invoke(new ResultResponse<PokerStatistic>(error));
			});

		}

		public static void GetStatistic(ulong userId, string section, Action<ResultResponse<UserStat>> callbackSuccess)
		{
			string url = $"/users/{userId}/statistics?section={section}";

			it.Managers.NetworkManager.Request(it.Managers.RequestType.get, url, (result) =>
			{

				it.Logger.Log("GetStatistic = " + result);

				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<UserStatResponse>(result).data;

				callbackSuccess?.Invoke(new ResultResponse<UserStat>(data));
			}, (error) =>
			{
				callbackSuccess?.Invoke(new ResultResponse<UserStat>(error));
			});
		}

		public static void GetUserActivity(Action<ResultResponse<List<TablePlayerSession>>> callbackSuccess)
		{
			string url = $"/auth/current_activity";

			it.Managers.NetworkManager.Request(RequestType.get, url, (result) =>
			{

				it.Logger.Log("GetUserActivity = " + result);

				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<TablePlayerSessionResponse>(result).active_table_player_sessions;

				callbackSuccess?.Invoke(new ResultResponse<List<TablePlayerSession>>(data));
			}, (error) =>
			{
				callbackSuccess?.Invoke(new ResultResponse<List<TablePlayerSession>>(error));
			});
		}

		public static void GetPromotions(Action<ResultResponse<PromotionsData>> callbackSuccess)
		{
			string url = $"/promo";

			it.Managers.NetworkManager.Request(RequestType.get, url, (result) =>
			{

				it.Logger.Log("GetPromotions = " + result);

				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<PromotionsDataResponse>(result).data;

				callbackSuccess?.Invoke(new ResultResponse<PromotionsData>(data));
			}, (error) =>
			{
				callbackSuccess?.Invoke(new ResultResponse<PromotionsData>(error));
			});
		}
		public static void GetTransferExchangeOut(Action<ResultResponse<List<CurrencyConversionBlock>>> callbackSuccess)
		{
			string url = $"/payments/exchangeOut";

			it.Managers.NetworkManager.Request(RequestType.get, url, (result) =>
			{
				it.Logger.Log("GetTransferExchangeOut = " + result);
				//CurrencyConversionResult
				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<CurrencyConversionResult>(result).data;

				callbackSuccess?.Invoke(new ResultResponse<List<CurrencyConversionBlock>>(data));
			}, (error) =>
			{
				callbackSuccess?.Invoke(new ResultResponse<List<CurrencyConversionBlock>>(error));
			});
		}

		public static void GetWebGLServers(string domen, Action<ResultResponse<ServersResponse>> callbackSuccess)
		{
			string url = $"{domen}/php/get-mirror.php?key=shZQGqFYxMxitp;asJ3CWEoFITr-EBegJd25Uk2Lezym6p;04I77Htt3124eech-j4KmEl0hlT9d*d4c3F2UjmJ-oC5s4hRgg7wwKbLCIH";

			it.Managers.NetworkManager.Request(RequestType.post, url, (result) =>
			{
				it.Logger.Log("GetWebGLServers = " + result);
				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ServersResponse>(result);

				callbackSuccess?.Invoke(new ResultResponse<ServersResponse>(data));
			}, (error) =>
			{
				callbackSuccess?.Invoke(new ResultResponse<ServersResponse>(error));
			});
		}

		public static void GetUserBank(Action<ResultResponse<UserTimeBanks>> callbackSuccess)
		{
			string url = $"/profile/time_bank";

			it.Managers.NetworkManager.Request(RequestType.get, url, (result) =>
			{
				it.Logger.Log("GetUserBank = " + result);
				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<UserTimeBanks>(result);

				callbackSuccess?.Invoke(new ResultResponse<UserTimeBanks>(data));
			}, (error) =>
			{
				callbackSuccess?.Invoke(new ResultResponse<UserTimeBanks>(error));
			});
		}

		#region Notes
		public static void NoteCreate(UserNote note, Action<ResultResponse<UserNote>> callbackSuccess)
		{
			string url = $"/users/create_note";

			List<KeyValuePair<string, object>> paramsList = new List<KeyValuePair<string, object>>();
			paramsList.Add(new KeyValuePair<string, object>("user_id", note.user_id));
			paramsList.Add(new KeyValuePair<string, object>("message", note.message));
			paramsList.Add(new KeyValuePair<string, object>("color", note.color));
			it.Managers.NetworkManager.Request(it.Managers.RequestType.post, url, paramsList, (result) =>
			{
				it.Logger.Log("NoteCreate = " + result);
				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<UserNote>(result);
				callbackSuccess?.Invoke(new ResultResponse<UserNote>(data));
			}, (error) =>
			{
				callbackSuccess?.Invoke(new ResultResponse<UserNote>(error));
			});
		}

		public static void NoteGet(ulong userId, Action<ResultResponse<UserNote>> callbackSuccess)
		{

			string url = $"/notes/{userId}";

			it.Managers.NetworkManager.Request(it.Managers.RequestType.get, url, (result) =>
			{
				it.Logger.Log("NoteGet = " + result);
				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<UserNoteResponse>(result).data;
				callbackSuccess?.Invoke(new ResultResponse<UserNote>(data));
				if (data == null)
					data = new UserNote();
				data.user_id = userId;
				callbackSuccess?.Invoke(new ResultResponse<UserNote>(data));
			}, (error) =>
			{
				callbackSuccess?.Invoke(new ResultResponse<UserNote>(error));
			});
		}


		#endregion


	}

}