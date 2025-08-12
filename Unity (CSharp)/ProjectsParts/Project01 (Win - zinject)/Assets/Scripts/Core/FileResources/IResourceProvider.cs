using System.Threading;
using Core.FileResources.Info;
using Cysharp.Threading.Tasks;

namespace Core.FileResources
{
	public interface IResourceProvider
	{
		//IResourceRequestCommand Request<TResource>(in ResourceInfo info,
		//	Action<TResource, string> onCompleted = null,
		//	Action<string> onFailure = null,
		//	Action onCanceled = null);

		UniTask<ResourceRequestResult<TResource>> RequestAsync<TResource>(ResourceInfo info,
			CancellationTokenSource cancellationTokenSource = null);
	}
}