using Game.Base;
using Game.Providers.Profile.Signals;
using Game.Providers.Ui.Factorys;
using Game.Scenes;
using UnityEngine;
using Zenject;

namespace Game.Providers.Profile.Handlers
{
	public class CoinsHandler : PlayerResourceHandler
	{

		private IProfileProvider _profileProvider;
		private SignalBus _signalBus;

		public override string ResourceType => RewardTypes.Coins;
		public override float CurrentValue => _profileProvider.Coins;
		public override string CurrentValueString => _profileProvider.Coins.ToString().Replace(",", ".");

		public CoinsHandler(SignalBus signalBus, IProfileProvider profileProvider, UiPlayerResourcesFactory uiPlayerResourcesFactory, IItemsAnimationParent itemParent) : base(uiPlayerResourcesFactory, itemParent)
		{
			_profileProvider = profileProvider;
			_signalBus = signalBus;

			_signalBus.Subscribe<ResourceAddSignal>(OnResourceAddSignal);
		}

		private void OnResourceAddSignal(ResourceAddSignal signal)
		{
			if (signal.Type != RewardTypes.Coins)
				return;
			SetValue((int) (CurrentValue + signal.Value), signal.SourcePoint);
		}

		protected override void SetValue(float value, RectTransform point)
		{
			if (value - CurrentValue >= 0 && point != null)
				AnimateMovePoint(RewardTypes.Coins, (int) (value - CurrentValue), point);
			_profileProvider.Coins = (int) value;
			_signalBus.Fire<CoinsChangeSignal>();
			_signalBus.Fire<MoneyChangeSignal>();
		}

	}
}
