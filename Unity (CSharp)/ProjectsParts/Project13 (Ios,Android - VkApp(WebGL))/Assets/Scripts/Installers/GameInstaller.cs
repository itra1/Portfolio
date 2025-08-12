using Game.Base.AppLaoder;
using Game.Game;
using Game.Game.Common;
using Game.Game.Elements.Barriers;
using Game.Game.Elements.Boards;
using Game.Game.Elements.Bonuses;
using Game.Game.Elements.Scenes;
using Game.Game.Elements.Weapons;
using Game.Game.Elements.Weapons.Factorys;
using Game.Game.Handlers;
using Game.Game.Settings;
using Game.Providers.Battles.Settings;
using Game.Providers.Profile.Common;
using Game.Providers.Profile.Handlers;
using Game.Providers.Smiles;
using Game.Providers.Smiles.Handlers;
using Game.Providers.TimeBonuses.Base;
using Game.Providers.TimeBonuses.Handlers;
using Game.Providers.Ui.Elements;
using Game.Providers.Ui.Factorys;
using Game.Providers.Ui.Settings;
using Zenject;

namespace Game.Installers
{
	public class GameInstaller : MonoInstaller
	{
		[Inject][UnityEngine.HideInInspector] public GameSettings _gameSettings;
		[Inject][UnityEngine.HideInInspector] public UiElements _uiPrefabs;
		[Inject][UnityEngine.HideInInspector] public IGameScene _gameScene;

