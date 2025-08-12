using System;
using com.ootii.Messages;
using Core.App;
using Core.Messages;
using Cysharp.Threading.Tasks;
using UI.Canvas.Presenter;

namespace UI.Canvas.Controller
{
	public class CanvasController : ICanvasController, IDisposable
	{
		private readonly IApplicationState _applicationState;
		private readonly ICanvasPresenter _root;

		private bool _isHiddenByScreensaver;

		public CanvasController(IApplicationState applicationState,
			ICanvasPresenter root)
		{
			_applicationState = applicationState;
			_root = root;

			MessageDispatcher.AddListener(MessageType.ScreenLock, OnScreenLockedEvent);
			MessageDispatcher.AddListener(MessageType.ScreensaverActivity, OnScreensaverActivity);
		}

		public void Dispose()
		{
			MessageDispatcher.RemoveListener(MessageType.ScreenLock, OnScreenLockedEvent);
			MessageDispatcher.RemoveListener(MessageType.ScreensaverActivity, OnScreensaverActivity);

			_isHiddenByScreensaver = false;
		}

		private void OnScreenLockedEvent(IMessage message)
		{
			if (_isHiddenByScreensaver)
				return;

			ScreenLockadSet(_applicationState.IsScreenLocked);
		}

		public void ScreenLockadSet(bool isScreenLocked)
		{
			var screenLayers = _root.ScreenLayers;

			if (isScreenLocked)
				screenLayers.HideAsync(() => MessageDispatcher.SendMessage(MessageType.PlaybackPause)).Forget();
			else
				screenLayers.ShowAsync().Forget();
		}

		private void OnScreensaverActivity(IMessage message)
		{
			var active = (bool) message.Data;

			var screenLayers = _root.ScreenLayers;

			if (active)
			{
				if (!_applicationState.IsScreenLocked)
					screenLayers.HideAsync(() => MessageDispatcher.SendMessage(MessageType.PlaybackPause)).Forget();

				_isHiddenByScreensaver = true;
			}
			else
			{
				_isHiddenByScreensaver = false;

				if (!_applicationState.IsScreenLocked)
					screenLayers.ShowAsync().Forget();
			}
		}
	}
}