using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using System.Threading;
using UnityEngine.Networking;
using BestHTTP;
using System.Text;
using it.Managers;
using System.Security.Cryptography;
using Leguar.TotalJSON;
using Sett = it.Settings;
using static UnityEditor.Progress;
using DG.Tweening.Plugins.Core.PathCore;
using it.Network.Rest;
//using UniRx.ObserveExtensions.s;

namespace it.Editor.Build
{

	public class BuildWindow : EditorWindow, IPostprocessBuildWithReport, IPreprocessBuildWithReport
	{
		private string EpArhivate => $"{Application.productName}.achivate";
		private string EpClearBuild => $"{Application.productName}.clearBuild";
		private readonly string _telegramChatId = "-783633710";

		private string _serverRelease = "https://apps.eximkaubad.com/api/v1";

		private static bool _build = false;

		private int _baseToolbarItem = 0;

		private static string platformType =
#if UNITY_STANDALONE_WIN
		"STANDALONE-WIN";
#elif UNITY_STANDALONE_OSX
		"STD-OSX";
#elif UNITY_WEBGL
		"WEBGL";
#elif UNITY_ANDROID
		"ANDROID";
#elif UNITY_IOS
		"IOS";
#endif

		private bool _isInit = false;
#if UNITY_ANDROID
		private string BuildPath => $"{Application.dataPath}/../build/GarillaPoker-{platformType}_{_version}.apk";
#else
		private string BuildPath => $"{Application.dataPath}/../build/GarillaPoker-{platformType}_{_version.Replace(".", "_")}";
#endif
		private string GetArchivePath => $"{BuildPath}/../";
		private string _description = "";
		private string _version;

		private string _server;
		private static string _token;
		private bool _archivate;
		private bool _clearBuild;
		private string _login;
		private string _password;
		private int _defaultTegs;

		private bool _isUploadStreamingAssets;
		private float _uploadStreaminAssetsProgress;
		private bool _isUploadBase;
		private float _uploadBaseProgress;

		private static List<RelelaseMaterial> _resourcesList = new List<RelelaseMaterial>();
		private static string[] _tagsFields = new string[0];

		public int callbackOrder => 0;
		private GUIStyle _descriptionStyle;

		private string ServerApi => Server + "/api2";

		public virtual string Server
		{
			get
			{
				return _server;
			}
		}

		private Dictionary<int, string> _versionType = new Dictionary<int, string>() { { 1, "Launcher" }, { 2, "Game" } };
		private Dictionary<int, string> _appType = new Dictionary<int, string>() { { 1, "Beta" }, { 2, "Production" } };
		private Dictionary<int, string> _users = new Dictionary<int, string>() { { 2, "Никифоров А.М." } };

		private string[] _versionTypeField = new string[0];

		private string[] VersionTypeField
		{
			get
			{
				if (_versionTypeField.Length == 0)
				{
					_versionTypeField = new string[_versionType.Count];
					for (int i = 0; i < _versionType.Count; i++)
						_versionTypeField[i] = _versionType[i];
				}
				return _versionTypeField;
			}
		}


#if UNITY_STANDALONE_WIN
		[MenuItem("Garilla Poker/Build/Build Settings STANDALONE WIN", false, 0)]
#elif UNITY_STANDALONE_OSX
		[MenuItem("Garilla Poker/Build/Build Settings STANDALONE OSX", false, 0)]
#elif UNITY_WEBGL
		[MenuItem("Garilla Poker/Build/Build Settings WEBGL", false, 0)]
#elif UNITY_ANDROID
		[MenuItem("Garilla Poker/Build/Build Settings ANDROID", false, 0)]
#elif UNITY_IOS
		[MenuItem("Garilla Poker/Build/Build Settings IOS", false, 0)]
#endif
		public static void OpenWindow()
		{
			EditorWindow w = new BuildWindow();
			w.Show();
		}
		private void OnEnable()
		{
			_isInit = false;


		}
		private void Init()
		{
			if (_isInit) return;

			_server = Sett.AppSettings.ReleaseServer;

			string title = "Build window: ";

#if UNITY_STANDALONE_WIN
			title += "STANDALONE WIN";
#elif UNITY_STANDALONE_OSX
			title += "STANDALONE OSX";
#elif UNITY_WEBGL
			title += "WEBGL";
#elif UNITY_ANDROID
			title += "ANDROID";
#elif UNITY_IOS
			title += "IOS";
#endif

			titleContent = new GUIContent(title);

			_archivate = EditorPrefs.GetBool(EpArhivate, true);
			_version = Application.version;

			_descriptionStyle = new GUIStyle();
			_descriptionStyle.stretchHeight = true;
			_descriptionStyle.normal.textColor = Color.white;
			_descriptionStyle.border = new RectOffset();
			_descriptionStyle.border.left = 2;
			_descriptionStyle.border.right = 2;
			_descriptionStyle.border.top = 2;
			_descriptionStyle.border.bottom = 2;
			_descriptionStyle.margin = new RectOffset();
			_descriptionStyle.margin.left = 2;
			_descriptionStyle.margin.right = 2;
			_descriptionStyle.margin.top = 2;
			_descriptionStyle.margin.bottom = 2;

			_isInit = true;
		}
		private void OnGUI()
		{
			Init();

			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			_version = EditorGUILayout.TextField("Version", _version);
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			_baseToolbarItem = GUILayout.Toolbar(_baseToolbarItem, new string[] { "Build", "Upload" });

			switch (_baseToolbarItem)
			{
				case 0:
					BuildGui();
					break;
				case 1:
					UploadGui();
					break;
			}

		}

