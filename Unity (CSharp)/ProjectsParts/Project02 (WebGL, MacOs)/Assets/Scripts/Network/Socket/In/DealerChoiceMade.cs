using it.Network.Rest;
using it.Network.Socket;
using System.Collections;
using UnityEngine;

namespace it.Network.Socket
{
	[SocketAction("dealer_choice_made", "")]
	public class DealerChoiceMade : SocketIn
	{
		public Table Table;

		public override void Parse()
		{
			//Table = (Table)it.Helpers.ParserHelper.Parse(typeof(Table), JSource.GetJSON("shared_data").GetJSON("table"));
			Table = Newtonsoft.Json.JsonConvert.DeserializeObject<Table>(JSource.GetJSON("shared_data").GetJSON("table").CreatePrettyString());
		}

		public override void Process()
		{
			com.ootii.Messages.MessageDispatcher.SendMessage(this,
			EventsConstants.DealerChoise(Chanel), this, 0.05f);
		}

		protected override void Disposing()
		{
			Table = null;
			base.Disposing();
		}
	}
}