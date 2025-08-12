using System.Collections;
using System.Collections.Generic;
using System.IO;
using AddressablesManager.Editor.Helpers;
using Engine.Scripts.Managers.Libraries;
using Engine.Scripts.Timelines;
using Ftp;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Playables;

namespace AddressablesManager.Editor
{
	public class AddressableBuildWindow : EditorWindow
	{
		private const string SettingsPath = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";
		private const string BuildScriptPath = "Assets/AddressableAssetsData/DataBuilders/BuildScriptPackedMode.asset";
		private const string AppSettingsPath = "Assets/Components/AddressablesManager/Editor/BuildAddressablesSettings.asset";
		private const string AddressableResourcesPath = "Assets/Resources_moved";
		private const string TargetProfileName = "WebGL";

		private List<TimelinesLibrary> _timelinesLibrarys = new();
		private AddressableAssetSettings _addressableSettings;
		private BuildAddressablesSettings _buildSettings;
		private AddressableAssetProfileSettings _profileSettings;
		private FtpClient _ftpClient;
		private CatalogHelper _catalogHelper;
		private IDataBuilder _dataBuilder;
		private string _targetProfileId;
		private bool _processUpload = false;
		private bool _uploadData = true;
		public string LocalAddressablePath => Application.dataPath + "/../ServerData";
		public AddressableAssetSettings AddressableSettings => _addressableSettings;
		public BuildAddressablesSettings BuildSettings => _buildSettings;

		[MenuItem("App/Addressables Build")]
		public static void OpenSetting()
		{
			var w = ScriptableObject.CreateInstance<AddressableBuildWindow>();
			w.Show();
		}

		private void OnEnable()
		{
			_buildSettings = AssetDatabase.LoadAssetAtPath<ScriptableObject>(AppSettingsPath) as BuildAddressablesSettings;
			_addressableSettings = AddressableAssetSettingsDefaultObject.GetSettings(true);
			_dataBuilder = AssetDatabase.LoadAssetAtPath<ScriptableObject>(BuildScriptPath) as IDataBuilder;
			_ftpClient = new(_buildSettings.Ftp.Host, _buildSettings.Ftp.UserName, _buildSettings.Ftp.Password);

			_profileSettings = _addressableSettings.profileSettings;
			_targetProfileId = _profileSettings.GetProfileId(TargetProfileName);
			_addressableSettings.activeProfileId = _targetProfileId;
		}

		private void OnGUI()
		{
			titleContent = new GUIContent("Build Addressables");

			EditorGUILayout.Space(10);

			var newPlayerVersion = EditorGUILayout.TextField("Имя профиля", _addressableSettings.OverridePlayerVersion);

			if (newPlayerVersion != _addressableSettings.OverridePlayerVersion)
			{
				_addressableSettings.OverridePlayerVersion = newPlayerVersion;
				_profileSettings.SetValue(_targetProfileId, "Remote.LoadPath", GetBundlePath(_addressableSettings.OverridePlayerVersion));
				_profileSettings.SetValue(_targetProfileId, "Local.LoadPath", GetBundlePath(_addressableSettings.OverridePlayerVersion));
			}

			var remoteLocalpath = _profileSettings.GetValueByName(_targetProfileId, "Remote.LoadPath");
			var localLocalpath = _profileSettings.GetValueByName(_targetProfileId, "Local.LoadPath");

			EditorGUILayout.LabelField($" Remote.LoadPath {remoteLocalpath}");
			EditorGUILayout.LabelField($" Local.LoadPath {localLocalpath}");

			EditorGUILayout.Space(10);

			_uploadData = EditorGUILayout.Toggle("Upload bundles", _uploadData);

			EditorGUILayout.Space(10);

			if (GUILayout.Button($"Собрать бандлы{(_uploadData ? " и отправить" : "")}"))
			{
				if (_processUpload)
					return;

				if (_addressableSettings == null)
				{
					Debug.LogError("Настройки Addressables не найдены");
					return;
				}
				if (_dataBuilder == null)
				{
					Debug.LogError("Данные сборки не найдены");
					return;
				}

				_processUpload = true;
				var proc = Process();

				while (proc.MoveNext())
				{ }

				_processUpload = false;
			}
		}

		private string GetBundlePath(string key)
			=> $"https://netarchitect.ru/realgames/addressable_musictap/{key}/[BuildTarget]";

		private IEnumerator Process()
		{
			_catalogHelper ??= new(this, _ftpClient);

			var loadCatalogProcess = _catalogHelper.LoadCatalog();

			while (loadCatalogProcess.MoveNext())
				yield return null;

			Assert.IsTrue(CheckCurrentPlatform(), "Не корректная платформа");

			Debug.Log("Удаление ссылок на сцене...");
			RemoveSceneLink();
			yield return new WaitForSeconds(0.3f);

			Debug.Log("Проверка треков...");
			LoadTimelinesLibrarys();
			Assert.IsFalse(CheckSongsUuid(), "Ошибка! Имеются дубликатыы UUID треков");
			CheckSongs();
			yield return new WaitForSeconds(0.3f);

			Debug.Log("Удаление ранее собранных бандлов...");
			RemoveExistsLocalBuild();
			yield return new WaitForSeconds(0.3f);

			Debug.Log("Сборка бандлов...");
			BuildAddressable();
			yield return new WaitForSeconds(0.3f);

			if (_uploadData)
			{

				Debug.Log("Удаление бандлов на сервере..");
				RemoveExistsDataFromServer();
				yield return new WaitForSeconds(0.3f);

				Debug.Log("Загружаем бандлы на сервер...");
				UploadData();
				yield return new WaitForSeconds(0.3f);

				_catalogHelper.MakeCatalog();
				_catalogHelper.UploadCatalog();
			}

			Debug.Log("Готово!");
		}

