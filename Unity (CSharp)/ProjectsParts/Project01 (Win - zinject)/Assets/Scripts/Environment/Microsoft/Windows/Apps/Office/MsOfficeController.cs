using System;
using System.Runtime.InteropServices;
using com.ootii.Messages;
using Core.Base;
using Core.Messages;
using Core.Options;
using Cysharp.Threading.Tasks;
using Environment.Microsoft.Windows.Api;
using Environment.Microsoft.Windows.Apps.Office.Server;

namespace Environment.Microsoft.Windows.Apps.Office
{
	public class MsOfficeController : IMsOfficeController, IDisposable, ILateInitialized
	{
		private readonly IApplicationOptions _options;
		private readonly IMsOfficePipeServer _server;

		private WindowApi.PROCESS_INFORMATION _processInfo;

		public bool IsInitialized => _server.IsInitialized;

		public string _applicationPath;

		public MsOfficeController(string officeApp, IApplicationOptions options, IMsOfficePipeServer server)
		{
			_applicationPath = officeApp;
			_options = options;
			_server = server;

			MessageDispatcher.AddListener(MessageType.AppInitialize, OnApplicationInitialized);
		}

		public void Dispose()
		{
			MessageDispatcher.RemoveListener(MessageType.AppInitialize, OnApplicationInitialized);

			CloseAsync().Forget();
		}

		private async UniTaskVoid CloseAsync()
		{
			if (_server.Connected)
				_ = await _server.DisconnectAsync();

			_server.Dispose();

			if (_options.IsMSOfficeUsed)
				TerminateProcess();
		}

		private bool CreateProcess(params string[] arguments)
		{
			WindowApi.STARTUPINFO sInfo = new()
			{
				dwXSize = 1,
				dwYSize = 1
			};

			WindowApi.SECURITY_ATTRIBUTES pSec = new();
			WindowApi.SECURITY_ATTRIBUTES tSec = new();

			pSec.nLength = Marshal.SizeOf(pSec);
			tSec.nLength = Marshal.SizeOf(tSec);

			if (!WindowApi.CreateProcess(_applicationPath,
							$" {string.Join(' ', arguments)}",
							ref pSec,
							ref tSec,
							false,
							(uint) WindowApi.ProcessPriority.NORMAL_PRIORITY_CLASS,
							IntPtr.Zero,
							null,
							ref sInfo,
							out var processInfo))
			{
				return false;
			}

			_processInfo = processInfo;

			return true;
		}

		private void TerminateProcess()
		{
			if (_processInfo.hProcess == IntPtr.Zero)
				return;

			_ = WindowApi.TerminateProcess(_processInfo.hProcess, 0);
			_processInfo = default;
		}

		private void OnApplicationInitialized(IMessage message)
		{
			MessageDispatcher.RemoveListener(MessageType.AppInitialize, OnApplicationInitialized);

			if (!_options.IsMSOfficeUsed)
				return;

			var serverName = Guid.NewGuid().ToString();

			if (CreateProcess(serverName))
				_server.ConnectAsync(serverName).Forget();
		}
	}
}