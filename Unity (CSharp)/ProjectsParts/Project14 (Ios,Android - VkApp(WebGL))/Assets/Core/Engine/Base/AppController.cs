using System.Threading.Tasks;
using Core.Engine.App.Common;
using Core.Engine.Components.SaveGame;
using Core.Engine.Signals;
using Core.Engine.uGUI.Screens;
using Zenject;

namespace Core.Engine.App.Base
{

	/// <summary>
	/// Контроллер по управлению игрой
	/// </summary>
	public class AppController :IAppController
	{
		protected IScreensProvider _screenProvider;
		protected string _gameState;
		protected SignalBus _signalBus;

		public string GameState => _gameState;
		public string AppState { get; protected set; }
		public ISaveGameProvider SessionProvider { get; protected set; }

		public AppController(ISaveGameProvider session
		, SignalBus signalBus
		, IScreensProvider screenProvider)
		{
			SessionProvider = session;
			_screenProvider = screenProvider;
			_signalBus = signalBus;

			signalBus.Subscribe<UGUIButtonClickSignal>(OnPlayEvent);
		}

		private void OnPlayEvent(UGUIButtonClickSignal cl)
		{
			if (cl.Name != ButtonTypes.PlayGame)
				return;

			PlayGame();
		}

		/// <summary>
		/// Запуск игрового режима
		/// </summary>
		public void RunGameModule()
		{
			_screenProvider.OpenWindow(ScreenTypes.FirstPage);
		}

		private async void ShwoStart()
		{
			//ShowSplash();
			await Task.Delay(2000);
			//ShowMainMenu();
		}

		public void GameOwer()
		{
			//SetState(AppStateType.EndGame);
			//GameOwerGameUi();
		}

		public virtual void PlayGame()
		{
			_screenProvider.OpenWindow(ScreenTypes.GamePlay);

			//_uiManager.OpenWindow(typeof(IGamePlayWindow));
			SetState(AppGameState.Game);
			//_signalBus.Fire<GameStartSignal>();
			//com.ootii.Messages.MessageDispatcher.SendMessage(EventConstants.GameStart);
		}

		public void SetState(string _targetState)
		{
			AppState = _targetState;
			_signalBus.Fire<GameStateChangeSignal>();
			//com.ootii.Messages.MessageDispatcher.SendMessage(EventConstants.GameStateChange);
		}
	}
}