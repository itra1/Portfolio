using System.Runtime.InteropServices;
using System.Text;

namespace OfficeControl.Helpers
{
	public static class WindowApi
	{

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

		public struct SECURITY_ATTRIBUTES
		{
			public int nLength;
			public IntPtr lpSecurityDescriptor;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct STARTUPINFO
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

		[StructLayout(LayoutKind.Sequential)]
		public struct PROCESS_INFORMATION
		{
			public IntPtr hProcess;
			public IntPtr hThread;
			public int dwProcessId;
			public int dwThreadId;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left;        // x position of upper-left corner
			public int Top;         // y position of upper-left corner
			public int Right;       // x position of lower-right corner
			public int Bottom;      // y position of lower-right corner
		}
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
		public enum SetWindowPosFlags :uint
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
		public enum WMessages :int
		{
			WM_LBUTTONDOWN = 0x201, //Left mousebutton down
			WM_LBUTTONUP = 0x202,   //Left mousebutton up
			WM_LBUTTONDBLCLK = 0x203, //Left mousebutton doubleclick
			WM_RBUTTONDOWN = 0x204, //Right mousebutton down
			WM_RBUTTONUP = 0x205,   //Right mousebutton up
			WM_RBUTTONDBLCLK = 0x206, //Right mousebutton do
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

		[DllImport("kernel32.dll",SetLastError = true,CharSet = CharSet.Auto)]
		public static extern bool CreateProcess(
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

		[DllImport("gdi32.dll",EntryPoint = "CreateCompatibleDC",SetLastError = true)]
		public static extern IntPtr CreateCompatibleDC([In] IntPtr hdc);

		[DllImport("gdi32.dll",EntryPoint = "CreateCompatibleDC",SetLastError = true)]
		public static extern int ReleaseDC([In] IntPtr hWnd,[In] IntPtr hdc);

		[DllImport("gdi32.dll",EntryPoint = "BitBlt",SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BitBlt([In] IntPtr hdc,int nXDest,int nYDest,int nWidth,int nHeight,[In] IntPtr hdcSrc,int nXSrc,int nYSrc,TernaryRasterOperations dwRop);

		[DllImport("gdi32.dll",EntryPoint = "CreateCompatibleBitmap")]
		public static extern IntPtr CreateCompatibleBitmap([In] IntPtr hdc,int nWidth,int nHeight);

		[DllImport("gdi32.dll",EntryPoint = "SelectObject")]
		public static extern IntPtr SelectObject([In] IntPtr hdc,[In] IntPtr hgdiobj);

		[DllImport("user32.dll")]
		public static extern IntPtr GetDC(IntPtr hwnd);

		[DllImport("user32.dll",SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool PrintWindow(IntPtr hwnd,IntPtr hDC,uint nFlags);

		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);

		[DllImport("user32.dll")]
		public static extern bool GetWindowRect(IntPtr hWnd,out RECT lpRect);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetRect(out RECT lprc,int xLeft,int yTop,int xRight,int yBottom);

		[DllImport("user32.dll",EntryPoint = "FindWindow")]
		public static extern IntPtr FindWindow(string ClassName,string WindowName);

		[DllImport("user32.dll",EntryPoint = "FindWindow")]
		public static extern IntPtr FindWindowA(string ClassName,string WindowName);

		[DllImport("kernel32.dll",SetLastError = true,CharSet = CharSet.Auto)]
		public static extern bool TerminateProcess([In] IntPtr hProcess,[In] int uExitCode);

		[DllImport("user32.dll")]
		public static extern IntPtr GetActiveWindow();

		[DllImport("user32.dll",SetLastError = true)]
		public static extern IntPtr SetActiveWindow(IntPtr hWnd);

		[DllImport("user32.dll",SetLastError = true)]
		public static extern IntPtr SetFocus(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern int SetWindowLong(IntPtr hWnd,int nIndex,uint dwNewLong);

		[DllImport("user32.dll")]
		public static extern void SetWindowState(IntPtr hWnd,int state);

		[DllImport("user32.dll")]
		public static extern bool ShowWindow(IntPtr hWnd,int nCmdShow);

		[DllImport("user32.dll",EntryPoint = "SwitchToThisWindow")]
		public static extern bool SwitchToThisWindow(IntPtr hwnd,bool fUnknown);

		[DllImport("user32.dll",SetLastError = true)]
		public static extern bool SetWindowPos(IntPtr hWnd,IntPtr hWndInsertAfter,int X,int Y,int cx,int cy,SetWindowPosFlags uFlags);

		[DllImport("user32.dll")]
		public static extern IntPtr WindowFromPoint(POINT pt);
		[DllImport("user32.dll")]
		public static extern IntPtr ChildWindowFromPoint(IntPtr hWndParent,POINT Point);

		[DllImport("user32",SetLastError = true,CharSet = CharSet.Auto)]
		public static extern int GetClassName(IntPtr hWnd,StringBuilder lpClassName,int nMaxCount);

		[DllImport("user32",CharSet = CharSet.Auto,SetLastError = true)]
		public static extern int GetWindowText(IntPtr hWnd,StringBuilder lpString,int nMaxCount);
		[DllImport("user32.dll",SetLastError = true)]
		public static extern IntPtr FindWindowEx(IntPtr parentHandle,IntPtr hWndChildAfter,string className,string windowTitle);
		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll")]
		public static extern bool PostMessage(IntPtr WindowHandle,int Msg,nint wParam,nint lParam);

		public static nint MAKELPARAM(int p,int p_2)
		{
			return ((p_2 << 16) | (p & 0xFFFF));
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int X;
			public int Y;

			public POINT(int x,int y)
			{
				this.X = x;
				this.Y = y;
			}

			public static implicit operator System.Drawing.Point(POINT p)
			{
				return new System.Drawing.Point(p.X,p.Y);
			}

			public static implicit operator POINT(System.Drawing.Point p)
			{
				return new POINT(p.X,p.Y);
			}

			public override string ToString()
			{
				return $"X: {X}, Y: {Y}";
			}
		}


	}
}
