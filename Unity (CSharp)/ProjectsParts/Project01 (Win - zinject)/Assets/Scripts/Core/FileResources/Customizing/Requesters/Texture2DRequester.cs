using System;
using Core.FileResources.Customizing.Requesters.Base;
using Core.Network.Http;
using Core.Network.Http.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.FileResources.Customizing.Requesters
{
    public class Texture2DRequester : IResourceRequester<Texture2D>
    {
        private readonly bool _readable;
        
        public Texture2DRequester(bool readable) => _readable = readable;
        
        public void Make(IHttpRequest request,
            string url,
            Func<Texture2D, UniTaskVoid> onCompletedAsync,
            Action<string> onFailure = null)
        {
            request.RequestTexture2D(url,
                HttpMethodType.Get,
                _readable,
                texture => onCompletedAsync(texture).Forget(),
                onFailure);
        }
    }
}