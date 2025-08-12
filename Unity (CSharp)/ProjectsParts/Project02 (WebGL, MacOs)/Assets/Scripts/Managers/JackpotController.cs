using Garilla.BadBeat;
using System.Collections;
using UnityEngine;
using it.Network.Rest;
using it.Api;

namespace Garilla.Jackpot
{
	public class JackpotAwardData
	{
		public ulong UserId;
		public decimal Amount;
		public ulong TableId;
	}

	public class JackpotController : AutoCreateInstance<JackpotController>
	{
		public event UnityEngine.Events.UnityAction<JackpotInfoResponse> OnUpdate;
		public event UnityEngine.Events.UnityAction<JackpotAwardData> OnAward;

		private JackpotInfoResponse _data;

		public JackpotInfoResponse Data => _data;

		private void Start()
		{
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.ServersLoaded, LoadServers);

			if (ServerManager.ExistsServers)
				RequestData();
		}

		private void OnDestroy()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.ServersLoaded, LoadServers);
		}

		private void LoadServers(com.ootii.Messages.IMessage handler)
		{
			if (ServerManager.ExistsServers)
				RequestData();
		}

#if UNITY_EDITOR

		[ContextMenu("TestEmitAward")]
		public void TestEmitAward()
		{
			SetAwardEvent(240, 3, 100);
		}

#endif

		/// <summary>
		/// Запросить данные
		/// </summary>
		/// <param name="OnComplete">Событие успешной загрузки данных</param>
		public void RequestData(UnityEngine.Events.UnityAction<JackpotInfoResponse> OnComplete = null)
		{
			if (!ServerManager.ExistsServers) return;
				AppApi.GetJackpot((result) =>
			{
				if (result.IsSuccess)
				{
					_data = result.Result;
					OnComplete?.Invoke(_data);
					OnUpdate?.Invoke(_data);
				}
			});
		}

		public void SetAwardEvent(ulong tableId, ulong userId, decimal amount)
		{
			OnAward.Invoke(new JackpotAwardData() { UserId = userId, TableId = tableId, Amount = amount });
		}

	}
}