		private bool CheckCurrentPlatform()
		{
#if UNITY_WEBGL
			return true;
#else
			return false;
#endif
		}

		private void RemoveSceneLink()
		{
			var playableDirector = GameObject.FindAnyObjectByType<PlayableDirector>();
			if (playableDirector != null)
			{
				if (playableDirector.playableGraph.IsValid())
				{
					playableDirector.playableGraph.Stop();
				}
				playableDirector.Stop();
				playableDirector.playableAsset = null;
				playableDirector.RebindPlayableGraphOutputs();
				playableDirector.ClearGenericBinding(null);
			}
		}

		private void LoadTimelinesLibrarys()
		{
			_timelinesLibrarys.Clear();

			string[] guids = AssetDatabase.FindAssets("*", new string[] { AddressableResourcesPath });

			foreach (var guid in guids)
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);
				var type = AssetDatabase.GetMainAssetTypeAtPath(path);

				if (type != typeof(TimelinesLibrary))
					continue;

				_timelinesLibrarys.Add(AssetDatabase.LoadAssetAtPath<TimelinesLibrary>(path));
			}
		}

		private bool CheckSongsUuid()
		{
			bool result = false;

			List<RhythmTimelineAsset> allTrecks = new List<RhythmTimelineAsset>();

			foreach (var timeLine in _timelinesLibrarys)
			{
				allTrecks.AddRange(timeLine.TimelinesList);
			}
			for (int i = 0; i < allTrecks.Count; i++)
			{
				for (int ii = i - 1; ii >= 0; ii--)
				{
					if (allTrecks[ii].Uuid == allTrecks[i].Uuid)
					{
						var source = allTrecks[i];
						var duble = allTrecks[ii];

						Debug.LogError($"Трек {source.Authour} - {source.FullName} имеет дублирующий uuid с треком {duble.Authour} - {duble.FullName}");
						result = true;
					}
				}
			}
			return result;
		}

		private void CheckSongs()
		{
			foreach (var timeLine in _timelinesLibrarys)
			{
				foreach (var song in timeLine.TimelinesList)
				{
					if (!song.Check(song.name))
						throw new System.Exception($"Не корректное заполнение трека {song.name}");
				}
			}
		}

		private void RemoveExistsLocalBuild()
		{
			var directoryes = Directory.GetDirectories(LocalAddressablePath);
			foreach (var item in directoryes)
				System.IO.Directory.Delete(item, true);

			var files = Directory.GetFiles(LocalAddressablePath);
			foreach (var item in files)
				System.IO.File.Delete(item);

		}

		private void BuildAddressable()
		{
			_addressableSettings.EnableJsonCatalog = true;

			// Устанавливаем активный профиль
			SetProfile(_addressableSettings, _buildSettings.Platform);

			// Выполняем сборку
			if (!BuildContent())
			{
				Debug.LogError("Ошибка при сборке Addressables");
			}
		}

		private void SetProfile(AddressableAssetSettings settings, string profileName)
		{
			string profileId = settings.profileSettings.GetProfileId(profileName);
			if (!string.IsNullOrEmpty(profileId))
			{
				settings.activeProfileId = profileId;
			}
			else
			{
				Debug.LogWarning($"Профиль {profileName} не найден");
			}
		}

		private bool BuildContent()
		{
			AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult result);
			return string.IsNullOrEmpty(result.Error);
		}

		private void RemoveExistsDataFromServer()
		{
			var addressableFtpPath = $"{_buildSettings.Ftp.AddressablePath}/{_addressableSettings.OverridePlayerVersion}";
			var addressablePlatformFtpPath = $"{addressableFtpPath}/{_buildSettings.Platform}";

			var dirList = _ftpClient.ListDirectory(addressableFtpPath);

			if (dirList.Length == 0)
				return;

			bool existsProfileFolder = false;

			foreach (var item in dirList)
			{
				if (item.Name == _buildSettings.Platform)
					existsProfileFolder = true;
			}

			if (existsProfileFolder)
			{
				dirList = _ftpClient.ListDirectory(addressablePlatformFtpPath);

				foreach (var item in dirList)
				{
					if (item.IsDirectory)
						_ftpClient.RemoveDirectory($"{addressablePlatformFtpPath}/{item.Name}");
					else
						_ftpClient.DeleteFile($"{addressablePlatformFtpPath}/{item.Name}");
				}
				_ftpClient.RemoveDirectory(addressablePlatformFtpPath);
			}
		}

		private void UploadData()
		{
			UploadDataRecursive($"{LocalAddressablePath}", $"{_buildSettings.Ftp.AddressablePath}/{_addressableSettings.OverridePlayerVersion}");
		}

		private void UploadDataRecursive(string filePath, string serverPath, string subdir = "")
		{
			var dirs = new DirectoryInfo($"{filePath}{subdir}");

			foreach (var item in dirs.GetDirectories())
			{
				_ftpClient.CreateDirectory($"{serverPath}{subdir}/{item.Name}");
				UploadDataRecursive(filePath, serverPath, $"{subdir}/{item.Name}");
			}
			foreach (var item in dirs.GetFiles())
			{
				_ftpClient.UploadFile($"{serverPath}{subdir}/{item.Name}", $"{filePath}{subdir}/{item.Name}");
			}
		}
	}
}
