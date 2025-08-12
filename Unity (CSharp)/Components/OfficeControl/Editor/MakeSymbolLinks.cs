using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using WinApi;

namespace OfficeControl.Editor
{
	[InitializeOnLoad]

	public class MakeSymbolLinks
	{
		static MakeSymbolLinks()
		{
			MakeOfficeAppRef();
		}

		[MenuItem("Components/OfficeApp/MakeShareSymbolLink")]
		public static void MakeOfficeAppRef()
		{
			var path = Application.dataPath + @"/Components/OfficeControl/Share";

			if (Directory.GetFiles(path).Length + Directory.GetDirectories(path).Length > 0)
			{
				return;
			}

			if (Directory.Exists(path))
			{
				Debug.Log("remove");
				Debug.Log(path + ".meta");
				Directory.Delete(path);
				File.Delete(path + ".meta");
			}

			var CommandLine = $" mklink /D {Application.dataPath}/Components/OfficeControl/Share {Application.dataPath}/../SubProject/OfficeApp/OfficeControl/Share/";

			_ = new
			PROCESS_INFORMATION();
			STARTUPINFO sInfo = new() { dwXSize = 1, dwYSize = 1 };
			SECURITY_ATTRIBUTES pSec = new();
			SECURITY_ATTRIBUTES tSec = new();
			pSec.nLength = Marshal.SizeOf(pSec);
			tSec.nLength = Marshal.SizeOf(tSec);
			_ = WindowApi.CreateProcess("cmd.exe", CommandLine,
				ref pSec, ref tSec, false, (uint)ProcessPreority.NORMAL_PRIORITY_CLASS,
				IntPtr.Zero, null, ref sInfo, out _);
		}
	}
}