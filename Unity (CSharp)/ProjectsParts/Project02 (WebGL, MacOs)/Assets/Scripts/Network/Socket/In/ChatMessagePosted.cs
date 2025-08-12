using it.Network.Rest;

namespace it.Network.Socket
{
	[SocketAction("chat_message_posted", "")]
	public class ChatMessagePosted : SocketIn
	{
		public TableChatMessage Message;

		public override void Parse()
		{
			//Message = (TableChatMessage)it.Helpers.ParserHelper.Parse(typeof(TableChatMessage), JSource.GetJSON("shared_data").GetJSON("message"));
			Message = Newtonsoft.Json.JsonConvert.DeserializeObject<TableChatMessage>(JSource.GetJSON("shared_data").GetJSON("message").CreatePrettyString());
		}

		public override void Process()
		{
			com.ootii.Messages.MessageDispatcher.SendMessage(this,
			EventsConstants.ChatMessage(Chanel), this, 0.05f);
		}

		protected override void Disposing()
		{
			Message = null;
			base.Disposing();
		}
	}

	[SocketAction("smile_sent", "")]
	public class SmileDrop : SocketIn
	{
		public ulong FromUserId;
		public ulong ToUserId;
		public ulong SmileId;

		public override void Parse()
		{
			FromUserId = JSource.GetJSON("shared_data").GetJNumber("from_user_id").AsULong();
			ToUserId = JSource.GetJSON("shared_data").GetJNumber("to_user_id").AsULong();
			SmileId = JSource.GetJSON("shared_data").GetJNumber("smile_id").AsULong();
		}

		public override void Process()
		{
			com.ootii.Messages.MessageDispatcher.SendMessage(this,
			EventsConstants.DropSmile(Chanel), this, 0.05f);
		}
	}


}