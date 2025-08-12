using Cysharp.Threading.Tasks;
using Providers.Network.Base;
using Providers.Network.Materials;
using UnityEngine;
using System.Collections.Generic;

namespace Providers.Network
{
	public partial class NetworkProvider
	{
		private string ServerApi => Server + "/api/v1";

		public async UniTask<(bool, object)> Authorization(string username, string password)
		{
			string url = ServerApi + "/user/login";
			RequestType requestType = RequestType.post;
			string jData = Newtonsoft.Json.JsonConvert.SerializeObject(new { email = username, password = password });

			(bool state, string resp) = await Request(requestType, url, jData);

			Debug.Log($"{state} {resp}");

			if (!state)
			{
				var responseError = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorData>(resp);
				return (state, responseError);
			}
			var response = Newtonsoft.Json.JsonConvert.DeserializeObject<UserData>(resp);

			return (state, response);
		}

		public async UniTask<(bool, object)> Registration(object regData)
		{
			string url = ServerApi + "/user";
			RequestType requestType = RequestType.post;
			string jData = Newtonsoft.Json.JsonConvert.SerializeObject(regData);

			(bool state, string resp) = await Request(requestType, url, jData);

			Debug.Log($"{state} {resp}");

			if (!state)
			{
				var responseError = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorData>(resp);
				return (state, responseError);
			}
			var response = Newtonsoft.Json.JsonConvert.DeserializeObject<UserData>(resp);

			return (state, response);
		}

		public async UniTask<(bool, object)> GetCountries(){

			string url = ServerApi + "/list/countries";
			RequestType requestType = RequestType.get;

			(bool state, string resp) = await Request(requestType, url);
			Debug.Log($"{state} {resp}");

			if (!state)
			{
				var responseError = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorData>(resp);
				return (state, responseError.message);
			}
			var response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CountryData>>(resp);

			return (state, response);
		}

		public async UniTask<(bool, object)> GetCurrency()
		{

			string url = ServerApi + "/list/currencies";
			RequestType requestType = RequestType.get;

			(bool state, string resp) = await Request(requestType, url);
			Debug.Log($"{state} {resp}");

			if (!state)
			{
				var responseError = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorData>(resp);
				return (state, responseError.message);
			}
			var response = Newtonsoft.Json.JsonConvert.DeserializeObject<CurrencyData[]>(resp);

			return (state, response);
		}

		public async UniTask<(bool, object)> GetPinRestorePassword(object data)
		{
			string url = ServerApi + "/user/request-password-restore";
			RequestType requestType = RequestType.post;
			string jData = Newtonsoft.Json.JsonConvert.SerializeObject(data);

			(bool state, string resp) = await Request(requestType, url, jData);
			Debug.Log($"{state} {resp}");

			if (!state)
			{
				var responseError = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorData>(resp);
				return (state, responseError.message);
			}
			var response = Newtonsoft.Json.JsonConvert.DeserializeObject<RestoreReturnData>(resp);

			return (state, response);
		}

		public async UniTask<(bool, object)> ChechPinPassword(object data)
		{
			string url = ServerApi + "/user/check-password-restore";
			RequestType requestType = RequestType.post;
			string jData = Newtonsoft.Json.JsonConvert.SerializeObject(data);

			(bool state, string resp) = await Request(requestType, url, jData);
			Debug.Log($"{state} {resp}");

			if (!state)
			{
				var responseError = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorData>(resp);
				return (state, responseError.message);
			}

			return (state, resp);
		}

		public async UniTask<(bool, object)> RequestUpdatePassword()
		{
			string url = ServerApi + "/user/request-password-update";
			RequestType requestType = RequestType.post;

			(bool state, string resp) = await Request(requestType, url);
			Debug.Log($"{state} {resp}");

			if (!state)
			{
				var responseError = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorData>(resp);
				return (state, responseError.message);
			}

			return (state, resp);
		}

		public async UniTask<(bool, object)> PasswordRestore(object data)
		{
			string url = ServerApi + "/user/password-restore";
			RequestType requestType = RequestType.put;
			string jData = Newtonsoft.Json.JsonConvert.SerializeObject(data);

			(bool state, string resp) = await Request(requestType, url, jData);
			Debug.Log($"{state} {resp}");

			if (!state)
			{
				var responseError = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorData>(resp);
				return (state, responseError.message);
			}

			return (state, resp);
		}

	}
}
