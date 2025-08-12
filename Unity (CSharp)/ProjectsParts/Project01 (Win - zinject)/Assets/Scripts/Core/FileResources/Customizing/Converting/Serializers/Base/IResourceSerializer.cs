using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.FileResources.Customizing.Converting.Serializers.Base
{
	public interface IResourceSerializer<TResource>
	{
		UniTask<TResource> SerializeAsync(string name, byte[] bytes, CancellationToken cancellationToken);
	}
}