		public override void InstallBindings()
		{
			_ = Container.BindFactory<RoundIndicator, RoundIndicator.Factory>()
			.FromPoolableMemoryPool<RoundIndicator, RoundIndicatorPool>(poolBinder => poolBinder
				.WithInitialSize(10)
				.FromComponentInNewPrefab(_gameSettings.RoundIndicatorPrefab)
				);

			_ = Container.BindFactory<BoardsSettings.BoardItem, Board, Board.Factory>()
			.FromPoolableMemoryPool<BoardsSettings.BoardItem, Board, BoardPool>(poolBinder => poolBinder
				.WithInitialSize(2)
				.FromComponentInNewPrefab(_gameSettings.BoardPrefab)
				.UnderTransform(_gameScene.BoardPoint)
				);

			_ = Container.BindInterfacesTo<WeaponFactory>().AsSingle().NonLazy();

			_ = Container.Bind<UiPlayerResourcesFactory>().AsSingle().NonLazy();

			//_ = Container.Bind<PointsModificatorHandler>().AsSingle().NonLazy();
			_ = Container.Bind<BoardFactory>().AsSingle().NonLazy();
			_ = Container.Bind<BonusFactory>().AsSingle().NonLazy();
			_ = Container.Bind<BarrierFactory>().AsSingle().NonLazy();

			_ = Container.BindInterfacesTo<WeaponSpawner>().AsSingle().NonLazy();
			_ = Container.Bind<BoardSpawner>().AsSingle().NonLazy();
			_ = Container.Bind<WeaponHelper>().AsSingle().NonLazy();

			_ = Container.Bind<CoinsHandler>().AsSingle().NonLazy();
			_ = Container.Bind<DollarHandler>().AsSingle().NonLazy();
			_ = Container.Bind<PlayerLevelHandler>().AsSingle().NonLazy();
			_ = Container.Bind<ExperienceHandler>().AsSingle().NonLazy();
			_ = Container.Bind<PlayerResourcesHandler>().AsSingle().NonLazy();
			_ = Container.Bind<LevelRewardsHandler>().AsSingle().NonLazy();
			_ = Container.Bind<TimeBonusHandler>().AsSingle().NonLazy();

			_ = Container.Bind<PlayerGameHelper>().AsSingle().NonLazy();
			_ = Container.Bind<OpenSettingsHandler>().AsSingle().NonLazy();
			_ = Container.Bind<OpenAddDollarHandler>().AsSingle().NonLazy();
			_ = Container.Bind<OpenAddCoinsHandler>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<SmilesProvider>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<SmileHndler>().AsSingle().NonLazy();
			_ = Container.BindInterfacesAndSelfTo<GameSession>().AsSingle().NonLazy();

			_ = Container.BindInterfacesAndSelfTo<AppRun>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<AppLoaderHandler>().AsSingle().NonLazy();

			_ = Container.BindFactory<AvatarItemView, AvatarItemView.Factory>()
			.FromPoolableMemoryPool<AvatarItemView, AvatarItemViewPool>(poolBinder => poolBinder
				.WithInitialSize(2)
				.FromComponentInNewPrefab(_uiPrefabs.AvatarViewPrefab)
				.UnderTransformGroup("UiElements")
				);

			_ = Container.BindFactory<HitIndicatorView, HitIndicatorView.Factory>()
			.FromPoolableMemoryPool<HitIndicatorView, HitIndicatorViewPool>(poolBinder => poolBinder
				.WithInitialSize(2)
				.FromComponentInNewPrefab(_uiPrefabs.HitIndicatorPrefab)
				.UnderTransformGroup("UiElements")
				);

			_ = Container.BindFactory<int, bool, StageIndicatorView, StageIndicatorView.Factory>()
			.FromPoolableMemoryPool<int, bool, StageIndicatorView, StageIndicatorViewPool>(poolBinder => poolBinder
				.WithInitialSize(10)
				.FromComponentInNewPrefab(_uiPrefabs.StageIndicatorPrefab)
				.UnderTransformGroup("UiElements")
				);

			_ = Container.BindFactory<DuelItemSettings, TournamentView, TournamentView.Factory>()
			.FromPoolableMemoryPool<DuelItemSettings, TournamentView, TournamentItemViewPool>(poolBinder => poolBinder
				.WithInitialSize(6)
				.FromComponentInNewPrefab(_uiPrefabs.TournamentViewPrefab)
				.UnderTransformGroup("UiElements")
				);

			_ = Container.BindFactory<BattleResult, TournamentResultView, TournamentResultView.Factory>()
			.FromPoolableMemoryPool<BattleResult, TournamentResultView, TournamentResultViewPool>(poolBinder => poolBinder
				.WithInitialSize(10)
				.FromComponentInNewPrefab(_uiPrefabs.TournamentResultViewPrefab)
				.UnderTransformGroup("UiElements")
				);

			_ = Container.BindFactory<ITimeBonus, TimeBonusView, TimeBonusView.Factory>()
			.FromPoolableMemoryPool<ITimeBonus, TimeBonusView, GiftViewPool>(poolBinder => poolBinder
				.WithInitialSize(2)
				.FromComponentInNewPrefab(_uiPrefabs.GiftViewPrefab)
				.UnderTransformGroup("UiElements")
				);

			_ = Container.BindFactory<PlayerLevel, PlayerLevelItemView, PlayerLevelItemView.Factory>()
			.FromPoolableMemoryPool<PlayerLevel, PlayerLevelItemView, PlayerLevelItemViewPool>(poolBinder => poolBinder
				.WithInitialSize(25)
				.FromComponentInNewPrefab(_uiPrefabs.PlayerLevelViewPrefab)
				.UnderTransformGroup("UiElements")
				);

		}

		public class AvatarItemViewPool : MonoPoolableMemoryPool<IMemoryPool, AvatarItemView> { }
		public class RoundIndicatorPool : MonoPoolableMemoryPool<IMemoryPool, RoundIndicator> { }
		public class BoardPool : MonoPoolableMemoryPool<BoardsSettings.BoardItem, IMemoryPool, Board> { }
		public class HitIndicatorViewPool : MonoPoolableMemoryPool<IMemoryPool, HitIndicatorView> { }
		public class StageIndicatorViewPool : MonoPoolableMemoryPool<int, bool, IMemoryPool, StageIndicatorView> { }
		public class TournamentItemViewPool : MonoPoolableMemoryPool<DuelItemSettings, IMemoryPool, TournamentView> { }
		public class TournamentResultViewPool : MonoPoolableMemoryPool<BattleResult, IMemoryPool, TournamentResultView> { }
		public class GiftViewPool : MonoPoolableMemoryPool<ITimeBonus, IMemoryPool, TimeBonusView> { }
		public class PlayerLevelItemViewPool : MonoPoolableMemoryPool<PlayerLevel, IMemoryPool, PlayerLevelItemView> { }
	}
}
