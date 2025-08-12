using Engine.Scripts.Managers;
using Engine.Scripts.Timelines;
using Game.Scripts.UI.Attributes;
using Game.Scripts.UI.Base;
using Game.Scripts.UI.Controllers.Base;
using Game.Scripts.UI.Presenters;
using Zenject;

namespace Game.Scripts.UI.Controllers
{
	[UiController(WindowPresenterType.Window, WindowPresenterNames.Home)]
	public class HomePresenterController : WindowPresenterController<HomePresenter>
	{
		private IGameHandler _gameHelper;
		public override bool AddInNavigationStack => true;

		[Inject]
		private void Build(IGameHandler gameHelper)
		{
			_gameHelper = gameHelper;
		}

		protected override void AfterCreateWindow()
		{
			base.AfterCreateWindow();

			Presenter.PlayTouchEvent.AddListener(PlayTrack);
			Presenter.ShopTouchEveny.AddListener(ShopOpen);
			Presenter.RatingTouchEvent.AddListener(RatingOpen);
			Presenter.CollectionTouchEvent.AddListener(CollectionsOpen);
			Presenter.ProfileTouchEvent.AddListener(ProfileOpen);
		}

		private void PlayTrack(RhythmTimelineAsset song)
		{
			_ = _gameHelper.PlaySong(song);
			_ = Close();
		}

		private void ShopOpen()
		{
			_ = UiNavigator.GetController<ShopPresenterController>().Open();
		}

		private void RatingOpen()
		{
			_ = UiNavigator.GetController<LeaderboardPresenterController>().Open();
		}

		private void CollectionsOpen()
		{
			_ = UiNavigator.GetController<CollectionTracksPresenterController>().Open();
		}

		private void ProfileOpen()
		{
			_ = UiNavigator.GetController<ProfilePresenterController>().Open();
		}
	}
}
