using System;
using Core.FileResources.Customizing.Requesters.Base;
using Core.Network.Http;
using Core.Network.Http.Data;
using Cysharp.Threading.Tasks;

namespace Core.FileResources.Customizing.Requesters
{
    public class TextRequester : IResourceRequester<string>
    {
        public void Make(IHttpRequest request,
            string url,
            Func<string, UniTaskVoid> onCompletedAsync,
            Action<string> onFailure = null)
        {
            request.Request(url,
                HttpMethodType.Get,
                text => onCompletedAsync(text).Forget(),
                onFailure);
        }
    }
}