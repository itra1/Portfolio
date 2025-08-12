using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using com.ootii.Messages;
using Core.App;
using Core.Messages;
using Core.Network.Socket.Packets.Incoming.Actions;
using Core.Options;
using Core.Options.Offsets;
using Core.UI.Timers.Data;
using Cysharp.Threading.Tasks;
using UI.Canvas.Presenter;
using UI.Timers.Controller.Items.Base;
using UI.Timers.Controller.Items.Factory;
using UI.Timers.Presenter;
using Debug = Core.Logging.Debug;

namespace UI.Timers.Controller
{
	public class TimersController : ITimersController, IDisposable
	{
		private readonly IScreenOffsets _screenOffsets;
		private readonly IApplicationOptions _options;
		private readonly IApplicationState _applicationState;
		private readonly ITimersFactory _factory;
		private readonly ICanvasPresenter _root;
		
		private CancellationTokenSource _initializeCancellationTokenSource;
		private CancellationTokenSource _updateCancellationTokenSource;
		
		private IDictionary<TimerType, ITimer> _timers;
		private ITimersPresenter _presenter;
		private bool _isHiddenByScreensaver;
		
		public TimersController(IScreenOffsets screenOffsets,
			IApplicationState applicationState,
			ITimersFactory factory,
			ICanvasPresenter root)
		{
			_screenOffsets = screenOffsets;
			_applicationState = applicationState;
			_factory = factory;
			_root = root;
			
			MessageDispatcher.AddListener(MessageType.AppLoadComplete, OnApplicationLoadingCompleted);
			MessageDispatcher.AddListener(MessageType.ScreenLock, OnScreenLocked);
			MessageDispatcher.AddListener(MessageType.ScreensaverActivity, OnScreensaverActivity);
		}

		private async UniTaskVoid InitializeAsync()
		{
			_initializeCancellationTokenSource = new CancellationTokenSource();
			
			try
			{
				if (!_factory.IsInitialized)
				{
					await UniTask.WaitUntil(() => _factory.IsInitialized,
						cancellationToken: _initializeCancellationTokenSource.Token);
				}
				
				_timers = CreateTimers(_factory);
				
				_presenter = InitializePresenter(_screenOffsets,
					_timers.Values.Cast<ITimerInfo>().ToArray(),
					_root);
				
				if (!_isHiddenByScreensaver && !_applicationState.IsScreenLocked)
					_presenter.ShowAsync().Forget();
				
				MessageDispatcher.AddListener(MessageType.TimerSetAlarm, OnTimerSetAlarm);
				MessageDispatcher.AddListener(MessageType.TimerPlayAlarm, OnTimerPlayAlarm);
				MessageDispatcher.AddListener(MessageType.TimerSetDisplay, OnTimerSetDisplay);
				MessageDispatcher.AddListener(MessageType.TimerSetPosition, OnTimerSetPosition);
				MessageDispatcher.AddListener(MessageType.TimerSetColor, OnTimerSetColor);
				MessageDispatcher.AddListener(MessageType.TimerPause, OnTimerPaused);
				MessageDispatcher.AddListener(MessageType.TimerReset, OnTimerReset);
				MessageDispatcher.AddListener(MessageType.TimerSet, OnTimerSet);
				MessageDispatcher.AddListener(MessageType.StopwatchSetPosition, OnStopwatchSetPosition);
				MessageDispatcher.AddListener(MessageType.StopwatchSetColor, OnStopwatchSetColor);
				MessageDispatcher.AddListener(MessageType.StopwatchStart, OnStopwatchStarted);
				MessageDispatcher.AddListener(MessageType.StopwatchPause, OnStopwatchPaused);
				MessageDispatcher.AddListener(MessageType.StopwatchReset, OnStopwatchReset);
				MessageDispatcher.AddListener(MessageType.StopwatchEdit, OnStopwatchEdited);
				MessageDispatcher.AddListener(MessageType.StopwatchLap, OnStopwatchLap);
				MessageDispatcher.AddListener(MessageType.StopwatchLapRemove, OnStopwatchLapRemove);
				MessageDispatcher.AddListener(MessageType.StopwatchLapReset, OnStopwatchLapReset);
				
				UpdateAsync().Forget();
			}
			catch (Exception exception) when (exception is not OperationCanceledException)
			{
				Debug.LogException(exception);
			}
			finally
			{
				if (_initializeCancellationTokenSource != null)
				{
					_initializeCancellationTokenSource.Dispose();
					_initializeCancellationTokenSource = null;
				}
			}
		}
		
		private async UniTaskVoid UpdateAsync()
		{
			_updateCancellationTokenSource = new CancellationTokenSource();
			
			try
			{
				var cancellationToken = _updateCancellationTokenSource.Token;
				var delayTimeSpan = TimeSpan.FromSeconds(1.0);
				
				while (_updateCancellationTokenSource != null && _timers != null)
				{
					foreach (var timer in _timers.Values)
					{
						if (timer.Active)
							timer.Update();
					}
					
					await UniTask.Delay(delayTimeSpan, cancellationToken: cancellationToken);
				}
			}
			catch (Exception exception) when (exception is not OperationCanceledException)
			{
				Debug.LogException(exception);
			}
		}
		
