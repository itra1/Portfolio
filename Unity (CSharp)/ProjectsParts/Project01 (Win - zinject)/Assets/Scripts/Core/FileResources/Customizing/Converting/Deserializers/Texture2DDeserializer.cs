using System.Threading;
using Core.FileResources.Customizing.Converting.Deserializers.Base;
using Core.Utils;
using Core.Utils.Enums;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.FileResources.Customizing.Converting.Deserializers
{
    public class Texture2DDeserializer : IResourceDeserializer<Texture2D>
    {
        public UniTask<byte[]> DeserializeAsync(Texture2D resource, CancellationToken cancellationToken) => 
            resource.ToBytesAsync(cancellationToken, FileEncodingFormat.RawTexture);
    }
}