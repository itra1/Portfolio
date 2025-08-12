using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Core.Base;
using Core.Materials.Attributes;
using Core.Materials.Data;
using Core.Materials.Loading.Loader.Command;
using Core.Materials.Loading.Loader.Command.Factory;
using Core.Materials.Loading.Loader.Info;
using Cysharp.Threading.Tasks;
using Debug = Core.Logging.Debug;

namespace Core.Materials.Loading.Loader
{
    public class MaterialDataLoader : IMaterialDataLoader, ILateInitialized, IDisposable
    {
        private const string UrlFormat = "{0}/{1}{2}";
        
        private readonly IMaterialDataLoadingCommandFactory _factory;
        private readonly IDictionary<Type, string> _materialUrlsByType;
        private readonly IList<MaterialDataLoadingCommand> _commands;
        private readonly CancellationTokenSource _disposeCancellationTokenSource;
        
        public bool IsInitialized { get; private set; }
        public bool InProgress => _commands.Count > 0;
        
        public MaterialDataLoader(IMaterialDataLoadingCommandFactory factory)
        {
            _factory = factory;
            _materialUrlsByType = new Dictionary<Type, string>();
            _commands = new List<MaterialDataLoadingCommand>();
            _disposeCancellationTokenSource = new CancellationTokenSource();
            
            CollectLoadInfoListAsync().Forget();
        }
        
        public async UniTask<MaterialDataLoadingResult> LoadAsync(MaterialDataLoadingInfo info)
        {
            IReadOnlyList<MaterialData> materials = null;
            string errorMessage = null;
            
            Load(info, result => materials = result, result => errorMessage = result);
            
            await UniTask.WaitUntil(() => materials != null || errorMessage != null || _disposeCancellationTokenSource.IsCancellationRequested);
            
            return new MaterialDataLoadingResult(materials, errorMessage);
        }
        
        public void Load(MaterialDataLoadingInfo info, Action<IReadOnlyList<MaterialData>> onCompleted = null, Action<string> onFailure = null)
        {
            if (_disposeCancellationTokenSource.IsCancellationRequested)
                return;
            
            if (!_materialUrlsByType.TryGetValue(info.Type, out var url))
            {
                onFailure?.Invoke($"Missing attribute MaterialDataLoader for material with info {info}. This material data is ignored to load.");
                return;
            }
            
            var command = _commands.FirstOrDefault(e => e.Info.Equals(info));
            
            if (command != null)
            {
                if (onCompleted != null)
                    command.Completed += onCompleted;
                if (onFailure != null)
                    command.Failed += onFailure;
            }
            else
            {
                command = _factory.Create();
                
                if (command == null)
                {
                    onFailure?.Invoke($"An error occurred while trying to create a material data loading command with info {info}. URL {url}");
                    return;
                }
                
                command.Disposed += OnCommandDisposed;
                
                _commands.Add(command);
                
                command.Initialize(info, string.Format(UrlFormat, url, info.Id, info.UrlPostfix), onCompleted, onFailure, _disposeCancellationTokenSource);
                command.Execute();
            }
        }
        
        public void Dispose()
        {
            if (!_disposeCancellationTokenSource.IsCancellationRequested)
            {
                _disposeCancellationTokenSource.Cancel();
                _disposeCancellationTokenSource.Dispose();
            }
            
            _commands.Clear();
            _materialUrlsByType.Clear();
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
                    
                    var materialDataTypeBase = typeof(MaterialData);
                    
                    foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        
                        if (!type.IsClass || !materialDataTypeBase.IsAssignableFrom(type))
                            continue;
                        
                        var attribute = type.GetCustomAttribute<MaterialDataLoaderAttribute>(false);
                        
                        if (attribute == null)
                            continue;
                        
                        _materialUrlsByType.Add(type, attribute.Url);
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

        private void OnCommandDisposed(MaterialDataLoadingCommand command)
        {
            command.Disposed -= OnCommandDisposed;
            
            _commands.Remove(command);
            _factory.Remove(command);
        }
    }
}