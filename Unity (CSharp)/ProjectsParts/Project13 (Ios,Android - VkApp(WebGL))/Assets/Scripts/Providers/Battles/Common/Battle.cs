using Cysharp.Threading.Tasks;
using Game.Game.Elements.Boards;
using Game.Game.Elements.Scenes;
using Game.Game.Handlers;
using Game.Providers.Battles.Settings;
using Game.Providers.Ui;
using UnityEngine.Events;
using Zenject;

namespace Game.Providers.Battles.Common
{
	public abstract partial class Battle
	{
		public UnityEvent OnStartStage = new();

		protected DiContainer _diContainer;
		protected IUiProvider _uiProvider;
		protected IGameScene _gameScene;
		protected SignalBus _signalBus;
		//protected GameSession _gameSession;
		protected PlayerGameHelper _playerGameHelper;
		protected BoardSpawner _boardSpawner;
		protected IBattleProvider _battleProvider;

		public bool IsActiveBattle { get; protected set; }
		public virtual int Stages { get; protected set; }

		protected Battle(IBattleProvider battleProvider)
		{
			_battleProvider = battleProvider;
		}

		[Inject]
		private void Constructor(
			DiContainer diContainer,
			IUiProvider uiProvider,
			IGameScene gameScene,
			SignalBus signalBus,
			//GameSession gameSession,
			PlayerGameHelper playerGameHelper,
			BoardSpawner boardSpawner
		)
		{
			_diContainer = diContainer;
			_uiProvider = uiProvider;
			_gameScene = gameScene;
			_signalBus = signalBus;
			//_gameSession = gameSession;
			_playerGameHelper = playerGameHelper;
			_boardSpawner = boardSpawner;
		}

		public virtual UniTask StartGame()
		{
			IsActiveBattle = true;
			return default;
		}

		protected async UniTask<BattleTypeSettings> LoadResourcesConfig(string name)
		{
			return await _battleProvider.GetBattleSettings(name);
		}
	}
}
