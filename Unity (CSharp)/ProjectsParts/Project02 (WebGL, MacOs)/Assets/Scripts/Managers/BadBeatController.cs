using it.Api;
using it.Network.Rest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Garilla.BadBeat
{
public class BedBeatAwardData{
		public bool IsWin;
		public ulong UserId;
		public ulong TableId;
		public decimal Award;
		public CardType[] Cards;
	}

	public class BadBeatController : AutoCreateInstance<BadBeatController>
	{
		public event UnityEngine.Events.UnityAction<JackpotInfoResponse> OnUpdate;
		public event UnityEngine.Events.UnityAction<BedBeatAwardData> OnAward;
		private JackpotInfoResponse _data;

		public JackpotInfoResponse Data { get => _data; private set => _data = value; }

		private void Start()
		{
			//GetData();

			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserLogin, UserLogin);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserLogin, UserLogin);
		}

		private void OnDestroy()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserLogin, UserLogin);
		}

		private void UserLogin(com.ootii.Messages.IMessage handler)
		{
			if (!UserController.IsLogin) return;

			GetData();
		}

		public void GetData(UnityEngine.Events.UnityAction<JackpotInfoResponse> OnComplete = null)
		{
			AppApi.GetBadBeat((response) =>
			{
				if (response.IsSuccess)
				{
					_data = response.Result.data;
					OnComplete?.Invoke(_data);
					OnUpdate?.Invoke(_data);
				}
			});
		}

		public void SetAward(ulong userId, bool isWin, ulong tableId, decimal award, CardType[] cards)
		{
			OnAward?.Invoke(new BedBeatAwardData() { IsWin = isWin, UserId = userId, TableId = tableId, Award = award, Cards = cards });
		}

	}
}