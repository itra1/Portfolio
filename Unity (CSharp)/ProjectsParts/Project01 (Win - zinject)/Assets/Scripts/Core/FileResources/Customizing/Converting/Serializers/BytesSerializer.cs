using System.Threading;
using Core.FileResources.Customizing.Converting.Serializers.Base;
using Cysharp.Threading.Tasks;

namespace Core.FileResources.Customizing.Converting.Serializers
{
    public class BytesSerializer : IResourceSerializer<byte[]>
    {
        public UniTask<byte[]> SerializeAsync(string name, byte[] bytes, CancellationToken cancellationToken) =>
            new(bytes);
    }
}