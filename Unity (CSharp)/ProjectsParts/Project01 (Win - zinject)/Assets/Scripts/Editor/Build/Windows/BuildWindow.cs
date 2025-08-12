using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using BestHTTP;
using Core.Configs;
using Core.Configs.Consts;
using Core.Materials.Parsing;
using Core.Network.Api.Consts;
using Core.Network.Http;
using Core.Network.Http.Data;
using Core.Options;
using Core.Settings;
using Core.Settings.Server;
using Cysharp.Threading.Tasks;
using Editor.Build.Checksum;
using Editor.Build.Materials;
using Editor.Build.Materials.Data;
using Editor.Build.Messengers.Telegram;
using Editor.Build.Prefs;
using Editor.Building;
using Leguar.TotalJSON;
using Settings;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Zenject;
using Debug = Core.Logging.Debug;

namespace Editor.Build.Windows
{
	public class BuildWindow : EditorWindow, IPostprocessBuildWithReport, IPreprocessBuildWithReport
	{
		private bool _isInitialized;
		private bool _isBuildInProgress;

		private string _server;
		private string _token;
		private string _login;
		private string _password;

		private bool _devServer;
		private bool _archivate;
		private bool _archivateStreamingAssets;
		private bool _archivateRemoveStreamingAssets;
		private string _description;
		private string _version;
		private int _defaultTags;

		private GUIStyle _descriptionStyle;
		private int _baseToolbarItem;

		private List<ReleaseMaterialData> _resources = new();
		private readonly List<ReleaseTagMaterialData> _tags = new();
		private string[] _tagFields = Array.Empty<string>();

		private readonly CancellationTokenSource _cancellationTokenSource = new();

		private bool _isBaseUploading;
		private float _baseUploadingProgress;
		private bool _areStreamingAssetsUploading;
		private float _streamingAssetsUploadingProgress;

		private bool _isWaitingForResourcesToFinishLoading;
		private Vector2 _scrollList;

		private bool _isArchivatePageDisabled;

		private IProjectSettings _projectSettings;
		private IUISettings _uiSettings;
		private IPrefabSettings _prefabSettings;
		private IHttpRequest _request;
		private ITelegramPostingProvider _telegramPostingProvider;
		private IChecksum _checksum;
		private IMaterialDataParsingHelper _parsingHelper;
		private IApplicationOptionsSetter _optionsSetter;
		private BuildWindowSettings _settings;

		public int callbackOrder => 0;

		private string Server
		{
			get
			{
				_ = InitializeIfNot();
				return _devServer ? _projectSettings.GetServer(ServerType.BuildPublic).Server : _server;
			}
		}

		private string ServerApi => $"{Server}/api2";

		//private string BuildPath
		//{
		//	get
		//	{
		//		var path = Application.dataPath;
		//		var parentDirectory = Directory.GetParent(path);
		//		path = parentDirectory != null ? parentDirectory.FullName : Path.Combine(path, "..");
		//		return Path.Combine(path, "build", $"VideoWall_{_version.Replace('.', '_')}");
		//	}
		//}
		public string BuildPath => $"{_settings.BuildPath}VideoWall_{_version.Replace(".", "_")}";

		private string ArchivePath
		{
			get
			{
				var path = BuildPath;
				var parentDirectory = Directory.GetParent(path);
				path = parentDirectory != null ? parentDirectory.FullName : Path.Combine(path, "..");
				return path;
			}
		}

		private string StreamingAssetsPath => Path.Combine(BuildPath, "VideoWall_Data", "StreamingAssets");

		private string ButtonUploadBaseName =>
			!_isBaseUploading
				? "Upload base"
				: $"Upload base. Progress: {_baseUploadingProgress:P2}";

		private string ButtonUploadStreamingAssetsName =>
			!_areStreamingAssetsUploading
				? "Upload streaming assets"
				: $"Upload streaming assets. Progress: {_streamingAssetsUploadingProgress:P2}";

