using System.IO.Pipes;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pipes.Base;

namespace Pipes
{
	public class PipeServer : Pipe, IPipeServer
	{
		private NamedPipeServerStream _server;

		public PipeServer() : base()
		{
			_streamEncoding = new UTF8Encoding();
		}

		public override void Create(string serverName)
		{
			_server = new NamedPipeServerStream(serverName, PipeDirection.InOut, 1);
			SetStreamString(_server);
		}

		public override async UniTask<bool> ConnectAsync()
		{
			if (Connected)
			{
				return false;
			}

			if (_disconnectCancellationTokenSource is { IsCancellationRequested: false })
			{
				return false;
			}

			if (_disposeCancellationTokenSource.IsCancellationRequested)
			{
				return false;
			}

			await WaitConnectClient();
			_ = RequestQueueProcess();
			return true;
		}

		private async UniTask WaitConnectClient()
		{
			await _server.WaitForConnectionAsync();
			Connected = true;

			_disconnectCancellationTokenSource = new CancellationTokenSource();

			OnConnected?.Invoke();
		}

		public override async UniTask<bool> DisconnectAsync()
		{
			if (!Connected)
			{
				return false;
			}

			Connected = false;

			if (_disconnectCancellationTokenSource is { IsCancellationRequested: false })
			{
				_disconnectCancellationTokenSource.Cancel();
				_disconnectCancellationTokenSource.Dispose();
				_disconnectCancellationTokenSource = null;
			}

			Close();

			return true;
		}

		protected override void Close()
		{
			if (_server != null)
			{
				_server.Close();
				_server = null;
			}

			_stringStream = null;
		}
	}
}
