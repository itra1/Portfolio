using Engine.Assets.Engine.Scripts.Timelines.NotesDestroy;
using Engine.Assets.Engine.Scripts.Timelines.NotesDestroy.Factorys;
using Engine.Scripts.Managers;
using Engine.Scripts.Timelines.Notes.Factorys;
using Game.Scripts.Controllers;
using Game.Scripts.Controllers.Sessions;
using Game.Scripts.Controllers.Sessions.Common;
using Game.Scripts.Helpers;
using Game.Scripts.Managers.Dialogs;
using Game.Scripts.Providers.Addressable;
using Game.Scripts.Providers.Networks;
using Game.Scripts.Scenes;
using Game.Scripts.Scoring;
using Game.Shared;
using Zenject;

namespace Game.Scripts.Installers
{
	public class CoreInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			SignalBusInstaller.Install(Container);

			_ = Container.BindInterfacesTo<Session>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<BattleSession>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<SessionController>().AsSingle().NonLazy();

			_ = Container.BindInterfacesTo<ApplicationSettingsController>().AsSingle().NonLazy();

			_ = Container.BindInterfacesTo<NoreDestroyMeshFactory>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<NoteDestroyFactory>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<NoteDestroySpawner>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<NoteFactory>().AsSingle().NonLazy();

			_ = Container.BindInterfacesTo<NetworkProvider>().AsSingle().NonLazy();

			GameScene scemeObjects = FindAnyObjectByType<GameScene>();
			_ = Container.BindInterfacesTo<GameScene>().FromInstance(scemeObjects).AsSingle().NonLazy();

			_ = Container.BindInterfacesTo<RhythmProcessor>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<RhythmDirector>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<AddressableProvider>().AsSingle().NonLazy();

			_ = Container.BindInterfacesTo<DialogVisibleOrderHelper>().AsSingle().NonLazy();
			_ = Container.Bind<SchedulerManager>().AsSingle().NonLazy();
			_ = Container.Bind<SceneModeHelper>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<DspTime>().AsSingle().NonLazy();
			_ = Container.BindInterfacesAndSelfTo<ScoreManager>().AsSingle().NonLazy();
			_ = Container.BindInterfacesAndSelfTo<PoolManager>().AsSingle().NonLazy();

		}
	}
}
