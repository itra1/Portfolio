using it.Network.Rest;
using it.Network.Socket;
using System.Collections;

using UnityEngine;

namespace it.Network.Socket
{
	[SocketAction("player_left_the_table", "")]
	public class PlayerLeftTheTable : SocketIn, GameUpdate
	{
		public SocketEventTable SocketEventTable;

		public SocketEventTable TableEvent => SocketEventTable;

		public override void Parse()
		{
			IsLockDispose = true;
			//SocketEventTable = (SocketEventTable)it.Helpers.ParserHelper.Parse(typeof(SocketEventTable), JSource.GetJSON("shared_data"));
			SocketEventTable = Newtonsoft.Json.JsonConvert.DeserializeObject<SocketEventTable>(JSource.GetJSON("shared_data").CreatePrettyString());
		}

		public override void Process()
		{
			com.ootii.Messages.MessageDispatcher.SendMessage(this,
			EventsConstants.GameTableUpdate(Chanel), this, 0.05f);
		}

		protected override void Disposing()
		{
			SocketEventTable = null;
			base.Disposing();
		}
	}
}