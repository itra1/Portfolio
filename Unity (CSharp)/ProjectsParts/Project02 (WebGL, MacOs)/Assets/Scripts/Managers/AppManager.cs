using it.Managers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Sett = it.Settings;
using Garilla;
using System;
using DG.Tweening;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using com.ootii.Messages;
using Garilla.Update;

public class AppManager : Singleton<AppManager>
{
	public WindowType AppState { get => _appState; set => _appState = value; }
	public SessionSaver Session { get => _session; set => _session = value; }

#if UNITY_STANDALONE

	//public bool IsLobbyTable => CommandLineController.GetIdTableFromExe() == null;

#endif

	private System.DateTime _unFocusTime = System.DateTime.Now;

	[SerializeField] private UiManager _uiManager;

	private WindowType _appState;
	private SessionSaver _session = new SessionSaver();

	public bool IsFullLoad =>
	(UserController.Instance != null && UserController.ReferenceData != null);

	private void Awake()
	{
#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
		AppConfig.IsDevApp = true;
#elif UNITY_STANDALONE
		CommandLineController.ConfirmArguments();

#endif

		Screen.sleepTimeout = SleepTimeout.NeverSleep;
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
		AppConfig.IsLog = true;
#endif
#if UNITY_STANDALONE
		string promoStr = CommandLineController.GetPromoExe();

		if (!string.IsNullOrEmpty(promoStr))
		{
			promoStr = Encoding.UTF8.GetString(Convert.FromBase64String(promoStr));

			PlayerPrefs.SetString("promo", promoStr);
		}
#endif


		callOnMainThreadList = new List<KeyValuePair<System.Action<object>, object>>();
		DontDestroyOnLoad(gameObject);

		MessageDispatcher.sStub = gameObject.AddComponent<MessageDispatcherStub>();
		gameObject.AddComponent<WWWCachier>();
		gameObject.AddComponent<TimerManager>();
		gameObject.AddComponent<SettingsController>();
		gameObject.AddComponent<NetworkManager>();
		gameObject.AddComponent<UpdateController>();

#if UNITY_STANDALONE_WIN
		gameObject.AddComponent<WinStandaloneController>();
#elif UNITY_STANDALONE_OSX
		gameObject.AddComponent<MacOSStandaloneController>();
		MacOSStandaloneController.PrintHelloFromMac();
#endif
#if UNITY_WEBGL && !UNITY_EDITOR
		gameObject.AddComponent<DeepLinksController>();
		gameObject.AddComponent<WebGLController>();
#endif
		gameObject.AddComponent<ServerManager>();



		ServerManager.OnServersSetComplete += () =>
		{
			CreateComponents();
		};
		//
	}

	private void OnApplicationFocus(bool focus)
	{
		if (Application.isEditor) return;

		if (!focus)
			_unFocusTime = System.DateTime.Now;
		else
		{
#if UNITY_WEBGL
		WebGLFocus();
#elif UNITY_ANDROID
			AndroidFocus();
#endif
		}

	}

#if UNITY_WEBGL
	private void WebGLFocus()
	{
		if((System.DateTime.Now - _unFocusTime).TotalMinutes > 5){
			reloadPage();
		}
	}

	[DllImport("__Internal")]
	private static extern void reloadPage();

#elif UNITY_ANDROID

	private void AndroidFocus()
	{
		if ((System.DateTime.Now - _unFocusTime).TotalMinutes < 5) return;

			using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			const int kIntent_FLAG_ACTIVITY_CLEAR_TASK = 0x00008000;
			const int kIntent_FLAG_ACTIVITY_NEW_TASK = 0x10000000;

			var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			var pm = currentActivity.Call<AndroidJavaObject>("getPackageManager");
			var intent = pm.Call<AndroidJavaObject>("getLaunchIntentForPackage", Application.identifier);

			intent.Call<AndroidJavaObject>("setFlags", kIntent_FLAG_ACTIVITY_NEW_TASK | kIntent_FLAG_ACTIVITY_CLEAR_TASK);
			currentActivity.Call("startActivity", intent);
			currentActivity.Call("finish");
			var process = new AndroidJavaClass("android.os.Process");
			int pid = process.CallStatic<int>("myPid");
			process.CallStatic("killProcess", pid);
		}
	}

