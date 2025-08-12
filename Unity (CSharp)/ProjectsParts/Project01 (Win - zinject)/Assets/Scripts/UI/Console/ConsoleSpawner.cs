using System;
using com.ootii.Messages;
using Core.Messages;
using Core.Options;
using UI.Canvas.Presenter;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI.Console
{
    public class ConsoleSpawner : IConsoleSpawner, IDisposable
    {
        private readonly IApplicationOptions _options;
        
        private GameObject _prefab;
        
        public ConsoleSpawner(IApplicationOptions options, ICanvasPresenter root)
        {
            _options = options;
            _prefab = root.ConsolePrefab;
            
            MessageDispatcher.AddListener(MessageType.AppInitialize, OnApplicationInitialized);
        }
        
        public void Dispose()
        {
            MessageDispatcher.RemoveListener(MessageType.AppInitialize, OnApplicationInitialized);
            _prefab = null;
        }
        
        private void Spawn() => Object.Instantiate(_prefab).name = _prefab.name;
        
        private void OnApplicationInitialized(IMessage message)
        {
            MessageDispatcher.RemoveListener(MessageType.AppInitialize, OnApplicationInitialized);
            
            if (_options.IsConsoleEnabled)
                Spawn();
        }
    }
}