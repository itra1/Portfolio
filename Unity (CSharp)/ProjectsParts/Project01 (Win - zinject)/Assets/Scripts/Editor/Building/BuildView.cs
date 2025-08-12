using System.IO;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Editor.Building
{
	public class BuildView
	{
		private readonly BuildWindow _window;

		public BuildView(BuildWindow window)
		{
			_window = window;
		}

		private static bool _build = false;
		private string BuildPath => _window.BuildPath;
		private string StreamingAssetsPath => BuildPath + @"/VideoWall_Data/StreamingAssets";

		public void Buildstart(bool cleanCache = false)
		{
			string path = BuildPath + @"/VideoWall.exe";
			PlayerSettings.bundleVersion = _window.Version;
			//EditorPrefs.SetBool(_epBuildDevServer, _devServer);
			//EditorPrefs.SetString(_epArchivePath, _archivePath);
			//EditorPrefs.SetString(_epBuildPath, _buildPath);
			if (Directory.Exists(BuildPath))
				Directory.Delete(BuildPath, true);

			_build = true;

			BuildOptions options = BuildOptions.None;

			if (cleanCache)
				options |= BuildOptions.CleanBuildCache;

			_ = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, path, BuildTarget.StandaloneWindows64, options);
		}

		public void OnPostprocessBuild(BuildReport report)
		{
			if (!_build)
				return;

			OnPostprocessBuildAsync().Forget();


		}

		private async UniTaskVoid OnPostprocessBuildAsync()
		{

			await UniTask.Delay(1000);
			MoveBat();
			RemoveEditorConfig();
			//CopyLibs();
			RenameExe();
			RemoveCrashHandler();
			await RemoveBackUpFolder();
			CopyDistrib();

			_build = false;
		}

		public void OnPreprocessBuild(BuildReport report)
		{
			if (Directory.Exists(BuildPath))
				Directory.Delete(BuildPath, true);

			ReadyUILibrary();

			Debug.Log("OnPreprocessBuild " + report.summary.outputPath);
		}
		private void RemoveEditorConfig()
		{
			if (File.Exists(StreamingAssetsPath + "/configEditor.txt"))
				File.Delete(StreamingAssetsPath + "/configEditor.txt");
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

		private void RenameExe()
		{

			string source = BuildPath + "/VideoWall";
			string target = BuildPath + "/VideoWall.exe";

			if (File.Exists(source) && !File.Exists(target))
				File.Move(source, target);
		}

		private void RemoveCrashHandler()
		{
			string crachHandler = BuildPath + "/UnityCrashHandler64.exe";
			if (File.Exists(crachHandler))
				File.Delete(crachHandler);
		}

		private async UniTask RemoveBackUpFolder()
		{
			Debug.Log("RemoveBackUpFolder = " + BuildPath);
			string[] dirs = Directory.GetDirectories(BuildPath);
			for (int i = 0; i < dirs.Length; i++)
			{

				if (dirs[i].Contains("BackUpThisFolder"))
				{

					int trying = 0;
					bool isDel = false;

					while (!isDel && trying < 5)
					{
						try
						{
							trying++;
							isDel = true;
							Directory.Delete(dirs[i], true);
						}
						catch (IOException)
						{
							isDel = false;
							if (trying == 5)
								Debug.LogError("IOException, Ошибка удаления каталога BackUpThisFolder ");
							await UniTask.Delay(100);
						}
					}
				}
				if (dirs[i].Contains("DoNotShip"))
				{

					int trying = 0;
					bool isDel = false;
					while (!isDel && trying < 5)
					{
						try
						{
							trying++;
							isDel = true;
							Directory.Delete(dirs[i], true);
						}
						catch (IOException)
						{
							isDel = false;
							if (trying == 5)
								Debug.LogError("IOException, Ошибка удаления каталога DoNotShip ");
							await UniTask.Delay(100);
						}
					}
				}
			}
		}

		private void CopyDistrib()
		{
			string distrubPath = Path.Combine(Application.dataPath, "..", "Distrib");
			string targetPath = Path.Combine(BuildPath, "Distrib");

			if (!Directory.Exists(distrubPath))
			{
				Debug.LogError("Отсутствует папка дистрибутива");
				return;
			}

			if (!Directory.Exists(targetPath))
				_ = Directory.CreateDirectory(targetPath);

			string[] files = Directory.GetFiles(distrubPath);

			for (int i = 0; i < files.Length; i++)
			{
				string[] fileSplit = files[i].Split(new char[] { '/', '\\' });
				string fileName = fileSplit[^1];

				string newFilePath = Path.Combine(targetPath, fileName);

				if (!File.Exists(newFilePath))
					File.Copy(files[i], newFilePath);
			}
		}

		private void MoveBat()
		{
			string source = Application.dataPath + @"\..\" + "WallRun.bat";
			string target = BuildPath + "/WallRun.bat";

			if (!File.Exists(target))
				File.Copy(source, target);
		}

	}
}