#endif

	private void Start()
	{
#if UNITY_WEBGL
		Application.targetFrameRate = 30;
#endif
		_oldUpdate = DateTime.Now;
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserLogin, (handler) =>
		{
			//GetComponent<TablesUIManager>().Init();
		});
		StartCoroutine(ResourcesClearCoroutines());
		//StartCoroutine(GCClearCoroutines());
	}


	IEnumerator ResourcesClearCoroutines()
	{
		yield return new WaitForSeconds(20);
		Resources.UnloadUnusedAssets();
	}
	//IEnumerator GCClearCoroutines()
	//{
	//	yield return new WaitForSeconds(1);
	//	GC.Collect();
	//}

	[ContextMenu("prit")]
	public void PrintTest()
	{
		it.Logger.Log(string.Format("{0:D2}:{1:D2}", 1, 12));
	}
	private void CreateComponents()
	{
#if UNITY_STANDALONE
		ConfigManager.Load();
		if (AppConfig.IsLog)
		{
			gameObject.AddComponent<FileLog>();

			ulong? tb = CommandLineController.GetIdTableFromExe();
			if (tb == null)
				FileLog.Instance.Init("lobby" + (CommandLineController.GetSession() == null ? "" : $"_session{CommandLineController.GetSession()}"));
			else
				FileLog.Instance.Init($"loading_table_{tb}" + (CommandLineController.GetSession() == null ? "" : $"_session{CommandLineController.GetSession()}"));
			//FileLog.Instance.Init($"loading_table_load");
		}
		CommandLineController.PrintArguments();
#endif
		gameObject.AddComponent<UserController>();
		gameObject.AddComponent<LocalizeController>();
		gameObject.AddComponent<TableManager>();
		gameObject.AddComponent<HelpMaterialController>();


#if UNITY_EDITOR && !(UNITY_WEBGL || UNITY_ANDROID || UNITY_IOS)
		gameObject.AddComponent<LobbyManager>();
		gameObject.AddComponent<GameManager>();
		var idTableFromExe = CommandLineController.GetIdTableFromExe();
		if (idTableFromExe != null)
			OpenGame((ulong)idTableFromExe);
#else
		InitBaseComponent();
#endif

	}

	private void InitBaseComponent()
	{
		it.Logger.Log("InitBaseComponent");
		var idTableFromExe = AppConfig.TableExe;

		it.Logger.Log("InitBaseComponent = " + idTableFromExe);
		if (idTableFromExe != null)
		{
			gameObject.AddComponent<GameManager>();
			OpenGame((ulong)idTableFromExe);
		}
		else
		{
			gameObject.AddComponent<LobbyManager>();
		}
	}

	public void OpenGame(ulong tableId)
	{
		GameManager.Instance.SetTableId(tableId);
	}

	public void CloseGame()
	{

	}

	public static void SetPointerCursor()
	{
		Cursor.SetCursor(Sett.AppSettings.Pointer, new Vector2(7, 0), CursorMode.Auto);
	}

	public static void SetDefaultCursor()
	{
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
	}

	private DateTime _oldUpdate;

	private void Update()
	{
		CheckCallOnMainThread();

		//DOTween.ManualUpdate((float)(DateTime.Now - _oldUpdate).TotalSeconds, (float)(DateTime.Now - _oldUpdate).TotalSeconds);

		//DOTween.ManualUpdate(1, 1);
		//_oldUpdate = DateTime.Now;
	}


	#region ???????????????

	public static System.Threading.Thread unityMainThread;

	public static bool IsUnityMainThread()
	{
#if UNITY_EDITOR
		if (unityMainThread == null) return false;
#endif
		return System.Threading.Thread.CurrentThread == unityMainThread;
	}

	[System.NonSerialized] private List<KeyValuePair<System.Action<object>, object>> callOnMainThreadList;
	public void CallOnMainThread(System.Action<object> callback, object data = null)
	{
		lock (callOnMainThreadList)
		{
			callOnMainThreadList.Add(new KeyValuePair<System.Action<object>, object>(callback, data));
		}
	}

	protected void CheckCallOnMainThread()
	{
		while (callOnMainThreadList.Count > 0)
		{    // Checking for count should be thread safe ish. May get 0 once even if it actually had 1+ but that is ok.
			KeyValuePair<System.Action<object>, object> callbackPair;
			int count;
			lock (callOnMainThreadList)
			{
				count = callOnMainThreadList.Count;
				if (count > 0)
				{
					callbackPair = callOnMainThreadList[0];
					callOnMainThreadList.RemoveAt(0);
				}
				else
				{
					callbackPair = new KeyValuePair<System.Action<object>, object>(null, null);
				}
			}

			if (count <= 0)
			{
				// Someone removed it from the list? Should not happen.
				break;
			}

			if (callbackPair.Key != null)
			{
				try
				{
					callbackPair.Key(callbackPair.Value);
				}
				catch
				{
				}

			}

		}
	}

	#endregion

}
