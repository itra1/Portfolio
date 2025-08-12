using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WalletType
{
	dollar,
	euro,
	bb
}

public enum WindowType
{
	Menu,
	Game
}
public enum PokerStatisticDateInticator
{
	Minute,
	Time,
	Day,
	Date,
	DateTime,
	Month
}
[System.Flags]
public enum DiagramDateVisible
{
	None = 0,
	ChetOffset = 1,
	ChetClear = 2
}

public enum MainPagesType
{
	Home = 0,
	Lobby = 1,
	Settings = 2,
	UserProfile = 3
}
public enum TablePlaceTypes
{
	Round = 0,
	Octo = 1
}

[System.Flags]
public enum NabigationsUse
{
	None = 0,
	SmartHud = 1,
	Statistic = 2,
	All = SmartHud | Statistic
}

public enum PromotionInfoCategory : uint
{
	None = 0,
	Poker_Hands = 1,
	Bet_Race = 2,
	WT_Race = 3,
	AoN_Race = 4,
	Game_Manager = 5
}

public enum PopupType
{
	None = 0,
	Authorization = 1,
	Welcome = 2,
	ForgotPassword = 3,
	Registration = 4,
	RegistrationSuccess = 5,
	Network = 6,
	Develop = 7,
	ObservingList = 8,
	ExitGame = 9,
	SmartHudOpponenty = 10,
	ColorLabels = 11,
	HandHistory = 12,
	BuyIn = 13,
	TableInfo = 14,
	AddChip = 15,
	Afk = 16,
	InsufficientBalance = 17,
	SitTimeOut = 18,
	WelcomeBonus = 19,
	Cashier = 20,
	FoldActionTable = 21,
	CreateTable = 22,
	MyAvatars = 23,
	Leaderboard = 24,
	DealerChoice = 25, // ���� ������ ���� � ������ Dealer Choise
	//Browser = 26,
	InactiveUser = 27,
	UnstableConnectionLost = 28,
	TableLimit = 29,
	Exit = 30,
	LogOut = 31,
	PasswordRecovery = 32,
	Confirm = 33,
	PasswordChange = 34,
	Info = 35,
	SmartHudPlayer = 36,
	SmartHudLightOpponenty = 37,
	AfkActivate = 38,
	TablePin = 39,
	ChipInNextHand = 40,
	TimeBankInfo = 41,
	WelcomeMobile = 42,
	ButtonAnte = 43,
	RegistrationMobile = 44,
	DisconnectExtraTime = 45,
	PhoneEdit = 46,
	NicknameEdit = 47,
	QR = 48,
	Update = 49,
	UpdateTimer = 50
}

public enum SinglePagesType
{
	None = 0,
	Events = 1,
	Rank = 2,
	Timebank = 3,
	MyAvatar = 4,
	PokerStatistic = 5,
	MyTables = 6,
	Jackpot = 7,
	Leaderboard = 8,
	WelcomeBonus = 9,
	Settings = 10,
	SmartHud = 11,
	Promotions = 12,
	BadBeat = 13
}

/// <summary>
/// �������� �����
/// </summary>
public enum TableAnimationsType
{
	CardsToPlayer = 0,
	CardToTable = 1,
	PlayerBetsOnTable = 2,
	CardToFold = 3,
	CardFromTableToOut = 4,
	CardPlayerToOut = 5,
	BankToPlayers = 6,
	PlayerChipToBank = 7
}

public enum CashierType
{
	None = 0,
	BlackRabbit = 1
}

public enum TableStyle
{
	Round,
	RoundLobby,
	Octo,
	OctoGold,
	OctoLobby,
	OctoLobbyAlt,
	OctoLobbyGold
}

