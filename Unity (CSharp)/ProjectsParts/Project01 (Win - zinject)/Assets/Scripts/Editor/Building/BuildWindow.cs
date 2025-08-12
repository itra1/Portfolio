using System;
using System.Collections.Generic;
using System.Text;
using BestHTTP;
using Core.Configs;
using Core.Configs.Consts;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Editor.Building
{

	public class BuildWindow : EditorWindow, IPostprocessBuildWithReport, IPreprocessBuildWithReport
	{
		private const string TabButtonActiveClass = "tabButtonActive";
		
		private static string _token;
		
		private BuildWindowSettings _settings;
		private BuildView _buildController;
		private Archivate _archivateController;
		private ReleasesView _resourcesController;
		
		private string _version;
		private TextField _versionInput;
		private TextField _buildPathSettingsInput;
		private Label _buildPathLabel;
		private VisualElement _root;
		private VisualElement _buildTabBody;
		private VisualElement _releasesTabBody;
		private VisualElement _telegramTabBody;
		private VisualElement _settingsTabBody;
		private Button _buildTabButton;
		private Button _releasesTabButton;
		private Button _telegramTabButton;
		private Button _settingsTabButton;
		private Button _buildButton;
		private Button _archiveSAButton;
		private Button _archiveBaseButton;
		private Button _archiveButton;
		private Button _uploadbaseButton;
		private Button _uploadSAButton;
		private Button _uploadButton;
		private Button _getResourcesButton;
		private Toggle _archiveToggle;
		private Toggle _removeSAToggle;
		private Toggle _archiveSAToggle;
		
		public string Version => _version;
		public string BuildPath => $"{_settings.BuildPath}VideoWall_{_version.Replace('.', '_')}";
		public int callbackOrder => 0;
		public bool Archivate => _settings.Archive;
		public bool ArchivateRemoveStreaminAsset => _settings.RemoveSA;
		public bool ArchivateStreamingAssets => _settings.ArchiveSA;
		public VisualElement Root => rootVisualElement;
		public string Token { get; set; } = _token;
		public string Role => "VideoWall";
		public string Server { get; set; }
		public string Login { get; set; }
		public string Password { get; set; }
		public string ServerApi => $"{Server}/api2";
		
		public TelegramView TelegramController { get; set; }

		[MenuItem("CnpOS/Build Window")]
		public static void ShowExample()
		{
			var window = GetWindow<BuildWindow>();
			window.titleContent = new GUIContent("Build CnpOS");
		}

		private void Update()
		{
			if (_root != null)
				_root.style.height = position.height;
		}

		private void Init()
		{
			var container = StaticContext.Container;
			_settings = BuildWindowSettings.Load();
			
			var config = container.TryResolve<IConfig>();
			Server = config.GetValue(ConfigKey.HttpServer);
			Login = config.GetValue(ConfigKey.Login);
			Password = config.GetValue(ConfigKey.Password);
			
			_buildController ??= new BuildView(this);
			_archivateController ??= new Archivate(this);
			TelegramController ??= new TelegramView(this, _settings);
			_resourcesController ??= new ReleasesView(this);
			_version = Application.version;
		}

		public void CreateGUI()
		{
			VisualElement root = rootVisualElement;
			
			Init();
			
			//// VisualElements objects can contain other VisualElement following a tree hierarchy.
			//VisualElement label = new Label("Hello World! From C#");
			//root.Add(label);

			//A stylesheet can be added to a VisualElement.
			// The style will be applied to the VisualElement and all of its children.
			//var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/Building/BuildWindowNew.uss");
			//VisualElement labelWithStyle = new Label("Hello World! With Style");
			//labelWithStyle.styleSheets.Add(styleSheet);
			
			// Import UXML
			var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/Building/BuildWindow.uxml");
			
			VisualElement labelFromUxml = visualTree.Instantiate();
			
			root.Add(labelFromUxml);

			_root = root.Q<VisualElement>("root");

			_versionInput = root.Q<TextField>("VersionInput");

			_buildTabBody = root.Q<VisualElement>("buildTab");
			_releasesTabBody = root.Q<VisualElement>("releasesTab");
			_telegramTabBody = root.Q<VisualElement>("telegramTab");
			_settingsTabBody = root.Q<VisualElement>("settingsTab");

			_buildTabButton = root.Q<Button>("buildTabButton");
			_releasesTabButton = root.Q<Button>("releasesTabButton");
			_telegramTabButton = root.Q<Button>("telegramTabButton");
			_settingsTabButton = root.Q<Button>("settingsTabButton");
			
			_buildButton = root.Q<Button>("buildButton");
			var buildClearButton = root.Q<Button>("buildClearButton");
			_archiveSAButton = root.Q<Button>("archiveSAButton");
			_archiveBaseButton = root.Q<Button>("archiveBaseButton");
			_archiveButton = root.Q<Button>("archiveButton");
			_uploadbaseButton = root.Q<Button>("uploadBaseButton");
			_uploadSAButton = root.Q<Button>("uploadSAAssetsButton");
			_uploadButton = root.Q<Button>("UploadAllButton");
			_getResourcesButton = root.Q<Button>("getResourcesButton");

			_buildPathLabel = root.Q<Label>("buildPathLabel");

			_archiveToggle = root.Q<Toggle>("archvateCheckbox");
			_removeSAToggle = root.Q<Toggle>("removeSACheckbox");
			_archiveSAToggle = root.Q<Toggle>("archivateSACheckbox");

			_buildPathSettingsInput = root.Q<TextField>("buildPath");

			_versionInput.value = _version;
			_versionInput.RegisterValueChangedCallback(value =>
			{
				_version = value.newValue;
				PrintBuildPath();
			});
			
			PrintBuildPath();

			_archiveToggle.value = _settings.Archive;
			_archiveToggle.RegisterValueChangedCallback(value =>
			{
				_settings.Archive = value.newValue;
				_settings.Save();
			});
			
			_archiveSAToggle.value = _settings.ArchiveSA;
			_archiveSAToggle.RegisterValueChangedCallback(value =>
			{
				_settings.ArchiveSA = value.newValue;
				_settings.Save();
			});
			
			_removeSAToggle.value = _settings.RemoveSA;
			_removeSAToggle.RegisterValueChangedCallback(value =>
			{
				_settings.RemoveSA = value.newValue;
				_settings.Save();
			});

			_buildTabButton.clicked += BuildTabButtonTouch;
			_releasesTabButton.clicked += UploadTabButtonTouch;
			_telegramTabButton.clicked += TelegramTabButtonTouch;
			_settingsTabButton.clicked += SettingsTabButtonTouch;
			_archiveButton.clicked += ArchiveButtonTouch;
			_archiveBaseButton.clicked += ArchiveBaseButtonTouch;
			_archiveSAButton.clicked += ArchiveSAButtonButtonTouch;

			_buildButton.clicked += () =>
			{
				_buildController.Buildstart();
			};
			
			buildClearButton.clicked += () =>
			{
				_buildController.Buildstart(true);
			};

			_buildPathSettingsInput.value = _settings.BuildPath;
			_buildPathSettingsInput.RegisterValueChangedCallback(value =>
			{
				_settings.BuildPath = value.newValue;
				_settings.Save();
				
				PrintBuildPath();
			});
			
			BuildTabButtonTouch();
		}

		#region build process events

		public void OnPostprocessBuild(BuildReport report)
		{
			Init();
			
			_buildController.OnPostprocessBuild(report);

			if (report.summary.totalErrors == 0)
			{
				Debug.Log("Build complete");
				
				if (_settings.Archive)
					ArchiveButtonTouch();
			}
			else
			{
				Debug.Log($"OnPostprocessBuild {report.summary.totalErrors}");
			}

		}

		public void OnPreprocessBuild(BuildReport report)
		{
			Init();
			
			_buildController.OnPreprocessBuild(report);
		}

		#endregion
		
		/// <summary>
		/// Вывод пути к сборке
		/// </summary>
		private void PrintBuildPath()
		{
			_buildPathLabel.text = $"Build path: {BuildPath}";
		}
		
		/// <summary>
		/// Отключение всех вкладок
		/// </summary>
		private void DisableAllTabs()
		{
			_buildTabBody.style.display = DisplayStyle.None;
			_settingsTabBody.style.display = DisplayStyle.None;
			_releasesTabBody.style.display = DisplayStyle.None;
			_telegramTabBody.style.display = DisplayStyle.None;
			_buildTabButton.RemoveFromClassList(TabButtonActiveClass);
			_settingsTabButton.RemoveFromClassList(TabButtonActiveClass);
			_releasesTabButton.RemoveFromClassList(TabButtonActiveClass);
			_telegramTabButton.RemoveFromClassList(TabButtonActiveClass);
		}
		
		/// <summary>
		/// Включение вкладки сборки
		/// </summary>
		private void BuildTabButtonTouch()
		{
			DisableAllTabs();
			_buildTabBody.style.display = DisplayStyle.Flex;
			_buildTabButton.AddToClassList(TabButtonActiveClass);
		}
		
		/// <summary>
		/// Включение вкладки архивации
		/// </summary>
		private void SettingsTabButtonTouch()
		{
			DisableAllTabs();
			_settingsTabBody.style.display = DisplayStyle.Flex;
			_settingsTabButton.AddToClassList(TabButtonActiveClass);
		}
		
		/// <summary>
		/// Включение вкладки загрузки
		/// </summary>
		private void UploadTabButtonTouch()
		{
			DisableAllTabs();
			_releasesTabBody.style.display = DisplayStyle.Flex;
			_releasesTabButton.AddToClassList(TabButtonActiveClass);
			_resourcesController.Draw();
		}
		
		/// <summary>
		/// Включение вкладки телеграма
		/// </summary>
		private void TelegramTabButtonTouch()
		{
			DisableAllTabs();
			_telegramTabBody.style.display = DisplayStyle.Flex;
			_telegramTabButton.AddToClassList(TabButtonActiveClass);
			TelegramController.VisibleRecords();
		}
		
		/// <summary>
		/// Архивация сборки
		/// </summary>
		private void ArchiveButtonTouch()
		{
			_archivateController.Compress();
		}
		/// <summary>
		/// архивация базовой части сборки
		/// </summary>
		private void ArchiveBaseButtonTouch()
		{
			_archivateController.ComplessionBase();
		}
		
		/// <summary>
		/// Архивация SA проекта
		/// </summary>
		private void ArchiveSAButtonButtonTouch()
		{
			_archivateController.CompressionStreamingAssets(null);
		}
		
		public void WebRequest(HTTPMethods requestType, 
			string url, 
			List<KeyValuePair<string, string>> paramsData, 
			string stringData, 
			UploadFileData uploadFile, 
			Action<bool, string> onFinish = null, 
			Action<float> onProgress = null)
		{
			var request = new HTTPRequest(new Uri(url), requestType, false, (req, resp) =>
			{
				EditorUtility.ClearProgressBar();
				
				switch (req.State)
				{
					// The request finished without any problem.
					case HTTPRequestStates.Finished:
						if (resp.IsSuccess)
						{
							onFinish?.Invoke(true, resp.DataAsText);
							Debug.Log("OnComplete");
						}
						else
						{
							Debug.Log($"Request finished Successfully, but the server sent an error. Status Code: {resp.StatusCode}-{resp.Message} Message: {resp.DataAsText} {req.Uri.AbsoluteUri}");
						}
						break;

					// The request finished with an unexpected error. The request's Exception property may contain more info about the error.
					case HTTPRequestStates.Error:
						onFinish?.Invoke(false, req.Exception.Message);
						Debug.LogError("Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));
						break;

					// The request aborted, initiated by the user.
					case HTTPRequestStates.Aborted:
						onFinish?.Invoke(false, "Request Aborted!");
						Debug.Log("Request Aborted!");
						break;

					// Connecting to the server is timed out.
					case HTTPRequestStates.ConnectionTimedOut:
						onFinish?.Invoke(false, "Connection Timed Out!");
						Debug.LogError("Connection Timed Out!");
						break;

					// The request didn't finished in the given time.
					case HTTPRequestStates.TimedOut:
						onFinish?.Invoke(false, "Processing the request Timed Out!");
						Debug.LogError("Processing the request Timed Out!");
						break;
				}
			});
			
			Debug.Log("Request " + request.Uri.AbsoluteUri);

			if (url.Contains(ServerApi) && !string.IsNullOrEmpty(Token))
			{
				request.AddHeader("Authorization", $"Bearer {Token}");
			}
			
			if (uploadFile != null)
			{
				request.MaxFragmentQueueLength = 2048;
				request.Timeout = TimeSpan.FromMinutes(100.0);
				request.AddBinaryData("file", uploadFile.Bytes, uploadFile.FileName, "application/zip");
			}

			if (!string.IsNullOrEmpty(stringData))
			{
				Debug.Log($"Body {stringData}");
				request.RawData = Encoding.UTF8.GetBytes(stringData);
			}
			
			if (paramsData is { Count: > 0 })
			{
				foreach (var itm in paramsData)
				{
					Debug.Log(itm.Key + " : " + itm.Value);
					request.AddField(itm.Key, itm.Value);
				}
			}
			
			if (onProgress != null)
			{
				request.OnUploadProgress += (_, down, length) =>
				{
					if (uploadFile != null)
					{
						var isCancel = EditorUtility.DisplayCancelableProgressBar(uploadFile.ProgressTitle, uploadFile.ProgressDescription, down / (float) length);
						
						onProgress.Invoke(down / (float) length);
						
						if (isCancel)
							request.Abort();
					}
				};
			}
			
			request.Send();
		}

	}

	public class UploadFileData
	{
		public string FileName;
		public byte[] Bytes;
		public string ProgressTitle;
		public string ProgressDescription;
	}
}