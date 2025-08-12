using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.IO;
using it.Network.Rest;
using System.Linq;
using DG.Tweening;
using System.Threading;
using UnityEngine.UI;
using DG.Tweening.Plugins.Core.PathCore;
using System.Security.Permissions;
using UnityEngine.UIElements;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using Path = System.IO.Path;

/// <summary>
/// Контроллер для виндоуса
/// </summary>
public class WinStandaloneController : StandaloneController
{

	private System.IntPtr _window;
	private IntPtr _taskbarHWnd;  // Описатель панели задач
	private bool _isDragPointer;
	private bool _isRezisePointer;
	private bool _isFull;
	private RECT _rect;
	private POINT mPos;
	private Vector2Int _windowSize;
	private RECT _beforeExpand;
	private RECT _taskBarRect;
	private double _lastCheck;
	private int indexOrder = -20;


	protected override void AwakeInit()
	{
		base.AwakeInit();
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
		
		//CommandLineController.ConfirmArguments();
		_taskbarHWnd = FindWindow("Shell_TrayWnd", null);
		_window = GetActiveWindow();
		_lastCheck = Time.timeAsDouble;
		SetWindowLong(_window, -16, /*(uint)WindowStyles.WS_VISIBLE |*/ /*(uint)WindowStyles.WS_POPUP |*/ (uint)WindowStyles.WS_MINIMIZE);
		//SetWindowLong(_window, ++indexOrder, /*(uint)WindowStyles.WS_VISIBLE |*/ (uint)WindowStyles.WS_POPUP | (uint)WindowStyles.WS_MINIMIZE | (uint)WindowStyles.WS_OVERLAPPED);

		if (AppConfig.IsLobby)
			SetLobbySize();
		else
			SetGameSize();

		//SetSize(1200, 740);
		_isFull = false;

#endif
	}


	/// <summary>
	/// Расстановка окон сеткой
	/// </summary>
	public override void SetGridTableOrder()
	{
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
		if (_tableWindows.Count == 0) return;

		float screenHeight = Screen.currentResolution.height;
		float screenWidth = Screen.currentResolution.width;

		GetWindowRect(_taskbarHWnd, out _taskBarRect);

		int taskBarHeight = _taskBarRect.Bottom - _taskBarRect.Top;
		int taskBarWidth = _taskBarRect.Right - _taskBarRect.Left;

		float startX;
		float startY;
		float width;
		float height;

		if (taskBarWidth > taskBarHeight)
			screenHeight -= taskBarHeight;
		else
			taskBarWidth -= taskBarWidth;

		if (_tableWindows.Count == 1)
		{
			startX = screenWidth / 2 - GAME_SIZE.x / 2;
			startY = screenHeight / 2 - GAME_SIZE.y / 2;
			SetWindowPos(_tableWindows[0].Descripter, (IntPtr)SpecialWindowHandles.HWND_TOP, (int)startX, (int)startY, GAME_SIZE.x, GAME_SIZE.y, SetWindowPosFlags.SWP_SHOWWINDOW);

			return;
		}

		if (_tableWindows.Count == 2)
		{
			startY = screenHeight / 2 - GAME_SIZE.y / 2;
			startX = 0;
			width = Mathf.Min(GAME_SIZE.x, screenWidth / 2);
			height = Mathf.Min(GAME_SIZE.y, ((float)GAME_SIZE.y / (float)GAME_SIZE.x) * width);

			for (int y = 0; y < _tableWindows.Count; y++)
			{
				SetWindowPos(_tableWindows[y].Descripter, (IntPtr)SpecialWindowHandles.HWND_TOP, (int)startX + y * (int)width, (int)startY, (int)width, (int)height, SetWindowPosFlags.SWP_SHOWWINDOW);
			}
			return;
		}

		int gridSize = 2;
		while (gridSize * gridSize < _tableWindows.Count)
			gridSize++;

		float maxHeight = Mathf.Min(GAME_SIZE.y, screenHeight / gridSize);
		float maxWidth = Mathf.Min(GAME_SIZE.x, screenWidth / gridSize);

		width = maxWidth;
		//height = maxHeight;
		height = maxWidth / GameProportion;

		if (height > maxHeight)
		{
			height = maxHeight;
			width = height * GameProportion;
		}

		for (int y = 0; y < gridSize; y++)
		{

			for (int x = 0; x < gridSize; x++)
			{
				int index = y * gridSize + x;

				if (_tableWindows.Count <= index) continue;

				var itm = _tableWindows[index];

				SetWindowPos(itm.Descripter, (IntPtr)SpecialWindowHandles.HWND_TOP, x * (int)width, y * (int)height, (int)width, (int)height, SetWindowPosFlags.SWP_SHOWWINDOW);
			}
		}
#endif
	}
	/// <summary>
	/// Расстановка окон очередью
	/// </summary>
	public override void SetQueueTableOrder()
	{
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
		if (_tableWindows.Count == 0) return;

		int distance = 30;
		float w = _tableWindows.Count * distance;
		float startX = Screen.currentResolution.width / 2 - w / 2 - GAME_SIZE.y / 2;
		float startY = Screen.currentResolution.height / 2 - w/2 - GAME_SIZE.y/2;
		//float startY = startX / GameProportion;

		for (int i = 0; i < _tableWindows.Count; i++)
		{

			var itm = _tableWindows[i];
			GetWindowRect(itm.Descripter, out RECT rt);

			SetWindowPos(itm.Descripter, (IntPtr)SpecialWindowHandles.HWND_TOP, (int)startX + i * distance, (int)startY + i * distance, GAME_SIZE.x, GAME_SIZE.y, SetWindowPosFlags.SWP_SHOWWINDOW);

		}
#endif
	}

