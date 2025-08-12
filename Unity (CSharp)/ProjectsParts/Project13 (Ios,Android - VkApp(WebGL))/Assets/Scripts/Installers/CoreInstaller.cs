using System;
using System.Linq;
using System.Reflection;
using Game.Common.Signals;
using Game.Game;
using Game.Game.Base;
using Game.Game.Components;
using Game.Game.Elements.Scenes;
using Game.Providers.Audio;
using Game.Providers.Audio.Common;
using Game.Providers.Audio.Handlers;
using Game.Providers.Audio.Settings;
using Game.Providers.Avatars;
using Game.Providers.Battles;
using Game.Providers.Battles.Helpers;
using Game.Providers.DailyBonus;
using Game.Providers.Nicknames;
using Game.Providers.Profile;
using Game.Providers.PushNotifications;
using Game.Providers.Saves;
using Game.Providers.Telegram.Handlers;
using Game.Providers.TimeBonuses;
using Game.Providers.Timers;
using Game.Providers.Ui;
using Game.Providers.Ui.Controllers.Factorys;
using Game.Providers.Ui.Popups;
using Game.Providers.Ui.Popups.Base;
using Game.Providers.Ui.Popups.Factorys;
using Game.Providers.Ui.Presenters.Factorys;
using Game.Providers.Ui.Windows;
using Game.Providers.Ui.Windows.Base;
using Game.Providers.Ui.Windows.Factorys;
using Game.Scenes;
using Zenject;

namespace Game.Installers
{
	public class CoreInstaller : MonoInstaller
	{
		[Inject][UnityEngine.HideInInspector] public AudioProviderSettings _audioSettings;
		public override void InstallBindings()
		{
			SignalBusInstaller.Install(Container);
			BindSignals();

			var sceneProvider = FindFirstObjectByType<BeginSceneProvider>();
			_ = Container.BindInterfacesTo<BeginSceneProvider>().FromInstance(sceneProvider).AsSingle().NonLazy();

			var blackMask = FindFirstObjectByType<BlackRoundMask>();
			_ = Container.BindInterfacesAndSelfTo<BlackRoundMask>().FromInstance(blackMask).AsSingle().NonLazy();

			_ = Container.BindInstance<IGameScene>(sceneProvider.GameScene);

			_ = Container.Bind<PushNotificationsProvider>().AsSingle().NonLazy();

			_ = Container.Bind<TelegramHandler>().AsSingle().NonLazy();

			_ = Container.BindInterfacesAndSelfTo<AudioProvider>().AsSingle().NonLazy();
			_ = Container.Bind<AudioSettingsHandler>().AsSingle().NonLazy();
			_ = Container.Bind<AudioHandler>().AsSingle().NonLazy();
			_ = Container.BindFactory<AudioSourcePoint, AudioSourcePoint.Factory>()
			.FromPoolableMemoryPool<AudioSourcePoint, AudioSourcePointPool>(poolBinder => poolBinder
				.WithInitialSize(5)
				.FromComponentInNewPrefab(_audioSettings.AudioSourcePrefab)
				.UnderTransformGroup("AudioPoints")
				);

			_ = Container.BindInterfacesTo<TimersProvider>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<TimeBonusProvider>().AsSingle().NonLazy();

			_ = Container.BindInterfacesTo<NicknamesProvider>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<SaveProvider>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<ProfileProvider>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<BattleProvider>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<BattleHelper>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<AvatarsProvider>().AsSingle().NonLazy();

			_ = Container.Bind<DailyBonusHandler>().AsSingle().NonLazy();
			_ = Container.BindInterfacesAndSelfTo<DailyBonusProvider>().AsSingle().NonLazy();
			_ = Container.Bind<WindowFactory>().AsSingle().NonLazy();
			_ = Container.BindInterfacesAndSelfTo<WindowProvider>().AsSingle().NonLazy();
			_ = Container.Bind<PopupFactory>().AsSingle().NonLazy();
			_ = Container.BindInterfacesAndSelfTo<PopupProvider>().AsSingle().NonLazy();

			_ = Container.BindInterfacesAndSelfTo<WindowPresenterFactory>().AsSingle().NonLazy();
			_ = Container.BindInterfacesAndSelfTo<WindowPresenterControllerFactory>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<UiProvider>().AsSingle().NonLazy();

			_ = Container.BindInterfacesAndSelfTo<GameProvider>().AsSingle().NonLazy();

			//Resolve();
		}

		private void BindSignals()
		{

			var playerAssemblies = AppDomain.CurrentDomain.GetAssemblies();

			foreach (var assamble in playerAssemblies)
			{
				var types = from t in Assembly.GetExecutingAssembly().GetTypes()
										where t.GetInterfaces().Contains(typeof(ISignal))
										select t;

				foreach (var elemClass in types)
				{
					_ = Container.DeclareSignal(elemClass);
				}
			}
		}

		private void Resolve()
		{
			_ = Container.Resolve<IPopupsParent>();
			_ = Container.Resolve<IWindowsParent>();
			_ = Container.Resolve<IWorldParent>();
			_ = Container.Resolve<IWindowsParent>();
			_ = Container.Resolve<IWindowProvider>();
			_ = Container.Resolve<IPopupsProvider>();
			_ = Container.Resolve<IGameProvider>();
		}
		public class AudioSourcePointPool : MonoPoolableMemoryPool<IMemoryPool, AudioSourcePoint> { }
	}
}