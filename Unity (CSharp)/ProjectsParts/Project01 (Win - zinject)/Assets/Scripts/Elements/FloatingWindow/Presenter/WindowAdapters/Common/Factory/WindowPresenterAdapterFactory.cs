using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Base.Presenter;
using Core.Base;
using Core.Materials.Attributes;
using Core.Materials.Data;
using Cysharp.Threading.Tasks;
using Elements.FloatingWindow.Presenter.WindowAdapters.Base;
using UnityEngine;
using Zenject;
using IPrefabProvider = UI.Prefabs.IPrefabProvider;
using Object = UnityEngine.Object;

namespace Elements.FloatingWindow.Presenter.WindowAdapters.Common.Factory
{
    public class WindowPresenterAdapterFactory : IWindowPresenterAdapterFactory, ILateInitialized, IDisposable
    {
        private readonly DiContainer _container;
        private readonly IPrefabProvider _prefabs;
        private readonly INonRenderedContainer _nonRenderedContainer;
        private readonly IDictionary<Type, Type> _presenterAdapterTypesByMaterialType;
        private readonly CancellationTokenSource _disposeCancellationTokenSource;
        
        public bool IsInitialized { get; private set; }
        
        public WindowPresenterAdapterFactory(DiContainer container,
	        IPrefabProvider prefabs,
	        INonRenderedContainer nonRenderedContainer)
        {
            _container = container;
            _prefabs = prefabs;
            _nonRenderedContainer = nonRenderedContainer;
            _presenterAdapterTypesByMaterialType = new Dictionary<Type, Type>();
            _disposeCancellationTokenSource = new CancellationTokenSource();
            
            CollectPresenterAdapterTypesAsync().Forget();
        }
        
        public IWindowPresenterAdapter Create(MaterialData material, RectTransform parent)
        {
	        if (material == null)
	        {
		        Debug.LogError("Material is null when trying to create a window presenter adapter");
		        return default;
	        }
			
	        var materialType = material.GetType();
			
	        if (!_presenterAdapterTypesByMaterialType.TryGetValue(materialType, out var presenterAdapterType))
	        {
		        Debug.LogError($"Presenter adapter type is not found by material {material}");
		        return default;
	        }
			
	        var prefab = _prefabs.GetPrefabOf(presenterAdapterType);
			
	        if (prefab == null)
	        {
		        Debug.LogError($"Prefab is not found by presenter adapter type: {presenterAdapterType.Name}");
		        return default;
	        }
			
	        if (parent == null)
	        {
		        Debug.LogError($"An attempt was detected to assign a null parent to the {presenterAdapterType.Name}");
		        return default;
	        }
			
	        var gameObject = Object.Instantiate(prefab, parent);
			
	        gameObject.SetActive(false);
			
	        var presenterAdapter = gameObject.GetComponent<IWindowPresenterAdapter>();
			
	        _container.Inject(presenterAdapter);
			
	        if (parent.TryGetComponent<IParentArea>(out var parentArea))
		        presenterAdapter.SetParentArea(parentArea);
			
	        if (presenterAdapter is INonRenderedCapable nonRenderedCapablePresenter)
		        nonRenderedCapablePresenter.SetNonRenderedContainer(_nonRenderedContainer);
			
	        presenterAdapter.SetParentOnInitialize(parent);
			
	        return presenterAdapter;
        }
        
        public void Dispose()
		{
			if (!_disposeCancellationTokenSource.IsCancellationRequested)
			{
				_disposeCancellationTokenSource.Cancel();
				_disposeCancellationTokenSource.Dispose();
			}
			
			_presenterAdapterTypesByMaterialType.Clear();
		}
		
		private async UniTaskVoid CollectPresenterAdapterTypesAsync()
		{
			try
			{
				var cancellationToken = _disposeCancellationTokenSource.Token;
				
				await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
				{
					await UniTask.SwitchToThreadPool();
					
					var presenterAdapterTypeBase = typeof(IWindowPresenterAdapter);
					
					foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
					{
						cancellationToken.ThrowIfCancellationRequested();
						
						if (!type.IsClass || type.IsAbstract || !presenterAdapterTypeBase.IsAssignableFrom(type))
							continue;
						
						var materialDataAttribute = type.GetCustomAttribute<MaterialDataAttribute>();
						
						if (materialDataAttribute == null)
							continue;
						
						_presenterAdapterTypesByMaterialType.Add(materialDataAttribute.Type, type);
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
    }
}