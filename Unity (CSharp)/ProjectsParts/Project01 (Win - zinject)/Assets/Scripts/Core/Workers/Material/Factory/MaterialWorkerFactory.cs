using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Core.Base;
using Core.Materials.Attributes;
using Core.Materials.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Debug = Core.Logging.Debug;

namespace Core.Workers.Material.Factory
{
	public class MaterialWorkerFactory : IMaterialWorkerFactory, ILateInitialized, IDisposable
	{
		private readonly DiContainer _container;
		private readonly IDictionary<Type, object> _workersByMaterialType;
		private readonly CancellationTokenSource _disposeCancellationTokenSource;
		
		public bool IsInitialized { get; private set; }
		
		public MaterialWorkerFactory(DiContainer container)
		{
			_container = container;
			_workersByMaterialType = new Dictionary<Type, object>();
			_disposeCancellationTokenSource = new CancellationTokenSource();
			
			CollectWorkersAsync().Forget();
		}
		
		public void Dispose()
		{
			if (!_disposeCancellationTokenSource.IsCancellationRequested)
			{
				_disposeCancellationTokenSource.Cancel();
				_disposeCancellationTokenSource.Dispose();
			}
			
			_workersByMaterialType.Clear();
		}

		public bool TryGetWorker<TMaterialWorker>(Type materialType, out TMaterialWorker worker)
		{
			if (_workersByMaterialType.TryGetValue(materialType, out var value) && value is TMaterialWorker w)
			{
				worker = w;
				return true;
			}
			
			worker = default;
			return false;
		}

		private async UniTaskVoid CollectWorkersAsync()
		{
			try
			{
				if (!Thread.CurrentThread.IsBackground && Application.isPlaying)
				{
					var cancellationToken = _disposeCancellationTokenSource.Token;
					
					await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
					{
						await UniTask.SwitchToThreadPool();
						cancellationToken.ThrowIfCancellationRequested();
						CollectWorkers();
					}
				}
				else
				{
					CollectWorkers();
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

		private void CollectWorkers()
		{
			var materialDataTypeBase = typeof(MaterialData);
			
			foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (!type.IsClass || !materialDataTypeBase.IsAssignableFrom(type))
					continue;
				
				var attribute = type.GetCustomAttribute<MaterialDataWorkerAttribute>();
				
				if (attribute == null)
					continue;
				
				var worker = Activator.CreateInstance(attribute.Type);
				
				_workersByMaterialType.Add(type, worker);
				
				_container.Inject(worker);
			}
		}
	}
}