		public void Dispose()
		{
			MessageDispatcher.RemoveListener(MessageType.TimerSetAlarm, OnTimerSetAlarm);
			MessageDispatcher.RemoveListener(MessageType.TimerPlayAlarm, OnTimerPlayAlarm);
			MessageDispatcher.RemoveListener(MessageType.TimerSetDisplay, OnTimerSetDisplay);
			MessageDispatcher.RemoveListener(MessageType.TimerSetPosition, OnTimerSetPosition);
			MessageDispatcher.RemoveListener(MessageType.TimerSetColor, OnTimerSetColor);
			MessageDispatcher.RemoveListener(MessageType.TimerPause, OnTimerPaused);
			MessageDispatcher.RemoveListener(MessageType.TimerReset, OnTimerReset);
			MessageDispatcher.RemoveListener(MessageType.TimerSet, OnTimerSet);
			MessageDispatcher.RemoveListener(MessageType.StopwatchSetPosition, OnStopwatchSetPosition);
			MessageDispatcher.RemoveListener(MessageType.StopwatchStart, OnStopwatchStarted);
			MessageDispatcher.RemoveListener(MessageType.StopwatchPause, OnStopwatchPaused);
			MessageDispatcher.RemoveListener(MessageType.StopwatchReset, OnStopwatchReset);
			MessageDispatcher.RemoveListener(MessageType.StopwatchEdit, OnStopwatchEdited);
			MessageDispatcher.RemoveListener(MessageType.StopwatchLap, OnStopwatchLap);
			MessageDispatcher.RemoveListener(MessageType.StopwatchLapRemove, OnStopwatchLapRemove);
			MessageDispatcher.RemoveListener(MessageType.StopwatchLapReset, OnStopwatchLapReset);
			
			MessageDispatcher.RemoveListener(MessageType.AppLoadComplete, OnApplicationLoadingCompleted);
			MessageDispatcher.RemoveListener(MessageType.ScreenLock, OnScreenLocked);
			MessageDispatcher.RemoveListener(MessageType.ScreensaverActivity, OnScreensaverActivity);
			
			if (_initializeCancellationTokenSource is { IsCancellationRequested: false })
				_initializeCancellationTokenSource.Cancel();
			
			if (_updateCancellationTokenSource != null)
			{
				_updateCancellationTokenSource.Cancel();
				_updateCancellationTokenSource.Dispose();
				_updateCancellationTokenSource = null;
			}
			
			_isHiddenByScreensaver = false;
			
			if (_presenter != null)
			{
				_presenter.Unload();
				_presenter = null;
			}

			if (_timers != null)
			{
				_timers.Clear();
				_timers = null;
			}
		}
		
		private IDictionary<TimerType, ITimer> CreateTimers(ITimersFactory factory)
		{
			var types = factory.Types;
			var typesCount = types.Count;
			var timers = new Dictionary<TimerType, ITimer>(typesCount);
			
			for (var i = 0; i < typesCount; i++)
			{
				var timer = factory.Create(types[i]);
				timers.Add(timer.Type, timer);
			}
			
			return timers;
		}
		
		private ITimersPresenter InitializePresenter(IScreenOffsets screenOffsets,
			IReadOnlyList<ITimerInfo> infoList,
			ICanvasPresenter root)
		{
			var presenter = root.Timers;
			
			presenter.Initialize(screenOffsets, infoList);
			presenter.AlignToParent();
			
			return presenter;
		}
		
		private bool TryGetTimer(TimerType type, out ITimer timer)
		{
			if (_timers.TryGetValue(type, out timer)) 
				return true;
			
			Debug.LogError($"Timer with type \"{type}\" is missing");
			return false;
		}
		
		private void SetTimerActive(TimerType type, bool active, long time = 0L)
		{
			if (!TryGetTimer(type, out var timer) || (timer.Active && active))
				return;
			
			if (active)
			{
				if (!timer.Paused)
					timer.Start(time);
				else
					timer.Resume();
			}
			else
			{
				timer.Stop();
			}
		}

		private void SetTimerPaused(TimerType type, bool paused)
		{
			if (!TryGetTimer(type, out var timer) || timer.Paused == paused)
				return;
			
			if (paused)
				timer.Pause();
			else
				timer.Resume();
		}
		
		private void SetTimerAlarm(TimerType type, bool isOn)
		{
			if (TryGetTimer(type, out var timer) && timer is IAlarmState alarmTimer)
				alarmTimer.IsAlarmOn = isOn;
		}
		
		private void AddStopwatchLap()
		{
			if (TryGetTimer(TimerType.Stopwatch, out var timer) && timer is ITimerLaps timerLaps)
				timerLaps.AddLap();
		}
		
		private void RemoveStopwatchLapAt(int index)
		{
			if (TryGetTimer(TimerType.Stopwatch, out var timer) && timer is ITimerLaps timerLaps)
				timerLaps.RemoveLapAt(index);
		}
		