[System.Flags]
public enum AppGameRules
{
	None = 0,
	Holdem = 1,
	Holdem6 = 2,
	OmahaHigh4 = 4,
	OmahaHigh5 = 8,
	OmahaLow4 = 16,
	OmahaLow5 = 32,
	China = 64,
	OmahaHigh6 = 128,
	OmahaHigh7 = 256,
	Holdem3 = 512,
	Montana = 1024,
	PLO_4 = OmahaHigh4 | OmahaLow4,
	PLO_5 = OmahaHigh5 | OmahaLow5,
	PLO_6 = OmahaHigh6,
	PLO_7 = OmahaHigh7,
	NLH = Holdem,
	ShortDeck = Holdem6,
	Memphis = Holdem3,
	PLO = PLO_4 | PLO_5 | PLO_6 | PLO_7,
	AON = Holdem | OmahaHigh4,
	OFC = China,
	FaceToFace = Holdem | Holdem6 | OmahaHigh4 | OmahaHigh5,
}

[System.Flags]
public enum SearchLobbyType
{
	None = 0,
	FreeTables = 1,
	Micro = 2,
	Average = 4,
	High = 8
}
public enum GameRuleType
{
	None,
	Holdem = 1,
	Holdem6 = 2,
	OmahaHigh4 = 3,
	OmahaHigh5 = 4,
	OmahaLow4 = 5,
	OmahaLow5 = 6,
	China = 7,
	OmahaHigh6 = 8,
	OmahaHigh7 = 9,
	Holdem3 = 10,
	Montana = 11,
	DealerChoice = 1000
}

public enum GameType
{
	None,
	Holdem = 1,
	Holdem6 = 2,          // Short desk 
	OmahaHigh4 = 3,
	OmahaHigh5 = 4,
	OmahaLow4 = 5,
	OmahaLow5 = 6,
	China = 7,
	OmahaHigh6 = 8,
	OmahaHigh7 = 9,
	Holdem3 = 10,         // Memfis
	Montana = 11,
	DealerChoice = 1000
}

public enum PokerGameType
{
	Holdem,
	PLO_4_hi,
	PLO_4_hi_lo,
	PLO_5_hi,
	PLO_5_hi_lo,
	ShortDesk,
	OFC,
	PLO_6_hi,
	PLO_6_hi_lo,
	PLO_7_hi,
	PLO_7_hi_lo,
}
public enum StringPositionType
{
	Prefix = 0,
	Postfix = 1
}

[System.Serializable]
public enum LobbyType
{
	None = 0,
	Holdem = 1,
	Plo = 2,
	ShortDesk = 3,
	AllOrNothing = 4,
	Ofc = 5,
	FaceToFace = 6,
	VipGame = 7,
	Mtt = 8,
	Montana = 9,
	DealerChoice = 10,
	Mempfis
}

public enum PopupMobile
{

}



public enum RankType
{
	knight = 1
, baron = 2
, viscount = 3
, earl = 4
, marquise = 5
, duke = 6
, king = 7
, god = 8
}

public enum GamePhase
{
	None = 0,
	Wait = 1,
	Game = 2,
	Finish = 3
}

/// <summary>
///     Special window handles
/// </summary>
public enum SpecialWindowHandles
{
	// ReSharper disable InconsistentNaming
	/// <summary>
	///     Places the window at the top of the Z order.
	/// </summary>
	HWND_TOP = 0,
	/// <summary>
	///     Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows.
	/// </summary>
	HWND_BOTTOM = 1,
	/// <summary>
	///     Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.
	/// </summary>
	HWND_TOPMOST = -1,
	/// <summary>
	///     Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a non-topmost window.
	/// </summary>
	HWND_NOTOPMOST = -2
	// ReSharper restore InconsistentNaming
}

[Flags]
public enum SetWindowPosFlags : uint
{
	// ReSharper disable InconsistentNaming

	/// <summary>
	///     If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request.
	/// </summary>
	SWP_ASYNCWINDOWPOS = 0x4000,

	/// <summary>
	///     Prevents generation of the WM_SYNCPAINT message.
	/// </summary>
	SWP_DEFERERASE = 0x2000,

	/// <summary>
	///     Draws a frame (defined in the window's class description) around the window.
	/// </summary>
	SWP_DRAWFRAME = 0x0020,

	/// <summary>
	///     Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
	/// </summary>
	SWP_FRAMECHANGED = 0x0020,

	/// <summary>
	///     Hides the window.
	/// </summary>
	SWP_HIDEWINDOW = 0x0080,

	/// <summary>
	///     Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
	/// </summary>
	SWP_NOACTIVATE = 0x0010,