		public bool InitializeIfNot()
		{
			if (_isInitialized)
				return true;

			_settings = BuildWindowSettings.Load();
			titleContent = new GUIContent("Build window");

			var container = StaticContext.Container;

			_projectSettings = container.TryResolve<IProjectSettings>();
			_uiSettings = container.TryResolve<IUISettings>();
			_prefabSettings = container.TryResolve<IPrefabSettings>();
			_request = container.TryResolve<IHttpRequest>();
			_telegramPostingProvider = container.TryResolve<ITelegramPostingProvider>();
			_checksum = container.TryResolve<IChecksum>();
			_parsingHelper = container.TryResolve<IMaterialDataParsingHelper>();
			_optionsSetter = container.TryResolve<IApplicationOptionsSetter>();

			var config = container.TryResolve<IConfig>();

			if (_projectSettings == null
					|| _uiSettings == null
					|| _prefabSettings == null
					|| _request == null
					|| _telegramPostingProvider == null
					|| _checksum == null
					|| _parsingHelper == null
					|| _optionsSetter == null
					|| config == null)
			{
				Debug.LogError("Required dependencies are not resolved");
				return false;
			}

			_server = config.GetValue(ConfigKey.HttpServer);
			_login = config.GetValue(ConfigKey.Login);
			_password = config.GetValue(ConfigKey.Password);

			_devServer = EditorPrefs.GetBool(BuildPrefsKey.DevServer, false);
			_archivate = EditorPrefs.GetBool(BuildPrefsKey.Archivate, true);
			_archivateStreamingAssets = EditorPrefs.GetBool(BuildPrefsKey.ArchivateStreamingAssets, true);
			_archivateRemoveStreamingAssets = EditorPrefs.GetBool(BuildPrefsKey.ArchivateRemoveStreamingAssets, true);
			_version = Application.version;
			_description = EditorPrefs.GetString(BuildPrefsKey.Description, string.Empty);
			_isArchivatePageDisabled = EditorPrefs.GetBool(BuildPrefsKey.ArchivatePageDisabled, false);

			_descriptionStyle = new GUIStyle
			{
				stretchHeight = true,
				wordWrap = true,
				normal =
				{
					textColor = Color.white
				},
				border = new RectOffset
				{
					left = 8,
					right = 8,
					top = 8,
					bottom = 8
				},
				margin = new RectOffset
				{
					left = 8,
					right = 8,
					top = 8,
					bottom = 8
				}
			};

			_isInitialized = true;

			return true;
		}

		private void OnEnable() => InitializeIfNot();

		private void OnGUI()
		{
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();

			_version = EditorGUILayout.TextField("Version", _version);

			EditorGUILayout.Separator();
			EditorGUILayout.Separator();

			_baseToolbarItem = GUILayout.Toolbar(_baseToolbarItem, new[]
			{
				"Build",
				"Archivate",
				"Upload"
			});

			switch (_baseToolbarItem)
			{
				case 0:
					DrawBuildPage();
					break;
				case 1:
					DrawArchivatePage();
					break;
				case 2:
					DrawUploadPage();
					break;
			}
		}

		private void OnDestroy()
		{
			if (!_cancellationTokenSource.IsCancellationRequested)
			{
				_cancellationTokenSource.Cancel();
				_cancellationTokenSource.Dispose();
			}

			if (_isArchivatePageDisabled)
				SetArchivatePageDisabled(false);
		}

		private void DrawBuildPage()
		{
			EditorGUILayout.Separator();

			_ = EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField($"Build path: {BuildPath}");
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.LabelField("Description:");

			var rect = EditorGUILayout.BeginHorizontal();

			rect = new Rect(rect.x - 4, rect.y - 4, rect.width + 8, rect.height + 8);

			GUI.Box(rect, GUIContent.none, GUI.skin.textArea);

			_description = EditorGUILayout.TextArea(_description, _descriptionStyle);

			EditorGUILayout.EndHorizontal();

			if (GUILayout.Button("Build"))
				AttemptToRunBuild();
		}

