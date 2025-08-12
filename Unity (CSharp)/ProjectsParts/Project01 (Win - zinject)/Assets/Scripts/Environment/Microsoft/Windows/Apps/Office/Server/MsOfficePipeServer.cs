using System;
using System.Collections.Concurrent;
using System.Threading;
using Components.Pipes;
using Core.Logging;
using Core.Options;
using Cysharp.Threading.Tasks;
using Environment.Microsoft.Windows.Apps.Office.Server.Data;
using Environment.Microsoft.Windows.Apps.Office.Server.Packets;
using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Base;
using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Converting.Deserializers;
using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Converting.Serializers;
using Pipes;

namespace Environment.Microsoft.Windows.Apps.Office.Server
{
	public class MsOfficePipeServer : IMsOfficePipeServer, IMsOfficeRequestAsync
	{
		private readonly IApplicationOptions _options;
		private readonly IPacketSerializer _packetSerializer;
		private readonly IPacketDeserializer _packetDeserializer;
		private IPipeServer _pipeServer;
		private readonly ConcurrentQueue<IMsOfficeRequestData> _requests;
		private readonly CancellationTokenSource _disposeCancellationTokenSource;

		private CancellationTokenSource _disconnectCancellationTokenSource;
		private bool _isAnyRequestInProcess;

		public bool IsInitialized => _packetDeserializer.IsInitialized;

		public bool Connected { get; private set; }

		public MsOfficePipeServer(IApplicationOptions options,
				IPacketSerializer packetSerializer,
				IPacketDeserializer packetDeserializer,
				IPipeServerFactory pipeServerFactory)
		{
			_options = options;
			_packetSerializer = packetSerializer;
			_packetDeserializer = packetDeserializer;
			_pipeServer = pipeServerFactory.Create();
			_pipeServer.LockedDisposeInFactory = true;
			_requests = new ConcurrentQueue<IMsOfficeRequestData>();
			_disposeCancellationTokenSource = new CancellationTokenSource();
		}

		public async UniTask<bool> ConnectAsync(string serverName)
		{

			if (Connected)
			{
				Debug.LogError("MS Office Pipe Server has already been connected");
				return false;
			}

			if (_disconnectCancellationTokenSource is { IsCancellationRequested: false })
			{
				Debug.LogError("MS Office Pipe Server is in progress to connect");
				return false;
			}

			if (_disposeCancellationTokenSource.IsCancellationRequested)
			{
				Debug.LogError("MS Office Pipe Server has been disposed");
				return false;
			}
			_disconnectCancellationTokenSource = new CancellationTokenSource();

			_ = CancellationTokenSource.CreateLinkedTokenSource(_disposeCancellationTokenSource.Token,
		_disconnectCancellationTokenSource.Token);

			_pipeServer.Create(serverName);

			_ = await _pipeServer.ConnectAsync();
			Connected = true;
			UpdateAsync().Forget();

			//CancellationTokenSource mergedCancellationTokenSource = null;

			//try
			//{
			//	_disconnectCancellationTokenSource = new CancellationTokenSource();

			//	mergedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_disposeCancellationTokenSource.Token,
			//			_disconnectCancellationTokenSource.Token);

			//	var cancellationToken = mergedCancellationTokenSource.Token;

			//	_server = new NamedPipeServerStream(serverName, PipeDirection.InOut, 1);

			//	await _server.WaitForConnectionAsync(cancellationToken);

			//	_stringStream = new StringStream(_server, new UTF8Encoding());

			//	var value = FirstPacketOptions.RequestText;

			//	_stringStream.Write(value);

			//	value = _stringStream.Read();

			//	if (value == FirstPacketOptions.ExpectedResponseText)
			//	{
			//		Connected = true;
			//		UpdateAsync().Forget();
			//	}
			//	else
			//	{
			//		throw new ApplicationException("Failed to connect to MS Office Pipe Server");
			//	}
			//}
			//catch (OperationCanceledException)
			//{
			//	Close();
			//	return false;
			//}
			//catch (Exception exception)
			//{
			//	Close();
			//	Debug.LogException(exception);
			//	return false;
			//}
			//finally
			//{
			//	mergedCancellationTokenSource?.Dispose();
			//}

			return true;
		}

