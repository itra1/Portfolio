using System;
using System.Threading;
using com.ootii.Messages;
using Core.Messages;
using Core.Network.Api.Consts;
using Core.Network.Http;
using Core.Network.Http.Data;
using Core.Network.Socket;
using Core.Network.Socket.Packets.Incoming.States.Data;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Core.Options;
using Core.Recovery;
using Debug = Core.Logging.Debug;

namespace Core.App
{
	public class ApplicationState : IApplicationState, IApplicationStateSetter, IApplicationStateAcceptor, IDisposable
	{
		private readonly IApplicationOptions _options;
		private readonly IPreviousSessionRecovery _recovery;
		private readonly IHttpRequest _request;
		private readonly ISocketState _socketState;
		private readonly IOutgoingStateController _outgoingState;
		private readonly CancellationTokenSource _disposeCancellationTokenSource;

		private bool _isIntroCompleted;
		private bool _isMusicPlayerPreloadCompleted;
		private bool _isScreenLocked;
		private bool _isAccepted;

		public bool IsScreenLocked
		{
			get => _isScreenLocked;
			set
			{
				if (_isScreenLocked == value)
					return;

				SetScreenLockState(value);

				_outgoingState.PrepareToSendIfAllowed();
			}
		}

		public bool IsAppLoadingCompleted { get; private set; }

		public ApplicationState(IApplicationOptions options,
			IPreviousSessionRecovery recovery,
			IHttpRequest request,
			ISocketState socketState,
			IOutgoingStateController outgoingState)
		{
			_options = options;
			_recovery = recovery;
			_request = request;
			_socketState = socketState;
			_outgoingState = outgoingState;
			_disposeCancellationTokenSource = new CancellationTokenSource();

			MessageDispatcher.AddListener(MessageType.IntroComplete, OnIntroCompleted);
			MessageDispatcher.AddListener(MessageType.SocketOpen, OnSocketConnected);
			MessageDispatcher.AddListener(MessageType.MusicPlayerPreloadCompleted, OnMusicPlayerPreloadCompleted);
		}

		public void Accept(IncomingStateData state, ulong? cursorId)
		{
			if (_disposeCancellationTokenSource.IsCancellationRequested || _isAccepted)
				return;

			_isAccepted = true;

			if (_options.IsPresetRestoredAtStart)
				_recovery.HandleStateData(state);
			else
				_recovery.SetDefaultDesktop();

			if (state.SoundVolume != null)
				MessageDispatcher.SendMessage(this, MessageType.AudioSet, state.SoundVolume, EnumMessageDelay.IMMEDIATE);

			if (cursorId != null)
				MessageDispatcher.SendMessage(this, MessageType.MouseCursorSelect, cursorId.Value, EnumMessageDelay.IMMEDIATE);
		}

		public void Dispose()
		{
			if (!_disposeCancellationTokenSource.IsCancellationRequested)
			{
				_disposeCancellationTokenSource.Cancel();
				_disposeCancellationTokenSource.Dispose();
			}

			MessageDispatcher.RemoveListener(MessageType.IntroComplete, OnIntroCompleted);
			MessageDispatcher.RemoveListener(MessageType.SocketOpen, OnSocketConnected);
			MessageDispatcher.RemoveListener(MessageType.MusicPlayerPreloadCompleted, OnMusicPlayerPreloadCompleted);

			_isIntroCompleted = false;
			_isMusicPlayerPreloadCompleted = false;
			_isScreenLocked = false;
			_isAccepted = false;

			IsAppLoadingCompleted = false;
		}

		private void SetScreenLockState(bool value)
		{
			_isScreenLocked = value;

			_outgoingState.Context.SetScreenAsLocked(value);

			if (!_options.IsStateSendingAllowed)
			{
				_request.Request(RestApiUrl.LockScreen,
					HttpMethodType.Patch,
					new[] { ("lockScreen", (object) value) });
			}

			MessageDispatcher.SendMessageData(MessageType.ScreenLock, value);
		}

		private void DispatchMessageThatAppLoadCompleted()
		{
			_outgoingState.IsSendingLocked = false;

			_socketState.IsIncomingPacketExecutionLocked = false;
			_socketState.IsOutgoingPacketExecutionLocked = false;

			IsAppLoadingCompleted = true;

			if (_options.IsManagersLogEnabled)
				Debug.Log("Application loading completed");

			MessageDispatcher.SendMessage(MessageType.AppLoadComplete);
		}

		private void OnIntroCompleted(IMessage message)
		{
			MessageDispatcher.RemoveListener(MessageType.IntroComplete, OnIntroCompleted);

			_isIntroCompleted = true;

			if (_isMusicPlayerPreloadCompleted)
				DispatchMessageThatAppLoadCompleted();
		}

		private void OnSocketConnected(IMessage message)
		{
			MessageDispatcher.RemoveListener(MessageType.SocketOpen, OnSocketConnected);

			if (!_options.IsUnlockedAtStart)
				SetScreenLockState(true);
			else
				SetScreenLockState(false);

			_outgoingState.ForceToSendIfAllowed();

			_outgoingState.IsSendingLocked = true;

			_socketState.IsIncomingPacketExecutionLocked = true;
			_socketState.IsOutgoingPacketExecutionLocked = true;
		}

		private void OnMusicPlayerPreloadCompleted(IMessage message)
		{
			MessageDispatcher.RemoveListener(MessageType.AutoMaterialDataPreloadComplete, OnMusicPlayerPreloadCompleted);

			_isMusicPlayerPreloadCompleted = true;

			if (_isIntroCompleted)
				DispatchMessageThatAppLoadCompleted();
		}
	}
}