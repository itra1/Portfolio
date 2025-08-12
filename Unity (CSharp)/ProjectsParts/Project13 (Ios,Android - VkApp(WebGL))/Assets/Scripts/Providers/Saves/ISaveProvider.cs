using Cysharp.Threading.Tasks;
using Game.Base.AppLaoder;
using Game.Providers.Saves.Data;

namespace Game.Providers.Saves
{
	public interface ISaveProvider : IAppLoaderElement
	{
		public T GetProperty<T>() where T : ISaveItem;

		UniTask Save();
	}
}