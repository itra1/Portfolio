using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using com.ootii.Messages;
using Core.Base;
using Core.Common.Consts;
using Core.Configs;
using Core.Configs.Consts;
using Core.Materials.Attributes;
using Core.Materials.Loading.AutoPreloader.Info;
using Core.Materials.Loading.AutoPreloader.Types;
using Core.Materials.Parsing;
using Core.Materials.Storage;
using Core.Messages;
using Core.Network.Http;
using Core.Options;
using Core.Workers.Material.Coordinator;
using Cysharp.Threading.Tasks;
using Debug = Core.Logging.Debug;

namespace Core.Materials.Loading.AutoPreloader
{
	/// <summary>
	/// Устаревшее название - "MaterialManager"
	/// Обеспечивает предварительную загрузку всех доступных материалов после авторизации
	/// </summary>
	public class AutoMaterialDataPreloader : IAutoMaterialDataPreloader, ILateInitialized, IDisposable
	{
		private readonly IConfig _config;
		private readonly IApplicationOptions _options;
		private readonly IAutoPreloadedMaterialDataTypes _materialDataTypes;
		private readonly IHttpRequest _request;
		private readonly IMaterialDataParser _parser;
		private readonly IMaterialDataStorage _materials;
		private readonly IMaterialWorkerCoordinator _workerCoordinator;
		private readonly IList<MaterialDataTypeLoadingInfo> _loadingInfoList;
		private readonly ConcurrentQueue<(MaterialDataTypeLoadingInfo, string)> _loadedDataQueue;
		private readonly CancellationTokenSource _disposeCancellationTokenSource;
		
		private int _maxRequestAttempts;
		private bool _inProgress;
		
		public bool IsInitialized { get; private set; }
		public bool IsLoadingCompleted { get; private set; }
		
		public AutoMaterialDataPreloader(IConfig config,
			IApplicationOptions options,
			IAutoPreloadedMaterialDataTypes materialDataTypes,
			IHttpRequest request,
			IMaterialDataParser parser,
			IMaterialDataStorage materials,
			IMaterialWorkerCoordinator workerCoordinator)
		{
			_config = config;
			_options = options;
			_materialDataTypes = materialDataTypes;
			_request = request;
			_parser = parser;
			_materials = materials;
			_workerCoordinator = workerCoordinator;
			_loadingInfoList = new List<MaterialDataTypeLoadingInfo>();
			_loadedDataQueue = new ConcurrentQueue<(MaterialDataTypeLoadingInfo, string)>();
			_disposeCancellationTokenSource = new CancellationTokenSource();
			
			if (!config.IsLoaded)
				MessageDispatcher.AddListener(MessageType.ConfigLoad, OnConfigLoaded);
			else
				AttemptToParseMaxRequestAttempts();
			
			CollectLoadInfoListAsync().Forget();
			
			MessageDispatcher.AddListener(MessageType.SocketOpen, OnSocketConnected);
		}
		
		public void Dispose()
		{
			MessageDispatcher.RemoveListener(MessageType.ConfigLoad, OnConfigLoaded);
			MessageDispatcher.RemoveListener(MessageType.SocketOpen, OnSocketConnected);
			
			if (!_disposeCancellationTokenSource.IsCancellationRequested)
			{
				_disposeCancellationTokenSource.Cancel();
				_disposeCancellationTokenSource.Dispose();
			}
			
			_inProgress = false;
			
			IsLoadingCompleted = false;
			
			_loadedDataQueue.Clear();
			_loadingInfoList.Clear();
			
			_maxRequestAttempts = 0;
		}
		
		private bool StartLoading()
		{
			if (_disposeCancellationTokenSource.IsCancellationRequested || !IsInitialized || _inProgress)
				return false;
			
			_inProgress = true;
			
			if (_options.IsManagersLogEnabled)
				Debug.Log("Preloading of material data list has been started");
			
			MessageDispatcher.SendMessage(MessageType.AppLoadStart);
			
			if (_loadingInfoList.Count > 0)
			{
				var i = 0;
				
				while (i < _loadingInfoList.Count)
				{
					if (TryToLoad(_loadingInfoList[i], 0))
						i++;
				}
			}
			else
			{
				OnAllLoadedDataParsingCompleted();
			}
			
			return true;
		}
		
		private bool TryToLoad(MaterialDataTypeLoadingInfo info, int currentRequestAttempt)
		{
			if (currentRequestAttempt < _maxRequestAttempts)
			{
				if (_options.IsManagersLogEnabled)
				{
					if (currentRequestAttempt == 0)
						Debug.Log($"Preloading of all materials with type {info.Type.Name} has been started");
					else 
						Debug.LogWarning($"Preloading of all materials with type {info.Type.Name} has been started again (attempt: {currentRequestAttempt})");
				}
				
				_request.Request(info.Url,
					result => 
					{
						if (_disposeCancellationTokenSource.IsCancellationRequested)
							return;
						
						if (_options.IsManagersLogEnabled)
							Debug.Log($"Preloading of all materials with type {info.Type.Name} has been completed");
						
						_loadedDataQueue.Enqueue((info, result));
						
						if (_loadingInfoList.Count == _loadedDataQueue.Count)
							ParseAllLoadedDataAsync().Forget();
					},
					_ => RetryToLoadAsync(info, currentRequestAttempt).Forget());
				
				return true;
			}
			
			Debug.LogError($"The maximum number of attempts to load material data with type {info.Type.Name} has been exceeded. This material data type is excluded from the preload list.");
            
			_loadingInfoList.Remove(info);
            
			if (_loadingInfoList.Count == _loadedDataQueue.Count)
				ParseAllLoadedDataAsync().Forget();
			
			return false;
		}

