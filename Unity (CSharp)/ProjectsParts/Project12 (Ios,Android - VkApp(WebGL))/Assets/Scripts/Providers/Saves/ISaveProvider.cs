using Game.Scripts.Providers.Saves.Data;
using Game.Scripts.App;

namespace Game.Scripts.Providers.Saves
{
	public interface ISaveProvider : IApplicationLoaderItem, ISaveHandler
	{
		public T GetProperty<T>() where T : ISaveItem;

	}
}