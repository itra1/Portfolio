using System.IO;
using Builder.Common;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Builder.Views
{
	public class BuildView : ViewBase
	{
		private TextField _versionInput;
		private Label _currentVersionLabel;
		private Label _currentPlatformLabel;
		private DropdownField _buildTargetDropdownField;
		private Label _buildPathLabel;
		private Toggle _archiveToggle;
		private Button _buildButton;
		private Button _archiveButton;
		private Toggle _developBuildToggle;
		private Toggle _cleanBuildToggle;
		private Toggle _connectWithProfilerToggle;
		private Toggle _deepProfilingSupportToggle;

		private string BuildPath => _buildData.Window.BuildPath;

		public override string Type => ViewsType.Build;

		public BuildView(BuildSession buildData) : base(buildData)
		{
		}

		protected override void LoadPrefab()
		{
			_viewPrefab ??= AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(BuilderWindow.BuildViewTemplate);
		}

		protected override void CreateUi()
		{
			base.CreateUi();
			_buildData.Version = Application.version;

			_currentPlatformLabel = _view.Q<Label>("currentPlatform");
			_currentVersionLabel = _view.Q<Label>("currentVersion");
			_versionInput = _view.Q<TextField>("versionInput");
			_buildPathLabel = _view.Q<Label>("buildPathLabel");
			_archiveToggle = _view.Q<Toggle>("archvateCheckbox");
			_buildTargetDropdownField = _view.Q<DropdownField>("buildTarget");
			FillBuildTargetDropdownField();

			_buildButton = _view.Q<Button>("buildButton");
			_archiveButton = _view.Q<Button>("archiveButton");

			int index = _view.IndexOf(_archiveToggle);

			_cleanBuildToggle = new Toggle("Clear Build");
			_ = _cleanBuildToggle.RegisterValueChangedCallback(value => ChangeBuildOptions(BuildOptions.CleanBuildCache, value.newValue));
			_view.Insert(++index, _cleanBuildToggle);

			_developBuildToggle = new Toggle("Develop Build");
			_ = _developBuildToggle.RegisterValueChangedCallback(value => ChangeBuildOptions(BuildOptions.Development, value.newValue));
			_view.Insert(++index, _developBuildToggle);

			_connectWithProfilerToggle = new Toggle("Connecting Profiler");
			_ = _connectWithProfilerToggle.RegisterValueChangedCallback(value => ChangeBuildOptions(BuildOptions.ConnectWithProfiler, value.newValue));
			_view.Insert(++index, _connectWithProfilerToggle);

			_deepProfilingSupportToggle = new Toggle("Deep Profiling Support");
			_ = _deepProfilingSupportToggle.RegisterValueChangedCallback(value => ChangeBuildOptions(BuildOptions.EnableDeepProfilingSupport, value.newValue));
			_view.Insert(++index, _deepProfilingSupportToggle);

			_currentPlatformLabel.text = $"Current platform: {_buildData.Platform}";
			_currentVersionLabel.text = $"Cerrent version: {Application.version}";
			_versionInput.value = _buildData.Version;

			_archiveToggle.value = _buildData.Settings.Archive;
			_ = _archiveToggle.RegisterValueChangedCallback(value =>
			{
				_buildData.Settings.Archive = value.newValue;
				_buildData.Settings.Save();
			});

			_ = _versionInput.RegisterValueChangedCallback(value =>
			{
				_buildData.Version = value.newValue;
				PrintBuildPath();
				ChangeButtonsTitles();
			});
			PrintBuildPath();
			ChangeButtonsTitles();

			ChangeBuildOptions(BuildOptions.Development, _developBuildToggle.value);

			_buildButton.clicked += BuildButtonTouch;
			_archiveButton.clicked += ArchiveButtonTouch;

			_buildData.Builder.AddBuildOptions(_view, index);
			_buildData.BodyElement.Add(_view);
		}

		private void ChangeButtonsTitles()
		{
			_buildButton.text = Application.version == _buildData.Version
			? "Build"
			: $"Build {Application.version} -> {_buildData.Version}";
		}

		private void ChangeBuildOptions(BuildOptions option, bool select)
		{
			if (select)
				_buildData.BuildOptions |= option;
			else
				_buildData.BuildOptions ^= option;

			if (option == BuildOptions.Development && select)
			{
				_connectWithProfilerToggle.enabledSelf = true;
				_deepProfilingSupportToggle.enabledSelf = true;
			}
			else if (option == BuildOptions.Development && !select)
			{
				_connectWithProfilerToggle.value = false;
				_deepProfilingSupportToggle.value = false;
				_connectWithProfilerToggle.enabledSelf = false;
				_deepProfilingSupportToggle.enabledSelf = false;
			}
		}

		private void BuildButtonTouch()
			=> Buildstart();

		private void ArchiveButtonTouch()
			=> _buildData.ArchivateHelper.Compress();

		/// <summary>
		/// Вывод пути к сборке
		/// </summary>
		private void PrintBuildPath()
		{
			_buildPathLabel.text = $"Build path: {BuildPath}";
		}

		private void FillBuildTargetDropdownField()
		{
			_buildTargetDropdownField.choices.Clear();
			_buildTargetDropdownField.choices.Add("Window64");
			_buildTargetDropdownField.choices.Add("WebGL");
		}

		public void Buildstart()
		{
			_buildData.Builder.Build();
		}

		public void OnPreprocessBuild(BuildReport report)
		{
			if (Directory.Exists(BuildPath))
				Directory.Delete(BuildPath, true);

			ReadyUILibrary();

			Debug.Log("OnPreprocessBuild " + report.summary.outputPath);
		}

		public void OnPostprocessBuild(BuildReport _)
		{
			ChangeButtonsTitles();
		}

		/// <summary>
		/// Копирование дополнительных внешних библиотек в каталог с exe
		/// </summary>
		private void CopyLibs()
		{

			string dirWithLibs = Application.dataPath + "/../AddLibs";

			string[] files = Directory.GetFiles(dirWithLibs);
			for (int i = 0; i < files.Length; i++)
			{
				string[] fileSplit = files[i].Split(new char[] { '/', '\\' });
				string fileName = fileSplit[^1];

				string newFilePath = Path.Combine(BuildPath, fileName);

				if (!File.Exists(newFilePath))
					File.Copy(files[i], newFilePath);
			}
		}

		private void ReadyUILibrary()
		{
			//var _library = UnityEngine.Resources.Load<CnpOS.UI.UILibrary>(CnpOS.Core.ProjectSettings.GuiLibrary);
			//_library.FindObjects();
			//EditorUtility.SetDirty(_library);
		}
	}
}