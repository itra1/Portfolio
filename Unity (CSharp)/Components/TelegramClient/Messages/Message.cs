using Telegram.Messages.Common;

namespace Telegram.Messages
{
	public class Message :MessageBase
	{
		public MessageData result;
		public class MessageData
		{
			public long message_id;
			public ulong date;
			public string text;
			public User from;
			public Chat chat;
		}
	}
}
