using Core.Engine.App.Settings;
using Core.Engine.Components.Audio;
using Core.Engine.Components.Avatars;
using Core.Engine.Components.Game;
using Core.Engine.Components.Leaderboard;
using Core.Engine.Components.SaveGame;
using Core.Engine.Components.Themes.Common;
using Core.Engine.Components.Timers;
using Core.Engine.Components.User;
using Core.Engine.Components.Vibrations;
using Core.Engine.Settings.Common;
using Core.Engine.uGUI.Popups;
using Core.Engine.uGUI.Screens;
using UnityEngine;
using Zenject;

namespace Core.Engine.Installers {
	public class AppInstaller : MonoInstaller {
		public override void InstallBindings() {

			_ = Container.BindInterfacesTo<PrefabSettings>()
				.FromInstance(Resources.Load<PrefabSettings>("PrefabSettings"))
				.AsSingle()
				.NonLazy();

			_ = Container.BindInterfacesTo(typeof(SceneComponentsBase)).FromInstance(FindAnyObjectByType(typeof(SceneComponentsBase))).AsSingle().NonLazy();
			_ = Container.BindInterfacesTo(typeof(ThemeComponents)).FromInstance(FindAnyObjectByType(typeof(ThemeComponents))).AsSingle().NonLazy();

			_ = Container.BindInterfacesAndSelfTo<SaveGameProvider>().AsSingle().NonLazy();
			_ = Container.BindInterfacesAndSelfTo<TimersProvider>().AsSingle().NonLazy();
			_ = Container.BindInterfacesAndSelfTo<AvatarsProvider>().AsSingle().NonLazy();
			_ = Container.BindInterfacesAndSelfTo<AudioProvider>().AsSingle().NonLazy();
			_ = Container.BindInterfacesAndSelfTo<PlayAudio>().AsSingle().NonLazy();
			_ = Container.BindInterfacesAndSelfTo<VibrationProvider>().AsSingle().NonLazy();
			_ = Container.BindInterfacesAndSelfTo<LeaderboardProvider>().AsSingle().NonLazy();
			_ = Container.BindInterfacesAndSelfTo<UserProvider>().AsSingle().NonLazy();
		}

		private void Awake() {
			_ = Container.Resolve<IPolicyUrl>();
			_ = Container.Resolve<IPrefabSettings>();
			_ = Container.Resolve<IScreensParent>();
			_ = Container.Resolve<IPopupsParent>();

			_ = Container.Resolve<ISaveGameProvider>();
			_ = Container.Resolve<ITimersProvider>();
			_ = Container.Resolve<IAudioProvider>();
			_ = Container.Resolve<IPlayAudio>();
			_ = Container.Resolve<IVibrationProvider>();
			_ = Container.Resolve<IUserProvider>();
			_ = Container.Resolve<IThemeComponents>();
		}
	}
}