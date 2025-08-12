using Zenject;

namespace Game.Providers.Profile.Handlers
{
	public class PlayerLevelHandler
	{
		private IProfileProvider _profileProvider;
		private SignalBus _signalBus;
		public PlayerLevelHandler(SignalBus signalBus, IProfileProvider profileProvider)
		{
			_profileProvider = profileProvider;
			_signalBus = signalBus;
		}
	}
}
