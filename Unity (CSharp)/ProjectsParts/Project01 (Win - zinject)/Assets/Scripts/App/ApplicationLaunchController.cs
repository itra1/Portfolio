using System;
using System.Threading;
using App.Parsing;
using com.ootii.Messages;
using Core.Base;
using Core.Messages;
using Core.Network.Http;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Core.Options;
using Core.Options.Offsets;
using Cysharp.Threading.Tasks;
using Installers;
using UnityEngine;
using Vuplex.WebView;
using Zenject;
using Debug = Core.Logging.Debug;

namespace App
{
	/// <summary>
	/// Устаревшее название - "GameManager"
	/// </summary>
	public class ApplicationLaunchController : IApplicationLaunchController, IDisposable
	{
		private readonly DiContainer _container;
		private readonly ICommandLineArgumentsParser _argumentsParser;
		private readonly IApplicationOptionsInfo _options;
#if UNITY_EDITOR
		private readonly IApplicationOptionsSetter _optionsSetter;
#endif
		private readonly IScreenOffsetsSetter _screenOffsets;
		private readonly IAuthorization _authorization;
		private readonly IOutgoingStateController _outgoingState;
		private readonly CancellationTokenSource _disposeCancellationTokenSource;

		public ApplicationLaunchController(DiContainer container,
			ICommandLineArgumentsParser argumentsParser,
			IApplicationOptionsInfo options,
#if UNITY_EDITOR
			IApplicationOptionsSetter optionsSetter,
#endif
			IScreenOffsetsSetter screenOffsets,
			IAuthorization authorization,
			IOutgoingStateController outgoingState)
		{
			_container = container;
			_argumentsParser = argumentsParser;
			_options = options;
#if UNITY_EDITOR
			_optionsSetter = optionsSetter;
#endif
			_screenOffsets = screenOffsets;
			_authorization = authorization;
			_outgoingState = outgoingState;
			_disposeCancellationTokenSource = new CancellationTokenSource();

			Configure();

			StartupAsync().Forget();
		}

		public void Dispose()
		{
			if (!_disposeCancellationTokenSource.IsCancellationRequested)
			{
				_disposeCancellationTokenSource.Cancel();
				_disposeCancellationTokenSource.Dispose();
			}
		}

		private void Configure()
		{
			Application.targetFrameRate = 30;
			Application.runInBackground = true;

			Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
			Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
			Application.SetStackTraceLogType(LogType.Assert, StackTraceLogType.ScriptOnly);
			Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
			Application.SetStackTraceLogType(LogType.Exception, StackTraceLogType.Full);
		}

		private async UniTask StartupAsync()
		{
			try
			{
				var cancellationToken = _disposeCancellationTokenSource.Token;

				_outgoingState.Context.SetSystemInfo(Application.version, Guid.NewGuid().ToString(), Application.buildGUID);

				await _argumentsParser.ParseAsync(cancellationToken);

#if UNITY_EDITOR
				ForceOptionsToDefault();
				if (!_options.QtBrowser)
					ForceWebViewToConfigure();
#endif
				if (!_options.QtBrowser)
					Web.SetStorageEnabled(false);

				if (!_options.IsSumAdaptiveModeActive)
				{
					_container.Install<ExtendedElementAddonsInstaller>();
					_container.Install<ExtendedElementsInstaller>();
				}

				await WaitForLateInitializationToComplete(cancellationToken);

				_screenOffsets.Apply();

				if (_options.IsManagersLogEnabled)
					Debug.Log(_options.GetInfo());

				MessageDispatcher.SendMessage(MessageType.AppInitialize);

				await UniTask.NextFrame(cancellationToken);

				MessageDispatcher.SendMessage(MessageType.AppStart);

				if (string.IsNullOrEmpty(_options.ServerToken))
					_authorization.Authorize();
				else
					MessageDispatcher.SendMessage(MessageType.AuthorizationConfirm, EnumMessageDelay.ONE_SECOND);
			}
			catch (Exception exception) when (exception is not OperationCanceledException)
			{
				Debug.LogException(exception);
			}
		}

#if UNITY_EDITOR
		private void ForceWebViewToConfigure()
		{
			StandaloneWebView.SetIgnoreCertificateErrors(true);

			StandaloneWebView.SetCommandLineArguments("--disable-gpu-sandbox");
			StandaloneWebView.SetCommandLineArguments("--disable-gpu");
			StandaloneWebView.SetCommandLineArguments("--disable-extensions");
			StandaloneWebView.SetCommandLineArguments("--no-proxy-server");
			StandaloneWebView.SetCommandLineArguments("--disable-gpu-rasterization");
			StandaloneWebView.SetCommandLineArguments("--disable-gpu-vsync");
			StandaloneWebView.SetCommandLineArguments("--no-sandbox");
			StandaloneWebView.SetCommandLineArguments("--disable-local-storage");
			StandaloneWebView.SetCommandLineArguments("--disable-site-isolation-trials");
			StandaloneWebView.SetCommandLineArguments("--enable-early-process-singleton");
			StandaloneWebView.SetCommandLineArguments("--disable-accelerated-2d-canvas");
			StandaloneWebView.SetCommandLineArguments("--disable-experimental-canvas-features");
			StandaloneWebView.SetCommandLineArguments("--disable-checker-imaging");
			StandaloneWebView.SetCommandLineArguments("--ui-disable-zero-copy");
			StandaloneWebView.SetCommandLineArguments("--disable-zero-copy");
			StandaloneWebView.SetCommandLineArguments("--disable-webgl-image-chromium");
			StandaloneWebView.SetCommandLineArguments("--lang=ru");
			StandaloneWebView.SetCommandLineArguments("--autoplay-policy");
		}

		private void ForceOptionsToDefault()
		{
			_optionsSetter.IsConsoleEnabled = true;
			_optionsSetter.IsManagersLogEnabled = true;
			_optionsSetter.IsPdfRenderingAsPicture = true;
			_optionsSetter.IsLoadingIndicatorEnabled = true;
			_optionsSetter.IsFpsCounterEnabled = true;
			_optionsSetter.IsStateSendingAllowed = true;
			_optionsSetter.IsPresetRestoredAtStart = true;
			//_optionsSetter.IsMSOfficeUsed = false;
			_optionsSetter.IsUnlockedAtStart = true;
			//_argumentsParser.Handle(typeof(MSOffice));
			_argumentsParser.Handle(typeof(App.Parsing.Handlers.QtBrowser));
		}
#endif

		private async UniTask WaitForLateInitializationToComplete(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var items = _container.ResolveAll<ILateInitialized>();

			while (true)
			{
				var i = 0;

				while (i < items.Count)
				{
					if (items[i].IsInitialized)
						items.RemoveAt(i);
					else
						i++;
				}

				if (items.Count == 0)
					break;

				await UniTask.NextFrame(cancellationToken);
			}
		}
	}
}