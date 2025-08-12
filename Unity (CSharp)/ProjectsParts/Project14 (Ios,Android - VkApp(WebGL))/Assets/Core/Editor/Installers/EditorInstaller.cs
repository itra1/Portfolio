using UnityEditor;
using Zenject;

namespace Editor.Installers {
	[InitializeOnLoad]
	public class EditorInstaller : EditorStaticInstaller<EditorInstaller> {
		static EditorInstaller() {
			EditorApplication.QueuePlayerLoopUpdate();
			Install();
		}

		public override void InstallBindings() {
			//_ = Container.BindInterfacesAndSelfTo(typeof(SceneComponentsBase)).FromInstance(GameObject.FindAnyObjectByType(typeof(SceneComponentsBase))).AsSingle().NonLazy();
			//var app = Resources.Load<AppSettings>("AppSettings");
			//app.InstallBindings();
			//var game = Resources.Load<GameSettings>("GameSettings");
			//game.InstallBindings();

			//_ = Container.BindInterfacesTo<AppSettings>()
			//	.FromInstance(Resources.Load<AppSettings>("AppSettings"))
			//	.AsSingle()
			//	.NonLazy();

			//_ = Container.BindInterfacesTo<PrefabSettings>()
			//	.FromInstance(Resources.Load<PrefabSettings>("PrefabSettings"))
			//	.AsSingle()
			//	.NonLazy();

			//_ = Container.BindInterfacesAndSelfTo<AvatarsProvider>().AsSingle().NonLazy();
			//_ = Container.BindInterfacesAndSelfTo<ThemeProvider>().AsSingle().NonLazy();

			ResolveAll();
		}

		private void ResolveAll() {
			//_ = Container.Resolve<IAppSettings>();
			//_ = Container.Resolve<IPopupSettings>();
			//_ = Container.Resolve<IScreenSettings>();
			//_ = Container.Resolve<IShopSettings>();
			//_ = Container.Resolve<IPrefabSettings>();
			//_ = Container.Resolve<IAvatarsProvider>();
			//_ = Container.Resolve<IThemeProvider>();
		}
	}
}