	public override void _OpenNewTableWindow(Table table, string password = "", bool join = false)
	{

		var path = "";
#if UNITY_STANDALONE_WIN
		path = Path.GetDirectoryName(Application.dataPath) + "/" + Application.productName + ".exe";
#elif UNITY_STANDALONE_OSX
		path = Path.GetDirectoryName(Application.dataPath) + "/Contents/MacOS/" + Application.productName;
#endif

		string arguments = GetOpenAppArguments(table, password, join);

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR


		const uint NORMAL_PRIORITY_CLASS = 0x0020;

		bool retValue;
		string AppRun = path;
		string CommandLine = arguments;
		PROCESS_INFORMATION pInfo = new PROCESS_INFORMATION();
		STARTUPINFO sInfo = new STARTUPINFO();
		SECURITY_ATTRIBUTES pSec = new SECURITY_ATTRIBUTES();
		SECURITY_ATTRIBUTES tSec = new SECURITY_ATTRIBUTES();
		pSec.nLength = Marshal.SizeOf(pSec);
		tSec.nLength = Marshal.SizeOf(tSec);

		//Open Notepad
		retValue = CreateProcess(AppRun, CommandLine,
		ref pSec, ref tSec, false, NORMAL_PRIORITY_CLASS,
		IntPtr.Zero, null, ref sInfo, out pInfo);

		it.Logger.Log("Process ID (PID): " + pInfo.dwProcessId);
		it.Logger.Log("Process Handle : " + pInfo.hProcess);


		//System.Diagnostics.Process.Start(path, arguments);
#endif
	}

	public override void OpenApp(string path)
	{
		base.OpenApp(path);

		string arguments = "";

		string[] argumentsArr = Environment.GetCommandLineArgs();

		for (int i = 0; i < argumentsArr.Length; i++)
			arguments += " " + argumentsArr[i];

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
		const uint NORMAL_PRIORITY_CLASS = 0x0020;
		bool retValue;
		string AppRun = path;
		string CommandLine = arguments;
		PROCESS_INFORMATION pInfo = new PROCESS_INFORMATION();
		STARTUPINFO sInfo = new STARTUPINFO();
		SECURITY_ATTRIBUTES pSec = new SECURITY_ATTRIBUTES();
		SECURITY_ATTRIBUTES tSec = new SECURITY_ATTRIBUTES();
		pSec.nLength = Marshal.SizeOf(pSec);
		tSec.nLength = Marshal.SizeOf(tSec);
		CreateProcess(AppRun, CommandLine,
			ref pSec, ref tSec, false, NORMAL_PRIORITY_CLASS,
			IntPtr.Zero, null, ref sInfo, out pInfo);
#endif
	}

	public override void RemoveGameFromParent(Table table)
	{
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
		return;
		int numberPage = Array.IndexOf(games.Keys.ToArray(), table.id);
		//ScreenSwiper.screens.RemoveAt(numberPage);
		games.Remove(table.id);
		if (games.Count == 0)
		{
			HideGames();
		}
		else
		{
			//ScreenSwiper.RefreshContents();
			//ScreenSwiper.GoToScreen(games.Count - 1, true);
		}

		TablesUIManager.Instance.UpdateCurrentTable();
#endif
	}

