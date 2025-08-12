using Builder.Common;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Builder.Views
{
	public class SettingsView : ViewBase
	{
		public UnityEvent<ChangeEvent<string>> OnBuildPathChangeEvent = new();
		public UnityEvent<ChangeEvent<string>> OnArchivePathChangeEvent = new();
		public UnityEvent<ChangeEvent<string>> OnTelegramBotKeyChangeEvent = new();
		public UnityEvent<ChangeEvent<string>> OnTelegramGroupChangeEvent = new();

		public override string Type => ViewsType.Settings;

		public SettingsView(BuildSession buildData) : base(buildData)
		{

		}

		protected override void LoadPrefab()
		{
			_viewPrefab ??= AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(BuilderWindow.SettingsViewTemplate);
		}

		protected override void CreateUi()
		{
			base.CreateUi();

			var buildPathInput = _view.Q<TextField>("buildPath");

			buildPathInput.value = _buildData.Settings.BuildPath;
			_ = buildPathInput.RegisterValueChangedCallback(value =>
			OnBuildPathChangeEvent?.Invoke(value));

			int index = _view.IndexOf(buildPathInput);

			////
			var archivePath = new TextField("Archive path")
			{
				value = _buildData.Settings.ArchivePath
			};
			_ = archivePath.RegisterValueChangedCallback(value =>
			OnArchivePathChangeEvent?.Invoke(value));
			_view.Insert(++index, archivePath);

			////
			var telegrambotKey = new TextField("Telegram Bot Key")
			{
				value = _buildData.Settings.TelegramBotKey
			};
			_ = telegrambotKey.RegisterValueChangedCallback(value =>
			OnTelegramBotKeyChangeEvent?.Invoke(value));
			_view.Insert(++index, telegrambotKey);

			////
			var telegramGroup = new TextField("Telegram Group")
			{
				value = _buildData.Settings.TelegramGroup
			};
			_ = telegramGroup.RegisterValueChangedCallback(value =>
			OnTelegramGroupChangeEvent?.Invoke(value));
			_view.Insert(++index, telegramGroup);

			_buildData.BodyElement.Add(_view);
		}
	}
}
