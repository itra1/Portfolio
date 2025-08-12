using Cysharp.Threading.Tasks;

namespace Base
{
	public interface IPreloadableAsync
	{
		UniTask<bool> PreloadAsync();
	}
}