		#region Archivate


		private static string GetNameArchive()
		{
			string version = "";
			if (EditorPrefs.HasKey("Build_V"))
			{
				version = EditorPrefs.GetString("Build_V");
			}

			if (version != PlayerSettings.bundleVersion)
			{
				version = PlayerSettings.bundleVersion;
			}

			EditorPrefs.SetString("Build_V", version);
			return $"GarillaPoker-{platformType}_{PlayerSettings.bundleVersion}.zip";
		}

		private void ArchivateGui()
		{

			EditorGUILayout.Separator();
			//EditorGUILayout.BeginHorizontal();
			//_archivePath = EditorGUILayout.TextField("Archive path", _archivePath);
			//if (GUILayout.Button("...", GUILayout.Width(30)))
			//{
			//  OpenFolderPanel("Select archive path", ref _archivePath, () =>
			//  {
			//	 EditorPrefs.SetString(_epArchivePath, _archivePath);
			//  });
			//}
			//EditorGUILayout.EndHorizontal();



		}


		//[MenuItem("Build/Compress", false, 0)]
		public void Compress()
		{
			Init();
			BuildArchivate.Compression(new BuildArchivate.ArchiveData()
			{
				Name = GetNameArchive(),
				Path = BuildPath,
				PathBase = GetArchivePath
			}, null);
		}


		#endregion

		#region Build

		private void BuildGui()
		{

			EditorGUILayout.Separator();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Путь сборки:   " + BuildPath);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			//EditorGUILayout.DropdownButton(0, FocusType.Keyboard, VersionTypeField);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();

			bool clearBuild = EditorGUILayout.Toggle("Чистка кеша", _clearBuild);
			if (clearBuild != _clearBuild)
			{
				_clearBuild = clearBuild;
				EditorPrefs.SetBool(EpClearBuild, _clearBuild);
			}
			EditorGUILayout.EndHorizontal();

#if !UNITY_ANDROID && !UNITY_IOS

			EditorGUILayout.BeginHorizontal();
			bool archivate = EditorGUILayout.Toggle("Архивировать", _archivate);
			if (archivate != _archivate)
			{
				_archivate = archivate;
				EditorPrefs.SetBool(EpArhivate, _archivate);
			}

			if (GUILayout.Button("Запустить архивацию"))
			{
				Compress();
			}
			//if (GUILayout.Button("RecordFile"))
			//{
			//	BuildReleadeFileWebgl();
			//}

			EditorGUILayout.EndHorizontal();

#endif

			EditorGUILayout.LabelField("Описание");
			_description = EditorGUILayout.TextArea(_description, _descriptionStyle);

			EditorGUILayout.Separator();
			if (GUILayout.Button("Собрать"))
			{
				Build();
			}
		}


