using Core.Engine.uGUI;
using Scripts.Signals;
using TMPro;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Elements
{
	public class LevelProgressBar :MonoBehaviour, IZInjection
	{
		[SerializeField] private TMP_Text _currentLevelLabel;
		[SerializeField] private TMP_Text _nextLevelLabel;
		[SerializeField] private RectTransform _progressbar;

		private SignalBus _signalbus;

		[Inject]
		public void Initiate(SignalBus signalBus)
		{
			_signalbus = signalBus;

			_signalbus.Subscribe<LevelStartSignal>(OnLevelStartSignal);
			_signalbus.Subscribe<PlatformCrossingSignal>(OnPlatformCrossingSignal);
		}

		private void OnPlatformCrossingSignal(PlatformCrossingSignal signal)
		{
			_progressbar.localScale = new Vector2((float)signal.PlatformIndex / (float)signal.PlatformsAll, _progressbar.localScale.y);
		}

		private void OnLevelStartSignal(LevelStartSignal signal)
		{
			_currentLevelLabel.text = (signal.Level.Level).ToString();
			_nextLevelLabel.text = (signal.Level.Level + 1).ToString();
			_progressbar.localScale = new Vector2(0, _progressbar.localScale.y);
		}
	}
}
