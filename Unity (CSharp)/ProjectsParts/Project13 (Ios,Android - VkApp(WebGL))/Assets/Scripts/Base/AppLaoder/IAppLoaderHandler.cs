using Cysharp.Threading.Tasks;
using UnityEngine.Events;

namespace Game.Base.AppLaoder
{
	public interface IAppLoaderHandler
	{
		UnityEvent<float> OnProgress { get; set; }

		UniTask AppLoad();
	}
}
