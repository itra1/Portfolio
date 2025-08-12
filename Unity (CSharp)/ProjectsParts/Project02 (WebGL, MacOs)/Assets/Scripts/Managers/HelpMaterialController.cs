using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leguar.TotalJSON;
using it.Api;

namespace it.Managers
{
	public class HelpMaterialController : Singleton<HelpMaterialController>
	{
		public List<it.Network.Rest.Region> RegionList = new List<Network.Rest.Region>();
		public bool Retrieved = false;
		private void Start()
		{
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.ServersLoaded, ServersLoaded);
			GetCountries();
		}

		private void OnDestroy()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.ServersLoaded, ServersLoaded);
		}

		private void ServersLoaded(com.ootii.Messages.IMessage handler)
		{
			if (ServerManager.ExistsServers)
				GetCountries();

		}

		private void GetCountries()
		{

			UserApi.GetCountries("", (result) =>
			{
				if (!result.IsSuccess)
				{

					it.Logger.Log("GetCountries error " + result.ErrorMessage);
					return;
				}
				Retrieved = true;
				RegionList = result.Result;

			});

			//string url = "https://garillapoker.com/api/v1/getCountries";

			//it.Managers.NetworkManager.Request(url, (result) =>
			//{

			//	it.Logger.Log("Regions RESPONSE " + result);

			//	JArray data = JArray.ParseString(result);

			//	for (int i = 0; i < data.Length; i++)
			//	{
			//		RegionList.Add((it.Network.Rest.Region)it.Helpers.ParserHelper.Parse(typeof(it.Network.Rest.Region), data.GetJSON(i)));
			//	}
			//	Retrieved = true;
			//},
			//(error) =>
			//{
			// it.Logger.LogError("Logout error " + error + " | Request: " + "/auth/login");
			////onError?.Invoke(error);
			////OutputError();
			//return;
			//});
		}

	}
}