	protected override bool _CloseAllTable()
	{
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
		base._CloseAllTable();
        var tablesOpen = TablesUIManager.tablesOpen;
        for (int i = 0; i < tablesOpen.Count; i++)
        {
            var windowPtr = FindWindow(null, tablesOpen[i]);
            if(windowPtr.ToString() != "0") DestroyWindow(windowPtr);
        }
#endif
		return true;
	}

	public static bool _FindTableWindow(Table table, bool focus)
	{
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        string windowName = GetWindowTableName(table); //$"{table.name}: {GameController.UserInfo.nickname}";
        IntPtr findWindow = FindWindow(null, windowName);
        if (findWindow.ToString() != "0")
        {
            if (focus) SwitchToThisWindow(findWindow, true);            
            return true;
        }
        else
        {
            return false;
        }
#else
		return false;
#endif
	}

//	public static bool FindEmptyWindow(string debug)
//	{
//#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
//        IntPtr findWindow = Instance.FindWindow(null, appWindowNameDefult);
//        if (findWindow.ToString() != "0")
//        {
//            if (debug != null && debug != "")
//            {
//                //SSTools.ShowMessage(debug, SSTools.Position.bottom, SSTools.Time.twoSecond);
//            }
//            return true;
//        }
//        else
//        {
//            return false;
//        }
//#else
//		return false;
//#endif
//	}

	#region Открытые дочерние окна

	public override void FindTableWindow(string name, ulong tableId)
	{
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
		var tableDiscription = FindWindow(null, name);

		if (tableDiscription.ToString() != "0")
		{
		Instance.RemoveOpenQueue(name);
			_tableWindows.Add(new WindowTable()
			{
				Name = name,
				Id = tableId,
				Descripter = tableDiscription
			});
			com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.WindowsTableListChange);
		}
#endif
	}

	public override void CheckExistsWindows(bool force = false)
	{
		base.CheckExistsWindows(force);

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR

		if (!force && Time.timeAsDouble - _lastCheck < 5)
			return;
		_lastCheck = Time.timeAsDouble;
		if (TableWindows.Count == 0) return;

		foreach (var elem in TableWindows)
		{
			var tableDiscription = FindWindow(null, elem.Name);
			if(tableDiscription == null || tableDiscription.ToString() == "0"){
				TableWindows.Remove(elem);
				CheckExistsWindows();
				com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.WindowsTableListChange);
				return;
			}
		}
