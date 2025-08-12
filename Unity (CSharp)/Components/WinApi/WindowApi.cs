using System;
using System.Runtime.InteropServices;
using System.Text;

namespace WinApi
{
	public partial class WindowApi
	{
		public static int MAKEWPARAMSCROLL(int direction, float deltaScroll, float multiplier, WParam button)
		{
			int delta = (int)(deltaScroll * multiplier);
			return (((delta << 16) * direction | (ushort)button));
		}
		/// <summary>
		/// Генерация lParam c координатами
		/// </summary>
		/// <param name="x">Координата на X</param>
		/// <param name="y">Координата на Y</param>
		/// <returns></returns>
		public static IntPtr MAKELPARAM(int x, int y) => (IntPtr)((y << 16) | (x & 0xFFFF));
		public static IntPtr MAKELKEYDOWNPARAM(uint scanCode) => (IntPtr)(0x00000001 | (scanCode << 16));
		public static IntPtr MAKELKEYUPPARAM(uint scanCode) => (IntPtr)(0xC0000001 | (scanCode << 16));

		public static IntPtr IntPtrAlloc<T>(T param)
		{
			IntPtr retval = System.Runtime.InteropServices.Marshal.AllocHGlobal(System.Runtime.InteropServices.Marshal.SizeOf(param));
			System.Runtime.InteropServices.Marshal.StructureToPtr(param, retval, false);
			return (retval);
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

		/// <summary>
		/// Отправка сообщения провессу
		/// </summary>
		/// <param name="WindowHandle"></param>
		/// <param name="message">Сообщения (максимум 12 символов)</param>
		/// <returns></returns>
		public static int SendMessage(IntPtr WindowHandle, string message)
		{
			//TODO Разобраться почему только 12 символов, должно быть больше
			if (message.Length > 12)
				throw new System.ApplicationException("Длинна сообщения не должна превышать 12 символов");

			IntPtr lpPtr = Marshal.StringToHGlobalAnsi(message);
			var result = SendMessage(WindowHandle, 0x004A, 0, lpPtr);
			//Marshal.FreeHGlobal(lpPtr);
			return result;
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
			{
				return IntPtr.Zero;
			}
#if UNITY_STANDALONE_WIN
			return CreateCompatibleBitmap(windowDC, width, height);
#else
			return IntPtr.Zero;
#endif
		}

		/// <summary>
		/// Запуск процесса
		/// </summary>
		/// <param name="lpApplicationName">Имя процесса</param>
		/// <param name="lpCommandLine">Аргументы коммандной строки</param>
		/// <param name="lpProcessAttributes"></param>
		/// <param name="lpThreadAttributes"></param>
		/// <param name="bInheritHandles"></param>
		/// <param name="dwCreationFlags"></param>
		/// <param name="lpEnvironment"></param>
		/// <param name="lpCurrentDirectory"></param>
		/// <param name="lpStartupInfo"></param>
		/// <param name="lpProcessInformation"></param>
		/// <see cref="https://learn.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-createprocessa"/>
		/// <returns></returns>
		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
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

		[DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC", SetLastError = true)]
		public static extern IntPtr CreateCompatibleDC([In] IntPtr hdc);

		[DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC", SetLastError = true)]
		public static extern int ReleaseDC([In] IntPtr hWnd, [In] IntPtr hdc);

		[DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

		[DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
		public static extern IntPtr CreateCompatibleBitmap([In] IntPtr hdc, int nWidth, int nHeight);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

		[DllImport("gdi32.dll", EntryPoint = "SelectObject")]
		public static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);

		[DllImport("user32.dll")]
		public static extern IntPtr GetDC(IntPtr hwnd);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
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
		public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr hWndChildAfter, string className, string windowTitle);
		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr ChildWindowFromPointEx(IntPtr hwnd, POINT pt, uint flags);

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern bool TerminateProcess([In] IntPtr hProcess, [In] int uExitCode);
		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		public static extern IntPtr GetActiveWindow();

		/// <summary>
		/// Извлекает идентификатор процесса вызывающего процесса.
		/// </summary>
		/// <see cref="https://learn.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-getcurrentprocessid"/>
		/// <returns></returns>
		[DllImport("kernel32.dll")]
		public static extern int GetCurrentProcessId();
		/// <summary>
		/// Извлекает идентификатор потока вызывающего потока.
		/// </summary>
		/// <see cref="https://learn.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-getcurrentthreadid"/>
		/// <returns></returns>
		[DllImport("kernel32.dll")]
		public static extern int GetCurrentThreadId();

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

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern uint MapVirtualKey(uint uCode, uint uMapType);
		/// <summary>
		/// Преобразует (сопоставляет) код виртуального ключа в код сканирования или символьное значение или преобразует код сканирования в код виртуального ключа.
		/// </summary>
		/// <see cref="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-mapvirtualkeya"/>
		/// <param name="uCode"></param>
		/// <param name="uMapType"></param>
		/// <returns></returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern uint MapVirtualKeyA(uint uCode, uint uMapType);
		/// <summary>
		/// Преобразует (сопоставляет) код виртуальной клавиши в код сканирования или символьное значение или преобразует код сканирования в код виртуальной клавиши.
		/// </summary>
		/// <see cref="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-mapvirtualkeyw"/>
		/// <param name="uCode"></param>
		/// <param name="uMapType"></param>
		/// <returns></returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern uint MapVirtualKeyW(uint uCode, uint uMapType);
		/// <summary>
		/// Преобразует (сопоставляет) код виртуальной клавиши в код сканирования или символьное значение или преобразует код сканирования в код виртуальной клавиши. Функция преобразует коды с использованием языка ввода и идентификатора локали ввода.
		/// </summary>
		/// <see cref="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-mapvirtualkeyexa"/>
		/// <param name="uCode"></param>
		/// <param name="uMapType"></param>
		/// <param name="dwhkl"></param>
		/// <returns></returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern uint MapVirtualKeyExA(uint uCode, uint uMapType);
		/// <summary>
		/// Преобразует (сопоставляет) код виртуальной клавиши в код сканирования или символьное значение или преобразует код сканирования в код виртуальной клавиши. Функция преобразует коды с использованием языка ввода и идентификатора локали ввода.
		/// </summary>
		/// <see cref="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-mapvirtualkeyexw"/>
		/// <param name="uCode"></param>
		/// <param name="uMapType"></param>
		/// <param name="dwhkl"></param>
		/// <returns></returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern uint MapVirtualKeyExW(uint uCode, uint uMapType);

		/// <summary>
		/// Получает снапшот активных процессов
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
		public static extern int SendMessage(IntPtr WindowHandle, uint Msg, int wParam, int lParam);
		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr WindowHandle, uint Msg, int wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

		[DllImport("user32.dll")]
		public static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

		[DllImport("user32.dll")]
		internal static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

		#region Keyboard

		#endregion
	}
}