		public async UniTask<IPacket> RequestAsync(IPacket outgoingPacket, CancellationToken cancellationToken)
		{
			if (!Connected)
			{
				Debug.LogError("Failed to make a request because MS Office Pipe Server has not been connected");
				return null;
			}

			if (_disconnectCancellationTokenSource == null || _disconnectCancellationTokenSource.IsCancellationRequested)
			{
				Debug.LogError("Failed to make a request because MS Office Pipe Server has been disconnected");
				return null;
			}

			if (_disposeCancellationTokenSource.IsCancellationRequested)
			{
				Debug.LogError("Failed to make a request because MS Office Pipe Server has been disposed");
				return null;
			}

			IMsOfficeRequestData request = new MsOfficeRequestData
			{
				OutgoingPacket = outgoingPacket
			};

			_requests.Enqueue(request);

			CancellationTokenSource mergedCancellationTokenSource = null;

			try
			{
				mergedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_disposeCancellationTokenSource.Token,
						_disconnectCancellationTokenSource.Token,
						cancellationToken);

				await UniTask.WaitUntil(() => request.Completed, cancellationToken: mergedCancellationTokenSource.Token);
			}
			catch (OperationCanceledException)
			{
				return null;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return null;
			}
			finally
			{
				mergedCancellationTokenSource?.Dispose();
			}

			return request.IncomingPacket;
		}

		public async UniTask<bool> DisconnectAsync()
		{
			if (!Connected)
			{
				Debug.LogError("MS Office Pipe Server has not been connected yet");
				return false;
			}
			_requests.Clear();
			_ = await RequestAsync(new CommonQuit(), CancellationToken.None);

			if (!_requests.IsEmpty || _isAnyRequestInProcess)
			{
				try
				{
					await UniTask.WaitUntil(() => _requests.IsEmpty && !_isAnyRequestInProcess, cancellationToken: CancellationToken.None);
				}
				catch (Exception exception) when (exception is not OperationCanceledException)
				{
					Debug.LogException(exception);
				}
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

		public void Dispose()
		{
			if (_disposeCancellationTokenSource is { IsCancellationRequested: false })
			{
				_disposeCancellationTokenSource.Cancel();
				_disposeCancellationTokenSource.Dispose();
			}

			_packetDeserializer.Dispose();

			_requests.Clear();
		}

		private void Close()
		{
			if (_pipeServer != null)
			{
				_ = _pipeServer.DisconnectAsync();
				_pipeServer = null;
			}
		}

		private async UniTaskVoid UpdateAsync()
		{
			CancellationTokenSource mergedCancellationTokenSource = null;

			try
			{
				mergedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_disposeCancellationTokenSource.Token,
						_disconnectCancellationTokenSource.Token);

				var cancellationToken = mergedCancellationTokenSource.Token;

				await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
				{
					await UniTask.SwitchToThreadPool();

					while (Connected)
					{
						if (!_requests.TryDequeue(out var request))
						{
							await UniTask.Yield(cancellationToken: cancellationToken);
							continue;
						}

						try
						{
							_isAnyRequestInProcess = true;

							SendMessage sendPackage = new()
							{
								OutgoingPacket = _packetSerializer.Serialize(request.OutgoingPacket)
							};

							var answer = await _pipeServer.Request(sendPackage);

							cancellationToken.ThrowIfCancellationRequested();

							if (!string.IsNullOrEmpty(answer.IncomingPacket))
								request.IncomingPacket = _packetDeserializer.Deserialize(answer.IncomingPacket);
						}
						finally
						{
							request.Completed = true;

							_isAnyRequestInProcess = false;
						}
					}
				}
			}
			catch (Exception exception) when (exception is not OperationCanceledException)
			{
				Debug.LogException(exception);
			}
			finally
			{
				if (Thread.CurrentThread.IsBackground)
					await UniTask.SwitchToMainThread();

				mergedCancellationTokenSource?.Dispose();
			}
		}
	}
}