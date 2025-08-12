using Core.Engine.Settings.Common;
using UnityEngine;
using Zenject;

namespace Core.Engine.App.Settings {
	[CreateAssetMenu(fileName = "AppSettings", menuName = "App/Create/Settings/AppSettings", order = 2)]
	public class AppSettings : ScriptableObjectInstaller {

		[SerializeField] private Paths _paths;
		[SerializeField] private Links _links;

		public override void InstallBindings() {
			_ = Container.BindInterfacesAndSelfTo<Paths>().FromInstance(_paths).NonLazy();
			_ = Container.BindInterfacesAndSelfTo<Links>().FromInstance(_links).NonLazy();

			ResolveAll();
		}

		private void ResolveAll() {
			//_ = Container.Resolve<IAppSettings>();
			//_ = Container.Resolve<IScreenSettings>();
			//_ = Container.Resolve<IPopupSettings>();
			//_ = Container.Resolve<IShopSettings>();
			//_ = Container.Resolve<ISoundSettings>();
			//_ = Container.Resolve<ISkinSettings>();
			//_ = Container.Resolve<IAvatarsSettings>();
			//_ = Container.Resolve<ILeaderboardSettings>();
		}

		[System.Serializable]
		public class Paths : IAppPaths {
			[SerializeField] private AnimationCurve _popapCurveAnimation;
			[SerializeField] private string _windowFolder = "Prefabs/UI/Screens";
			[SerializeField] private string _popupFolder = "Prefabs/UI/Popups";
			[SerializeField] private string _shopProductsFolder = "Prefabs/Shop";
			[SerializeField] private string _skinFolder = "Prefabs/Skins";
			[SerializeField] private string _themesPath = "Prefabs/Themes";
			[SerializeField] private string _soundSettings = "Sounds/Settings";
			[SerializeField] private string _leaderboardSettings = "Leaderboard/LeaderboardSettings";
			[SerializeField] private string _avatarsFolder = "Avatars";

			public AnimationCurve PopupCurveAnimation => _popapCurveAnimation;
			public string ScreensFolder => _windowFolder;
			public string PopupFolder => _popupFolder;
			public string ShopProductsFolder => _shopProductsFolder;
			public string SkinFolder => _skinFolder;
			public string SoundSettingsFolder => _soundSettings;
			public string LeaderboardSettings => _leaderboardSettings;
			public string AvatarsFolder => _avatarsFolder;
			public string ThemesPath => _themesPath;
		}

		[System.Serializable]
		public class Links : IPolicyUrl {
			[SerializeField] private string _policyUrl;
			public string PolicyUrl => _policyUrl;
		}
	}
}