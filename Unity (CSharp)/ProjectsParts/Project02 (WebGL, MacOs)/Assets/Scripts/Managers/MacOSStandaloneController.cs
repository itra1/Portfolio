using System.Collections;
using it.Network.Rest;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Path = System.IO.Path;

public class MacOSStandaloneController : StandaloneController
{
    private Vector3 _pointerPosition;
    private bool _isDrag;

    protected override void AwakeInit()
    {
        base.AwakeInit();
        it.Logger.Log("Awake Init");
        
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
        SubscrbeCallbackOSX((mess) =>
        {
            it.Logger.Log("[MacOSPlugin] " + mess);
        });

        InitAppOSX();

        if (AppConfig.IsLobby)
            SetLobbySize();
        else
            SetGameSize();

        SetFocusThisWindowOSX();
		it.Logger.Log(Application.identifier);
        SetBundleIdentifierOSX(Application.identifier);
        SetTitleWindowOSX("Custom title");
#endif
        
	}

    public override void MinimizeApplication()
    {
        base.MinimizeApplication();

#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
        SeMinimizeWindowsOSX();
#endif
    }

    public override void _OpenNewTableWindow(Table table, string password = "", bool join = false)
    {
        base._OpenNewTableWindow(table, password, join);
            
        var path = Path.GetDirectoryName(Application.dataPath) + "/Contents/MacOS/" + Application.productName;

        string arguments = GetOpenAppArguments(table, password, join);

        Process.Start(path, arguments);
        
    }

    public override void WindowDragStart()
	{
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
        base.WindowDragStart();
        _isDrag = true;
        _pointerPosition = Input.mousePosition;
        OnDragStartWindowOSX();
#endif
	}

	public override void WindowDragStop()
    {
        base.WindowDragStop();
        _isDrag = false;
    }

    protected override void UpdateAny()
	{
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
        base.UpdateAny();

        if (_isDrag)
        {
            Vector2 delta = _pointerPosition - Input.mousePosition;
            if(delta.magnitude != 0)
            {
                OnDragWindowOSX(delta.x, delta.y);
            }
        }
#endif
	}

	public static void PrintHelloFromMac()
	{
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
        UnityEngine.Debug.Log("tetst print " + Marshal.PtrToStringAuto(TestData()));
        //HideTitleBar();
#endif
	}

	public override void SetLobbySize()
	{
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
        base.SetLobbySize();

        SetWindoeSize(LOBBY_SIZE.x, LOBBY_SIZE.y);
#endif
	}

	public override void SetGameSize()
	{
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
        base.SetGameSize();
        SetWindoeSize(GAME_SIZE.x, GAME_SIZE.y);
#endif
    }

    public override void FocusMain()
	{
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
        base.FocusMain();
        SetFocusThisWindowOSX();
#endif
    }

    public override void _SetNewWindowName(string newName)
    {
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
        base._SetNewWindowName(newName);
        appWindowName = newName;
        SetTitleWindowOSX(appWindowName);
        SaveWindowPlayerPrefs(appWindowName);
#endif
    }

    public override void _SetNewWindowName(Table table)
    {
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
        base._SetNewWindowName(table);
        string windowName = GetWindowTableName(table);
        PlayerPrefs.SetString(windowName, table.id.ToString());
        SetNewWindowName(windowName);
#endif
    }

    protected override bool _CloseAllTable()
    {
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
        return base._CloseAllTable();
#endif
        return true;
    }

#if UNITY_STANDALONE_OSX

    [DllImport("MacOSPlugin")]
    private static extern IntPtr TestData();

    [DllImport("MacOSPlugin")]
    private static extern IntPtr PrintHello2();

    [DllImport("MacOSPlugin")]
    private static extern void InitAppOSX();

    /// <summary>
    /// Установка заголовка осна
    /// </summary>
    /// <param name="title"></param>
    [DllImport("MacOSPlugin")]
    private static extern void SetTitleWindowOSX(string title);

    /// <summary>
    /// Установка размера окна
    /// </summary>
    /// <param name="sizeX">Ширина окна</param>
    /// <param name="sizeY">Высота окна</param>
    [DllImport("MacOSPlugin")]
    private static extern void SetWindoeSize(float sizeX, float sizeY);

    [DllImport("MacOSPlugin")]
    private static extern void OnDragStartWindowOSX();

    /// <summary>
    /// Перемещение окна
    /// </summary>
    /// <param name="deltaX"></param>
    /// <param name="deltaY"></param>
    [DllImport("MacOSPlugin")]
    private static extern void OnDragWindowOSX(float deltaX, float deltaY);

    /// <summary>
    /// Фокус окна
    /// </summary>
    [DllImport("MacOSPlugin")]
    private static extern void SetFocusThisWindowOSX();

    /// <summary>
    /// Фокус окна
    /// </summary>
    [DllImport("MacOSPlugin")]
    private static extern void SeMinimizeWindowsOSX();

    /// <summary>
    /// Установка идентификатора бандла
    /// </summary>
    [DllImport("MacOSPlugin")]
    private static extern void SetBundleIdentifierOSX(string bundleId);

    public delegate void UnityLogCallback(string message);

    [DllImport("MacOSPlugin")]
    private static extern void SubscrbeCallbackOSX(UnityLogCallback callback);
#endif
}