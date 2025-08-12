using System.Collections;
using UnityEngine;

namespace it.Network.Socket
{
	[SocketAction("support_chat_message_created", "")]
	public class SupportChatMessageCreated : SocketIn
	{
		public bool IsSupport;
		public override void Parse()
		{
			IsSupport = true;
		}

		public override void Process()
		{
			com.ootii.Messages.MessageDispatcher.SendMessage(this,
			EventsConstants.SupportChatMessageCreates(Chanel), this, 0.05f);
		}

		protected override void Disposing()
		{
			base.Disposing();
		}
	}
}