		private async UniTaskVoid RetryToLoadAsync(MaterialDataTypeLoadingInfo info, int currentRequestAttempt)
		{
			if (_disposeCancellationTokenSource.IsCancellationRequested)
				return;
			
			Debug.LogWarning($"Preloading of all materials with type {info.Type.Name} has been failed");
			
			try
			{
				await UniTask.Delay(TimeSpan.FromSeconds(1.0), cancellationToken: _disposeCancellationTokenSource.Token);
			}
			catch (OperationCanceledException)
			{
				return;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return;
			}
			
			TryToLoad(info, ++currentRequestAttempt);
		}
		
		private async UniTaskVoid ParseAllLoadedDataAsync()
		{
			if (_options.IsManagersLogEnabled)
				Debug.Log("Preloading of material data list has been completed");
			
			if (_loadedDataQueue.Count == 0)
				return;

			if (_options.IsManagersLogEnabled)
				Debug.Log("Parsing of material data list has been started");
			
			try
			{
				var cancellationToken = _disposeCancellationTokenSource.Token;
				
				await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
				{
					await UniTask.SwitchToThreadPool();
					
					cancellationToken.ThrowIfCancellationRequested();
					
					var subtasks = new UniTask[_loadedDataQueue.Count];
					
					for (var i = 0; i < subtasks.Length; i++)
						subtasks[i] = ParseLoadedDataAsync();
					
					await UniTask.WhenAll(subtasks);
					
					cancellationToken.ThrowIfCancellationRequested();
					
					foreach (var material in _materials.GetList())
						_workerCoordinator.PerformActionAfterAddingToStorageOf(material);
				}
			}
			catch (OperationCanceledException)
			{
				return;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			finally
			{
				if (Thread.CurrentThread.IsBackground)
					await UniTask.SwitchToMainThread();
			}
			
			OnAllLoadedDataParsingCompleted();
		}

		private async UniTask ParseLoadedDataAsync()
		{
			try
			{
				var cancellationToken = _disposeCancellationTokenSource.Token;
				
				await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
				{
					await UniTask.SwitchToThreadPool();
					
					cancellationToken.ThrowIfCancellationRequested();
					
					(MaterialDataTypeLoadingInfo, string) loadedData;
					
					while (!_loadedDataQueue.TryDequeue(out loadedData)) { }
					
					cancellationToken.ThrowIfCancellationRequested();
					
					ParseLoadedData(loadedData.Item1, loadedData.Item2);
				}
			}
			catch (Exception exception) when (exception is not OperationCanceledException)
			{
				Debug.LogException(exception);
			}
		}

		private void ParseLoadedData(MaterialDataTypeLoadingInfo info, string rawData)
		{
			if (_options.IsManagersLogEnabled)
				Debug.Log($"Parsing of all materials with type {info.Type.Name} has been started");
            
			try
			{
				_parser.Parse(info, rawData);
			}
			catch (Exception exception)
			{
				Debug.LogError($"Parsing of all materials with type {info.Type.Name} has been aborted. Error: {exception.Message}{Environment.NewLine}{exception.StackTrace}");
				return;
			}
            
			if (_options.IsManagersLogEnabled)
				Debug.Log($"Parsing of all materials with type {info.Type.Name} has been completed");
		}
		
		private void AttemptToParseMaxRequestAttempts()
		{
			if (_config.TryGetValue(ConfigKey.MaxMaterialRequestAttempts, out var rawValue) && int.TryParse(rawValue, out var value))
				_maxRequestAttempts = value;
			else 
				_maxRequestAttempts = DefaultValue.MaxMaterialRequestAttempts;
		}
		
		private async UniTaskVoid CollectLoadInfoListAsync()
		{
			try
			{
				var cancellationToken = _disposeCancellationTokenSource.Token;
				
				await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
				{
					await UniTask.SwitchToThreadPool();
					
					cancellationToken.ThrowIfCancellationRequested();
					
					foreach (var type in _materialDataTypes)
					{
						cancellationToken.ThrowIfCancellationRequested();
						
						var attribute = type.GetCustomAttribute<MaterialDataLoaderAttribute>(false);
						
						_loadingInfoList.Add(new MaterialDataTypeLoadingInfo(type, attribute.Url));
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
				
				IsInitialized = true;
			}
		}
		
		private void OnAllLoadedDataParsingCompleted()
		{
			_inProgress = false;
			
			IsLoadingCompleted = true;
			
			if (_options.IsManagersLogEnabled)
				Debug.Log("Parsing of material data list has been completed");
			
			MessageDispatcher.SendMessage(MessageType.AutoMaterialDataPreloadComplete);
		}
		
		private void OnConfigLoaded(IMessage message)
		{
			MessageDispatcher.RemoveListener(MessageType.ConfigLoad, OnConfigLoaded);
			AttemptToParseMaxRequestAttempts();
		}
		
		private void OnSocketConnected(IMessage message)
		{
			MessageDispatcher.RemoveListener(MessageType.SocketOpen, OnSocketConnected);
			
			if (!StartLoading())
				Debug.LogError("Unable to start preloading material data list");
		}
	}
}