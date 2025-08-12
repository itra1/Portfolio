using System.Collections.Generic;
using Game.Providers.Telegram.Settings;
using UnityEngine.Networking;

namespace Game.Providers.Telegram.Handlers {
	public class TelegramHandler {
		private TelegramSettings _settings;
		public TelegramHandler(TelegramSettings settings) {
			_settings = settings;

		}

		public void Send(string message) {
			Request(_settings.ChatId, message);
		}

		private void Request(string chatId, string message) {

			var url = $"https://api.telegram.org/bot{_settings.BotKey}/sendMessage";

			Dictionary<string, string> options = new() {
				{ "chat_id", chatId},
				{ "parse_mode", "HTML"},
				{ "text", message},
			};

			var request = UnityWebRequest.Post(url, options);
			request.SendWebRequest();

		}
	}
}
