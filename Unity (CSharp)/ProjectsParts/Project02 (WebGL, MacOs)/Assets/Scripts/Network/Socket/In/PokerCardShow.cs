using it.Network.Rest;
using it.Network.Socket;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Network.Socket.In
{
	[SocketAction("cards_shown", "Показать сброшенные карты")]
	public class PokerCardShow : SocketIn
	{
		public ulong UserId;
		public ulong DistributionId;
		public List<DistributionCard> CardList;
		public string ServerTime;

		public override void Parse()
		{
			var SharedData = JSource.GetJSON("shared_data");
			ServerTime = JSource.GetString("server_time").ToString();
			UserId = SharedData.GetJNumber("user_id").AsULong();
			DistributionId = SharedData.GetJNumber("distribution_id").AsULong();
			//CardList = (List<DistributionCard>)it.Helpers.ParserHelper.Parse(typeof(List<DistributionCard>), SharedData.GetJArray("cards"));
			CardList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DistributionCard>>(SharedData.GetJArray("cards").CreatePrettyString());
		}

		public override void Process()
		{
			com.ootii.Messages.MessageDispatcher.SendMessage(this, EventsConstants.GameTableShowCards(Chanel), this, 0.05f);
		}

		protected override void Disposing()
		{
			CardList.Clear();
			base.Disposing();
		}
	}
}