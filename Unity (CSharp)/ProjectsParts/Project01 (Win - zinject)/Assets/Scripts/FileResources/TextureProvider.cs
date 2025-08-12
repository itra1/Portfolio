using System;
using System.Collections.Generic;
using System.Threading;
using Core.FileResources;
using Core.FileResources.Customizing.Category;
using Core.FileResources.Info;
using Cysharp.Threading.Tasks;
using FileResources.Info;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FileResources
{
    public class TextureProvider : ITextureProvider, IDisposable
    {
        private readonly IResourceProvider _resources;
        private readonly IDictionary<string, ITextureRequestResult> _resultsByUrl;
        private readonly CancellationTokenSource _unloadCancellationTokenSource;
        
        public TextureProvider(IResourceProvider resources)
        {
            _resources = resources;
            _resultsByUrl = new Dictionary<string, ITextureRequestResult>();
            _unloadCancellationTokenSource = new CancellationTokenSource();
        }
        
        public async UniTask<Texture2D> RequestAsync(string url, string name)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogWarning($"URL is null or empty when trying to request a texture named \"{name}\"");
                return null;
            }
            
            var cancellationToken = _unloadCancellationTokenSource.Token;
            
            Texture2D texture = null;
            
            if (_resultsByUrl.TryGetValue(url, out var result))
            {
                if (result.InProgress)
                {
                    try
                    {
                        await UniTask.WaitWhile(() => result.InProgress, cancellationToken: cancellationToken);
                    }
                    catch (Exception exception) when (exception is not OperationCanceledException)
                    {
                        Debug.LogException(exception);
                    }
                }
                
                texture = result.Target;
            }
            else
            {
                result = new TextureRequestResult { InProgress = true };
                
                _resultsByUrl.Add(url, result);
                
                try
                {
                    var resourceResult = await _resources.RequestAsync<Texture2D>(new ResourceInfo(url, ResourceCategory.Texture2D, name));
                    
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    result.Target = texture = resourceResult.Resource;
                }
                catch (Exception exception) when (exception is not OperationCanceledException)
                {
                    Debug.LogException(exception);
                }
                finally
                {
                    result.InProgress = false;
                }
            }
            
            if (texture != null)
                result.OwnerCount++;
            
            return texture;
        }
        
        public void Release(Texture texture)
        {
            if (texture == null)
                return;
            
            foreach (var (url, requestResult) in _resultsByUrl)
            {
                if (requestResult.Target != texture) 
                    continue;
                
                if (--requestResult.OwnerCount <= 0 && _resultsByUrl.Remove(url))
                    Object.Destroy(texture);
                
                break;
            }
        }
        
        public void Dispose()
        {
            if (!_unloadCancellationTokenSource.IsCancellationRequested)
            {
                _unloadCancellationTokenSource.Cancel();
                _unloadCancellationTokenSource.Dispose();
            }
            
            if (_resultsByUrl.Count > 0)
            {
                foreach (var result in _resultsByUrl.Values)
                {
                    var texture = result.Target;
                    
                    if (texture == null) 
                        continue;

                    result.OwnerCount = 0;
                    
                    Object.Destroy(texture);
                }
                
                _resultsByUrl.Clear();
            }
        }
    }
}