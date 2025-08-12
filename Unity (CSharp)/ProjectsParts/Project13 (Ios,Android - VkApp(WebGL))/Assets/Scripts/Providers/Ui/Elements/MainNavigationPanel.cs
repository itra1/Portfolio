using Game.Base;
using Game.Providers.Ui.Controllers;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class MainNavigationPanel : MonoBehaviour, IInjection
	{
		public UnityAction<string> OnNavigation;

		[SerializeField] private MainNavigationButtonView _leaderboardButton;
		[SerializeField] private MainNavigationButtonView _tournamentButton;
		[SerializeField] private MainNavigationButtonView _giftsButton;
		[SerializeField] private RectTransform _selectBack;

		private IUiProvider _uiProvider;

		[Inject]
		public void Constructor(IUiProvider uiProvider)
		{
			_uiProvider = uiProvider;
		}

		public void Awake()
		{
			_leaderboardButton.OnClick = LeaderboardButtonTouch;
			_tournamentButton.OnClick = PlayButtonTouch;
			_giftsButton.OnClick = GiftsButtonTouch;
		}

		private void LeaderboardButtonTouch()
		{
			var developController = _uiProvider.GetController<DevelopWindowPresenterController>();
			//var developPopup = (DevelopPopup) _popupProvider.GetPopup(PopupsNames.Develop);
			_ = developController.Show(null);
			developController.Presenter.SetDefault();
			developController.Presenter.SetDescription("Worldwide rating is coming soon!");
			//SetSelect(HomePageNames.Leaderboard);
		}
		private void PlayButtonTouch()
		{
		}
		private void GiftsButtonTouch()
		{
		}

		public void SetSelect(string type)
		{
			OnNavigation?.Invoke(type);
		}
	}
}