		private void RemoveAllStopwatchLaps()
		{
			if (TryGetTimer(TimerType.Stopwatch, out var timer) && timer is ITimerLaps timerLaps)
				timerLaps.RemoveAllLaps();
		}

		private void SetTimerPosition(TimerType type, float x, float y)
		{
			if (TryGetTimer(type, out var timer) && timer is ITimerPosition timerPosition)
				timerPosition.SetPosition(x, y);
		}
		
		private void SetTimerColor(TimerType type, string color)
		{
			if (TryGetTimer(type, out var timer) && timer is ITimerColor timerColor)
				timerColor.SetColor(color);
		}
		
		private void SetWidgetActive(TimerType type, bool active)
		{
			if (active)
			{
				var isAnyWidgetVisible = _presenter.IsAnyWidgetVisible;
				
				if (_presenter.ShowWidget(type) && !isAnyWidgetVisible)
					_presenter.Activate();
			}
			else
			{
				if (_presenter.HideWidget(type) && !_presenter.IsAnyWidgetVisible)
					_presenter.Deactivate();
			}
		}
		
		private void OnTimerSetAlarm(IMessage message)
		{
			var action = (TimerSetAlarm) message.Sender;
			SetTimerAlarm(action.Type, action.Alarm);
		}
		
		private void OnTimerPlayAlarm(IMessage message) => _presenter.PlayAlarm();
		
		private void OnTimerSetDisplay(IMessage message)
		{
			var action = (TimerSetDisplay) message.Sender;
			SetWidgetActive(action.Type, action.Display);
		}
		
		private void OnTimerSetPosition(IMessage message)
		{
			var action = (TimerSetPosition) message.Sender;
			SetTimerPosition(action.Type, action.X, action.Y);
		}
		
		private void OnTimerSetColor(IMessage message)
		{
			var action = (TimerSetColor) message.Sender;
			SetTimerColor(action.Type, action.Color);
		}
		
		private void OnTimerPaused(IMessage message)
		{
			var action = (TimerPause) message.Sender;
			SetTimerPaused(action.Type, action.Paused);
		}
		
		private void OnTimerReset(IMessage message)
		{
			var action = (TimerReset) message.Sender;
			SetTimerActive(action.Type, false);
		}
		
		private void OnTimerSet(IMessage message)
		{
			var action = (TimerSet) message.Sender;
			var type = action.Type;
			SetTimerActive(type, true, action.Time);
			SetTimerAlarm(type, action.Alarm);
			SetWidgetActive(type, action.Display);
		}
		
		private void OnStopwatchSetPosition(IMessage message)
		{
			var action = (StopwatchSetPosition) message.Sender;
			SetTimerPosition(TimerType.Stopwatch, action.X, action.Y);
		}
		
		private void OnStopwatchSetColor(IMessage message)
		{
			var action = (StopwatchSetColor) message.Sender;
			SetTimerColor(TimerType.Stopwatch, action.Color);
		}

		private void OnStopwatchStarted(IMessage message)
		{
			var action = (StopwatchStart) message.Sender;
			SetTimerActive(TimerType.Stopwatch, true);
			SetWidgetActive(TimerType.Stopwatch, action.Display);
		}
		
		private void OnStopwatchPaused(IMessage message) => SetTimerPaused(TimerType.Stopwatch, true);
		
		private void OnStopwatchReset(IMessage message) => SetTimerActive(TimerType.Stopwatch, false);
		
		private void OnStopwatchEdited(IMessage message)
		{
			var action = (StopwatchEdit) message.Sender;
			SetWidgetActive(TimerType.Stopwatch, action.Display);
		}
		
		private void OnStopwatchLap(IMessage message) => AddStopwatchLap();
		
		private void OnStopwatchLapRemove(IMessage message)
		{
			var action = (StopwatchLapRemove) message.Sender;
			RemoveStopwatchLapAt(action.Index);
		}
		
		private void OnStopwatchLapReset(IMessage message) => RemoveAllStopwatchLaps();
		
		private void OnScreenLocked(IMessage message)
		{
			if (_isHiddenByScreensaver)
				return;
			
			if (_applicationState.IsScreenLocked)
				_presenter?.HideAsync().Forget();
			else
				_presenter?.ShowAsync().Forget();
		}
		
		private void OnScreensaverActivity(IMessage message)
		{
			var active = (bool) message.Data;
			
			if (active)
			{
				if (!_applicationState.IsScreenLocked)
					_presenter?.HideAsync().Forget();
				
				_isHiddenByScreensaver = true;
			}
			else
			{
				_isHiddenByScreensaver = false;
				
				if (!_applicationState.IsScreenLocked)
					_presenter?.ShowAsync().Forget();
			}
		}
		
		private void OnApplicationLoadingCompleted(IMessage message)
		{
			MessageDispatcher.RemoveListener(MessageType.AppLoadComplete, OnApplicationLoadingCompleted);
			InitializeAsync().Forget();
		}
	}
}