		private void AttemptToRunBuild()
		{
			if (_isBuildInProgress)
				return;

			var path = Path.Combine(BuildPath, "VideoWall.exe");

			PlayerSettings.bundleVersion = _version;

			EditorPrefs.SetBool(BuildPrefsKey.DevServer, _devServer);
			EditorPrefs.SetString(BuildPrefsKey.Description, _description);

			if (Directory.Exists(BuildPath))
				Directory.Delete(BuildPath, true);

			_ = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, path, BuildTarget.StandaloneWindows64, BuildOptions.None);
		}

		private void DrawArchivatePage()
		{
			EditorGUI.BeginDisabledGroup(_isArchivatePageDisabled);

			EditorGUILayout.Separator();

			var archivate = GUILayout.Toggle(_archivate, "Archivate");

			if (archivate != _archivate)
			{
				_archivate = archivate;
				EditorPrefs.SetBool(BuildPrefsKey.Archivate, _archivate);
			}

			var removeStreamingAssets = GUILayout.Toggle(_archivateRemoveStreamingAssets, "Remove streaming assets");

			if (removeStreamingAssets != _archivateRemoveStreamingAssets)
			{
				_archivateRemoveStreamingAssets = removeStreamingAssets;
				EditorPrefs.SetBool(BuildPrefsKey.ArchivateRemoveStreamingAssets, _archivateRemoveStreamingAssets);
			}

			var archivateStreamingAssets = GUILayout.Toggle(_archivateStreamingAssets, "Archivate streaming assets");

			if (archivateStreamingAssets != _archivateStreamingAssets)
			{
				_archivateStreamingAssets = archivateStreamingAssets;
				EditorPrefs.SetBool(BuildPrefsKey.ArchivateStreamingAssets, _archivateStreamingAssets);
			}

			if (GUILayout.Button("Archivate streaming assets"))
				PerformStreamingAssetsCompression().Forget();

			if (GUILayout.Button("Archivate base"))
				PerformBaseCompression().Forget();

			if (GUILayout.Button("Archivate"))
				PerformCompression().Forget();

			EditorGUI.EndDisabledGroup();
		}

		private async UniTaskVoid DisplayProgressBar(string titleText, string infoText, IReadOnlyList<int> progress, IReadOnlyList<int> total)
		{
			do
			{
				var p = progress[0];
				var t = total[0];

				if (p > t)
					break;

				EditorUtility.DisplayProgressBar(titleText, $"{infoText} {p} / {t}", (float) p / t);

				await UniTask.Yield();
			}
			while (progress[0] < total[0]);

			EditorUtility.ClearProgressBar();
		}

		private async UniTaskVoid PerformStreamingAssetsCompression()
		{
			SetArchivatePageDisabled(true);
			await ProcessStreamingAssetsCompressionAsync();
			SetArchivatePageDisabled(false);
		}

		private async UniTaskVoid PerformBaseCompression()
		{
			SetArchivatePageDisabled(true);
			await ProcessBaseCompressionAsync();
			SetArchivatePageDisabled(false);
		}

		private async UniTaskVoid PerformCompression()
		{
			SetArchivatePageDisabled(true);

			if (_archivateStreamingAssets)
				await ProcessStreamingAssetsCompressionAsync(ProcessBaseCompressionAsync);
			else
				await ProcessBaseCompressionAsync();

			SetArchivatePageDisabled(false);
		}

		private async UniTask ProcessStreamingAssetsCompressionAsync(Func<UniTask> onCompleted = null)
		{
			var streamingAssetsPath = StreamingAssetsPath;

			if (!Directory.Exists(streamingAssetsPath))
			{
				Debug.LogError($"Directory does not exist: {streamingAssetsPath}");
				return;
			}

			var filesCount = lzip.getAllFiles(streamingAssetsPath);

			if (filesCount == 0)
				return;

			var progress = new int[1];
			var total = new[] { filesCount };

			DisplayProgressBar("Streaming assets compression", "Compressing is in progress:", progress, total).Forget();

			if (!InitializeIfNot())
				return;

			try
			{
				var cancellationToken = _cancellationTokenSource.Token;

				await UniTask.Yield(cancellationToken);

				var zipFile = GetArchiveFileName(true, _archivateStreamingAssets);

				var archivePath = ArchivePath;

				await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
				{
					await UniTask.SwitchToThreadPool();
					cancellationToken.ThrowIfCancellationRequested();
					_ = lzip.compressDir(streamingAssetsPath, 9, Path.Combine(archivePath, zipFile), progress: progress);
				}

				progress[0] = total[0];

				await UniTask.Yield(cancellationToken);

				if (onCompleted != null)
					await onCompleted();

				Debug.Log("Streaming assets compression completed");
			}
			catch (Exception exception) when (exception is not OperationCanceledException)
			{
				Debug.LogException(exception);
			}
			finally
			{
				if (Thread.CurrentThread.IsBackground)
					await UniTask.SwitchToMainThread();
			}
		}

