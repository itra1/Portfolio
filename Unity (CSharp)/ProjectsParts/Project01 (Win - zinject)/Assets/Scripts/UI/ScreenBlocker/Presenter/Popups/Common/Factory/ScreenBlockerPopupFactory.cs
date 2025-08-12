using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Core.Base;
using Cysharp.Threading.Tasks;
using Settings;
using UI.ScreenBlocker.Presenter.Popups.Base;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;
using Debug = Core.Logging.Debug;

namespace UI.ScreenBlocker.Presenter.Popups.Common.Factory
{
    public class ScreenBlockerPopupFactory : IScreenBlockerPopupFactory, ILateInitialized, IDisposable
    {
        private readonly DiContainer _container;
        private readonly IPrefabSettings _prefabs;
        private readonly IDictionary<Type, GameObject> _prefabsByScreenBlockerType;
        private readonly CancellationTokenSource _disposeCancellationTokenSource;
        
        public bool IsInitialized { get; private set; }
        
        public ScreenBlockerPopupFactory(DiContainer container, IPrefabSettings prefabs)
        {
            _container = container;
            _prefabs = prefabs;
            _prefabsByScreenBlockerType = new Dictionary<Type, GameObject>();
            _disposeCancellationTokenSource = new CancellationTokenSource();
            
            CollectScreenBlockerPopupsAsync().Forget();
        }
        
        public IScreenBlockerPopup Create<TScreenBlockerPopup>(RectTransform parent) 
            where TScreenBlockerPopup : IScreenBlockerPopup
        {
            var type = typeof(TScreenBlockerPopup);
            var typeName = type.Name;
            
            if (!_prefabsByScreenBlockerType.TryGetValue(type, out var prefab))
            {
                Debug.LogError($"No prefab found with component type {typeName} when trying to create a screen blocker popup");
                return null;
            }
            
            var gameObject = Object.Instantiate(prefab, parent);
            
            gameObject.name = typeName;
            gameObject.SetActive(false);
            
            if (!gameObject.TryGetComponent<IScreenBlockerPopup>(out var popup))
            {
                Object.Destroy(gameObject);
                
                Debug.LogError($"No component found with type {typeName} when trying to create a screen blocker popup");
                return null;
            }
            
            _container.Inject(popup);
            
            return popup;
        }
        
        public void Dispose()
        {
            if (!_disposeCancellationTokenSource.IsCancellationRequested)
            {
                _disposeCancellationTokenSource.Cancel();
                _disposeCancellationTokenSource.Dispose();
            }
            
            _prefabsByScreenBlockerType.Clear();
        }
        
        private async UniTaskVoid CollectScreenBlockerPopupsAsync()
        {
            try
            {
                var cancellationToken = _disposeCancellationTokenSource.Token;
                var types = new HashSet<Type>();
                
                await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
                {
                    await UniTask.SwitchToThreadPool();
                    
                    cancellationToken.ThrowIfCancellationRequested();
					
                    var blockerPopupTypeBase = typeof(IScreenBlockerPopup);

                    foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        
                        if (!type.IsClass || !blockerPopupTypeBase.IsAssignableFrom(type))
                            continue;
                        
                        types.Add(type);
                    }
                }
                
                await UniTask.SwitchToMainThread(cancellationToken: _disposeCancellationTokenSource.Token);
                
                foreach (var type in types)
                {
                    if (!_prefabs.TryGetComponent(type, out var component))
                        continue;
                    
                    _prefabsByScreenBlockerType.Add(type, component.gameObject);
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