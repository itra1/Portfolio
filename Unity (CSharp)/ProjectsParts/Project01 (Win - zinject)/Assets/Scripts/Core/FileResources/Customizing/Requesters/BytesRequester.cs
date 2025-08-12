using System;
using Core.FileResources.Customizing.Requesters.Base;
using Core.Network.Http;
using Core.Network.Http.Data;
using Cysharp.Threading.Tasks;

namespace Core.FileResources.Customizing.Requesters
{
    public class BytesRequester : IResourceRequester<byte[]>
    {
        public void Make(IHttpRequest request,
            string url,
            Func<byte[], UniTaskVoid> onCompletedAsync,
            Action<string> onFailure = null)
        {
            request.RequestBytes(url,
                HttpMethodType.Get,
                bytes => onCompletedAsync(bytes).Forget(),
                onFailure);
        }
    }
}