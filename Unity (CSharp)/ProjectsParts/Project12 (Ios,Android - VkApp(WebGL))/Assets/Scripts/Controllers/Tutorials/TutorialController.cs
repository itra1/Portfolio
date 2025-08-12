using Cysharp.Threading.Tasks;
using Game.Scripts.Providers.Profiles;
using Game.Scripts.Providers.Songs.Helpers;
using Game.Scripts.UI;
using Game.Scripts.UI.Controllers;

namespace Game.Scripts.Controllers.Tutorials
{
	public class TutorialController : ITutorialController
	{
		private IProfileProvider _profileProvider;
		private ISongsHelper _songsHelper;
		private IUiNavigator _uiNavigator;

		public bool IsReady
		{
			get
			{
				if (!_profileProvider.IsFirstLogin)
					return false;

				return _songsHelper.GetTutorialSongs().Count >= 4;
			}
		}

		public TutorialController(IProfileProvider profileProvider, ISongsHelper songsHelper, IUiNavigator uiNavigator)
		{
			_profileProvider = profileProvider;
			_songsHelper = songsHelper;
			_uiNavigator = uiNavigator;
		}

		public async UniTask Show()
		{
			var tutorialPresenter = _uiNavigator.GetController<TutorialSelectPresenterController>();
			_ = await tutorialPresenter.Open();
		}
	}
}
