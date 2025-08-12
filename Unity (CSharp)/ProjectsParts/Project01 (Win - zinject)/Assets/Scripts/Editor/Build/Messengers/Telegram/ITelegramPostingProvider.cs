namespace Editor.Build.Messengers.Telegram
{
	public interface ITelegramPostingProvider
	{
		void Post(string message);
		void Post(string channelId, string message);
	}
}