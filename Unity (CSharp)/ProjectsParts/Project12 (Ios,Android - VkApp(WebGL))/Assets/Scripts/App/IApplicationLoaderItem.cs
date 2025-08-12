using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Scripts.App
{
	public interface IApplicationLoaderItem
	{
		UniTask StartAppLoad(IProgress<float> onProgress, CancellationToken cancellationToken);
	}
}
