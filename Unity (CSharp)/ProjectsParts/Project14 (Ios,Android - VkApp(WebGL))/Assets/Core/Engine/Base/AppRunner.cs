using Core.Engine.App.Base;
using Core.Engine.Components.SaveGame;
using Core.Engine.Components.User;
using Core.Engine.Services;
using Core.Engine.uGUI.Screens;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Core.Engine.Base
{
	public class AppRunner :IInitializable
	{
		private readonly IGameService _gameService;
		private readonly ISaveGameProvider _saveGameProvider;
		private readonly IScreensProvider _screenProvider;
		private readonly IUserProvider _userProvider;

		public AppRunner(
			IGameService gameService
		,ISaveGameProvider saveGameProvider
		,IAppController appController
		,IScreensProvider screenProvider
		,IUserProvider userProvider
		)
		{
			_userProvider = userProvider;
			_saveGameProvider = saveGameProvider;
			_gameService = gameService;
			_gameService.SetGameController(appController);
			_screenProvider = screenProvider;
		}

		public void Initialize()
		{
			RunnApp().Forget();
		}

		private async UniTask RunnApp()
		{
			bool waitInitComponent = false;

			_screenProvider.OpenWindow(ScreenTypes.Splash);

			await UniTask.WaitUntil(() => _saveGameProvider.IsInitiated);

			_userProvider.Initiate();
			await UniTask.WaitUntil(() => _userProvider.IsInitiated);

			await UniTask.WaitUntil(() => !waitInitComponent);

			_gameService.RunGame();
		}
	}
}