		private void Build()
		{
#if UNITY_STANDALONE_WIN
			string path = BuildPath + "\\GarillaPoker.exe";
#elif UNITY_STANDALONE_OSX
			string path = BuildPath + "\\GarillaPoker";
#elif UNITY_WEBGL
			string path = BuildPath;
#elif UNITY_ANDROID
			string path = BuildPath;
#elif UNITY_IOS
			string path = BuildPath;
#endif
			PlayerSettings.bundleVersion = _version;
			if (Directory.Exists(BuildPath))
				Directory.Delete(BuildPath, true);

			_build = true;

			BuildOptions options = BuildOptions.None;

			if (_clearBuild)
				options |= BuildOptions.CleanBuildCache;

#if UNITY_STANDALONE_WIN
			string[] scenes = new string[] { "Assets/Scenes/Game.unity" };
			BuildPipeline.BuildPlayer(scenes, path, BuildTarget.StandaloneWindows64, options);
#elif UNITY_STANDALONE_OSX
			string[] scenes = new string[] { "Assets/Scenes/Game.unity" };
			BuildPipeline.BuildPlayer(scenes, path, BuildTarget.StandaloneOSX, options);
#elif UNITY_WEBGL
			string[] scenes = new string[] { "Assets/Scenes/Mobile/Game.unity" };
			BuildPipeline.BuildPlayer(scenes, path, BuildTarget.WebGL, options);
#elif UNITY_ANDROID
			string[] scenes = new string[] { "Assets/Scenes/Mobile/Game.unity" };
			PlayerSettings.keystorePass = "GarillaPoker";
			PlayerSettings.keyaliasPass = "GarillaPoker";
			BuildPipeline.BuildPlayer(scenes, path, BuildTarget.Android, options);
#elif UNITY_IOS
			string[] scenes = new string[] { "Assets/Scenes/Mobile/Game.unity" };
			BuildPipeline.BuildPlayer(scenes, path, BuildTarget.WebGL, options);
#endif


		}

		private void RenameExe()
		{

			string source = BuildPath + "/GarillaPoker";
			string target = BuildPath + "/GarillaPoker.exe";

			if (File.Exists(source) && !File.Exists(target))
				File.Move(source, target);
		}
		private void RemoveCrashHandler()
		{
			string crachHandler = BuildPath + "/UnityCrashHandler64.exe";
			if (File.Exists(crachHandler))
				File.Delete(crachHandler);
		}
		private void RemoveBackUpFolder()
		{
			it.Logger.Log("RemoveBackUpFolder = " + BuildPath);
			string[] dirs = Directory.GetDirectories(BuildPath);
			for (int i = 0; i < dirs.Length; i++)
			{
				if (dirs[i].Contains("BackUpThisFolder"))
				{
					Directory.Delete(dirs[i], true);
				}
			}
		}

		#endregion

		private void OpenFolderPanel(string title, ref string folder, System.Action OnComplete)
		{
			string newFolder = EditorUtility.OpenFolderPanel(title, folder, "");

			if (!string.IsNullOrEmpty(newFolder))
				folder = newFolder;

			OnComplete?.Invoke();
		}

		public void OnPreprocessBuild(BuildReport report)
		{
			if (!_build) return;
			Init();

			if (Directory.Exists(BuildPath))
				Directory.Delete(BuildPath, true);

			//ReadyUILibrary();

			it.Logger.Log("OnPreprocessBuild " + report.summary.outputPath);
		}

		//private void ReadyUILibrary()
		//{
		//	var _library = Resources.Load<it.UI.UILibrary>(it.Core.ProjectSettings.GuiLibrary);
		//	_library.FindObjects();
		//	EditorUtility.SetDirty(_library);

		//}

		public void OnPostprocessBuild(BuildReport report)
		{
			if (!_build) return;

			Init();
#if UNITY_STANDALONE_WIN
			RenameExe();
			RemoveCrashHandler();
			RemoveBackUpFolder();
#endif

#if UNITY_WEBGL
			BuildReleadeFileWebgl();
#endif
			_build = false;
			if (report.summary.totalErrors == 0)
			{
				it.Logger.Log("Build complete");

#if !UNITY_ANDROID
				if (_archivate)
				{
					Init();
					Compress();
				}
#endif

			}
			else
			{
				it.Logger.Log("OnPostprocessBuild " + report.summary.totalErrors);
			}
		}

		private void BuildReleadeFileWebgl()
		{

			BuildItem release = new BuildItem();
			release.app_version = Application.version.ToString();
			release.crucially = 1;

			string serData = Newtonsoft.Json.JsonConvert.SerializeObject(release);

			using (FileStream fstream = new FileStream(BuildPath + "/release.txt", FileMode.OpenOrCreate))
			{
				byte[] buffer = Encoding.Default.GetBytes(serData);
				fstream.Write(buffer, 0, buffer.Length);
				Console.WriteLine("Текст записан в файл");
			}

		}


		#region Upload

		private void UploadGui()
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Separator();

			if (_tagsFields.Length > 0)
			{
				int oldSelectTag = _defaultTegs;
				_defaultTegs = EditorGUILayout.MaskField(_defaultTegs, _tagsFields);

				if (oldSelectTag != _defaultTegs)
				{
					for (int i = 0; i < _tagsFields.Length; i++)
					{
						int layer = 1 << i;
						if ((_defaultTegs & layer) != 0)
						{
							it.Logger.Log(_tagsFields[i]);
						}
					}
				}
				EditorGUILayout.Space();
			}
			EditorGUILayout.EndHorizontal();

