using System;
using System.Collections.Generic;
using System.Threading;
using Core.Materials.Data;
using Core.Network.Api.Consts;
using Core.Network.Http;
using Core.Network.Http.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Debug = Core.Logging.Debug;
#if UNITY_EDITOR
using System.IO;
#endif

namespace Preview.Stages
{
	[DisallowMultipleComponent]
    public class PreviewPublisher : MonoBehaviour, IPreviewPublisher, IDisposable
    {
#if UNITY_EDITOR
        private string _directoryPath;
#endif
	    
	    private IHttpRequestAsync _requestAsync;
	    private Queue<(AreaMaterialData, byte[])> _queue;
	    private CancellationTokenSource _disposeCancellationTokenSource;
	    
        public void Initialize(IHttpRequestAsync requestAsync)
        {
#if UNITY_EDITOR
	        _directoryPath = Application.dataPath + "/../out";
            
	        if (!Directory.Exists(_directoryPath))
		        Directory.CreateDirectory(_directoryPath);
#endif
	        
	        _requestAsync = requestAsync;
	        _queue = new Queue<(AreaMaterialData, byte[])>();
	        _disposeCancellationTokenSource = new CancellationTokenSource();
        }
        
        public void AttemptToPublishBasedOnQueue(AreaMaterialData areaMaterial, byte[] bytes)
        {
	        if (_disposeCancellationTokenSource.IsCancellationRequested)
		        return;
	        
	        if (_queue.Count > 0)
				_queue.Enqueue((areaMaterial, bytes));
	        else
		        PublishAsync(areaMaterial, bytes).Forget();
        }
        
        public void Dispose()
        {
	        if (!_disposeCancellationTokenSource.IsCancellationRequested)
	        {
		        _disposeCancellationTokenSource.Cancel();
		        _disposeCancellationTokenSource.Dispose();
	        }
		    
	        _queue.Clear();
		    
#if UNITY_EDITOR
	        if (!string.IsNullOrEmpty(_directoryPath))
	        {
		        var files = new DirectoryInfo(_directoryPath).GetFiles();
			    
		        for (var i = files.Length - 1; i >= 0; i--)
		        {
			        try
			        {
				        files[i].Delete();
			        }
			        catch
			        {
				        // ignored
			        }
		        }
	        }
#endif

	        _requestAsync = null;
        }
        
	    private async UniTask PublishAsync(AreaMaterialData areaMaterial, byte[] bytes)
	    {
#if UNITY_EDITOR
		    await PutIntoCacheAsync(areaMaterial, bytes);
#endif
		    await PostPreviewAsync(areaMaterial, bytes);
	    }
	    
#if UNITY_EDITOR
	    private async UniTask PutIntoCacheAsync(AreaMaterialData areaMaterial, byte[] bytes)
	    {
		    try
		    {
			    var cancellationToken = _disposeCancellationTokenSource.Token;
			    
			    await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
			    {
				    await UniTask.SwitchToThreadPool();
				    
				    cancellationToken.ThrowIfCancellationRequested();
				    
				    await using var fileStream = new FileStream($"{_directoryPath}/{areaMaterial.Id}.png",
					    FileMode.OpenOrCreate,
					    FileAccess.Write,
					    FileShare.Write,
					    4096,
					    FileOptions.Asynchronous);
				    
				    await fileStream.WriteAsync(bytes, cancellationToken);
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
		    }
	    }
#endif
	    
	    private async UniTask PostPreviewAsync(AreaMaterialData areaMaterial, byte[] bytes)
	    {
		    try
		    {
			    var cancellationToken = _disposeCancellationTokenSource.Token;
			    
			    await _requestAsync.RequestAsync(string.Format(RestApiUrl.PreviewFormat, areaMaterial.Id),
				    HttpMethodType.Post,
				    bytes,
				    cancellationToken);
		    }
		    catch (Exception exception) when (exception is not OperationCanceledException)
		    {
			    Debug.LogException(exception);
		    }
		    
		    AttemptToPublishNext();
	    }
	    
	    private void AttemptToPublishNext()
	    {
		    if (_queue.Count == 0)
			    return;
			
		    var (areaMaterial, bytes) = _queue.Dequeue();
		    
		    PublishAsync(areaMaterial, bytes).Forget();
	    }
    }
}