		private async UniTask ProcessBaseCompressionAsync()
		{
			var buildPath = BuildPath;

			if (!Directory.Exists(buildPath))
			{
				Debug.LogError($"Directory does not exist: {buildPath}");
				return;
			}

			var streamingAssetsPath = StreamingAssetsPath;

			if (_archivateRemoveStreamingAssets)
				RemoveStreamingAssets(streamingAssetsPath);

			var filesCount = lzip.getAllFiles(buildPath);

			if (filesCount == 0)
				return;

			var progress = new int[1];
			var total = new[] { filesCount };

			DisplayProgressBar("Base compression", "Compressing is in progress:", progress, total).Forget();

			if (!InitializeIfNot())
				return;

			try
			{
				var cancellationToken = _cancellationTokenSource.Token;

				await UniTask.Yield(cancellationToken);

				var zipFile = GetArchiveFileName(false, !_archivateStreamingAssets);

				var dataPath = Application.dataPath;
				var archivePath = ArchivePath;

				await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
				{
					await UniTask.SwitchToThreadPool();
					cancellationToken.ThrowIfCancellationRequested();
					CopyFile(dataPath, buildPath, "WallRun.bat");
					_ = lzip.compressDir(buildPath, 9, Path.Combine(archivePath, zipFile), progress: progress);
				}

				progress[0] = total[0];

				await UniTask.Yield(cancellationToken);

				Debug.Log("Base compression completed");
			}
			catch (Exception exception) when (exception is not OperationCanceledException)
			{
				Debug.LogException(exception);
			}
			finally
			{
				if (Thread.CurrentThread.IsBackground)
					await UniTask.SwitchToMainThread();
			}
		}

		private void SetArchivatePageDisabled(bool value)
		{
			_isArchivatePageDisabled = value;
			EditorPrefs.SetBool(BuildPrefsKey.ArchivatePageDisabled, value);
		}

		private void RemoveStreamingAssets(string path)
		{
			if (Directory.Exists(path))
				Directory.Delete(path, true);
		}

		private void RemoveConfigEditor(string path)
		{
			path = Path.Combine(path, "configEditor.txt");

			if (File.Exists(path))
				File.Delete(path);
		}

		private void CopyFile(string sourcePath, string destinationPath, string fileName)
		{
			destinationPath = Path.Combine(destinationPath, fileName);

			if (!File.Exists(destinationPath))
			{
				var parentDirectory = Directory.GetParent(sourcePath);

				sourcePath = parentDirectory != null ? parentDirectory.FullName : Path.Combine(sourcePath, "..");
				sourcePath = Path.Combine(sourcePath, fileName);

				File.Copy(sourcePath, destinationPath);
			}
		}

		private string GetArchiveFileName(bool streamingAssets = false, bool buildIncrement = true)
		{
			var build = 0;
			var version = string.Empty;

			if (EditorPrefs.HasKey(BuildPrefsKey.Version))
				version = EditorPrefs.GetString(BuildPrefsKey.Version);

			if (version != PlayerSettings.bundleVersion)
			{
				version = PlayerSettings.bundleVersion;
				build = 0;
			}

			EditorPrefs.SetString(BuildPrefsKey.Version, version);
			EditorPrefs.SetInt(BuildPrefsKey.Build, build);

			return $"VideoWall_{PlayerSettings.bundleVersion}{(streamingAssets ? "_SA" : string.Empty)}.zip";
		}

