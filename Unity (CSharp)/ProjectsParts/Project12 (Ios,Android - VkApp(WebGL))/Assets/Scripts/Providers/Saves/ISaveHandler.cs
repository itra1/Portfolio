using Cysharp.Threading.Tasks;

namespace Game.Scripts.Providers.Saves
{
	public interface ISaveHandler
	{
		void CrearProgress();
		UniTask Save();
	}
}