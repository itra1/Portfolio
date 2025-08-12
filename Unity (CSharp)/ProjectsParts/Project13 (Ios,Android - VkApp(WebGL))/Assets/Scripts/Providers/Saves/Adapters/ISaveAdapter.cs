using Cysharp.Threading.Tasks;

namespace Game.Providers.Saves.Adapters
{
	public interface ISaveAdapter
	{
		UniTask<string> Load();
		UniTask Save(string data);
	}
}
