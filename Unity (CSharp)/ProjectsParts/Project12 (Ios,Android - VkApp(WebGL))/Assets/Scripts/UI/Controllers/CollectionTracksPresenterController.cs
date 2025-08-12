using Cysharp.Threading.Tasks;
using Engine.Scripts.Managers;
using Engine.Scripts.Timelines;
using Game.Scripts.UI.Attributes;
using Game.Scripts.UI.Base;
using Game.Scripts.UI.Controllers.Base;
using Game.Scripts.UI.Presenters;
using Zenject;

namespace Game.Scripts.UI.Controllers
{
	[UiController(WindowPresenterType.Window, WindowPresenterNames.CollectionTracks)]
	public class CollectionTracksPresenterController : WindowPresenterController<CollectionTracksPresenter>
	{
		private IGameHandler _gameHandler;
		public override bool AddInNavigationStack => true;

		[Inject]
		private void Build(IGameHandler gameHandler)
		{
			_gameHandler = gameHandler;
		}

		protected override void AfterCreateWindow()
		{
			base.AfterCreateWindow();

			Presenter.OnPlayTrackEvent.AddListener(SelectTrack);
			Presenter.OnBackTrackEvent.AddListener(() => _ = BackTrackEvent());
		}

		private async UniTask BackTrackEvent()
		{
			//_ = UiNavigator.GetController<HomePresenterController>().Open(true, true);
			//_ = Close();
			_ = await Close();
			//_ = UiNavigator.BackNavigation();
		}

		private void SelectTrack(RhythmTimelineAsset song)
		{
			UiNavigator.CloseAll();
			_ = _gameHandler.PlaySong(song);
			_ = Close();
		}
	}
}
