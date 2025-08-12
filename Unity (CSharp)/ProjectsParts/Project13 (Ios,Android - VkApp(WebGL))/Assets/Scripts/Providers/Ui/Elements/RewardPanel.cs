using Cysharp.Threading.Tasks;
using Game.Base;
using Game.Providers.Profile;
using Game.Providers.Profile.Signals;
using Game.Providers.Ui.Popups;
using Game.Providers.Ui.Popups.Common;
using Game.Providers.Ui.Windows;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class RewardPanel : MonoBehaviour, IInjection
	{
		[SerializeField] private TMP_Text _levelLabel;
		[SerializeField] private TMP_Text _progressLabel;
		[SerializeField] private RectTransform _progressLine;
		[SerializeField] private Image _lightBack;
		[SerializeField] private Image _defaultBack;

		private SignalBus _signalBus;
		private WindowProvider _windowProvider;
		private PopupProvider _popupProvider;
		private IProfileProvider _profileProvider;
		private IUiProvider _uiProvider;

		[Inject]
		public void Constructor(SignalBus signalBus,
		WindowProvider windowProvider,
		PopupProvider popupProvider,
		IProfileProvider profileProvider,
		IUiProvider uiProvider)
		{
			_signalBus = signalBus;
			_windowProvider = windowProvider;
			_popupProvider = popupProvider;
			_profileProvider = profileProvider;
			_uiProvider = uiProvider;
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
			_defaultBack.gameObject.SetActive(!_profileProvider.ExistsRewardReady);
			_lightBack.gameObject.SetActive(_profileProvider.ExistsRewardReady);
		}

		public void MainTouch()
		{
			_ = _windowProvider.GetActiveWindow();
			//var profileWindow = _uiProvider.GetController<ProfileWindowPresenterController>();
			//_windowProvider.GetWindow(WindowsNames.Profile);

			var levelProgress = _popupProvider.GetPopup(PopupsNames.LevelProgress);
			levelProgress.Show().Forget();
		}
	}
}
