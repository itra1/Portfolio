using Core.Engine.App.Base;
using Core.Engine.Base;
using Core.Engine.Components.DailyBonus;
using Core.Engine.Components.GameQuests;
using Core.Engine.Components.Shop;
using Core.Engine.Components.Skins;
using Core.Engine.Components.Themes;
using Core.Engine.Components.User;
using Core.Engine.Services;
using Core.Engine.Signals;
using Core.Engine.uGUI.Popups;
using Core.Engine.uGUI.Screens;
using System;
using System.Linq;
using Zenject;

namespace Core.Engine.Installers {
	public class CoreInstaller : MonoInstaller {

		[Inject]
		private UserProvider _userProvider;
		public override void InstallBindings() {
			SignalBusInstaller.Install(Container);

			//_ = Container.BindInterfacesAndSelfTo<IGameSettings>()
			//	.FromInstance(Resources.Load<ScriptableObject>("GameSettings"))
			//	.AsSingle()
			//	.NonLazy();

			_ = Container.BindInterfacesAndSelfTo<DailyBonusProvider>().AsSingle().NonLazy();
			_ = Container.BindInterfacesAndSelfTo<ShopProvider>().AsSingle().NonLazy();

			_ = Container.BindInterfacesAndSelfTo<SkinProvider>().AsSingle().NonLazy();

			_ = Container.BindInterfacesAndSelfTo<ThemeProvider>().AsSingle().Lazy();

			_ = Container.BindInterfacesAndSelfTo<GameQuestsProvider>().AsSingle().NonLazy();

			_ = Container.Bind<ScreenFactory>().AsSingle().NonLazy();
			_ = Container.BindInterfacesAndSelfTo<ScreensProvider>().AsSingle().NonLazy();

			_ = Container.Bind<PopupFactory>().AsSingle().NonLazy();
			_ = Container.BindInterfacesAndSelfTo<PopupProvider>().AsSingle().NonLazy();

			_ = Container.BindInterfacesAndSelfTo<AppController>().AsSingle().NonLazy();
			_ = Container.BindInterfacesAndSelfTo<GameService>().AsSingle().NonLazy();

			_ = Container.BindInterfacesAndSelfTo<AppRunner>().AsSingle().NonLazy();

			BindSignals();

		}

		private void BindSignals() {

			var playerAssemblies = AppDomain.CurrentDomain.GetAssemblies();

			foreach (var assamble in playerAssemblies) {
				var types = from t in assamble.GetTypes()
										where t.GetInterfaces().Contains(typeof(ISignal))
										select t;

				foreach (var elemClass in types) {
					_ = Container.DeclareSignal(elemClass);
				}
			}
		}

		protected void Awake() {
			_ = Container.Resolve<IDailyBonusProvider>();
			_ = Container.Resolve<ISkinProvider>();
			_ = Container.Resolve<IGameQuestProvider>();
			_ = Container.Resolve<IGameService>();
			_ = Container.Resolve<IThemeProvider>();
			//_ = Container.Resolve<IGameSettings>();
			var screenProvider = Container.Resolve<IScreensProvider>();
			_ = Container.Resolve<IAppController>();
			var popupProvider = Container.Resolve<IPopupProvider>();

			_userProvider.InitComponents(popupProvider, screenProvider);
		}

	}
}