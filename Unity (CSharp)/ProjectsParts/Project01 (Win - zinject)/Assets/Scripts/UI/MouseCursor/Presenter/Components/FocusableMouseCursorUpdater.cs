using System;
using System.Threading;
using Core.Configs;
using Core.Configs.Consts;
using Cysharp.Threading.Tasks;
using Settings.Data;
using UnityEngine;
using Zenject;
using Debug = Core.Logging.Debug;

namespace UI.MouseCursor.Presenter.Components
{
    public class FocusableMouseCursorUpdater : MouseCursorUpdaterBase
	{
		private const float DefaultFocusTimeout = 3.0f;
		private const float DoubleClickDurationMax = 0.3f;
		
		private float _focusTimeout;
		private int _clickCount;
		private float _lastClickTime;
		
		private CancellationTokenSource _disableCancellationTokenSource;
		private CancellationTokenSource _focusTimeoutCancellationTokenSource;
		private CancellationTokenSource _doubleClickTimeoutCancellationTokenSource;
		
		protected override void Awake()
		{
			base.Awake();
			
			var config = ProjectContext.Instance.Container.Resolve<IConfig>();

			if (config.TryGetValue(ConfigKey.CursorFocusTimeout, out var rawValue)
			    && !string.IsNullOrEmpty(rawValue)
			    && float.TryParse(rawValue, out var value))
			{
				_focusTimeout = value;
			}
			else
			{
				_focusTimeout = DefaultFocusTimeout;
			}
		}
		
		protected void OnEnable() => UpdateAsync().Forget();
		
		protected override void OnDisable()
		{
			_disableCancellationTokenSource?.Cancel();
			_focusTimeoutCancellationTokenSource?.Cancel();
			_doubleClickTimeoutCancellationTokenSource?.Cancel();
			
			base.OnDisable();
		}
		
		private async UniTask UpdateAsync()
		{
			using var cancellationTokenSource = new CancellationTokenSource();
			
			_disableCancellationTokenSource = cancellationTokenSource;
			
			try
			{
				while (!cancellationTokenSource.IsCancellationRequested)
				{
					switch (MouseCursorAccess.CurrentState)
					{
						case MouseCursorState.Arrow:
						{
							if (Input.GetMouseButtonDown(0))
								SetFocusTimeoutAsync().Forget();
						
							break;
						}
						case MouseCursorState.Focus:
						{
							if (Input.GetMouseButtonDown(0))
							{
								_clickCount++;
							
								if (_clickCount == 1)
								{
									_lastClickTime = Time.time;
								
									SetDoubleClickTimeoutAsync().Forget();
								}
								else
								{
									var doubleClickDuration = Time.time - _lastClickTime;

									ResetDoubleClickParameters();

									if (doubleClickDuration <= DoubleClickDurationMax)
										MouseCursorAccess.Remove(this);
								}
							}

							break;
						}
					}
				
					if (Input.GetMouseButtonUp(0))
						CancelFocusTimeoutIfExists();
				
					await UniTask.NextFrame(cancellationTokenSource.Token);
				}
			}
			catch (Exception exception) when (exception is not OperationCanceledException)
			{
				Debug.LogException(exception);
			}
			finally
			{
				if (_disableCancellationTokenSource == cancellationTokenSource)
					_disableCancellationTokenSource = null;
			}
		}
		
		private async UniTask SetFocusTimeoutAsync()
		{
			CancelFocusTimeoutIfExists();
			
			using var cancellationTokenSource = new CancellationTokenSource();
			
			_focusTimeoutCancellationTokenSource = cancellationTokenSource;
			
			try
			{
				await SetFocusTimeoutAsync(cancellationTokenSource);
			}
			catch (Exception exception) when (exception is not OperationCanceledException)
			{
				Debug.LogException(exception);
			}
			finally
			{
				if (_focusTimeoutCancellationTokenSource == cancellationTokenSource)
					_focusTimeoutCancellationTokenSource = null;
			}
		}
		
		private async UniTask SetFocusTimeoutAsync(CancellationTokenSource cancellationTokenSource)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(_focusTimeout), cancellationToken: cancellationTokenSource.Token);
			SetFocus();
		}

		private void CancelFocusTimeoutIfExists() => _focusTimeoutCancellationTokenSource?.Cancel();
		
		private void SetFocus()
		{
			CancelFocusTimeoutIfExists();
			MouseCursorAccess.Set(MouseCursorState.Focus, this);
		}

		private async UniTask SetDoubleClickTimeoutAsync()
		{
			CancelDoubleClickTimeoutIfExists();
			
			using var cancellationTokenSource = new CancellationTokenSource();
			
			_doubleClickTimeoutCancellationTokenSource = cancellationTokenSource;
			
			try
			{
				await SetDoubleClickTimeoutAsync(cancellationTokenSource);
			}
			catch (Exception exception) when (exception is not OperationCanceledException)
			{
				Debug.LogException(exception);
			}
			finally
			{
				if (_doubleClickTimeoutCancellationTokenSource == cancellationTokenSource)
					_doubleClickTimeoutCancellationTokenSource = null;
			}
		}

		private async UniTask SetDoubleClickTimeoutAsync(CancellationTokenSource cancellationTokenSource)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(DoubleClickDurationMax), cancellationToken: cancellationTokenSource.Token);
			ResetDoubleClickParameters();
		}

		private void CancelDoubleClickTimeoutIfExists() => _doubleClickTimeoutCancellationTokenSource?.Cancel();

		private void ResetDoubleClickParameters()
		{
			CancelDoubleClickTimeoutIfExists();
			
			_clickCount = 0;
			_lastClickTime = 0;
		}
	}
}