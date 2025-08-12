using System.Collections.Generic;
using Builder.Common;

namespace Builder.Telegrams
{
	public class TelegramClient
	{
		private readonly string _botKey;
		private readonly string _chatId;
		private NetworkHelper _network;

		private string Server => $"https://api.telegram.org";
		private string BotUrl => $"{Server}/{_botKey}";

		public TelegramClient(string botKey, string chatId)
		{
			_botKey = botKey;
			_chatId = chatId;
			_network = new();
		}

		public void Message(string message)
		{
			Request(message);
		}

		private void Request(string message)
		{
			Dictionary<string, string> sendData = new(){
				{ "chat_id", _chatId },
				{ "parse_mode", "HTML" },
				{ "text", message }
			};

			var request = _network.Post($"{BotUrl}{Api.SendMessage}", sendData);

			while (request.MoveNext())
			{ }
		}
	}
}
