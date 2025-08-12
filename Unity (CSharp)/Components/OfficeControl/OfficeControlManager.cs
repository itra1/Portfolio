using System;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using OfficeControl.Common;
using OfficeControl.Pipes.Base;
using UnityEngine;
using WinApi;

namespace OfficeControl
{
	public class OfficeControlManager :IDisposable
	{
		public static OfficeControlManager Instance { get; private set; }
		private string _serverName = "";
		private PackageProcessor _processor;
		private PROCESS_INFORMATION _pInfo;
		public string OfficeApp => Application.streamingAssetsPath + @"\OfficeApp\OfficeControl.exe";

		public static async UniTask<Package> Request(Package pack) => await Instance._processor.Send(pack);

		public OfficeControlManager()
		{
			Instance = this;
			_serverName = System.Guid.NewGuid().ToString();
			_processor = new(_serverName);
			CreateProcess();
		}

		~OfficeControlManager()
		{
			Dispose();
		}

		private void CreateProcess()
		{
			var CommandLine = $" {_serverName}";

			STARTUPINFO sInfo = new() { dwXSize = 1, dwYSize = 1 };
			SECURITY_ATTRIBUTES pSec = new();
			SECURITY_ATTRIBUTES tSec = new();
			pSec.nLength = Marshal.SizeOf(pSec);
			tSec.nLength = Marshal.SizeOf(tSec);

			PROCESS_INFORMATION pInfo;
			if (WindowApi.CreateProcess(OfficeApp, CommandLine,
				ref pSec, ref tSec, false, (uint)ProcessPreority.NORMAL_PRIORITY_CLASS,
				IntPtr.Zero, null, ref sInfo, out pInfo))
			{
				_pInfo = pInfo;
			}
		}

		private async UniTask DelayActive(IntPtr myWindow)
		{
			for (var i = 0; i < 50; i++)
			{
				_ = WindowApi.SetFocus(myWindow);
				_ = WindowApi.SwitchToThisWindow(myWindow, true);
				_ = WindowApi.SetActiveWindow(myWindow);
				await UniTask.Delay(50);
			}
		}

		private void TerminateProcess()
		{
			if (_pInfo.hProcess == IntPtr.Zero)
			{
				return;
			}

			_ = WindowApi.TerminateProcess(_pInfo.hProcess, 0);
			_pInfo = new();
		}

		public async void Dispose()
		{
			TerminateProcess();
		}
	}
}
