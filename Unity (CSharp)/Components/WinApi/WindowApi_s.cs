using System;
using System.Runtime.InteropServices;

namespace WinApi
{
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

#pragma warning disable 649
	internal struct INPUT
	{
		public UInt32 Type;
		public MOUSEKEYBDHARDWAREINPUT Data;
	}

	[StructLayout(LayoutKind.Explicit)]
	internal struct MOUSEKEYBDHARDWAREINPUT
	{
		[FieldOffset(0)]
		public MOUSEINPUT Mouse;
	}

	internal struct MOUSEINPUT
	{
		public Int32 X;
		public Int32 Y;
		public UInt32 MouseData;
		public UInt32 Flags;
		public UInt32 Time;
		public IntPtr ExtraInfo;
	}
	public struct COPYDATASTRUCT
	{
		public uint dwData;
		public int cbData;
		public IntPtr lpData;
	}

#pragma warning restore 649

	[StructLayout(LayoutKind.Sequential)]
	public struct POINT
	{
		public int X, Y;

		public POINT(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		public static implicit operator System.Drawing.Point(POINT p) => new(p.X, p.Y);
		public static implicit operator POINT(System.Drawing.Point p) => new(p.X, p.Y);
		public override readonly string ToString() => $"X: {X}, Y: {Y}";
	}
}
