using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.FileResources.Customizing.Loaders.Base
{
	public interface IResourceLoader
	{
		UniTask UploadAsync(string path, byte[] bytes, CancellationToken cancellationToken);
		UniTask<byte[]> DownloadAsync(string path, CancellationToken cancellationToken);
	}
}