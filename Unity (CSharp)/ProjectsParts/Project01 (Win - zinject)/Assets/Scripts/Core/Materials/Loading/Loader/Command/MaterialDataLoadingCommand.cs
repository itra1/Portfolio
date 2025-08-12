using System;
using System.Collections.Generic;
using System.Threading;
using Core.Materials.Data;
using Core.Materials.Loading.Loader.Info;
using Core.Materials.Parsing;
using Core.Materials.Storage;
using Core.Network.Http;
using Core.Options;
using Core.Workers.Material.Coordinator;
using Cysharp.Threading.Tasks;
using Debug = Core.Logging.Debug;

namespace Core.Materials.Loading.Loader.Command
{
    public class MaterialDataLoadingCommand
    {
        private IApplicationOptions _options;
        private IHttpRequest _request;
        private IMaterialDataParser _parser;
        private IMaterialDataStorage _materials;
        private IMaterialWorkerCoordinator _workerCoordinator;
        private string _url;
        private int _maxRequestAttempts;
        private CancellationTokenSource _cancellationTokenSource;
        
        public event Action<IReadOnlyList<MaterialData>> Completed;
        public event Action<string> Failed;
        public event Action<MaterialDataLoadingCommand> Disposed;

        public MaterialDataLoadingInfo Info { get; private set; }

        public void Configure(IApplicationOptions options, 
            IHttpRequest request, 
            IMaterialDataParser parser,
            IMaterialDataStorage materials,
            IMaterialWorkerCoordinator workerCoordinator,
            int maxRequestAttempts)
        {
            _options = options;
            _request = request;
            _parser = parser;
            _materials = materials;
            _workerCoordinator = workerCoordinator;
            _maxRequestAttempts = maxRequestAttempts;
        }
        
        public void Initialize(in MaterialDataLoadingInfo info,
            string url,
            Action<IReadOnlyList<MaterialData>> onCompleted, 
            Action<string> onFailure,
            CancellationTokenSource cancellationTokenSource)
        {
            Info = info;
            
            _url = url;
            
            if (onCompleted != null)
                Completed += onCompleted;
            if (onFailure != null)
                Failed += onFailure;
            
            _cancellationTokenSource = cancellationTokenSource;
        }

        public void Execute()
        {
            if (!CheckForCancellation())
                Load(0);
        }
        
        private void Dispose()
        {
            Info = default;
            
            _url = null;
            
            _maxRequestAttempts = 0;
            _cancellationTokenSource = null;
            
            _options = null;
            _request = null;
            _parser = null;
            _materials = null;
            _workerCoordinator = null;
            
            Completed = null;
            Failed = null;
            
            Disposed?.Invoke(this);
        }
        
        private bool CheckForCancellation()
        {
            if (_cancellationTokenSource == null)
                return true;
            
            if (!_cancellationTokenSource.IsCancellationRequested)
                return false;
            
            Dispose();
            return true;
        }
        
        private void Load(int currentRequestAttempt)
        {
            if (currentRequestAttempt >= _maxRequestAttempts)
            {
                var errorMessage = $"The maximum number of attempts to load material data with type {Info.Type.Name} and id {Info.Id} has been exceeded. This material data is ignored to load. URL: {_url}.";
                
                Debug.LogError(errorMessage);
                Failed?.Invoke(errorMessage);
                
                Dispose();
                return;
            }
            
            if (_options.IsManagersLogEnabled)
            {
                if (currentRequestAttempt <= 1)
                    Debug.Log($"{Info.Type.Name} with id {Info.Id} loading has been started. URL: {_url}");
                else 
                    Debug.LogWarning($"{Info.Type.Name} with id {Info.Id} loading has been started again. Attempt: {currentRequestAttempt}, URL: {_url}");
            }
            
            _request.Request(_url, OnLoadingCompleted, _ => OnLoadingFailureAsync(currentRequestAttempt).Forget());
        }
        
        private async UniTaskVoid ParseLoadedDataAsync(string rawData)
        {
            MaterialData[] resultMaterials;
            
            try
            {
                var cancellationToken = _cancellationTokenSource.Token;
                
                await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
                {
                    await UniTask.SwitchToThreadPool();
                    
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    if (_options.IsManagersLogEnabled)
                        Debug.Log($"{Info.Type.Name} with id {Info.Id} parsing has been started");
                    
                    var materials = _parser.Parse(Info, rawData);
                    
                    if (materials == null || materials.Count == 0)
                        throw new Exception("List of materials is null or empty");
                    
                    if (_options.IsManagersLogEnabled)
                        Debug.Log($"{Info.Type.Name} with id {Info.Id} parsing has been completed");
                    
                    var originalMaterialsCount = materials.Count;
                    
                    resultMaterials = new MaterialData[originalMaterialsCount];
                    
                    for (var i = 0; i < originalMaterialsCount; i++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        
                        var material = materials[i];
                        var resultMaterial = _materials.Get(material.GetType(), material.Id);
                        
                        _workerCoordinator.PerformActionAfterAddingToStorageOf(resultMaterial);
                        
                        resultMaterials[i] = resultMaterial;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Dispose();
                return;
            }
            catch (Exception exception)
            {
                var errorDetails = $"Error: {exception.Message}{Environment.NewLine}{exception.StackTrace}";
                var errorMessage =$"{Info.Type.Name} with id {Info.Id} parsing has been aborted. {errorDetails}";
                
                Debug.LogError(errorMessage);
                Failed?.Invoke(errorMessage);
                
                Dispose();
                return;
            }
            finally
            {
                if (Thread.CurrentThread.IsBackground)
                    await UniTask.SwitchToMainThread();
            }
            
            Completed?.Invoke(resultMaterials);
            Dispose();
        }

        private void OnLoadingCompleted(string result)
        {
            if (CheckForCancellation())
                return;

            if (_options.IsManagersLogEnabled)
                Debug.Log($"{Info.Type.Name} with id {Info.Id} loading has been completed");

            ParseLoadedDataAsync(result).Forget();
        }

        private async UniTaskVoid OnLoadingFailureAsync(int currentRequestAttempt)
        {
            if (CheckForCancellation())
                return;
            
            Debug.LogWarning($"{Info.Type.Name} with id {Info.Id} loading has been failed");
            
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1.0), cancellationToken: _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                Dispose();
                return;
            }
            
            Load(++currentRequestAttempt);
        }
    }
}