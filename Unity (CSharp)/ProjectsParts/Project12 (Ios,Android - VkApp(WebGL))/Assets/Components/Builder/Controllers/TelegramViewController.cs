using Builder.Common;
using Builder.Telegrams;
using Builder.Views;

namespace Builder.Controllers
{
	public class TelegramViewController : ViewController<TelegramView>
	{
		private NetworkHelper _network = new();
		private TelegramClient _client;

		private string _botKey => _session.Settings.TelegramBotKey;
		public TelegramViewController(BuildSession session) : base(session)
		{
			_client = new TelegramClient(session.Settings.TelegramBotKey, session.Settings.TelegramGroup);

			View.OnSendMessage.AddListener(SendMessage);
		}

		private void SendMessage(string message)
		{
			_client.Message(message);
		}
	}
}
