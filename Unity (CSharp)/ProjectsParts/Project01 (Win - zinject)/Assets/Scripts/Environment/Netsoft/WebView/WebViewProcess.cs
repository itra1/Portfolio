using System;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Environment.Microsoft.Windows.Api;
using Environment.Netsoft.WebView.Settings;
using UnityEngine;
using Debug = Core.Logging.Debug;

namespace Environment.Netsoft.WebView
{
	public class WebViewProcess : IWebViewProcess
	{
		private WindowApi.PROCESS_INFORMATION _processInfo;

		private WindowApi.PROCESS_INFORMATION ProcessInfo => _processInfo;

		private string ApplicationPath
		{
			get
			{
				return @$"{Application.dataPath}\.."
#if !UNITY_EDITOR
				+ @"\.."
#endif
				+ @"\browser\Browser.exe";
			}
		}

		public async UniTask<bool> CreateProcess(IApplicationRunOptions arguments)
		{
			Debug.Log($"Create browser {ApplicationPath}");
			bool loaded = false;

			WindowApi.STARTUPINFO sInfo = new()
			{
				dwXSize = 1,
				dwYSize = 1
			};

			WindowApi.SECURITY_ATTRIBUTES pSec = new();
			WindowApi.SECURITY_ATTRIBUTES tSec = new();

			pSec.nLength = Marshal.SizeOf(pSec);
			tSec.nLength = Marshal.SizeOf(tSec);

			if (WindowApi.CreateProcess(ApplicationPath,
							arguments.MakeArgumentsrunApp(),
							ref pSec,
							ref tSec,
							false,
							(uint) WindowApi.ProcessPriority.NORMAL_PRIORITY_CLASS,
							IntPtr.Zero,
							null,
							ref sInfo,
							out var _processInfo))
			{
				loaded = true;
			}

			await UniTask.WaitUntil(() => loaded);
			await UniTask.Delay(200);

			if (_processInfo.hProcess == IntPtr.Zero)
			{
				Debug.LogError("No create browserProcess");
				return false;
			}

			return true;
		}

		public void TerminateProcess()
		{
			if (_processInfo.hProcess == IntPtr.Zero)
				return;

			_ = WindowApi.TerminateProcess(_processInfo.hProcess, 0);
			_processInfo = default;
		}
	}
}
