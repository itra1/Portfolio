using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Base.AppLaoder
{
	public interface IAppLoaderElement
	{
		bool IsLoaded { get; }
		UniTask FirstLoad(IProgress<float> OnProgress, CancellationToken cancellationToken);
	}
}
