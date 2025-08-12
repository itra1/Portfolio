using Cysharp.Threading.Tasks;
using Game.Base.AppLaoder;
using Game.Game.Elements.Scenes;
using Game.Providers.DailyBonus;
using Game.Providers.Profile;
using Game.Providers.Ui;
using Game.Providers.Ui.Controllers;
using UnityEngine;
using Zenject;

namespace Game.Game
{
	public class AppRun : IInitializable
	{
		private readonly IGameScene _gameScene;
		private readonly IAppLoaderHandler _loaderHandler;
		private readonly DailyBonusProvider _dailyBonus;
		private readonly DailyBonusHandler _dailybonusHandler;
		private readonly IProfileProvider _profileProvider;
		private readonly IUiProvider _uiProvider;

		public AppRun(IGameScene gameScene
		, IAppLoaderHandler loaderHandler
		, DailyBonusProvider dailyBonus
		, DailyBonusHandler dailybonusHandler
		, IProfileProvider profileProvider
		, IUiProvider uiProvider)
		{
			_gameScene = gameScene;
			_loaderHandler = loaderHandler;
			_dailyBonus = dailyBonus;
			_dailybonusHandler = dailybonusHandler;
			_profileProvider = profileProvider;
			_uiProvider = uiProvider;
		}

		public void Initialize()
		{
			InitAsync().Forget();
		}

		private async UniTask InitAsync()
		{
			(_gameScene as Component).gameObject.SetActive(false);

			var loadingWindows = _uiProvider.GetController<LoaderWindowPresenterController>();
			var homeWindow = _uiProvider.GetController<HomeWindowPresenterController>();
			//var welcomeWindow = _uiProvider.GetController<WelcomeWindowPresenterController>();

			await loadingWindows.Show(null);

			//var loadingWindows = (LoadingWindowPresenter)_windowProvider.GetWindow(WindowsNames.Loading, true);
			//loadingWindows.Show().Forget();

#if UNITY_IOS || UNITY_ANDROID
			var statusATT = await Knifes.Services.ATTracks.ATTrackService.RequestATT();
#endif

			await _loaderHandler.AppLoad();
			await UniTask.Delay(500);

			//var homeWindow = _windowProvider.GetWindow(WindowsNames.Home);
			await homeWindow.Show(null);
			await loadingWindows.Hide();

			if (!_profileProvider.WelcomeShow)
			{
				_profileProvider.WelcomeShow = true;
				//await welcomeWindow.Open(null);
				//welcomeWindow.Presenter.RunTutorialAction = () =>
				//{
				//	runTutorial = true;
				//};
				//await UniTask.WaitUntil(() => !welcomeWindow.Presenter.gameObject.activeInHierarchy);
			}
			//runTutorial = true;
			//runTutorial = false;

			//if (!runTutorial && _dailybonusHandler.ExistsDailyBonus)
			//{
			//	await _dailybonusHandler.Open();
			//}

		}
	}
}
