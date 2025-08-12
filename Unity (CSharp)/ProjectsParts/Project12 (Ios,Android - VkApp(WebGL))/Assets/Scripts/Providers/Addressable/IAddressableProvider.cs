using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.App;

namespace Game.Scripts.Providers.Addressable
{
	public interface IAddressableProvider : IApplicationLoaderItem
	{
		UniTask<T> LoadAssetAsync<T>(string assetKey, IProgress<float> progress = null, CancellationToken cancellationToken = default);
	}
}