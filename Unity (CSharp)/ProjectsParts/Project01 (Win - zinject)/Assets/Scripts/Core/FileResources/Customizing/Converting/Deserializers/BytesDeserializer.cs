using System.Threading;
using Core.FileResources.Customizing.Converting.Deserializers.Base;
using Cysharp.Threading.Tasks;

namespace Core.FileResources.Customizing.Converting.Deserializers
{
    public class BytesDeserializer : IResourceDeserializer<byte[]>
    {
        public UniTask<byte[]> DeserializeAsync(byte[] resource, CancellationToken cancellationToken) => 
            new (resource);
    }
}