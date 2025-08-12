using Game.Common.Settings;
using Game.Providers.Ui.Popups;
using Game.Providers.Ui.Windows;
using UnityEngine;
using Zenject;

namespace Game.Providers.Ui.Settings
{
	[CreateAssetMenu(fileName = "UiSettings", menuName = "App/Create/Settings/UiSettings", order = 2)]
	public class UiSettings : ScriptableObjectInstaller<AppSettings>
	{
		public WindowsPresentersSettings _windowsPresentersSettings;
		public WindowsSettings WindowsSettings;
		public PopupSettings PopupSettings;
		public FlyingResourcesSettings FlyingResourcesSettings;
		public UiElements Elements;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<WindowsSettings>().FromInstance(WindowsSettings).IfNotBound();
			Container.BindInterfacesAndSelfTo<PopupSettings>().FromInstance(PopupSettings).IfNotBound();
			Container.BindInterfacesAndSelfTo<FlyingResourcesSettings>().FromInstance(FlyingResourcesSettings).IfNotBound();
			Container.Bind<UiElements>().FromInstance(Elements).IfNotBound();
			_ = Container.BindInterfacesTo<WindowsPresentersSettings>().FromInstance(_windowsPresentersSettings).AsSingle().NonLazy();

			ResolveAll();
		}

		private void ResolveAll()
		{
			//_ = Container.Resolve<IWindowsSettings>();
			//_ = Container.Resolve<IPopupSettings>();
		}
	}
}