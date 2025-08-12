using System;
using Core.Network.Http;
using Cysharp.Threading.Tasks;

namespace Core.FileResources.Customizing.Requesters.Base
{
    public interface IResourceRequester<out TResource>
    {
        void Make(IHttpRequest request,
            string url,
            Func<TResource, UniTaskVoid> onCompletedAsync,
            Action<string> onFailure = null);
    }
}