using it.Network.Rest;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StandaloneController : MonoBehaviour
{
	public static StandaloneController Instance { get; private set; }

	public List<WindowTable> TableWindows { get => _tableWindows; set => _tableWindows = value; }
	public Dictionary<string, DateTime> WaitOpenWindow { get => _waitOpenWindow; set => _waitOpenWindow = value; }

	protected Vector2Int LOBBY_SIZE => new Vector2Int(1200, 740);
	protected Vector2Int GAME_SIZE => new Vector2Int(920, 650);
	protected float GameProportion => (float)GAME_SIZE.x / (float)GAME_SIZE.y;

	public class WindowTable
	{
		public string Name;
		public ulong Id;
		public IntPtr Descripter;
	}

	public static string appWindowName = "GarillaPoker";
	public static string appWindowNameDefult = "GarillaPoker";
	public List<WindowTable> _tableWindows = new List<WindowTable>();

	protected Dictionary<string, DateTime> _waitOpenWindow = new Dictionary<string, DateTime>();
	protected Dictionary<ulong, RectTransform> games = new Dictionary<ulong, RectTransform>();

	private void Awake()
	{
		Instance = this;
		AwakeInit();
	}

	private void Update()
	{
		CheckExistsWindows();
		RemoveOldWaitWindow();

		UpdateAny();
	}

	protected virtual void AwakeInit()
	{

	}

	protected virtual void UpdateAny()
	{

	}

	public string GetLobbyWindowName()
	{
		return $"Garilla Poker - {UserController.User.nickname}";
	}
	public static string GetWindowTableName(Table table)
	{
		return $"{table.id} {table.name}: {UserController.User.nickname}";
	}
	public void ShowGames()
	{
		gameObject.SetActive(true);
	}

	public void HideGames()
	{
		gameObject.SetActive(false);
	}
	public void CloseApplication()
	{
		Application.Quit();
	}

	public void SaveWindowPlayerPrefs(string name)
	{
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
		PlayerPrefs.SetString(StringConstants.WINDOW_NEW, name);
#endif
	}

	#region Очередь открытия

	/// <summary>
	/// Добавляет в очередь открытия
	/// </summary>
	/// <param name="windowName">Название окна</param>
	public void AddOpenQueue(string windowName)
	{
		_waitOpenWindow.Add(windowName, System.DateTime.Now);
	}
	/// <summary>
	/// Удаление из очереди открытия
	/// </summary>
	/// <param name="windowName">Название окна</param>
	public void RemoveOpenQueue(string windowName)
	{
		_waitOpenWindow.Remove(windowName);
	}
	/// <summary>
	/// Проверка на очередь открытия
	/// </summary>
	/// <param name="windowName">Название окна</param>
	/// <returns></returns>
	public bool CheckExistsQueue(string windowName)
	{
		return _waitOpenWindow.ContainsKey(windowName);
	}
	List<string> _toRemove = new List<string>();
	/// <summary>
	/// Удаление устаревшие
	/// </summary>
	private void RemoveOldWaitWindow()
	{
#if UNITY_STANDALONE && !UNITY_EDITOR
		foreach (var key in _waitOpenWindow.Keys)
			if ((System.DateTime.Now - _waitOpenWindow[key]).TotalSeconds > 30)
			{
				_toRemove.Add(key);
			}

		if (_toRemove.Count > 0)
		{
			for (int i = 0; i < _toRemove.Count; i++)
			{
				RemoveOpenQueue(_toRemove[i]);
			}
		_toRemove.Clear();
		}

#endif
	}

	#endregion

	public virtual void CheckExistsWindows(bool force = false)
	{

	}

	public static bool CloseAllTable()
	{
		return Instance._CloseAllTable();
	}

	protected virtual bool _CloseAllTable()
	{
		PlayerPrefs.SetInt(StringConstants.CloseNow, 1);
		return true;
	}
	protected string GetOpenAppArguments(Table table, string password = "", bool join = false)
	{
		string arguments = $" tableId={table.id}";

#if UNITY_STANDALONE

		if (join) arguments += " autoJoin";
		int? _session = CommandLineController.GetSession();
		if (_session != null)
			arguments += " session=" + _session;
		if (!string.IsNullOrEmpty(password))
			arguments += " password=" + password;
		if (!string.IsNullOrEmpty(AppConfig.CustomServer))
			arguments += " serverApi=" + AppConfig.CustomServer;
		if (!string.IsNullOrEmpty(AppConfig.CustomServerWS))
			arguments += " serverWS=" + AppConfig.CustomServerWS;

		if (!string.IsNullOrEmpty(ServerManager.SourceDataBase64))
		{
			arguments += " -servData " + ServerManager.SourceDataBase64;
		}
		if (AppConfig.IsLog)
			arguments += " -log";

#endif

		return arguments;
	}

	public virtual void FocusMain() { }

	public virtual void FocusWindow(it.Network.Rest.Table table) { }

	public virtual void FocusWindow() { }

	public virtual void OpenApp(string path) { }

	public virtual void SetLobbySize()
	{
#if !UNITY_EDITOR
		Screen.fullScreen = false;
		UiManager.Instance.AspectRationFilder.aspectRatio = (float)LOBBY_SIZE.x / (float)LOBBY_SIZE.y;
#endif
	}
	public virtual void SetGameSize()
	{
#if !UNITY_EDITOR
		Screen.fullScreen = false;
		CanvasScaler cs = UiManager.Instance.GetComponentInParent<CanvasScaler>();
		cs.referenceResolution = new Vector2(GAME_SIZE.x, GAME_SIZE.y);
		UiManager.Instance.AspectRationFilder.aspectRatio = (float)GAME_SIZE.x / (float)GAME_SIZE.y;
#endif
	}
	public virtual void ExpandApplication(bool fromDrag = false) { }
	public virtual void WindowResizeStart() { }
	public virtual void WindowDragStop() { }
	public virtual void MinimizeApplication() { }
	public virtual void WindowDragStart() { }
	public virtual void RemoveGameFromParent(it.Network.Rest.Table table) { }
	public virtual void WindowResizeEnd() { }
	public virtual void SetGridTableOrder() { }
	public virtual void SetQueueTableOrder() { }
	public virtual void FindTableWindow(string name, ulong tableId) { }
	public static void SetNewWindowName(string newName)
	{
		Instance._SetNewWindowName(newName);
	}
	public virtual void _SetNewWindowName(string newName)
	{

	}
	public static void SetNewWindowName(it.Network.Rest.Table table)
	{
		Instance._SetNewWindowName(table);
	}
	public virtual void _SetNewWindowName(it.Network.Rest.Table table)
	{

	}
	public static bool FindTableWindow(Table table, bool focus)
	{
		return Instance._FindTableWindow(table, focus);
	}
	public virtual bool _FindTableWindow(Table table, bool focus)
	{
		return true;
	}
	public static void OpenNewTableWindow(Table table, string password = "", bool join = false)
	{
		it.Logger.Log("start window");
		Instance.CheckExistsWindows(true);
#if UNITY_STANDALONE && !UNITY_EDITOR
		if (Instance.CheckExistsQueue(GetWindowTableName(table))) return;
		if (Instance.TableWindows.Find(x => x.Name == (GetWindowTableName(table))) != null) return;
		Instance.AddOpenQueue(GetWindowTableName(table));

		SessionSaver.SaveTokenForMultiWindow();

		Instance._OpenNewTableWindow(table, password, join);
#endif
	}
	public virtual void _OpenNewTableWindow(Table table, string password = "", bool join = false)
	{
	}

	public void AddNewGame(Table table)
	{
		ShowGames();
		GameHelper.SelectTable = table;
		if (!games.ContainsKey(table.id))
		{
			//var script = Instantiate(GameInitManager, ScreenSwiper.Content);

			var script = UiManager.Instance.OpenGame();
			var rect = script.GetComponent<RectTransform>();
			script.Init(table);


			//games.Add(table.Id, rect);
			//ScreenSwiper.screens.Add(rect);
			//ScreenSwiper.RefreshContents();
			//ScreenSwiper.GoToScreen(games.Count - 1, true);
		}
		else
		{
			//int numberPage = Array.IndexOf(games.Keys.ToArray(), table.Id);
			//ScreenSwiper.GoToScreen(numberPage, true);
		}
	}

}