using UnityEngine.Events;

namespace Game.Scripts.Providers.Profiles.Handlers
{
	public interface IProfileLevelHandler
	{
		UnityEvent<int> OnLevelChangeEvent { get; }

		void AddOneLevel();
	}
}