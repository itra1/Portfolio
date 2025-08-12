using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using BestHTTP;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Transports;
using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Consts;
using Core.Network.Socket.Enums;
using Core.Network.Socket.Packets.Incoming.Actions;
using Core.Network.Socket.Packets.Incoming.Base;
using Core.Network.Socket.Packets.Incoming.Error;
using Core.Network.Socket.Packets.Incoming.States;
using Cysharp.Threading.Tasks;
using PlatformSupport.Collections.ObjectModel;
using Zenject;
using Debug = Core.Logging.Debug;
using IncomingPacket = Core.Network.Socket.Packets.Incoming.Base.IncomingPacket;

namespace Core.Network.Socket
{
	/// <summary>
	/// Устаревшее название - "Socket3Manager"
	/// </summary>
	public class SocketConnection : ISocketConnection, ISocketState, ISocketSender, ITickable, IDisposable
	{
		private readonly ISocketOptions _options;
		
		private readonly ConcurrentQueue<IIncomingPacket> _receivedIncomingPackets;
		private readonly ConcurrentQueue<IIncomingPacket> _incomingPacketsToProcess;
		private readonly Queue<ValueTuple<string, object>> _pendingOutgoingPackets;
		private readonly ConcurrentQueue<ValueTuple<string, object>> _outgoingPacketsToSend;
		
		private readonly CancellationTokenSource _disposeCancellationTokenSource;
		
		private SocketManager _manager;
		private SocketState _currentState;
		
		public bool IsIncomingPacketExecutionLocked { get; set; }
		public bool IsOutgoingPacketExecutionLocked { get; set; }
		
		public SocketConnection(ISocketOptions options)
		{
			_options = options;
			
			_receivedIncomingPackets = new ConcurrentQueue<IIncomingPacket>();
			_incomingPacketsToProcess = new ConcurrentQueue<IIncomingPacket>();
			_pendingOutgoingPackets = new Queue<ValueTuple<string, object>>();
			_outgoingPacketsToSend = new ConcurrentQueue<ValueTuple<string, object>>();
			
			_disposeCancellationTokenSource = new CancellationTokenSource();
			
			_currentState = SocketState.Initialized;
			
			MessageDispatcher.AddListener(MessageType.UserInstallationPreloadComplete, OnUserInstallationPreloadCompleted);
		}
		
		public void ForcedConnect() => AttemptToConnect(true);
		
		public void Send(string eventName, object data)
		{
			if (_currentState == SocketState.Connected)
			{
				Send((eventName, data));
			}
			else
			{
				var outgoingPacket = (eventName, data);
				
				if (!_pendingOutgoingPackets.Contains(outgoingPacket))
					_pendingOutgoingPackets.Enqueue(outgoingPacket);
			}
		}
		
		public void Tick()
		{
			if (_disposeCancellationTokenSource.IsCancellationRequested || _currentState != SocketState.Connected)
				return;
			
			if (!IsIncomingPacketExecutionLocked)
			{
				while (_incomingPacketsToProcess.TryDequeue(out var incomingPacket))
					incomingPacket.Process();
			}
			
			while (_pendingOutgoingPackets.Count > 0)
				Send(_pendingOutgoingPackets.Dequeue());
		}
		
		public void Dispose()
		{
			MessageDispatcher.RemoveListener(MessageType.UserInstallationPreloadComplete, OnUserInstallationPreloadCompleted);
			
			if (!_disposeCancellationTokenSource.IsCancellationRequested)
			{
				_disposeCancellationTokenSource.Cancel();
				_disposeCancellationTokenSource.Dispose();
			}
			
			Disconnect();
			
			_receivedIncomingPackets.Clear();
			_incomingPacketsToProcess.Clear();
			_pendingOutgoingPackets.Clear();
			_outgoingPacketsToSend.Clear();
			
			_currentState = SocketState.Disposed;
			
			Debug.Log(SocketLogMessage.Disposed);
			
			IsIncomingPacketExecutionLocked = false;
			IsOutgoingPacketExecutionLocked = false;
		}
		
