using Cysharp.Threading.Tasks;

namespace Game.Scripts.Providers.Saves.Adapters
{
	public interface ISaveAdapter
	{
		UniTask<string> Load();
		UniTask Save(string data);
	}
}