#endif
	}

	#endregion


	public override void SetLobbySize()
	{
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
		base.SetLobbySize();
		SetSize(LOBBY_SIZE.x, Mathf.Min(Screen.currentResolution.height, LOBBY_SIZE.y));
#endif
	}

	public override void SetGameSize()
	{
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
		base.SetGameSize();
		SetSize(GAME_SIZE.x, Mathf.Min(Screen.currentResolution.height, GAME_SIZE.y));
#endif
	}

	public void SetSize(int width, int height)
	{
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
		_windowSize = new Vector2Int(width, height);
		Thread myThread = new Thread(() =>
	{
		GetWindowRect(_window, out RECT rt);
		do
		{
			Thread.Sleep(100);
			SetWindowPos(_window, (IntPtr)SpecialWindowHandles.HWND_TOP, rt.Left, rt.Top, width, height, SetWindowPosFlags.SWP_SHOWWINDOW);
			GetWindowRect(_window, out rt);
		} while ((rt.Right - rt.Left) != width);
	});
		myThread.Start();
#endif
	}

	public override void ExpandApplication(bool fromDrag = false)
	{

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
		_isFull = !_isFull;
		GetWindowRect(_taskbarHWnd, out _taskBarRect);

		int taskBarHeight = _taskBarRect.Bottom - _taskBarRect.Top;
		int taskBarWidth = _taskBarRect.Right - _taskBarRect.Left;

		GetWindowRect(_window, out RECT wRect);
		if (_isFull)
			_beforeExpand = wRect;

		int height = Screen.currentResolution.height;

		if (taskBarWidth > taskBarHeight)
			height -= taskBarHeight;

		int startPositionY = _isFull ? 0 : (fromDrag ? wRect.Top : _beforeExpand.Top);
		if (_isFull && _taskBarRect.Top == 0 && taskBarWidth > taskBarHeight)
			startPositionY += taskBarHeight;

		SetWindowPos(_window, (IntPtr)SpecialWindowHandles.HWND_TOP,
		wRect.Left, startPositionY, 1200, _isFull ? height : _windowSize.y,
		SetWindowPosFlags.SWP_SHOWWINDOW);
#endif
	}

	public static void FocusWindow(bool bl)
	{
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        SwitchToThisWindow(FindWindow(null, appWindowName), bl);
#endif
	}

	public override void _SetNewWindowName(string newName)
	{
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        //var windowPtr = FindWindow(null, appWindowName);
        appWindowName = newName;
        SetWindowText(_window, appWindowName);
		SaveWindowPlayerPrefs(appWindowName);
#endif
	}

	public override void _SetNewWindowName(Table table)
	{
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        string windowName = GetWindowTableName(table);
		PlayerPrefs.SetString(windowName, table.id.ToString());
		SetNewWindowName(windowName);
#endif
	}

	public override void FocusMain()
	{
		base.FocusMain();
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
		IntPtr findWindow = FindWindow(null, $"Garilla Poker - " + UserController.User.nickname);
		FocusWindow(findWindow);
#endif
	}

	public override void FocusWindow(it.Network.Rest.Table table)
	{
		base.FocusWindow(table);

		foreach (var elem in TableWindows)
		{
			if (elem.Id == table.id)
			{
				FocusWindow(elem.Descripter);


#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
				////		DOVirtual.DelayedCall(0.2f, () =>
				////{
				////SetSize(_windowSize.x, _windowSize.y);
				//if (GetWindowRect(elem.Descripter, out RECT rt))
				//{

				//	int sizeX = rt.Right - rt.Left;
				//	int sizeY = rt.Bottom - rt.Top;
				//	int posX = rt.Left;
				//	int posY = rt.Top;
				//	//if (posX < 0) posX = 0;
				//	//if (posY < 0) posY = 0;

				//	//SetWindowLong(elem.Descripter, -16, /*(uint)WindowStyles.WS_VISIBLE |*//* (uint)WindowStyles.WS_POPUP |*/ (uint)WindowStyles.WS_MINIMIZE);
				//	SetFocus(elem.Descripter);
				//	SetActiveWindow(elem.Descripter);
				//	FlashWindow(elem.Descripter, true);
				//	SwitchToThisWindow(elem.Descripter, true);

				//	SetWindowPos(elem.Descripter, (IntPtr)SpecialWindowHandles.HWND_TOPMOST, posX, posY, sizeX, sizeY, SetWindowPosFlags.SWP_SHOWWINDOW);

				//	it.Logger.Log(rt.Left + " : " + rt.Top + " : " + sizeX + " : " + sizeY);
				//}

				////});
#endif


			}
		}
	}

	public void FocusWindow(IntPtr targetWindow)
	{
		//todo Разобраться изменении окна при фокусе
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
		//GetWindowRect(_window, out RECT rt);
		//SetWindowLong(targetWindow, ++indexOrder, /*(uint)WindowStyles.WS_VISIBLE |*/ (uint)WindowStyles.WS_POPUP | (uint)WindowStyles.WS_MINIMIZE | (uint)WindowStyles.WS_OVERLAPPED);
		SetFocus(targetWindow);
		FlashWindow(targetWindow, true);
		SwitchToThisWindow(targetWindow, true);
		SetActiveWindow(targetWindow);

		//if (AppManager.Instance.IsLobbyTable)
		//	SetLobbySize();
		//	else
		//{
		//	SetWindowPos(_window, (IntPtr)SpecialWindowHandles.HWND_TOP,
		//	rt.Left, rt.Top, rt.Right - rt.Left, rt.Bottom - rt.Top,
		//	SetWindowPosFlags.SWP_SHOWWINDOW);
		//}

#endif
	}

	public override void FocusWindow()
	{
		FocusWindow(_window);
	}

	public override void MinimizeApplication()
	{
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
		ShowWindow(_window, 2);
#endif
	}

	protected override void UpdateAny()
	{
		base.UpdateAny();

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR


		if (_isDragPointer)
		{
			GetCursorPos(out POINT nPoint);
			Vector2 delta = new Vector2(nPoint.X - mPos.X, nPoint.Y - mPos.Y);

			if (_isFull && delta.sqrMagnitude > 0)
				ExpandApplication(true);

			int xSize = _rect.Right - _rect.Left;
			int ySize = _rect.Bottom - _rect.Top;
			SetWindowPos(_window, (IntPtr)SpecialWindowHandles.HWND_TOP, _rect.Left + (int)delta.x, _rect.Top + (int)delta.y, xSize, ySize, SetWindowPosFlags.SWP_SHOWWINDOW);
		}
		if (_isRezisePointer)
		{
			GetCursorPos(out POINT nPoint);
			Vector2 delta = new Vector2(nPoint.X - mPos.X, nPoint.Y - mPos.Y);

			if (_isFull && delta.sqrMagnitude > 0)
				ExpandApplication(true);

			int xSize = _rect.Right - _rect.Left;
			int ySize = _rect.Bottom - _rect.Top;

			int h = ySize + (int)delta.y;

			if (h < _windowSize.y)
				h = _windowSize.y;

			SetWindowPos(_window, (IntPtr)SpecialWindowHandles.HWND_TOP, _rect.Left, _rect.Top , xSize, h, SetWindowPosFlags.SWP_SHOWWINDOW);
		}
#endif
	}

	public override void WindowDragStart()
	{
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
		if (_isFull) return;
		if (!GetWindowRect(_window, out RECT rct))
		{
			return;
		}
		_rect = rct;
		_isDragPointer = true;
		GetCursorPos(out POINT mPoint);
		this.mPos = mPoint;
#endif
	}


	public override void WindowDragStop()
	{
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
		_isDragPointer = false;
		
#endif
	}
	public override void WindowResizeStart()
	{
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
		if (_isFull) return;
		if (!GetWindowRect(_window, out RECT rct))
		{
			return;
		}
		_rect = rct;
		_isRezisePointer = true;
		GetCursorPos(out POINT mPoint);
		this.mPos = mPoint;
#endif
	}

	public override void WindowResizeEnd()
	{

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
		_isRezisePointer = false;
#endif
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct RECT
	{
		public int Left;        // x position of upper-left corner
		public int Top;         // y position of upper-left corner
		public int Right;       // x position of lower-right corner
		public int Bottom;      // y position of lower-right corner
	}
	public struct POINT
	{
		public int X;
		public int Y;

		public POINT(int x, int y)
		{
			X = x;
			Y = y;
		}
	}

#if UNITY_STANDALONE_WIN
	[DllImport("user32.dll", EntryPoint = "FindWindow")]
	private static extern IntPtr FindWindow(string ClassName, string WindowName);
	[DllImport("user32.dll")]
	public static extern IntPtr GetActiveWindow();

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	static extern bool SetRect(out RECT lprc, int xLeft, int yTop, int xRight, int yBottom);

	[DllImport("user32.dll", SetLastError = true)]
	static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

	[DllImport("user32.dll")]
	static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern IntPtr SetActiveWindow(IntPtr hWnd);

	[DllImport("user32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	static extern bool GetCursorPos(out POINT lpPoint);

	[DllImport("Shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);

	[DllImport("user32.dll", EntryPoint = "SwitchToThisWindow")]
	private static extern bool SwitchToThisWindow(IntPtr hwnd, bool fUnknown);

	public static IntPtr SetWindowLongPtr(HandleRef hWnd, int nIndex, IntPtr dwNewLong)
	{
		if (IntPtr.Size == 8)
			return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
		else
			return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
	}

	[DllImport("user32.dll", EntryPoint = "SetWindowLong")]
	private static extern int SetWindowLong32(HandleRef hWnd, int nIndex, int dwNewLong);

	[DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
	private static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, IntPtr dwNewLong);

	[DllImport("user32.dll")]
	static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

	[DllImport("user32.dll", EntryPoint = "GetWindowLong")]
	static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

	[DllImport("user32.dll", SetLastError = true)]
	static extern IntPtr SetFocus(IntPtr hWnd);

	[DllImport("user32.dll")]
	static extern bool FlashWindow(IntPtr hwnd, bool bInvert);

	[DllImport("user32.dll")]
	static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
	[DllImport("user32.dll", EntryPoint = "DestroyWindow")]
	private static extern bool DestroyWindow(IntPtr hwnd);
	[DllImport("user32.dll", EntryPoint = "GetTopWindow")]
	public static extern IntPtr GetTopWindow();
	[DllImport("user32.dll", EntryPoint = "SetWindowText")]
	public static extern bool SetWindowText(System.IntPtr hwnd, System.String lpString);

	[return: MarshalAs(UnmanagedType.Bool)]
	[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
	static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
	[StructLayout(LayoutKind.Sequential)]
	struct SECURITY_ATTRIBUTES
	{
		public int nLength;
		public IntPtr lpSecurityDescriptor;
	}
	[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
	static extern bool CreateProcess(
	 string lpApplicationName,
	string lpCommandLine,
	 ref SECURITY_ATTRIBUTES lpProcessAttributes,
	 ref SECURITY_ATTRIBUTES lpThreadAttributes,
	 bool bInheritHandles,
	uint dwCreationFlags,
	 IntPtr lpEnvironment,
	 string lpCurrentDirectory,
	 [In] ref STARTUPINFO lpStartupInfo,
	 out PROCESS_INFORMATION lpProcessInformation);

	[StructLayout(LayoutKind.Sequential)]
	struct PROCESS_INFORMATION
	{
		public IntPtr hProcess;
		public IntPtr hThread;
		public int dwProcessId;
		public int dwThreadId;
	}

	[StructLayout(LayoutKind.Sequential)]
	struct STARTUPINFO
	{
		public uint cb;
		public IntPtr lpReserved;
		public IntPtr lpDesktop;
		public IntPtr lpTitle;
		public uint dwX;
		public uint dwY;
		public uint dwXSize;
		public uint dwYSize;
		public uint dwXCountChars;
		public uint dwYCountChars;
		public uint dwFillAttributes;
		public uint dwFlags;
		public ushort wShowWindow;
		public ushort cbReserved;
		public IntPtr lpReserved2;
		public IntPtr hStdInput;
		public IntPtr hStdOutput;
		public IntPtr hStdErr;
	}
	[DllImport("user32.dll", EntryPoint = "CreateWindowStation", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern IntPtr CreateWindowStation(
										[MarshalAs(UnmanagedType.LPWStr)] string name,
										[MarshalAs(UnmanagedType.U4)] int reserved,      // must be zero.
										[MarshalAs(UnmanagedType.U4)] WINDOWS_STATION_ACCESS_MASK desiredAccess,
										[MarshalAs(UnmanagedType.LPStruct)] SecurityAttributes attributes);


	[Flags]
	public enum WINDOWS_STATION_ACCESS_MASK : uint
	{
		WINSTA_NONE = 0,

		WINSTA_ENUMDESKTOPS = 0x0001,
		WINSTA_READATTRIBUTES = 0x0002,
		WINSTA_ACCESSCLIPBOARD = 0x0004,
		WINSTA_CREATEDESKTOP = 0x0008,
		WINSTA_WRITEATTRIBUTES = 0x0010,
		WINSTA_ACCESSGLOBALATOMS = 0x0020,
		WINSTA_EXITWINDOWS = 0x0040,
		WINSTA_ENUMERATE = 0x0100,
		WINSTA_READSCREEN = 0x0200,

		WINSTA_ALL_ACCESS = (WINSTA_ENUMDESKTOPS | WINSTA_READATTRIBUTES | WINSTA_ACCESSCLIPBOARD |
										WINSTA_CREATEDESKTOP | WINSTA_WRITEATTRIBUTES | WINSTA_ACCESSGLOBALATOMS |
										WINSTA_EXITWINDOWS | WINSTA_ENUMERATE | WINSTA_READSCREEN)
	}
	public class SecurityAttributes
	{
	#region Struct members
		[MarshalAs(UnmanagedType.U4)]
		private int mStuctLength;

		private IntPtr mSecurityDescriptor;

		[MarshalAs(UnmanagedType.U4)]
		private bool mInheritHandle;
	#endregion
		public SecurityAttributes()
		{
			mStuctLength = Marshal.SizeOf(typeof(SecurityAttributes));
			mSecurityDescriptor = IntPtr.Zero;
		}

		public IntPtr SecurityDescriptor
		{
			get { return mSecurityDescriptor; }
			set { mSecurityDescriptor = value; }
		}

		public bool Inherit
		{
			get { return mInheritHandle; }
			set { mInheritHandle = value; }
		}
	}

#endif

}