		private void DrawUploadPage()
		{
			EditorGUILayout.Separator();

			var devServer = GUILayout.Toggle(_devServer, "Dev server");

			if (devServer != _devServer)
			{
				_devServer = devServer;
				EditorPrefs.SetBool(BuildPrefsKey.DevServer, _devServer);
			}

			_ = EditorGUILayout.BeginHorizontal();

			if (_tagFields.Length > 0)
			{
				var oldSelectTag = _defaultTags;

				_defaultTags = EditorGUILayout.MaskField(_defaultTags, _tagFields);

				if (oldSelectTag != _defaultTags)
				{
					for (var i = 0; i < _tagFields.Length; i++)
					{
						var layer = 1 << i;

						if ((_defaultTags & layer) != 0)
							Debug.Log(_tagFields[i]);
					}
				}

				EditorGUILayout.Space();
			}

			EditorGUILayout.EndHorizontal();

			DrawResources();

			if (GUILayout.Button(ButtonUploadBaseName))
				UploadBase();

			if (GUILayout.Button(ButtonUploadStreamingAssetsName))
				UploadStreamingAssets();

			if (GUILayout.Button("Upload"))
				UploadBase();

			if (GUILayout.Button("Get resources"))
				GetResources();
		}

		private void UploadBase()
		{
			_isBaseUploading = true;
			_baseUploadingProgress = 0;

			var fileName = GetArchiveFileName(false, false);
			var path = Path.Combine(ArchivePath, fileName);

			Debug.Log(path);

			if (!File.Exists(path))
			{
				Debug.LogError("Archive is not found");
				return;
			}

			Debug.Log(ServerApi);

			var parameters = new List<KeyValuePair<string, string>>
			{
				new ("version", _version),
				new ("checksum", _checksum.Calculate(path)),
				new ("description", _description),
				new ("type", ReleaseMaterialType.VideoWall)
			};

			if (_tagFields.Length > 0)
			{
				var targetTags = GetSelectTags();
				parameters.Add(new KeyValuePair<string, string>("tagsIds", targetTags.CreateString()));
			}

			Debug.Log($"File path: {path}");

			var file = File.ReadAllBytes(path);

			Debug.Log($"File length: {file.Length}");

			Authorization(() =>
				{
					UploadBuild("/releases", parameters, file, fileName,
						result =>
						{
							_isBaseUploading = false;

							var description = !string.IsNullOrEmpty(_description)
								? "\n<b>Description</b>\n" + _description
								: string.Empty;

							_telegramPostingProvider.Post($"#releaseWall {_version} published{description}");

							Repaint();
							GetResources();
						},
						error =>
						{
							_isBaseUploading = false;
							Repaint();
						},
						progress =>
						{
							_baseUploadingProgress = progress;
							Repaint();
						});
				},
				error =>
				{
					Debug.LogError($"Authorization error: {error}");
				});
		}

		private void UploadStreamingAssets()
		{
			_areStreamingAssetsUploading = true;
			_streamingAssetsUploadingProgress = 0;

			var fileName = GetArchiveFileName(true, false);
			var path = Path.Combine(ArchivePath, fileName);

			Debug.Log(path);

			if (!File.Exists(path))
			{
				Debug.LogError("Archive is not found");
				return;
			}

			Debug.Log(ServerApi);

			var parameters = new List<KeyValuePair<string, string>>
			{
				new ("version", _version),
				new ("checksum", _checksum.Calculate(path)),
				new ("description", ""),
				new ("type", ReleaseMaterialType.StreamingAssets)
			};

			Debug.Log($"File path: {path}");

			var file = File.ReadAllBytes(path);

			Debug.Log($"File length: {file.Length}");

			Authorization(() =>
				UploadBuild("/releases", parameters, file, fileName,
					result =>
					{
						_areStreamingAssetsUploading = false;
						Repaint();
					},
					error =>
					{
						_areStreamingAssetsUploading = false;
						Repaint();
					},
					progress =>
					{
						_streamingAssetsUploadingProgress = progress;
						Repaint();
					}),
				error => Debug.LogError($"Authorization error: {error}"));
		}

