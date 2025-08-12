using Game.Base;
using Game.Providers.Profile;
using Game.Providers.Profile.Signals;
using UnityEngine;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class ProfileLevelRewardNotificationView : MonoBehaviour, IInjection
	{
		[SerializeField] private RectTransform _icone;

		private SignalBus _signalBus;
		private IProfileProvider _profileProvider;

		[Inject]
		public void Constructor(SignalBus signalBus, IProfileProvider profileProvider)
		{
			_signalBus = signalBus;
			_profileProvider = profileProvider;
		}

		public void OnEnable()
		{
			_signalBus.Subscribe<ExperienceChangeSignal>(ConfirmState);
			ConfirmState();
		}

		public void OnDisable()
		{
			_signalBus.Unsubscribe<ExperienceChangeSignal>(ConfirmState);
		}

		private void ConfirmState()
		{
			_icone.gameObject.SetActive(_profileProvider.ExistsRewardReady);
		}

	}
}
