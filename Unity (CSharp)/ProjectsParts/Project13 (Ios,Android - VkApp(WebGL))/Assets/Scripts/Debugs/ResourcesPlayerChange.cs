using Game.Base;
using Game.Providers.Profile.Signals;
using UnityEngine;
using Zenject;

namespace Game.Debugs
{
	public class ResourcesPlayerChange : MonoBehaviour, IInjection
	{

		private SignalBus _signalBus;

		[Inject]
		public void Constructor(SignalBus signalbus)
		{
			_signalBus = signalbus;
		}

		public void CoinsAdd()
		{
			_signalBus.Fire(new ResourceAddSignal(RewardTypes.Coins, 100, null));
		}

		public void CoinsRemove()
		{
			_signalBus.Fire(new ResourceAddSignal(RewardTypes.Coins, -100, null));
		}

		public void DollarAdd()
		{
			_signalBus.Fire(new ResourceAddSignal(RewardTypes.Dollar, 10, null));
		}

		public void DollarRemove()
		{
			_signalBus.Fire(new ResourceAddSignal(RewardTypes.Dollar, -10, null));
		}

		public void ExperienceAdd()
		{
			_signalBus.Fire(new ResourceAddSignal(RewardTypes.Experience, 10, null));
		}

		public void ExperienceRemove()
		{
			_signalBus.Fire(new ResourceAddSignal(RewardTypes.Experience, -10, null));
		}

	}
}
