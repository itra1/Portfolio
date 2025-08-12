using Builder.Common;
using Builder.Views;
using UnityEngine.UIElements;

namespace Builder.Controllers
{
	public class SettingsViewController : ViewController<SettingsView>
	{
		public SettingsViewController(BuildSession session) : base(session)
		{
			View.OnBuildPathChangeEvent.AddListener(BuildPathChange);
			View.OnArchivePathChangeEvent.AddListener(ArchivePathChange);
			View.OnTelegramBotKeyChangeEvent.AddListener(TelegramBotKeyChange);
			View.OnTelegramGroupChangeEvent.AddListener(TelegramGroupChange);
		}

		private void BuildPathChange(ChangeEvent<string> value)
		{
			_session.Settings.BuildPath = value.newValue;
			_session.Settings.Save();
		}

		private void ArchivePathChange(ChangeEvent<string> value)
		{
			_session.Settings.ArchivePath = value.newValue;
			_session.Settings.Save();
		}

		private void TelegramBotKeyChange(ChangeEvent<string> value)
		{
			_session.Settings.TelegramBotKey = value.newValue;
			_session.Settings.Save();
		}

		private void TelegramGroupChange(ChangeEvent<string> value)
		{
			_session.Settings.TelegramGroup = value.newValue;
			_session.Settings.Save();
		}
	}
}
