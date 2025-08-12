using System.Collections.Generic;
using System.Threading;
using Core.Network.Http.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Network.Http
{
    /// <summary>
    /// Устаревшее название - "RestManager"
    /// </summary>
    public interface IHttpRequestAsync
    {
        UniTask<HttpResponseData<string>> RequestAsync(string url,
            CancellationToken cancellationToken);
        
        UniTask<HttpResponseData<string>> RequestAsync(string url,
            IList<(string, object)> parameters,
            CancellationToken cancellationToken);
        
        UniTask<HttpResponseData<string>> RequestAsync(string url,
            string rawData,
            CancellationToken cancellationToken);
        
        UniTask<HttpResponseData<string>> RequestAsync(string url,
            HttpMethodType methodType,
            CancellationToken cancellationToken);
        
        UniTask<HttpResponseData<string>> RequestAsync(string url,
            HttpMethodType methodType,
            IList<(string, object)> parameters,
            CancellationToken cancellationToken);
        
        UniTask<HttpResponseData<string>> RequestAsync(string url,
            HttpMethodType methodType,
            string rawData,
            CancellationToken cancellationToken);
        
        UniTask<HttpResponseData<string>> RequestAsync(string url,
            HttpMethodType methodType,
            byte[] rawData,
            CancellationToken cancellationToken);
        
        UniTask<HttpResponseData<byte[]>> RequestBytesAsync(string url,
            HttpMethodType type,
            CancellationToken cancellationToken);

        UniTask<HttpResponseData<Texture2D>> RequestTexture2DAsync(string url,
            HttpMethodType type,
            bool readable,
            CancellationToken cancellationToken);
    }
}