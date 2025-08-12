using System;
using System.Threading;
using Core.Logging;
using Cysharp.Threading.Tasks;
using Environment.Netsoft.WebView.Actions;
using Environment.Netsoft.WebView.Data;
using Pipes.Data;

namespace Environment.Netsoft.WebView
{
	public class PingPong : IPingPong
	{
		public Action<PingData> OnStateDataChange { get; set; }

		private IWebViewApplication _browserApplication;
		private IPackageData _sendPackage;
		private CancellationTokenSource _processCancellationTokenSource;

		public PingData PingData { get; private set; }

		public PingPong(IWebViewApplication browserApplication)
		{
			_browserApplication = browserApplication;
			_sendPackage = new ActionPackage();
		}

		public void Process()
		{
			_processCancellationTokenSource = new();

			SendPingProcess().Forget();
		}

		private async UniTask SendPingProcess()
		{
			try
			{
				await UniTask.SwitchToThreadPool();

				while (!_processCancellationTokenSource.IsCancellationRequested)
				{
					await UniTask.Delay(1000, cancellationToken: _processCancellationTokenSource.Token);
					_browserApplication.Send(ActionsNames.Ping, null, (result, data) =>
					{
						if (data != null)
						{
							var pingDataString = data.ToString();
							PingData = Newtonsoft.Json.JsonConvert.DeserializeObject<PingData>(pingDataString);
							PingDataChange();
						}
					});
				}
			}
			catch (OperationCanceledException)
			{
				_processCancellationTokenSource.Dispose();
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"Ping browser exception {ex.Message}");
			}
		}

		public void Terminate()
		{
			if (_processCancellationTokenSource == null)
				return;

			if (_processCancellationTokenSource is { IsCancellationRequested: true })
				_processCancellationTokenSource.Cancel();
		}

		private void PingDataChange()
		{
			OnStateDataChange?.Invoke(PingData);
		}
	}
}
