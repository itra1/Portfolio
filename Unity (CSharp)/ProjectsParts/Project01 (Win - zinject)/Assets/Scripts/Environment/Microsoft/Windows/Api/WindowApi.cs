using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Environment.Microsoft.Windows.Api
{
	public partial class WindowApi
	{
		public static int MAKEWPARAMSCROLL(int direction, float deltaScroll, float multiplier, WParam button)
		{
			var delta = (int) (deltaScroll * multiplier);
			return (delta << 16) * direction | (ushort) button;
		}

		public static int MAKELPARAM(int p, int p_2) => (p_2 << 16) | (p & 0xFFFF);

		public static int MAKELKEYPARAM(int scanCode) => (0x00000001 | (scanCode << 16));

		public static IntPtr MAKELKEYDOWNPARAM(uint scanCode) => (IntPtr) (0x00000001 | (scanCode << 16));
		public static IntPtr MAKELKEYUPPARAM(uint scanCode) => (IntPtr) (0xC0000001 | (scanCode << 16));

		public static IntPtr IntPtrAlloc<T>(T param)
		{
			var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(param));
			Marshal.StructureToPtr(param, ptr, false);
			return ptr;
		}

		public static IntPtr GetWindow(string className = null, string windowName = null)
		{
			if (className == null && windowName == null)
			{
				return IntPtr.Zero;
			}

#if UNITY_STANDALONE_WIN
			return FindWindowA(className, windowName);
#else
			return IntPtr.Zero;
#endif

		}

		public static IntPtr GetWindowDC(IntPtr windowHWDN)
		{
			if (windowHWDN == IntPtr.Zero)
			{
				return IntPtr.Zero;
			}

#if UNITY_STANDALONE_WIN
			return GetDC(windowHWDN);
#else
			return IntPtr.Zero;
#endif
		}

		public static int SetReleaseDC(IntPtr windowHWDN, IntPtr windowDC)
		{
			if (windowHWDN == IntPtr.Zero)
			{
				return 0;
			}

#if UNITY_STANDALONE_WIN
			return ReleaseDC(windowHWDN, windowDC);
#else
			return 0;
#endif
		}

		public static bool GetRectWindow(IntPtr windowHWDN, out RECT rect)
		{
			rect = new RECT();

			if (windowHWDN == IntPtr.Zero)
			{
				return false;
			}

#if UNITY_STANDALONE_WIN
			return GetWindowRect(windowHWDN, out rect);
#else
			return false;
#endif
		}

		public static IntPtr SetCreateCompatibleDC(IntPtr windowDC)
		{
			if (windowDC == IntPtr.Zero)
			{
				return IntPtr.Zero;
			}

#if UNITY_STANDALONE_WIN
			return CreateCompatibleDC(windowDC);
#else
			return IntPtr.Zero;
#endif
		}

		public static IntPtr SetCreateCompatibleBitmap(IntPtr windowDC, int width, int height)
		{
			if (windowDC == IntPtr.Zero)
				return IntPtr.Zero;

#if UNITY_STANDALONE_WIN
			return CreateCompatibleBitmap(windowDC, width, height);
#else
			return IntPtr.Zero;
#endif
		}

		[DllImport("kernel32.dll")]
		public static extern int GetCurrentThreadId();

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern bool CreateProcess(string lpApplicationName,
			string lpCommandLine,
			ref SECURITY_ATTRIBUTES lpProcessAttributes,
			ref SECURITY_ATTRIBUTES lpThreadAttributes,
			bool bInheritHandles,
			uint dwCreationFlags,
			IntPtr lpEnvironment,
			string lpCurrentDirectory,
			[In] ref STARTUPINFO lpStartupInfo,
			out PROCESS_INFORMATION lpProcessInformation);

		[DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC", SetLastError = true)]
		public static extern IntPtr CreateCompatibleDC([In] IntPtr hdc);

		[DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC", SetLastError = true)]
		public static extern int ReleaseDC([In] IntPtr hWnd, [In] IntPtr hdc);

		[DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BitBlt([In] IntPtr hdc,
			int nXDest,
			int nYDest,
			int nWidth,
			int nHeight,
			[In] IntPtr hdcSrc,
			int nXSrc,
			int nYSrc,
			TernaryRasterOperations dwRop);

		[DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
		public static extern IntPtr CreateCompatibleBitmap([In] IntPtr hdc, int nWidth, int nHeight);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool SetWindowPos(IntPtr hWnd,
			IntPtr hWndInsertAfter,
			int X,
			int Y,
			int cx,
			int cy,
			SetWindowPosFlags uFlags);

		[DllImport("gdi32.dll", EntryPoint = "SelectObject")]
		public static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);

		[DllImport("user32.dll")]
		public static extern IntPtr GetDC(IntPtr hwnd);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);

		[DllImport("user32.dll")]
		public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetRect(out RECT lprc, int xLeft, int yTop, int xRight, int yBottom);

		[DllImport("user32.dll", EntryPoint = "FindWindow")]
		public static extern IntPtr FindWindow(string ClassName, string WindowName);

		[DllImport("user32.dll", EntryPoint = "FindWindow")]
		public static extern IntPtr FindWindowA(string ClassName, string WindowName);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindowEx(IntPtr parentHandle,
			IntPtr hWndChildAfter,
			string className,
			string windowTitle);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr ChildWindowFromPointEx(IntPtr hwnd, POINT pt, uint flags);

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern bool TerminateProcess([In] IntPtr hProcess, [In] int uExitCode);

		[DllImport("user32.dll")]
		public static extern IntPtr GetActiveWindow();

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr SetActiveWindow(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr SetFocus(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

		[DllImport("user32.dll")]
		public static extern void SetWindowState(IntPtr hWnd, int state);

		[DllImport("user32.dll")]
		public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll", EntryPoint = "SwitchToThisWindow")]
		public static extern bool SwitchToThisWindow(IntPtr hwnd, bool fUnknown);

		[DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

		/// <summary>
		/// Gets snapshots of active processes
		/// </summary>
		/// <param name="dwFlags"></param>
		/// <param name="th32ProcessID"></param>
		/// <returns></returns>
		[DllImport("toolhelp.dll")]
		private static extern IntPtr CreateToolhelp32Snapshot(SnapshotFlags dwFlags, int th32ProcessID);

		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll")]
		public static extern bool PostMessage(IntPtr WindowHandle, uint Msg, IntPtr wParam, IntPtr lParam);
		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll")]
		public static extern bool PostMessage(IntPtr WindowHandle, uint Msg, uint wParam, IntPtr lParam);
		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll")]
		public static extern bool PostMessage(IntPtr WindowHandle, int Msg, int wParam, int lParam);
		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll")]
		public static extern bool PostMessage(IntPtr WindowHandle, int Msg, byte wParam, int lParam);
		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern bool PostMessage(IntPtr WindowHandle, uint Msg, int wParam, IntPtr lParam);
		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll")]
		public static extern bool PostMessage(IntPtr WindowHandle, uint Msg, IntPtr wParam, int lParam);

		[DllImport("user32.dll")]
		public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

		[DllImport("user32.dll")]
		public static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

		[DllImport("user32.dll")]
		internal static extern uint SendInput(uint nInputs,
			[MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs,
			int cbSize);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern uint MapVirtualKey(uint uCode, uint uMapType);
	}
}