	/// <summary>
	///     Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.
	/// </summary>
	SWP_NOCOPYBITS = 0x0100,

	/// <summary>
	///     Retains the current position (ignores X and Y parameters).
	/// </summary>
	SWP_NOMOVE = 0x0002,

	/// <summary>
	///     Does not change the owner window's position in the Z order.
	/// </summary>
	SWP_NOOWNERZORDER = 0x0200,

	/// <summary>
	///     Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
	/// </summary>
	SWP_NOREDRAW = 0x0008,

	/// <summary>
	///     Same as the SWP_NOOWNERZORDER flag.
	/// </summary>
	SWP_NOREPOSITION = 0x0200,

	/// <summary>
	///     Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
	/// </summary>
	SWP_NOSENDCHANGING = 0x0400,

	/// <summary>
	///     Retains the current size (ignores the cx and cy parameters).
	/// </summary>
	SWP_NOSIZE = 0x0001,

	/// <summary>
	///     Retains the current Z order (ignores the hWndInsertAfter parameter).
	/// </summary>
	SWP_NOZORDER = 0x0004,

	/// <summary>
	///     Displays the window.
	/// </summary>
	SWP_SHOWWINDOW = 0x0040,

	// ReSharper restore InconsistentNaming
}

[Flags]
enum WindowStyles : uint
{
	WS_OVERLAPPED = 0x00000000,
	WS_POPUP = 0x80000000,
	WS_CHILD = 0x40000000,
	WS_MINIMIZE = 0x20000000,
	WS_VISIBLE = 0x10000000,
	WS_DISABLED = 0x08000000,
	WS_CLIPSIBLINGS = 0x04000000,
	WS_CLIPCHILDREN = 0x02000000,
	WS_MAXIMIZE = 0x01000000,
	WS_BORDER = 0x00800000,
	WS_DLGFRAME = 0x00400000,
	WS_VSCROLL = 0x00200000,
	WS_HSCROLL = 0x00100000,
	WS_SYSMENU = 0x00080000,
	WS_THICKFRAME = 0x00040000,
	WS_GROUP = 0x00020000,
	WS_TABSTOP = 0x00010000,

	WS_MINIMIZEBOX = 0x00020000,
	WS_MAXIMIZEBOX = 0x00010000,

	WS_CAPTION = WS_BORDER | WS_DLGFRAME,
	WS_TILED = WS_OVERLAPPED,
	WS_ICONIC = WS_MINIMIZE,
	WS_SIZEBOX = WS_THICKFRAME,
	WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,

	WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
	WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
	WS_CHILDWINDOW = WS_CHILD,

	WS_EX_DLGMODALFRAME = 0x00000001,
	WS_EX_NOPARENTNOTIFY = 0x00000004,
	WS_EX_TOPMOST = 0x00000008,
	WS_EX_ACCEPTFILES = 0x00000010,
	WS_EX_TRANSPARENT = 0x00000020,

	WS_EX_MDICHILD = 0x00000040,
	WS_EX_TOOLWINDOW = 0x00000080,
	WS_EX_WINDOWEDGE = 0x00000100,
	WS_EX_CLIENTEDGE = 0x00000200,
	WS_EX_CONTEXTHELP = 0x00000400,

	WS_EX_RIGHT = 0x00001000,
	WS_EX_LEFT = 0x00000000,
	WS_EX_RTLREADING = 0x00002000,
	WS_EX_LTRREADING = 0x00000000,
	WS_EX_LEFTSCROLLBAR = 0x00004000,
	WS_EX_RIGHTSCROLLBAR = 0x00000000,

	WS_EX_CONTROLPARENT = 0x00010000,
	WS_EX_STATICEDGE = 0x00020000,
	WS_EX_APPWINDOW = 0x00040000,

	WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE),
	WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST),

	WS_EX_LAYERED = 0x00080000,

	WS_EX_NOINHERITLAYOUT = 0x00100000, // Disable inheritence of mirroring by children
	WS_EX_LAYOUTRTL = 0x00400000, // Right to left mirroring

	WS_EX_COMPOSITED = 0x02000000,
	WS_EX_NOACTIVATE = 0x08000000

}
