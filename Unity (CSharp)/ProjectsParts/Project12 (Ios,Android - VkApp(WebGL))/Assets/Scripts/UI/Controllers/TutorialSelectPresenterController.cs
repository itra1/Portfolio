using Cysharp.Threading.Tasks;
using Engine.Scripts.Managers;
using Engine.Scripts.Timelines;
using Game.Scripts.Providers.Profiles;
using Game.Scripts.Providers.Songs.Helpers;
using Game.Scripts.UI.Attributes;
using Game.Scripts.UI.Base;
using Game.Scripts.UI.Controllers.Base;
using Game.Scripts.UI.Presenters;
using Zenject;

namespace Game.Scripts.UI.Controllers
{
	[UiController(WindowPresenterType.Window, WindowPresenterNames.TutorialSelect)]
	public class TutorialSelectPresenterController : WindowPresenterController<TutorialSelectPresenter>
	{
		private IGameHandler _gameHandler;
		private ISongsHelper _songHelper;
		private IProfileProvider _profileProvider;
		public override bool AddInNavigationStack => false;

		[Inject]
		public void Build(IGameHandler gameHandler, ISongsHelper songHelper, IProfileProvider profileProvider)
		{
			_gameHandler = gameHandler;
			_songHelper = songHelper;
			_profileProvider = profileProvider;
		}

		protected override void AfterCreateWindow()
		{
			base.AfterCreateWindow();

			Presenter.OnPlayEvent = PlaySongListener;
		}

		private void PlaySongListener(RhythmTimelineAsset song)
		{
			_ = _gameHandler.PlaySong(song);
			_profileProvider.FirstRun();
			_ = Close();
		}

		public override async UniTask<bool> Open()
		{
			if (!await base.Open())
				return false;

			var songList = _songHelper.GetTutorialSongs();
			Presenter.MakeGrid(songList);

			return true;
		}
	}
}
