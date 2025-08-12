using System.Threading;
using Core.FileResources.Customizing.Category;
using Cysharp.Threading.Tasks;

namespace Core.FileResources.Caching
{
	public interface IResourceCachingService
	{
		bool IsAlreadyCached(ResourceCategory category, string url);
		bool IsAlreadyCached(ResourceCategory category, string url, out string path);
		UniTask<string> PutIntoCacheAsync(ResourceCategory category, string url, byte[] bytes, CancellationToken cancellationToken);
		UniTask<(byte[], string)> GetFromCacheAsync(ResourceCategory category, string url, CancellationToken cancellationToken);
	}
}