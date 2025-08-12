using System;
using System.Collections.Concurrent;
using System.Threading;
using Components.Pipes;
using Core.Elements.Windows.WebView.Data;
using Cysharp.Threading.Tasks;
using Environment.Netsoft.WebView.Actions;
using Environment.Netsoft.WebView.Data;
using Environment.Netsoft.WebView.Deserializers;
using Environment.Netsoft.WebView.Serializer;
using Environment.Netsoft.WebView.Settings;
using Pipes;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Environment.Netsoft.WebView
{
	public class WebViewApplication : IWebViewApplication
	{
		public UnityEvent OnApplicationRestart { get; } = new();
		public UnityEvent<WebViewStateData> OnStateChange { get; } = new();

		private string _id;
		private IWebViewProcess _process;
		private IPipeServerFactory _pipeFactory;
		private IActionSerializer _serializer;
		private IActionDeserializer _deserializer;
		private INetsofrWebViewSettings _settings;
		private IPipeServer _server;
		private IPingPong _pingPong;
		private INsWebViewMaterialData _materialData;
		private WebViewState _startStateData;
		private bool _isClosed = false;
		private IApplicationRunOptions _options;

		private ConcurrentQueue<INativePackage> _requests;
		private CancellationTokenSource _requestCancellationTokenSource;

		public string Id => _id;
		public bool Connected => _server is { Connected: true };
		public WebViewStateData StateData { get; set; }

		public WebViewApplication()
		{
			_process = new WebViewProcess();
			_pingPong = new PingPong(this);

			_pingPong.OnStateDataChange = (data) =>
			{
				StateData = data.State;
				OnStateChange?.Invoke(StateData);
			};
		}

		[Inject]
		private void Initialize(IPipeServerFactory pipeFactory, IActionSerializer serializer, IActionDeserializer deserializer, INetsofrWebViewSettings settings)
		{
			_pipeFactory = pipeFactory;
			_serializer = serializer;
			_deserializer = deserializer;
			_settings = settings;

			_server = _pipeFactory.Create();
			_server.OnPipeIsBroken.AddListener(() => PipeIsBroken().Forget());

			_requests = new();

			_id = System.Guid.NewGuid().ToString();

			_options = new ApplicationRunOptions()
			{
				Id = _id,
				PipeTimeout = _settings.PipeTimeout,
				InitializeTimeout = _settings.InitializeTimeout
			};
		}

		public void Create(INsWebViewMaterialData materialData, WebViewState stateData)
		{
			_materialData = materialData;
			_startStateData = stateData;
		}

		public async UniTask<bool> RunApplication()
		{
			_ = _process.CreateProcess(_options);
			_server.Create(_id);

			_ = await _server.ConnectAsync();

			_pingPong.Process();
			RequestProcess().Forget();

			Send(ActionsNames.Material, _materialData);
			Send(ActionsNames.State, _startStateData);

			return true;
		}

		private async UniTask RequestProcess()
		{
			_requestCancellationTokenSource = new();

			try
			{
				await UniTask.SwitchToTaskPool();

				while (_server.Connected)
				{

					if (!_requests.TryDequeue(out var package))
					{
						await UniTask.Delay(300);
						_requestCancellationTokenSource.Token.ThrowIfCancellationRequested();
						continue;
					}

					ActionPackage sendPackage = new()
					{
						OutgoingPacket = _serializer.Serialize(package.Action, package.Data)
					};

					var answer = await _server.Request(sendPackage);

					var incomingPack = _deserializer.Deserialize(answer.IncomingPacket);

					package.OnComplete?.Invoke(incomingPack[0].ToString(), (incomingPack.Length > 1 ? incomingPack[1] : null));
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

				_requestCancellationTokenSource?.Dispose();
			}
		}

		public void Send(string action, object data = null, UnityAction<string, object> OnCompleteCallback = null)
		{
			if (!_server.Connected)
				return;

			_requests.Enqueue(new NativePackage()
			{
				Action = action,
				Data = data,
				OnComplete = OnCompleteCallback
			});
		}

		private async UniTask PipeIsBroken()
		{
			if (_isClosed)
				return;
			CloseExistsProcess();
			_ = await RunApplication();
			OnApplicationRestart?.Invoke();
		}

		private void CloseExistsProcess()
		{
			_pingPong.Terminate();
			_ = _server.DisconnectAsync();
			_process.TerminateProcess();
			_requests.Clear();

			if (_requestCancellationTokenSource is { IsCancellationRequested: false })
			{
				_requestCancellationTokenSource.Cancel();
			}
		}

		public void Close()
		{
			_isClosed = true;

			CloseExistsProcess();
		}
	}
}
