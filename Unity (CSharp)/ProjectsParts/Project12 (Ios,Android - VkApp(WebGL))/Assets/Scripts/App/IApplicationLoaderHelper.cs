using Cysharp.Threading.Tasks;
using UnityEngine.Events;

namespace Game.Scripts.App
{
	public interface IApplicationLoaderHelper
	{
		UnityEvent<float> OnProgress { get; set; }

		UniTask AppLoad();
	}
}