		private JArray GetSelectTags()
		{
			var tags = new JArray();

			for (var i = 0; i < _tagFields.Length; i++)
			{
				var layer = 1 << i;

				if ((_defaultTags & layer) != 0)
				{
					tags.Add(_tags.Find(x => x.Name == _tagFields[i]).Id);
					Debug.Log(_tagFields[i]);
				}
			}

			return tags;
		}

		private void Authorization(Action onCompleted, Action<string> onFailure)
		{
			if (!InitializeIfNot())
				return;

			var url = $"{ServerApi}{RestApiUrl.Login}";

			var parameters = new (string, object)[]
			{
				new ("username", _login),
				new ("password", _password),
				new ("role", "VideoWall")
			};

			_request.Request(RestApiUrl.Login,
				parameters,
				result =>
				{
					var json = JSON.ParseString(result);
					_token = json.GetString("token");
					Debug.Log($"Auth token: {_token}");
					_optionsSetter.ServerToken = _token;
					onCompleted?.Invoke();
				},
				error =>
				{
					onFailure?.Invoke(error);
					Debug.LogError($"Login error: {error}. Request: {url}");
				});
		}

		private void UploadBuild(string url, List<KeyValuePair<string, string>> paramsData, byte[] file, string filename,
			Action<string> onCompleted = null, Action<string> onFailure = null, Action<float> onProgress = null)
		{
			var request = new HTTPRequest(new Uri(ServerApi + url),
				HTTPMethods.Post,
				true,
				true,
				(request, response) =>
				{
					EditorUtility.ClearProgressBar();

					switch (request.State)
					{
						case HTTPRequestStates.Finished:

							if (response.IsSuccess)
							{
								onCompleted?.Invoke(response.DataAsText);
							}
							else
							{
								Debug.LogError(string.Format("Request is completed successfully, but the server sent an error. Status code: {0}-{1}. Message: {2} {3}",
																	 response.StatusCode,
																	 response.Message,
																	 response.DataAsText,
																	 request.Uri.AbsoluteUri));
							}

							break;

						case HTTPRequestStates.Error:

							onFailure?.Invoke(request.Exception.Message);

							var error = request.Exception != null
								? $"{request.Exception.Message}{System.Environment.NewLine}{request.Exception.StackTrace}"
								: "Unknown exception";

							Debug.LogError($"Request is completed with an error. Error: {error}");

							break;

						case HTTPRequestStates.Aborted:
							onFailure?.Invoke("Request is aborted");
							Debug.LogError("Request is aborted");
							break;

						case HTTPRequestStates.ConnectionTimedOut:
							onFailure?.Invoke("Connection is timed out");
							Debug.LogError("Connection is timed out");
							break;

						case HTTPRequestStates.TimedOut:
							onFailure?.Invoke("Request is timed out");
							Debug.LogError("Request is timed out");
							break;
					}
				});

			if (!string.IsNullOrEmpty(_token))
				request.AddHeader("Authorization", $"Bearer {_token}");

			if (paramsData is { Count: > 0 })
			{
				foreach (var item in paramsData)
				{
					Debug.Log($"{item.Key} : {item.Value}");
					request.AddField(item.Key, item.Value);
				}
			}

			request.MaxFragmentQueueLength = 2048;
			request.Timeout = TimeSpan.FromMinutes(100.0);

			request.AddBinaryData("file", file, filename, "application/zip");

			request.OnUploadProgress += (_, down, length) =>
			{
				var progress = down / (float) length;
				var version = string.IsNullOrEmpty(_version) ? string.Empty : $" {_version}";
				EditorUtility.DisplayProgressBar("Simple Progress Bar", $"Uploading build{version}...", progress);
				onProgress?.Invoke(progress);
			};

			_ = request.Send();
		}

