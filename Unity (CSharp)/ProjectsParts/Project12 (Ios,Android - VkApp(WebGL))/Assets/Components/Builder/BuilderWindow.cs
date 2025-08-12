using System.Collections.Generic;
using System.Threading.Tasks;
using Builder.Common;
using Builder.Controllers;
using Builder.Platforms;
using Builder.Views;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Builder
{
	public class BuilderWindow : EditorWindow, IPostprocessBuildWithReport, IPreprocessBuildWithReport
	{
		public static BuilderWindow BuilderWindowInstance { get; set; }

		public const string Menu = "App/Builder";
		public const string WindowTitle = "Builder";

		public const string BuildWindowTemplate = "Assets/Components/Builder/Templates/BuilderWindow.uxml";
		public const string BuildViewTemplate = "Assets/Components/Builder/Templates/BuildView.uxml";
		public const string ReleasesViewTemplate = "Assets/Components/Builder/Templates/ReleasesView.uxml";
		public const string TelegramViewTemplate = "Assets/Components/Builder/Templates/TelegramView.uxml";
		public const string SettingsViewTemplate = "Assets/Components/Builder/Templates/SettingsView.uxml";
		public const string BuildRecordTemplate = "Assets/Components/Builder/Templates/BuildRecord.uxml";
		public const string TelegramTemplate = "Assets/Components/Builder/Templates/TelegramChatRecord.uxml";

		private const string TabButtonActiveClass = "tabButtonActive";

		public string _activeTab;

		private BuildSession _buildData;
		private List<ViewControllerBbase> _viewsList;
		private Dictionary<string, Button> _navigationButtons;

		public string BuildPath
			=> $"{_buildData.Settings.BuildPath}/{_buildData.Platform}_{_buildData.Version.Replace('.', '_')}";
		public int callbackOrder => 0;
		public bool Archivate => _buildData.Settings.Archive;
		public VisualElement Root => rootVisualElement;
		public TelegramView TelegramController { get; set; }

		[MenuItem(Menu)]
		public static void ShowExample()
		{
			var window = GetWindow<BuilderWindow>();
			window.titleContent = new GUIContent(WindowTitle);
		}

		public void CreateGUI()
		{
			_activeTab = ViewsType.Build;
			_viewsList = new();
			_navigationButtons = new();
			_ = CreateUiAsync();
		}

		private void Update()
		{
			if (_buildData.RootElement != null)
			{
				_buildData.RootElement.style.height = position.height;
				_buildData.BodyElement.style.height = position.height;
			}
		}

		private void OnDestroy()
		{
			BuilderWindowInstance = null;
		}
		private void OnBecameVisible()
		{
			BuilderWindowInstance = this;
		}

		private async Task Init()
		{
			_ = StaticContext.Container;

			while (rootVisualElement.childCount == 0)
				await Task.Delay(10);

			var rootElement = rootVisualElement.Q<VisualElement>("root");
			var bodyElement = rootVisualElement.Q<VisualElement>("body");

			_buildData ??= new()
			{
				Window = this,
				Version = Application.version,
				Settings = Settings.Load(),
				BodyElement = bodyElement,
				RootElement = rootElement,
				Platform = GetCurrentPlatform()
			};
			_buildData.Builder ??= CreateBuilder(_buildData);
			_buildData.ArchivateHelper = new(_buildData);

			if (_viewsList.Count == 0)
			{
				_viewsList.Add(new BuildViewController(_buildData));
				_viewsList.Add(new ReleasesViewController(_buildData));
				_viewsList.Add(new TelegramViewController(_buildData));
				_viewsList.Add(new SettingsViewController(_buildData));
			}
		}

		private string GetCurrentPlatform()
		{
#if UNITY_WEBGL
			return "WebGL";
#else
			return "No supported";
#endif
		}

		private PlatformBuilder CreateBuilder(BuildSession buildData)
		{
#if UNITY_WEBGL
			return new WebGlPlatformBuilder(buildData);
#else
			throw new System.NotImplementedException("No supported");
#endif
		}

		private async Task CreateUiAsync()
		{
			VisualElement root = rootVisualElement;

			// Import UXML
			var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(BuildWindowTemplate);

			VisualElement labelFromUxml = visualTree.Instantiate();

			root.Add(labelFromUxml);

			await Init();

			var buildTabButton = root.Q<Button>("buildTabButton");
			buildTabButton.clicked += BuildTabButtonTouch;

			var releasesTabButton = root.Q<Button>("releasesTabButton");
			releasesTabButton.clicked += ReleasesTabButtonTouch;

			var telegramTabButton = root.Q<Button>("telegramTabButton");
			telegramTabButton.clicked += TelegramTabButtonTouch;

			var settingsTabButton = root.Q<Button>("settingsTabButton");
			settingsTabButton.clicked += SettingsTabButtonTouch;

			_navigationButtons.Add(ViewsType.Build, buildTabButton);
			_navigationButtons.Add(ViewsType.Releases, releasesTabButton);
			_navigationButtons.Add(ViewsType.Telegram, telegramTabButton);
			_navigationButtons.Add(ViewsType.Settings, settingsTabButton);

			BuildTabButtonTouch();
		}

		#region build process events

		public void OnPreprocessBuild(BuildReport report)
		{
			_ = BuilderWindowInstance?.OnPreprocessBuildAsync(report);
		}

		public void OnPostprocessBuild(BuildReport report)
		{
			_ = BuilderWindowInstance?.OnPostprocessBuildAsync(report);
		}

		private async Task OnPreprocessBuildAsync(BuildReport report)
		{
			await Init();
			_buildData.Builder.OnPreprocessBuild(report);
		}

		private async Task OnPostprocessBuildAsync(BuildReport report)
		{
			await Init();

			_buildData.Builder.OnPostprocessBuild(report);

			if (report.summary.totalErrors == 0)
			{
				Debug.Log("Build complete");

				if (_buildData.Settings.Archive)
					_buildData.ArchivateHelper.Compress();
			}
			else
			{
				Debug.Log($"OnPostprocessBuild {report.summary.totalErrors}");
			}

			//(_viewsList.Find(x => x.Type == ViewsType.Build) as BuildView).OnPostprocessBuild(report);
		}

		#endregion

		private void VisibleView(string viewName)
		{
			_viewsList.ForEach(x => x.SetVisible(x.Type == viewName));

			foreach (var item in _navigationButtons)
			{
				if (item.Key == viewName)
					item.Value.AddToClassList(TabButtonActiveClass);
				else
					item.Value.RemoveFromClassList(TabButtonActiveClass);
			}
		}

		private void BuildTabButtonTouch() => VisibleView(ViewsType.Build);
		private void ReleasesTabButtonTouch() => VisibleView(ViewsType.Releases);
		private void SettingsTabButtonTouch() => VisibleView(ViewsType.Settings);
		private void TelegramTabButtonTouch() => VisibleView(ViewsType.Telegram);

		//public void WebRequest(
		//	HTTPMethods requestType,
		//	string url,
		//	List<KeyValuePair<string, string>> paramsData,
		//	string stringData,
		//	UploadFileData uploadFile,
		//	Action<bool, string> onFinish = null,
		//	Action<float> onProgress = null
		//	)
		//{
		//	var request = new HTTPRequest(new Uri(url), requestType, false, (req, resp) =>
		//	{
		//		EditorUtility.ClearProgressBar();

		//		switch (req.State)
		//		{
		//			// The request finished without any problem.
		//			case HTTPRequestStates.Finished:
		//				if (resp.IsSuccess)
		//				{
		//					onFinish?.Invoke(true, resp.DataAsText);
		//					Debug.Log("OnComplete");
		//				}
		//				else
		//				{
		//					Debug.Log($"Request finished Successfully, but the server sent an error. Status Code: {resp.StatusCode}-{resp.Message} Message: {resp.DataAsText} {req.Uri.AbsoluteUri}");
		//				}
		//				break;

		//			// The request finished with an unexpected error. The request's Exception property may contain more info about the error.
		//			case HTTPRequestStates.Error:
		//				onFinish?.Invoke(false, req.Exception.Message);
		//				Debug.LogError("Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));
		//				break;

		//			// The request aborted, initiated by the user.
		//			case HTTPRequestStates.Aborted:
		//				onFinish?.Invoke(false, "Request Aborted!");
		//				Debug.Log("Request Aborted!");
		//				break;

		//			// Connecting to the server is timed out.
		//			case HTTPRequestStates.ConnectionTimedOut:
		//				onFinish?.Invoke(false, "Connection Timed Out!");
		//				Debug.LogError("Connection Timed Out!");
		//				break;

		//			// The request didn't finished in the given time.
		//			case HTTPRequestStates.TimedOut:
		//				onFinish?.Invoke(false, "Processing the request Timed Out!");
		//				Debug.LogError("Processing the request Timed Out!");
		//				break;
		//		}
		//	});

		//	Debug.Log("Request " + request.Uri.AbsoluteUri);

		//	if (uploadFile != null)
		//	{
		//		request.MaxFragmentQueueLength = 2048;
		//		request.Timeout = TimeSpan.FromMinutes(100.0);
		//		request.AddBinaryData("file", uploadFile.Bytes, uploadFile.FileName, "application/zip");
		//	}

		//	if (!string.IsNullOrEmpty(stringData))
		//	{
		//		Debug.Log($"Body {stringData}");
		//		request.RawData = Encoding.UTF8.GetBytes(stringData);
		//	}

		//	if (paramsData is { Count: > 0 })
		//	{
		//		foreach (var itm in paramsData)
		//		{
		//			Debug.Log(itm.Key + " : " + itm.Value);
		//			request.AddField(itm.Key, itm.Value);
		//		}
		//	}

		//	if (onProgress != null)
		//	{
		//		request.OnUploadProgress += (_, down, length) =>
		//		{
		//			if (uploadFile != null)
		//			{
		//				var isCancel = EditorUtility.DisplayCancelableProgressBar(uploadFile.ProgressTitle, uploadFile.ProgressDescription, down / (float) length);

		//				onProgress.Invoke(down / (float) length);

		//				if (isCancel)
		//					request.Abort();
		//			}
		//		};
		//	}

		//	request.Send();
		//}
	}

	public class UploadFileData
	{
		public string FileName;
		public byte[] Bytes;
		public string ProgressTitle;
		public string ProgressDescription;
	}
}