using System;

namespace WinApi
{
	/// <summary>
	/// ��������� ����������
	/// <see cref="https://learn.microsoft.com/en-us/windows/win32/menurc/keyboard-accelerator-notifications"/>
	/// </summary>
	public enum KeyboardAcceleratorNotifications :int
	{
		WM_SYSCOMMAND = 0x0112
	}

	public enum MapVkKeys :uint
	{
		MAPVK_VK_TO_VSC = 0,
		MAPVK_VSC_TO_VK = 1,
		MAPVK_VK_TO_CHAR = 2,
		MAPVK_VSC_TO_VK_EX = 3,
		MAPVK_VK_TO_VSC_EX = 4
	}

	public enum SysCommands :int
	{
		SC_CLOSE = 0xF060,        // Closes the window.
		SC_CONTEXTHELP = 0xF180,  // Changes the cursor to a question mark with a pointer. If the user then clicks a control in the dialog box, the control receives a WM_HELP message.
		SC_DEFAULT = 0xF160,      // Selects the default item; the user double-clicked the window menu.
		SC_HOTKEY = 0xF150,       // Activates the window associated with the application-specified hot key. The lParam parameter identifies the window to activate.
		SC_HSCROLL = 0xF080,      // Scrolls horizontally.
		SCF_ISSECURE = 0x00000001, // Indicates whether the screen saver is secure.
		SC_KEYMENU = 0xF100,      // Retrieves the window menu as a result of a keystroke. For more information, see the Remarks section.
		SC_MAXIMIZE = 0xF030,     // Maximizes the window.
		SC_MINIMIZE = 0xF020,     // Minimizes the window.
		SC_MONITORPOWER = 0xF170, // Sets the state of the display. This command supports devices that have power-saving features, such as a battery-powered personal computer.
															////		The lParam parameter can have the following values:
															//// -1 (the display is powering on)
															//// 1 (the display is going to low power)
															//// 2 (the display is being shut off)
		SC_MOUSEMENU = 0xF090,    // Retrieves the window menu as a result of a mouse click.
		SC_MOVE = 0xF010,         // Moves the window.
		SC_NEXTWINDOW = 0xF040,   // Moves to the next window.
		SC_PREVWINDOW = 0xF050,   // Moves to the previous window.
		SC_RESTORE = 0xF120,      // Restores the window to its normal position and size.
		SC_SCREENSAVE = 0xF140,   // Executes the screen saver application specified in the [boot] section of the System.ini file.
		SC_SIZE = 0xF000,         // Sizes the window.
		SC_TASKLIST = 0xF130,     // Activates the Start menu.
		SC_VSCROLL = 0xF070       // Scrolls vertically.
	}
	public enum WMessages :uint
	{
		WM_CAPTURECHANGED = 0x0215,
		WM_KEYDOWN = 0x0100,
		WM_KEYUP = 0x0101,
		WM_CHAR = 0x0102,
		WM_SYSKEYDOWN = 0x0104,
		WM_SYSKEYUP = 0x0105,
		WM_LBUTTONDOWN = 0x201,
		WM_LBUTTONUP = 0x202,
		WM_LBUTTONDBLCLK = 0x203,
		WM_RBUTTONDOWN = 0x204,
		WM_RBUTTONUP = 0x205,
		WM_RBUTTONDBLCLK = 0x206,
		WM_MOUSEWHEEL = 0x020A,
		WM_COPYDATA = 0x004A,
		WM_MBUTTONDOWN = 0x0207,
		WM_MBUTTONUP = 0x0208,
		WM_MBUTTONDBLCLK = 0x0209,
		WM_MOUSEMOVE = 0x0200,
		WM_MOUSEHOVER = 0x02A1,
		WM_MOUSELEAVE = 0x02A3,
		WM_NCMOUSEMOVE = 0x00A0,
		WM_MOUSEACTIVATE = 0x0021
	}
	public enum WParam :uint
	{
		MK_NONE = 0x0000,
		MK_LBUTTON = 0x0001, // ����� ������ ���� ����.
		MK_RBUTTON = 0x0002, // ������ ������ ���� ����.
		MK_SHIFT = 0x0004, // ������� SHIFT ����.
		MK_CONTROL = 0x0008, // ������� CTRL ����.
		MK_�BUTTON = 0x0010, // ������� ������ ���� ����.
	}
	public enum WVisibleType :int
	{
		CWP_ALL = 0x0000,
		CWP_SKIPDISABLED = 0x0002,
		CWP_SKIPINVISIBLE = 0x0001,
		CWP_SKIPTRANSPARENT = 0x0004
	}
	public enum TernaryRasterOperations :uint
	{
		SRCCOPY = 0x00CC0020,
		SRCPAINT = 0x00EE0086,
		SRCAND = 0x008800C6,
		SRCINVERT = 0x00660046,
		SRCERASE = 0x00440328,
		NOTSRCCOPY = 0x00330008,
		NOTSRCERASE = 0x001100A6,
		MERGECOPY = 0x00C000CA,
		MERGEPAINT = 0x00BB0226,
		PATCOPY = 0x00F00021,
		PATPAINT = 0x00FB0A09,
		PATINVERT = 0x005A0049,
		DSTINVERT = 0x00550009,
		BLACKNESS = 0x00000042,
		WHITENESS = 0x00FF0062,
		CAPTUREBLT = 0x40000000
	}
	public enum SpecialWindowHandles
	{
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
	}
	[Flags]
	public enum SetWindowPosFlags :uint
	{
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
		SWP_SHOWWINDOW = 0x0040
	}
	[Flags]
	public enum WindowStyles :uint
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
	[Flags]
	public enum SnapshotFlags :uint
	{
		HeapList = 0x00000001,
		Process = 0x00000002,
		Thread = 0x00000004,
		Module = 0x00000008,
		Module32 = 0x00000010,
		Inherit = 0x80000000,
		All = 0x0000001F,
		NoHeaps = 0x40000000
	}

	/// <summary>
	/// ��������� ������� ��������
	/// <see cref="https://learn.microsoft.com/ru-ru/windows/win32/api/processthreadsapi/nf-processthreadsapi-setpriorityclass"/>
	/// </summary>
	public enum ProcessPreority :uint
	{
		ABOVE_NORMAL_PRIORITY_CLASS = 0x00008000,
		BELOW_NORMAL_PRIORITY_CLASS = 0x00004000,
		HIGH_PRIORITY_CLASS = 0x00000080,
		IDLE_PRIORITY_CLASS = 0x00000040,
		NORMAL_PRIORITY_CLASS = 0x00000020,
		PROCESS_MODE_BACKGROUND_BEGIN = 0x00100000,
		PROCESS_MODE_BACKGROUND_END = 0x00200000,
		REALTIME_PRIORITY_CLASS = 0x00000100
	}

}