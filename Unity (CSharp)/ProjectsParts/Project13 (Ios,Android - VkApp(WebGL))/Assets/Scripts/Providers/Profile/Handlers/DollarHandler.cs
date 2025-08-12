using Game.Base;
using Game.Providers.Profile.Signals;
using Game.Providers.Ui.Factorys;
using Game.Scenes;
using UnityEngine;
using Zenject;

namespace Game.Providers.Profile.Handlers
{
	public class DollarHandler : PlayerResourceHandler
	{

		private IProfileProvider _profileProvider;
		private SignalBus _signalBus;

		public override string ResourceType => RewardTypes.Dollar;
		public override float CurrentValue => _profileProvider.Dollar;
		public override string CurrentValueString => _profileProvider.Dollar.ToString().Replace(",", ".");

		public DollarHandler(SignalBus signalBus, IProfileProvider profileProvider, UiPlayerResourcesFactory uiPlayerResourcesFactory, IItemsAnimationParent itemParent) : base(uiPlayerResourcesFactory, itemParent)
		{
			_profileProvider = profileProvider;
			_signalBus = signalBus;

			_signalBus.Subscribe<ResourceAddSignal>(OnResourceAddSignal);
		}

		private void OnResourceAddSignal(ResourceAddSignal signal)
		{
			if (signal.Type != RewardTypes.Dollar)
				return;
			SetValue((CurrentValue + signal.Value), signal.SourcePoint);
		}

		protected override void SetValue(float value, RectTransform point)
		{
			if (value - CurrentValue >= 0 && point != null)
				AnimateMovePoint(RewardTypes.Dollar, (int) (value - CurrentValue), point);
			_profileProvider.Dollar = value;
			_signalBus.Fire<DollarChangeSignal>();
			_signalBus.Fire<MoneyChangeSignal>();
		}
	}
}
