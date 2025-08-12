using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Telegram.Messages;

namespace Telegram
{
	public partial class TelergamClient
	{

		/// <summary>
		/// Отправить сообщение в чат
		/// </summary>
		/// <see cref="https://core.telegram.org/bots/api#sendmessage"/>
		/// <param name="chatId"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		public async UniTask<Message> SendMessage(string chatId, string message)
		{
			Dictionary<string, string> data = new()
			{
				{ "parse_mode", "HTML" },
				{ "chat_id", chatId },
				{ "text", message }
			};
			(_, string response) = await Request(TelegramApi.SendMessage, data);

			UnityEngine.Debug.Log(response);

			var messageObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Message>(response);

			return messageObject;
		}

		/// <summary>
		/// Редактирование сообщения
		/// </summary>
		/// <see cref="https://core.telegram.org/bots/api#editmessagetext"/>
		/// <param name="chatId"></param>
		/// <param name="messageId"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		public async UniTask<string> EditMessage(string chatId, long messageId, string message)
		{
			Dictionary<string, string> data = new()
			{
				{ "parse_mode", "HTML" },
				{ "chat_id", chatId },
				{ "message_id", messageId.ToString() },
				{ "text", message }
			};

			(_, string response) = await Request(TelegramApi.EditTextMessage, data);

			return response;
		}

		/// <summary>
		/// Удаление сообщения
		/// </summary>
		/// <see cref="https://core.telegram.org/bots/api#deletemessage"/>
		/// <param name="chatId"></param>
		/// <param name="messageId"></param>
		/// <returns></returns>
		public async UniTask<string> DeleteMessage(string chatId, long messageId)
		{
			Dictionary<string, string> data = new()
			{
				{ "chat_id", chatId },
				{ "message_id", messageId.ToString() }
			};

			(_, string response) = await Request(TelegramApi.DeleteMessage, data);

			return response;
		}
	}
}
