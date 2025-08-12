using Cysharp.Threading.Tasks;
using UnityEngine.Events;

namespace Game.Scripts.Providers.Networks
{
	public interface INetworkProvider
	{
		UniTask<bool> Get(string url, UnityAction<string> callback = null);
	}
}