		private async UniTaskVoid UpdateAsync()
		{
			try
			{
				var cancellationToken = _disposeCancellationTokenSource.Token;
				
				await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
				{
					await UniTask.SwitchToThreadPool();
					
					while (_currentState == SocketState.Connected)
					{
						cancellationToken.ThrowIfCancellationRequested();
						
						while (!IsIncomingPacketExecutionLocked && _receivedIncomingPackets.TryDequeue(out var incomingPacket))
						{
							cancellationToken.ThrowIfCancellationRequested();
							
							try
							{
								incomingPacket.Execute();
								
								if (incomingPacket.Parse())
								{
									_incomingPacketsToProcess.Enqueue(incomingPacket);
								}
								else
								{
									var packetType = incomingPacket.PacketType;
									
									Debug.Log(string.IsNullOrEmpty(packetType) 
										? PacketLogMessage.PacketIgnored
										: string.Format(PacketLogMessage.PacketIgnoredFormat, packetType));
								}
							}
							catch (Exception exception)
							{
								string message;
								
								if (incomingPacket != null)
								{
									message = string.Format(PacketLogMessage.FullPacketParsingErrorFormat,
										incomingPacket.GetType().Name,
										exception.Message,
										Environment.NewLine,
										exception.StackTrace);
								}
								else
								{
									message = string.Format(PacketLogMessage.ShortPacketParsingErrorFormat,
										exception.Message,
										Environment.NewLine,
										exception.StackTrace);
								}
								
								Debug.LogError(message);
							}
						}
						
						while (!IsOutgoingPacketExecutionLocked && _outgoingPacketsToSend.TryDequeue(out var outgoingPacket))
						{
							cancellationToken.ThrowIfCancellationRequested();
							
							_manager.Socket.Emit(outgoingPacket.Item1, out var packet, outgoingPacket.Item2);
							
							Debug.Log(string.Format(PacketLogMessage.PacketSentFormat, packet.Payload));
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
			}
		}
		
		private void AttemptToConnect(bool forced = false)
		{
			switch (_currentState)
			{
				case SocketState.Connecting:
				case SocketState.Connected:
				case SocketState.Disposed:
					Debug.Log(string.Format(SocketLogMessage.ConnectingRefusedFormat, _currentState));
					return;
			}
			
			_currentState = SocketState.Connecting;
			
			Debug.Log(string.Format(SocketLogMessage.ConnectingFormat, _options.Server));
			
			if (_options.UseProxy)
			{
				var proxy = _options.Proxy;

				if (!string.IsNullOrEmpty(proxy))
				{
					Debug.Log(string.Format(SocketLogMessage.ConnectionGoesThroughProxyFormat, proxy));
				
					HTTPManager.Proxy = new HTTPProxy(new Uri(proxy), null, true);
				}
			}
			
			var socketOptions = new BestHTTP.SocketIO3.SocketOptions
			{
				Auth = (_, _) => new { token = _options.ServerToken },
				ConnectWith = TransportTypes.WebSocket,
				AutoConnect = false,
				Timeout = TimeSpan.FromDays(1),
				AdditionalQueryParams = new ObservableDictionary<string, string>()
			};
			
			if (forced)
				socketOptions.AdditionalQueryParams.Add("force", "true");

			_manager = new SocketManager(new Uri(_options.Server), socketOptions);
			
			var socket = _manager.Socket;

			if (socket != null)
			{
				socket.On(SocketIOEventTypes.Connect, OnConnected);
				socket.On(SocketIOEventTypes.Disconnect, OnDisconnected);
				socket.On<ErrorPacket>(SocketIOEventTypes.Error, OnError);
				socket.On<ActionPacket>(CustomSocketEventName.Action, OnReceived);
				socket.On<IncomingState>(CustomSocketEventName.State, OnReceived);
			}
			
			_manager.Open();
		}
		
		private void Disconnect()
		{
			switch (_currentState)
			{
				case SocketState.Initialized:
				case SocketState.Disconnecting:
				case SocketState.Disconnected:
				case SocketState.Disposed:
					Debug.Log(string.Format(SocketLogMessage.DisconnectingRefusedFormat, _currentState));
					return;
			}
			
			_currentState = SocketState.Disconnecting;
			
			Debug.Log(string.Format(SocketLogMessage.DisconnectingFormat, _options.Server));
			
			var socket = _manager.Socket;

			if (socket != null)
			{
				socket.Off(SocketIOEventTypes.Connect);
				socket.Off(SocketIOEventTypes.Disconnect);
				socket.Off(SocketIOEventTypes.Error);
				socket.Off(CustomSocketEventName.Action);
				socket.Off(CustomSocketEventName.State);
			}
			
			_manager.Close();
			_manager = null;
		}
		
		private void Send(in ValueTuple<string, object> data) => _outgoingPacketsToSend.Enqueue(data);
		
		private void OnConnected() => OnConnectedAsync().Forget();
		private void OnDisconnected() => OnDisconnectedAsync().Forget();
		private void OnError(ErrorPacket packet) => OnErrorAsync(packet).Forget();
		
		private void OnReceived(IncomingPacket packet)
		{
			Debug.Log(string.Format(PacketLogMessage.PacketReceivedFormat, packet));
			
			_receivedIncomingPackets.Enqueue(packet);
		}

		private async UniTaskVoid OnConnectedAsync()
		{
			if (Thread.CurrentThread.IsBackground)
				await UniTask.SwitchToMainThread();
			
			_currentState = SocketState.Connected;
			
			Debug.Log(SocketLogMessage.Connected);
			
			UpdateAsync().Forget();
			
			MessageDispatcher.SendMessage(MessageType.SocketOpen);
		}

		private async UniTaskVoid OnDisconnectedAsync()
		{
			if (Thread.CurrentThread.IsBackground)
				await UniTask.SwitchToMainThread();
			
			_currentState = SocketState.Disconnected;
			
			Debug.Log(SocketLogMessage.Disconnected);
			
			MessageDispatcher.SendMessage(MessageType.SocketClose);
		}

		private async UniTaskVoid OnErrorAsync(ErrorPacket packet)
		{
			if (Thread.CurrentThread.IsBackground)
				await UniTask.SwitchToMainThread();
			
			var errorCode = packet.Code;
			var message = packet.Message;
			
			Debug.LogError(string.Format(SocketLogMessage.ErrorFormat, errorCode, message));
			
			MessageDispatcher.SendMessage(this, MessageType.SocketError, errorCode, EnumMessageDelay.IMMEDIATE);
			
			Disconnect();
		}
		
		private void OnUserInstallationPreloadCompleted(IMessage message)
		{
			MessageDispatcher.RemoveListener(MessageType.UserInstallationPreloadComplete, OnUserInstallationPreloadCompleted);
			
			AttemptToConnect();
		}
	}
}