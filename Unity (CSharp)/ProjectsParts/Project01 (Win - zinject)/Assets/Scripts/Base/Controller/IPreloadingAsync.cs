using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Base.Controller
{
	public interface IPreloadingAsync
	{
		UniTask<bool> PreloadAsync(RectTransform parent);
	}
}