using Game.Base;
using Game.Providers.Profile;
using Game.Providers.Profile.Signals;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class LevelPanel : MonoBehaviour, IInjection
	{
		[SerializeField] private TMP_Text _levelLabel;
		[SerializeField] private TMP_Text _levelProgressLabel;
		[SerializeField] private RectTransform _progressLine;

		private IProfileProvider _profileProvider;
		private SignalBus _signalBus;
		private float _progressWidth;

		[Inject]
		public void Constructor(SignalBus signalBus, IProfileProvider profileProvider)
		{
			_signalBus = signalBus;
			_profileProvider = profileProvider;
			_progressWidth = _progressLine.sizeDelta.x;
		}

		public void OnEnable()
		{
			_signalBus.Subscribe<ExperienceChangeSignal>(SetData);
			SetData();
		}
		public void OnDisable()
		{
			_signalBus.Unsubscribe<ExperienceChangeSignal>(SetData);
		}

		private void SetData()
		{
			_levelProgressLabel.text = $"{_profileProvider.CurrentExperienceInLevel}/{_profileProvider.ExperienceInLevel}";
			_progressLine.sizeDelta = new Vector2(_progressWidth * (_profileProvider.CurrentExperienceInLevel / (float) _profileProvider.ExperienceInLevel), _progressLine.sizeDelta.y);
			_levelLabel.text = _profileProvider.Level.ToString();
		}
	}
}
