using Game.Base;
using Game.Providers.Profile.Signals;
using Game.Providers.Ui.Factorys;
using Game.Scenes;
using UnityEngine;
using Zenject;

namespace Game.Providers.Profile.Handlers
{
	public class ExperienceHandler : PlayerResourceHandler
	{

		private IProfileProvider _profileProvider;
		private SignalBus _signalBus;

		public override string ResourceType => RewardTypes.Experience;
		public override float CurrentValue => _profileProvider.Experience;
		public override string CurrentValueString => _profileProvider.Experience.ToString().Replace(",", ".");

		public ExperienceHandler(SignalBus signalBus, IProfileProvider profileProvider, UiPlayerResourcesFactory uiPlayerResourcesFactory, IItemsAnimationParent itemParent) : base(uiPlayerResourcesFactory, itemParent)
		{
			_profileProvider = profileProvider;
			_signalBus = signalBus;

			_signalBus.Subscribe<ResourceAddSignal>(OnResourceAddSignal);
		}

		private void OnResourceAddSignal(ResourceAddSignal signal)
		{
			if (signal.Type != RewardTypes.Experience)
				return;

			if (_profileProvider.IsMaxLevel)
				return;

			SetValue((int) (CurrentValue + signal.Value), signal.SourcePoint);

			while (_profileProvider.CurrentExpirience >= _profileProvider.ExperienceToNextLevel && !_profileProvider.IsMaxLevel)
			{
				_profileProvider.Level++;
				_signalBus.Fire<ExperienceChangeSignal>();
			}
		}

		protected override void SetValue(float value, RectTransform point)
		{
			if (value - CurrentValue >= 0 && point != null)
				AnimateMovePoint(RewardTypes.Experience, (int) (value - CurrentValue), point);
			if (value == 0)
				return;
			_profileProvider.Experience = (int) value;
			_signalBus.Fire<ExperienceChangeSignal>();
		}
	}
}
