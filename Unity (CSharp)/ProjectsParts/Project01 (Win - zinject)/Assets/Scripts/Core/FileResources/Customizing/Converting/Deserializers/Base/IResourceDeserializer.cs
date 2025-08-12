using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.FileResources.Customizing.Converting.Deserializers.Base
{
    public interface IResourceDeserializer<in TResource>
    {
        UniTask<byte[]> DeserializeAsync(TResource resource, CancellationToken cancellationToken);
    }
}