
using it.Network.Rest;
using System.Collections.Generic;

namespace it.Network.Socket
{
	[SocketAction("distribution_events", "Событие обновления стола игры")]
	public class DistributonEvents : SocketIn
	{
		//public Leguar.TotalJSON.JSON shared_data;
		public SocketEventDistributionUserData SocketEventUserData;
		public SocketEventChinaDistributionSharedData SocketEventChinaSharedData;
		public SocketEventDistributionSharedData SocketEventSharedData;
		public string server_time;
		public bool IsChina = false;

		public class IncomingPachage{
			public SocketEventDistributionSharedData shared_data;
			public SocketEventDistributionUserData user_data;
			public string server_time;
		}

		public override void Parse()
		{
			IsLockDispose = true;
			var p = Newtonsoft.Json.JsonConvert.DeserializeObject<IncomingPachage>(sSource);
			SocketEventSharedData = p.shared_data;
			SocketEventUserData = p.user_data;
			server_time = p.server_time;

			p = null;
			System.GC.Collect();
			//SocketEventUserData = (SocketEventDistributionUserData)it.Helpers.ParserHelper.Parse(typeof(SocketEventDistributionUserData), JSource.GetJSON("user_data"));
			//	SocketEventUserData = Newtonsoft.Json.JsonConvert.DeserializeObject<SocketEventDistributionUserData>(JSource.GetJSON("user_data").CreatePrettyString());

			//shared_data = JSource.GetJSON("shared_data");
			//server_time = JSource.GetString("server_time");
			//if (shared_data.GetString("game_rule_system_id") == "china_poker")
			//{
			//	IsChina = true;
			//	//SocketEventChinaSharedData = (SocketEventChinaDistributionSharedData)it.Helpers.ParserHelper.Parse(typeof(SocketEventChinaDistributionSharedData), SharedData);
			//	SocketEventChinaSharedData = Newtonsoft.Json.JsonConvert.DeserializeObject<SocketEventChinaDistributionSharedData>(shared_data.CreatePrettyString());
			//}
			//else
			//{
			//	//SocketEventSharedData = (SocketEventDistributionSharedData)it.Helpers.ParserHelper.Parse(typeof(SocketEventDistributionSharedData), SharedData);
			//	SocketEventSharedData = Newtonsoft.Json.JsonConvert.DeserializeObject<SocketEventDistributionSharedData>(shared_data.CreatePrettyString());
			//}
		}

		public override void Process()
		{
			if (IsChina)
			{
				com.ootii.Messages.MessageDispatcher.SendMessage(this, EventsConstants.GameTableEvent(Chanel), this, 0.05f);
			}
			else
			{
				com.ootii.Messages.MessageDispatcher.SendMessage(this, EventsConstants.GameTableEvent(Chanel), this, 0.05f);

			}
		}

		protected override void Disposing()
		{
			//shared_data = null;
			SocketEventUserData = null;
			SocketEventChinaSharedData = null;
			SocketEventSharedData = null;
			base.Disposing();
		}
	}
}