			DrawResourcesList();

			if (GUILayout.Button(ButtonUploadBaseName))
			{
				UploadBuild();
			}

			//if (GUILayout.Button(ButtonUploadStreamingAssetsName))
			//{
			//	UploadStreamingAssets();
			//}

			if (GUILayout.Button("Upload"))
			{
				UploadButton();

			}
			if (GUILayout.Button("Upload Test"))
			{
				UploadTest();

			}
			if (GUILayout.Button("Get Resources"))
			{
				GetResources();
			}

		}


		private string ButtonUploadBaseName
		{
			get => !_isUploadBase ? "Upload" : string.Format("Upload base. Progress: {0:P2}", _uploadBaseProgress);
		}
		//private string ButtonUploadStreamingAssetsName
		//{
		//	get => !_isUploadStreamingAssets ? "Upload streaming assets" : string.Format("Upload streaming assets. Progress: {0:P2}", _uploadStreaminAssetsProgress);
		//}

		private void UploadBuild()
		{
			_isUploadBase = true;
			_uploadBaseProgress = 0;
			string fileName = GetNameArchive();
			string path = GetArchivePath + "/" + fileName;

			it.Logger.Log(path);
			if (!File.Exists(path))
			{
				it.Logger.LogError("Архив не найден");
				return;
			}


			it.Logger.Log(ServerApi);

			List<KeyValuePair<string, string>> paramsData = new List<KeyValuePair<string, string>>();

			paramsData.Add(new KeyValuePair<string, string>("version", _version));
			paramsData.Add(new KeyValuePair<string, string>("checksum", GetHash(path)));
			paramsData.Add(new KeyValuePair<string, string>("description", _description));
			//paramsData.Add(new KeyValuePair<string, string>("type", RelelaseMaterial.RELEASE_TYPE));

			it.Logger.Log("File path: " + path);
			byte[] file = System.IO.File.ReadAllBytes(path);

			it.Logger.Log("File: " + file.Length);

			Authorization(() =>
			{
				UploadBuild("/releases", paramsData, file, fileName, (result) =>
				{
					_isUploadBase = false;

					TelegrammMessage($"#releaseWall {_version} опубликована" + (string.IsNullOrEmpty(_description) ? "" : "\n<b>Описание</b>\n" + _description));

					Repaint();
				}, (err) =>
				{
					_isUploadBase = false;
					Repaint();
				}, (progress) =>
				{
					_uploadBaseProgress = progress;
					Repaint();
				});

			}, (err) =>
			{
				it.Logger.Log("Authorization error " + err);
				return;
			});
		}
		private void UploadStreamingAssets()
		{
			_isUploadStreamingAssets = true;
			_uploadStreaminAssetsProgress = 0;
			string fileName = GetNameArchive();
			string path = GetArchivePath + "/" + fileName;

			it.Logger.Log(path);
			if (!File.Exists(path))
			{
				it.Logger.LogError("Архив не найден");
				return;
			}

			it.Logger.Log(ServerApi);

			List<KeyValuePair<string, string>> paramsData = new List<KeyValuePair<string, string>>();

			paramsData.Add(new KeyValuePair<string, string>("version", _version + "_SA"));
			paramsData.Add(new KeyValuePair<string, string>("checksum", GetHash(path)));
			paramsData.Add(new KeyValuePair<string, string>("description", ""));
			//paramsData.Add(new KeyValuePair<string, string>("type", RelelaseMaterial.RELEASE_TYPE));

			it.Logger.Log("File path: " + path);

			byte[] file = System.IO.File.ReadAllBytes(path);

			it.Logger.Log("File Length: " + file.Length);

			Authorization(() =>
			{
				UploadBuild("/releases", paramsData, file, fileName, (result) =>
				{
					_isUploadStreamingAssets = false;
					Repaint();
				}, (err) =>
				{
					_isUploadStreamingAssets = false;
					Repaint();
				}, (progress) =>
				{
					_uploadStreaminAssetsProgress = progress;
					Repaint();
				});

			}, (err) =>
			{
				it.Logger.Log("Authorization error " + err);
				return;
			});
		}