		private void GetResources()
		{
			if (_isWaitingForResourcesToFinishLoading)
				return;

			_isWaitingForResourcesToFinishLoading = true;

			if (!string.IsNullOrEmpty(_token))
			{
				if (_tagFields.Length <= 0)
					LoadTags();

				LoadResourcesData();
			}
			else
			{
				Authorization(() =>
					{
						LoadTags();
						LoadResourcesData();
					},
					error => Debug.Log($"Authorization error: {error}"));
			}
		}

		private void LoadResourcesData()
		{
			var url = $"{ServerApi}/releases";

			_request.Request(new Uri(url),
				HttpMethodType.Get,
				null,
				_token,
				result =>
				{
					Debug.Log(result);

					_isWaitingForResourcesToFinishLoading = false;

					_resources.Clear();

					var jsonArray = JArray.ParseString(result);

					for (var i = 0; i < jsonArray.Length; i++)
					{
						var data = _parsingHelper.Parse<ReleaseMaterialData>(jsonArray.GetJSON(i));

						if (data != null)
							_resources.Add(data);
					}

					_resources = _resources.OrderByDescending(x => x.CreatedAt).ToList();

					PrefillSelectTag();
				},
				error =>
				{
					Debug.LogError($"Error: {error} | Request: {url}");
				});
		}

		private void LoadTags()
		{
			var url = $"{ServerApi}/release-tags";

			_request.Request(new Uri(url),
				HttpMethodType.Get,
				null,
				_token,
				result =>
				{
					Debug.Log($"Tags: {result}");

					_isWaitingForResourcesToFinishLoading = false;

					_resources.Clear();

					var jsonArray = JArray.ParseString(result);

					for (var i = 0; i < jsonArray.Length; i++)
					{
						var data = _parsingHelper.Parse<ReleaseTagMaterialData>(jsonArray.GetJSON(i));

						if (data != null)
							_tags.Add(data);
					}

					_tagFields = new string[_tags.Count];

					for (var i = 0; i < _tags.Count; i++)
						_tagFields[i] = _tags[i].Name;

					PrefillSelectTag();
				},
				error =>
				{
					Debug.LogError($"Error: {error} | Request: {url}");
				});
		}

		private void PrefillSelectTag()
		{
			if (_resources.Count == 0 || _tags.Count == 0)
				return;

			foreach (var data in _resources)
			{
				foreach (var tag in data.TagIds.Select(tagId => _tags.Find(t => t.Id == tagId)))
				{
					for (var i = 0; i < _tagFields.Length; i++)
					{
						if (_tagFields[i] != tag.Name)
							continue;

						data.SelectedTagsMask |= 1 << i;
					}
				}
			}
		}

		private void DrawResources()
		{
			if (!_isWaitingForResourcesToFinishLoading && _resources.Count == 0)
				GetResources();

			if (_resources.Count == 0)
				return;

			_scrollList = EditorGUILayout.BeginScrollView(_scrollList);

			_ = EditorGUILayout.BeginVertical();

			foreach (var data in _resources)
				DrawResourceItem(data);

			EditorGUILayout.EndVertical();
			EditorGUILayout.EndScrollView();
		}

		private void DrawResourceItem(ReleaseMaterialData data)
		{
			_ = EditorGUILayout.BeginHorizontal("box");
			EditorGUILayout.LabelField(data.Version);

			data.Type = EditorGUILayout.TextField(data.Type);

			var selectedTagsMask = data.SelectedTagsMask;

			data.SelectedTagsMask = EditorGUILayout.MaskField(data.SelectedTagsMask, _tagFields);

			if (selectedTagsMask != data.SelectedTagsMask)
			{
				for (var i = 0; i < _tagFields.Length; i++)
				{
					var layer = 1 << i;

					if ((data.SelectedTagsMask & layer) != 0)
						Debug.Log(_tagFields[i]);
				}
			}

			GUI.backgroundColor = Color.green;

			if (GUILayout.Button("Save"))
				SaveResourceItem(data);

			GUI.backgroundColor = Color.red;

			if (GUILayout.Button("X"))
				DeleteResourcesItem(data);

			GUI.backgroundColor = Color.white;

			EditorGUILayout.EndHorizontal();
		}

