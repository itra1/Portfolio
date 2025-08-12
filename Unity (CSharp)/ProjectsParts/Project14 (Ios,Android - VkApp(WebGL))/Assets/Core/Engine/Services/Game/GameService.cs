using Core.Engine.App.Base;
using Zenject;

namespace Core.Engine.Services
{
	public class GameService :IGameService, IInitializable
	{
		public bool IsInitComplete { get; set; } = false;
		private IAppController _gameController;
		private DiContainer _diContainer;

		public GameService(DiContainer diContainer)
		{
			_diContainer = diContainer;
		}
		public void SetGameController(IAppController gameController)
		{
			_gameController = gameController;
		}

		public void Initialize()
		{
#if !WEBVIEW_SERVICE
			//SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
#endif
		}

		public void RunGame()
		{
			_gameController.RunGameModule();
		}

	}
}