		private void UploadButton()
		{
			UploadBuild();
		}
		private void UploadTest()
		{
			string filename = "VideoWall.zip";
			string path = GetArchivePath + "/" + filename;

			it.Logger.Log(path);
			if (!File.Exists(path))
			{
				it.Logger.LogError("Архив не найден");
				return;
			}

			Init();

			it.Logger.Log(ServerApi);

			List<KeyValuePair<string, string>> paramsData = new List<KeyValuePair<string, string>>();

			paramsData.Add(new KeyValuePair<string, string>("version", _version));
			paramsData.Add(new KeyValuePair<string, string>("checksum", "xx1"));
			paramsData.Add(new KeyValuePair<string, string>("description", "xx1"));


			it.Logger.Log("File path: " + path);
			byte[] file = System.IO.File.ReadAllBytes(path);

			it.Logger.Log("File: " + file.Length);

			Authorization(() =>
			{

				Authorization(() =>
				{
					UploadBuild("/releases", paramsData, file, filename, (result) =>
				{
					_isUploadBase = false;
					Repaint();
				}, (err) =>
				{
					_isUploadBase = false;
					Repaint();
				}, (progress) =>
				{
					_uploadBaseProgress = progress;
					Repaint();
				});

				}, (err) =>
				{
					it.Logger.Log("Authorization error " + err);
					return;
				});

			}, (err) =>
			{
				it.Logger.Log("Authorization error " + err);
				return;
			});
		}


		private void WebRequestBestHttp(HTTPMethods requestType, string url, List<KeyValuePair<string, string>> paramsData, string stringData, System.Action<string> OnComplete = null, System.Action<string> OnFalse = null)
		{

			var request = new HTTPRequest(new Uri(_serverRelease + url), requestType, false, (req, resp) =>
		 {

			 switch (req.State)
			 {
				 // The request finished without any problem.
				 case HTTPRequestStates.Finished:
					 if (resp.IsSuccess)
					 {
						 OnComplete?.Invoke(resp.DataAsText);
						 it.Logger.Log("OnComplete");

					 }
					 else
					 {
						 it.Logger.Log(string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2} {3}",
																 resp.StatusCode,
																 resp.Message,
																 resp.DataAsText,
																 req.Uri.AbsoluteUri));
					 }
					 break;

				 // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
				 case HTTPRequestStates.Error:
					 OnFalse?.Invoke(req.Exception.Message);
					 it.Logger.LogError("Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));
					 break;

				 // The request aborted, initiated by the user.
				 case HTTPRequestStates.Aborted:
					 OnFalse?.Invoke("Request Aborted!");
					 it.Logger.Log("Request Aborted!");
					 break;

				 // Connecting to the server is timed out.
				 case HTTPRequestStates.ConnectionTimedOut:
					 OnFalse?.Invoke("Connection Timed Out!");
					 it.Logger.LogError("Connection Timed Out!");
					 break;

				 // The request didn't finished in the given time.
				 case HTTPRequestStates.TimedOut:
					 OnFalse?.Invoke("Processing the request Timed Out!");
					 it.Logger.LogError("Processing the request Timed Out!");
					 break;
			 }


		 });
			it.Logger.Log("Request " + request.Uri.AbsoluteUri);

			//if (!string.IsNullOrEmpty(_token))
			//{
			//	request.AddHeader("Authorization", "Bearer app_id_1225119954916842d648ee3b4c193821116198bc4fa41c096c92fa937584f7c6");
			//}
			//request.AddHeader("Authorization", Encoding.UTF8.GetString(Encoding.Default.GetBytes("Bearer app_id_1225119954916842d648ee3b4c193821116198bc4fa41c096c92fa937584f7c6")));
			//request.AddHeader("Content-Type", "application/json");
			//request.AddField("Authorization", Encoding.UTF8.GetString(Encoding.Default.GetBytes("Bearer app_id_5535fg34fd")));
			//request.AddHeader("Authorization", utf8.GetString(utf8.GetBytes("Bearer app_id_34f98fuig89u458hn0o2")));
			//request.AddHeader("Token", utf8.GetString(utf8.GetBytes("Bearer app_id_34f98fuig89u458hn0o2")));


			if (!string.IsNullOrEmpty(stringData))
			{
				it.Logger.Log("Body " + stringData);
				//request.Context.Add("body", stringData);
				request.RawData = Encoding.UTF8.GetBytes(stringData);
			}

			//request.AddField(itm.Key, itm.Value);
			if (paramsData != null && paramsData.Count > 0)
			{
				foreach (var itm in paramsData)
				{
					it.Logger.Log(itm.Key + " : " + itm.Value);
					request.AddField(itm.Key, itm.Value);
				}

				//request.FormUsage = BestHTTP.Forms.HTTPFormUsage.Multipart;
			}
			UTF8Encoding utf8 = new UTF8Encoding();
			it.Logger.Log("Request " + Encoding.UTF8.GetString(Encoding.Default.GetBytes("Bearer app_id_5535fg34fd")));
			//request.AddHeader("Authorization", Encoding.UTF8.GetString(Encoding.Default.GetBytes("Bearer app_id_5535fg34fd")));
			request.RemoveHeader("authorization");
			request.AddHeader("Authorization", Encoding.UTF8.GetString(Encoding.Default.GetBytes("Bearer app_id_5535fg34fd")));