		private void SaveResourceItem(ReleaseMaterialData data)
		{
			data.TagIds.Clear();

			for (var i = 0; i < _tagFields.Length; i++)
			{
				var layer = 1 << i;

				if ((data.SelectedTagsMask & layer) != 0)
					data.TagIds.Add(_tags.Find(x => x.Name == _tagFields[i]).Id);
			}

			var url = $"/releases/{data.Id}";

			var parameters = new (string, object)[]
			{
				new ("version", data.Version),
				new ("checksum", data.Checksum),
				new ("description", data.Description),
				new ("type", data.Type),
				new ("tagsIds", data.TagIds)
			};

			_request.Request(url,
				parameters,
				result => Debug.Log($"Update: {result}"),
				error => Debug.LogError($"Error: {error} | Request: {url}"));
		}

		private void DeleteResourcesItem(ReleaseMaterialData data)
		{
			var url = $"{ServerApi}/releases/{data.Id}";

			_request.Request(new Uri(url),
				HttpMethodType.Delete,
				null,
				_token,
				result =>
				{
					_resources.Clear();
					_telegramPostingProvider.Post($"#releaseWall {data.Version} removed");
					Repaint();
				});
		}

		private void RenameVideoWallFileAsExecutable()
		{
			var sourcePath = Path.Combine(BuildPath, "VideoWall");
			var targetPath = Path.Combine(BuildPath, "VideoWall.exe");

			if (File.Exists(sourcePath) && !File.Exists(targetPath))
				File.Move(sourcePath, targetPath);
		}

		private void RemoveCrashHandlerFile()
		{
			var crashHandlerPath = Path.Combine(BuildPath, "UnityCrashHandler64.exe");

			if (File.Exists(crashHandlerPath))
				File.Delete(crashHandlerPath);
		}

		private void RemoveBackUpFolder()
		{
			foreach (var directory in Directory.GetDirectories(BuildPath))
				if (directory.Contains("BackUpThisFolder"))
					Directory.Delete(directory, true);
		}

		private void CopyDistributive()
		{
			var path = Application.dataPath;
			var parentDirectory = Directory.GetParent(path);
			path = parentDirectory != null ? parentDirectory.FullName : Path.Combine(path, "..");
			var distributivePath = Path.Combine(path, "Distrib");
			var targetPath = Path.Combine(BuildPath, "Distrib");

			if (!Directory.Exists(distributivePath))
			{
				Debug.Log("Distributive directory is missing");
				return;
			}

			if (!Directory.Exists(targetPath))
				_ = Directory.CreateDirectory(targetPath);

			var files = Directory.GetFiles(distributivePath);

			foreach (var file in files)
			{
				var fileName = file.Split('/', '\\')[^1];
				var newFilePath = Path.Combine(targetPath, fileName);

				if (!File.Exists(newFilePath))
					File.Copy(file, newFilePath);
			}
		}

		public void OnPreprocessBuild(BuildReport report)
		{
			if (_isBuildInProgress)
				return;

			_isBuildInProgress = true;

			if (!InitializeIfNot())
				return;

			if (Directory.Exists(BuildPath))
				Directory.Delete(BuildPath, true);

			_prefabSettings.Actualize(_uiSettings);

			Debug.Log($"OnPreprocessBuild outputPath: {report.summary.outputPath}");
		}

		public void OnPostprocessBuild(BuildReport report)
		{
			if (!_isBuildInProgress)
				return;

			if (!InitializeIfNot())
				return;

			RemoveConfigEditor(StreamingAssetsPath);
			RenameVideoWallFileAsExecutable();
			RemoveCrashHandlerFile();
			RemoveBackUpFolder();
			CopyDistributive();

			if (report.summary.totalErrors == 0)
			{
				Debug.Log("Build complete");

				if (_archivate && !_isArchivatePageDisabled)
					PerformCompression().Forget();
			}
			else
			{
				Debug.Log($"OnPostprocessBuild total errors: {report.summary.totalErrors}");
			}

			_isBuildInProgress = false;
		}
	}
}

