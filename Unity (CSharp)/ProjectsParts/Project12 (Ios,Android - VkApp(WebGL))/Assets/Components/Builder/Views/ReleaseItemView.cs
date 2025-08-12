using System.IO;
using Builder.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Builder
{
	public class ReleaseItemView
	{
		public UnityAction OnChangeUploadState;

		public Release Mat { get; private set; }
		public VisualElement _view;
		public VisualElement _root;

		private readonly BuilderWindow _window;
		private VisualElement _infoBlock;
		private VisualElement _serverBuildIcone;
		private VisualElement _extendIcone;
		private Button _uploadButton;
		private Button _removeButton;
		private bool _isExtend = false;

		public ReleaseItemView(Release mat, BuilderWindow window)
		{
			Mat = mat;
			_window = window;
		}
		public VisualElement SpawnItem(VisualTreeAsset prefab)
		{
			_isExtend = false;
			_view ??= prefab.Instantiate();
			var versionLabel = _view.Q<Label>("version");
			var platformLabel = _view.Q<Label>("platform");
			var createLabel = _view.Q<Label>("createAt");
			var extendButton = _view.Q<Button>("extendButton");
			_removeButton = _view.Q<Button>("remove");
			var infoField = _view.Q<TextField>("descriptionInput");
			_uploadButton = _view.Q<Button>("upload");
			_root = _view.Q<VisualElement>("record");
			_infoBlock = _view.Q<VisualElement>("info");
			_serverBuildIcone = _view.Q<VisualElement>("serverIcone");
			_extendIcone = _view.Q<VisualElement>("extendIcone");
			_infoBlock.visible = _isExtend;
			versionLabel.text = Mat.Version;
			createLabel.text = Mat.CreateTime.ToString();
			platformLabel.text = Mat.Platform;
			infoField.value = Mat.Description;

			extendButton.clicked += ExtendButtonTouch;
			_uploadButton.clicked += UploadButtonTouch;
			//_updateButton.clicked += UpdateButtonTouch;
			//_removeButton.clicked += RemoveButtonTouch;
			_ = infoField.RegisterValueChangedCallback((dt) =>
			{
				Mat.Description = dt.newValue;
				//Mat.IsChangeDescription = true;
				LocalEditState();
				RecordFileInfo();
			});

			ConfirmVisibleElements();

			return _view;
		}

		private void RecordFileInfo()
		{
			string filePath = _window.BuildPath + @"\..\..\Builds\" + Mat.FileName.Remove(Mat.FileName.Length - 4) + ".txt";

			if (!File.Exists(filePath))
			{
				var stream = File.Create(filePath);
				stream.Close();
			}
			if (File.Exists(filePath))
				File.WriteAllText(filePath, Mat.Description);
		}

		private void ConfirmVisibleElements()
		{
			//_hardBuildIcone.visible = Mat.IsLocal;
			//_serverBuildIcone.visible = Mat.IsServer;
			//_uploadButton.visible = !Mat.IsServer && Mat.IsLocal;
			//_updateButton.visible = Mat.IsServer;
			//_removeButton.visible = Mat.IsServer;
			LocalEditState();
		}

		private void LocalEditState()
		{
			//_editIcone.visible = Mat.IsChangeDescription;
		}

		private void ExtendButtonTouch()
		{
			_isExtend = !_isExtend;

			_extendIcone.transform.rotation = Quaternion.Euler(0, 0, _isExtend ? 90 : 0);
			_root.style.height = _isExtend ? 125 : 25;
			_infoBlock.visible = _isExtend;
			_infoBlock.style.height = _isExtend ? 100 : 0;
		}

		private void UploadButtonTouch()
		{
			//string url = _window.ServerApi + "/releases";
			//List<KeyValuePair<string, string>> paramsData = new()
			//{
			//	new KeyValuePair<string, string>("version", Mat.Version),
			//	//TODO определить чексум
			//	new KeyValuePair<string, string>("checksum", ""),
			//	new KeyValuePair<string, string>("description", Mat.Description),
			//	new KeyValuePair<string, string>("data", "{}"),
			//	new KeyValuePair<string, string>("type", Mat.Type)
			//};
			//byte[] file = File.ReadAllBytes(Mat.FilePath);

			//UploadFileData fud = new()
			//{
			//	Bytes = File.ReadAllBytes(Mat.FilePath),
			//	FileName = Mat.FileName,
			//	ProgressTitle = $"Upload {Mat.Type}",
			//	ProgressDescription = "Please wait"
			//};

			//_window.WebRequest(HTTPMethods.Post, url, paramsData, "", fud, (isComplete, result) =>
			//{
			//	OnChangeUploadState?.Invoke();

			//	if (isComplete)
			//	{
			//		StringBuilder message = new();
			//		_ = message.Append(Mat.Type switch
			//		{
			//			ReleaseMaterialType.Launcher => $"#releaseLauncher Лаунчер опубликован. Версия {Mat.Version}",
			//			ReleaseMaterialType.StreamingAssets => $"#releaseWall Дополнительные активы опубликованы. Версия {Mat.Version}",
			//			_ => $"#releaseWall Видеостена опубликована. Версия {Mat.Version}"
			//		});
			//		_ = message.Append(string.IsNullOrEmpty(Mat.Description) ? "" : "\n\n<b>Описание:</b>\n" + Mat.Description);
			//		_window.TelegramController.Message(message.ToString());
			//		_ = message.Clear();
			//	}
			//	OnChangeUploadState?.Invoke();

			//}, _ =>
			//{

			//});
		}

		//private void UpdateButtonTouch()
		//{
		//	string url = _window.ServerApi + "/releases/" + Mat.Id;
		//	List<KeyValuePair<string, string>> paramsData = new()
		//	{
		//		new KeyValuePair<string, string>("version", Mat.Version),

		//		//TODO определить чексум
		//		//new KeyValuePair<string, string>("checksum", FileHelper.GetFileHash(Mat.FilePath)),
		//		new KeyValuePair<string, string>("description", Mat.Description),
		//		new KeyValuePair<string, string>("data", "{}"),
		//		new KeyValuePair<string, string>("type", Mat.Type)
		//	};

		//	UploadFileData fud = new()
		//	{
		//		Bytes = File.ReadAllBytes(Mat.FilePath),
		//		FileName = Mat.FileName,
		//		ProgressTitle = $"Update {Mat.Type}",
		//		ProgressDescription = "Please wait"
		//	};

		//	_window.WebRequest(HTTPMethods.Patch, url, paramsData, "", fud, (isComplete, result) =>
		//	{
		//		OnChangeUploadState?.Invoke();

		//		if (isComplete)
		//		{
		//			StringBuilder message = new();
		//			_ = message.Append(Mat.Type switch
		//			{
		//				ReleaseMaterialType.Launcher => $"#releaseLauncher Лаунчер обновлен. Версия {Mat.Version}",
		//				ReleaseMaterialType.StreamingAssets => $"#releaseWall Дополнительные активы обновлены. Версия {Mat.Version}",
		//				_ => $"#releaseWall Видеостена обновлена. Версия {Mat.Version}"
		//			});

		//			_ = message.Append(string.IsNullOrEmpty(Mat.Description) ? "" : "\n\n<b>Описание:</b>\n" + Mat.Description);
		//			_window.TelegramController.Message(message.ToString());
		//			_ = message.Clear();
		//		}
		//		OnChangeUploadState?.Invoke();

		//	}, (progressValue) =>
		//	{

		//	});
		//}

		//private void RemoveButtonTouch()
		//{
		//	string url = _window.ServerApi + "/releases/" + Mat.Id;
		//	_window.WebRequest(HTTPMethods.Delete, url, null, "", null, (isComplete, result) =>
		//	{
		//		if (isComplete)
		//		{
		//			StringBuilder message = new();
		//			_ = message.Append(Mat.Type switch
		//			{
		//				ReleaseMaterialType.Launcher => $"#releaseLauncher Лаунчер удален. Версия {Mat.Version}",
		//				ReleaseMaterialType.StreamingAssets => $"#releaseWall Дополнительные активы удалены. Версия {Mat.Version}",
		//				_ => $"#releaseWall Видеостена удалена. Версия {Mat.Version}"
		//			});
		//			_window.TelegramController.Message(message.ToString());
		//			_ = message.Clear();
		//		}

		//		OnChangeUploadState?.Invoke();
		//	});
		//}
	}
}