			//request.UseAlternateSSL = true;

			request.Send();

		}


		private void Authorization(System.Action OnComplete, System.Action<string> OnFalse)
		{

			List<KeyValuePair<string, string>> paramsList = new List<KeyValuePair<string, string>>();

			paramsList.Add(new KeyValuePair<string, string>("username", _login));
			paramsList.Add(new KeyValuePair<string, string>("password", _password));

			WebRequestBestHttp(HTTPMethods.Post, "/auth/login", paramsList, "", (resut) =>
			 {
				 var res = JsonUtility.FromJson<AuthResult>(resut);
				 _token = res.token;
				 it.Logger.Log("Auth token " + _token);
				 //GameManager.ServerToken = _token;
				 OnComplete?.Invoke();
			 },
			(error) =>
			{
				OnFalse?.Invoke(error);
				it.Logger.LogError("Login error " + error + " | Request: " + ServerApi + "/auth/login");
			});
		}

		private string GetHash(string path)
		{

			using (FileStream fs = System.IO.File.OpenRead(path))
			{
				MD5 md5 = new MD5CryptoServiceProvider();
				byte[] fileData = new byte[fs.Length];
				fs.Read(fileData, 0, (int)fs.Length);
				byte[] checkSum = md5.ComputeHash(fileData);
				string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);
				return result;
			}

		}

		IEnumerator UploadTheZip(string url, List<KeyValuePair<string, string>> paramsData, byte[] file, string filename, System.Action<string> OnComplete = null, System.Action<string> OnFalse = null)
		{
			// Create a form.
			WWWForm form = new WWWForm();

			for (int i = 0; i < paramsData.Count; i++)
			{
				form.AddField(paramsData[i].Key, paramsData[i].Value);
			}
			form.AddBinaryData("file", file);

			// Add the file.
			form.AddBinaryData("myTestFile.zip", file, "myFile.zip", "application/zip");

			// Send POST request.
			WWW POSTZIP = new WWW(ServerApi + url, form);

			it.Logger.Log("Sending zip...");
			yield return POSTZIP;
			it.Logger.Log("Zip sent!");
		}

		private IEnumerator UploadBuildCor(string url, List<KeyValuePair<string, string>> paramsData, byte[] file, string filename, System.Action<string> OnComplete = null, System.Action<string> OnFalse = null)
		{
			WWWForm form = new WWWForm();

			for (int i = 0; i < paramsData.Count; i++)
			{
				form.AddField(paramsData[i].Key, paramsData[i].Value);
			}
			form.AddBinaryData("file", file, filename, "application/zip");

			UnityWebRequest www = UnityWebRequest.Post(ServerApi + url, form);

			it.Logger.Log("Request " + www.url);

			//if (!string.IsNullOrEmpty(GameManager.ServerToken))
			//{
			//	www.SetRequestHeader("Authorization", "Bearer " + GameManager.ServerToken);
			//}

			yield return www.SendWebRequest();

			if (www.isNetworkError || www.isHttpError)
			{
				it.Logger.Log("Request err" + www.error);
				OnFalse?.Invoke(www.error);
			}
			else
			{
				if (www != null && www.downloadHandler != null)
				{
					it.Logger.Log("Request ok" + www.downloadHandler.text);
					OnComplete?.Invoke(www.downloadHandler.text);
				}
			}
		}

		private void TelegrammMessage(string message)
		{
			var request = new HTTPRequest(new Uri("https://api.telegram.org/bot1744139829:AAGdUcPAeo0-m0_8Xb02Kf65FKPA8j66lNU/sendMessage"), HTTPMethods.Post, true, true, (req, resp) =>
			{
				EditorUtility.ClearProgressBar();
				switch (req.State)
				{
					// The request finished without any problem.
					case HTTPRequestStates.Finished:
						if (resp.IsSuccess)
						{
							it.Logger.Log("OnComplete");

						}
						else
						{
							it.Logger.Log(string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2} {3}",
																 resp.StatusCode,
																 resp.Message,
																 resp.DataAsText,
																 req.Uri.AbsoluteUri));
						}
						break;

					// The request finished with an unexpected error. The request's Exception property may contain more info about the error.
					case HTTPRequestStates.Error:
						it.Logger.LogError("Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));
						break;

					// The request aborted, initiated by the user.
					case HTTPRequestStates.Aborted:
						it.Logger.Log("Request Aborted!");
						break;

					// Connecting to the server is timed out.
					case HTTPRequestStates.ConnectionTimedOut:
						it.Logger.LogError("Connection Timed Out!");
						break;

					// The request didn't finished in the given time.
					case HTTPRequestStates.TimedOut:
						it.Logger.LogError("Processing the request Timed Out!");
						break;
				}
			});

			request.SetHeader("Content-Type", "application/json");

			request.AddField("chat_id", _telegramChatId);
			request.AddField("parse_mode", "HTML");
			request.AddField("text", message);

			request.Send();
		}

		private void UploadBuild(string url, List<KeyValuePair<string, string>> paramsData, byte[] file, string filename, System.Action<string> OnComplete = null, System.Action<string> OnFalse = null, System.Action<float> OnProgress = null)
		{
			var request = new HTTPRequest(new Uri(ServerApi + url), HTTPMethods.Post, true, true, (req, resp) =>
				{
					EditorUtility.ClearProgressBar();
					switch (req.State)
					{
						// The request finished without any problem.
						case HTTPRequestStates.Finished:
							if (resp.IsSuccess)
							{
								OnComplete?.Invoke(resp.DataAsText);
								it.Logger.Log("OnComplete");

							}
							else
							{
								it.Logger.Log(string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2} {3}",
																	 resp.StatusCode,
																	 resp.Message,
																	 resp.DataAsText,
																	 req.Uri.AbsoluteUri));
							}
							break;

						// The request finished with an unexpected error. The request's Exception property may contain more info about the error.
						case HTTPRequestStates.Error:
							OnFalse?.Invoke(req.Exception.Message);
							it.Logger.LogError("Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));
							break;

						// The request aborted, initiated by the user.
						case HTTPRequestStates.Aborted:
							OnFalse?.Invoke("Request Aborted!");
							it.Logger.Log("Request Aborted!");
							break;

						// Connecting to the server is timed out.
						case HTTPRequestStates.ConnectionTimedOut:
							OnFalse?.Invoke("Connection Timed Out!");
							it.Logger.LogError("Connection Timed Out!");
							break;

						// The request didn't finished in the given time.
						case HTTPRequestStates.TimedOut:
							OnFalse?.Invoke("Processing the request Timed Out!");
							it.Logger.LogError("Processing the request Timed Out!");
							break;
					}
				});

			if (!string.IsNullOrEmpty(_token))
			{
				request.AddHeader("Authorization", "Bearer " + _token);
			}

			if (paramsData != null && paramsData.Count > 0)
			{
				foreach (var itm in paramsData)
				{
					it.Logger.Log(itm.Key + " : " + itm.Value);
					request.AddField(itm.Key, itm.Value);
				}
			}

			request.MaxFragmentQueueLength = 2048;
			request.Timeout = TimeSpan.FromSeconds(6000);

			request.AddBinaryData("file", file, filename, "application/zip");
			request.OnUploadProgress += (req, down, length) =>
			{
				EditorUtility.DisplayProgressBar("Simple Progress Bar", "Doing some work...", down / (float)length);
				OnProgress?.Invoke(down / (float)length);
				//it.Logger.Log(string.Format("Progress: {0:P2}", down / (float)length));
			};

			request.Send();
		}


		#endregion

		#region Resources

		private void GetResources()
		{
			if (_waitLoadResources) return;
			_waitLoadResources = true;


			Init();
			LoadResourcesData();

			//if (!string.IsNullOrEmpty(_token))
			//{
			//	//if (_tagsFields.Length <= 0)
			//	//	LoadTags();
			//	LoadResourcesData();
			//}
			//else
			//{
			//	Authorization(() =>
			//	{
			//		//LoadTags();
			//		LoadResourcesData();
			//	}, (err) =>
			//	{
			//		it.Logger.Log("Authorization error " + err);
			//		return;
			//	});
			//}
		}

		private void LoadResourcesData()
		{
			string urlPath = "/getGameList";
#if UNITY_WEBGL
			urlPath = "/getWebGLList";
#endif

			WebRequestBestHttp(HTTPMethods.Get, urlPath, null, "", (resut) =>
			{
				it.Logger.Log(resut);
				_waitLoadResources = false;

				_resourcesList.Clear();

				JArray jRes = JSON.ParseString(resut).GetJArray("items");

				for (int i = 0; i < jRes.Length; i++)
				{
					//var el = (RelelaseMaterial)it.Helpers.ParserHelper.Parse(typeof(RelelaseMaterial), jRes.GetJSON(i));
					var el = Newtonsoft.Json.JsonConvert.DeserializeObject<RelelaseMaterial>(jRes.GetJSON(i).CreatePrettyString());

					if (el != null)
						_resourcesList.Add(el);
				}
				//_resourcesList = _resourcesList.OrderByDescending(x => x.CreateTime).ToList();
				_resourcesList = _resourcesList;
				//PrefillSelectTag();
			},
			(error) =>
			{
				it.Logger.LogError("Error " + error + " | Request: " + ServerApi + "/releases");
			});
		}

		#endregion

		#region DrawBuildList

		private bool _waitLoadResources;
		private Vector2 _scrollList;

		void DrawResourcesList()
		{
			if (!_waitLoadResources && _resourcesList.Count == 0)
				GetResources();

			if (_resourcesList.Count == 0) return;

			_scrollList = EditorGUILayout.BeginScrollView(_scrollList);
			EditorGUILayout.BeginVertical();

			foreach (var elem in _resourcesList)
				DrawResourceItem(elem);

			EditorGUILayout.EndVertical();
			EditorGUILayout.EndScrollView();

		}

		void DrawResourceItem(RelelaseMaterial item)
		{
			EditorGUILayout.BeginHorizontal("box");
			EditorGUILayout.LabelField(item.app_version_type_title);
			EditorGUILayout.LabelField(item.app_version);

			item.app_version_type_title = EditorGUILayout.TextField(item.app_version_type_title);

			if (string.IsNullOrEmpty(item.app_version_type_title))
				item.app_version_type_title = "Game";


			//int oldSelectTag = item.SelectTags;

			//item.SelectTags = EditorGUILayout.MaskField(item.SelectTags, _tagsFields);

			//if (oldSelectTag != item.SelectTags)
			//{
			//	for (int i = 0; i < _tagsFields.Length; i++)
			//	{
			//		int layer = 1 << i;
			//		if ((item.SelectTags & layer) != 0)
			//		{
			//			it.Logger.Log(_tagsFields[i]);
			//		}
			//	}
			//}

			GUI.backgroundColor = Color.green;
			if (GUILayout.Button("Save"))
			{
				//item.TagsIds.Clear();
				//for (int i = 0; i < _tagsFields.Length; i++)
				//{
				//	int layer = 1 << i;
				//	if ((item.SelectTags & layer) != 0)
				//	{
				//		item.TagsIds.Add(_tagsList.Find(x => x.Name == _tagsFields[i]).Id);
				//	}
				//}

				////	WebRequestBestHttp(HTTPMethods.Post, "/auth/login", paramsList, "", (resut) =>
				////	{
				////		var res = JsonUtility.FromJson<AuthResult>(resut);
				////		_token = res.token;
				////		it.Logger.Log("Auth token " + _token);
				////		GameManager.ServerToken = _token;
				////		OnComplete?.Invoke();
				////	},
				////(error) =>
				////{
				////	OnFalse?.Invoke(error);
				////	it.Logger.LogError("Login error " + error + " | Request: " + ServerApi + "/auth/login");
				////});

				//item.Save();
			}
			GUI.backgroundColor = Color.red;
			if (GUILayout.Button("Del"))
			{
				DeleteResourcesItem(item);
			}
			GUI.backgroundColor = Color.white;


			EditorGUILayout.EndHorizontal();
		}

		private void DeleteResourcesItem(RelelaseMaterial release)
		{
			WebRequestBestHttp(HTTPMethods.Delete, "/releases/" + release.id, null, "", (resut) =>
			 {
				 _resourcesList.Clear();
				 TelegrammMessage($"#releaseWall {release.app_version} удалена");
				 Repaint();

			 });
		}

		#endregion

	}

	public class RelelaseMaterial
	{
		
		public ulong id;
		
		public string file;
		
		public string app_version;
		
		public int app_version_type;
		
		public int app_type;
		
		public int crucially;
		
		public string descr;
		
		public ulong user_id;
		
		public string date_time_upload;
		
		public string date_time_changed;
		
		public string app_version_type_title;
		
		public string app_type_title;
	}
	/*

	{
      "id": 195,
      "file": "https:\/\/app.garillapoker.com\/system\/apps\/game\/052440\/GarillaPoker_0.5.244.0.zip",
      "app_version": "0.5.244.0",
      "app_version_type": 2,
      "app_type": 2,
      "crucially": 0,
      "descr": "",
      "user_id": 2,
      "date_time_upload": "2022-07-31 11:02:04",
      "date_time_changed": null,
      "app_version_type_title": "Game",
      "app_type_title": "Production"
    }

	*/
}