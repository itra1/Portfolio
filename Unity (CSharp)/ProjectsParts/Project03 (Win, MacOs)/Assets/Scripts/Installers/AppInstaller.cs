
using Zenject;
using UnityEngine;
using Settings;
using Settings.Prefabs;
using Common;
using UGui.Screens.Elements;
using Providers.SystemMessage.Common;
using Providers.SystemMessage.Presenters;
using Providers.Splash.Presenter;
using Providers.Splash.Common;

namespace Installers
{
	public class AppInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<Settings.Settings>()
				.FromInstance(Resources.Load<Settings.Settings>("Settings"))
				.AsSingle()
				.NonLazy();

			Container.BindInterfacesTo<PrefabSettings>()
				.FromInstance(Resources.Load<PrefabSettings>("PrefabSettings"))
				.AsSingle()
				.NonLazy();

			var sceneConponent = (SceneComponents)FindAnyObjectByType(typeof(SceneComponents));
			var systemMessageParent = sceneConponent.SystemMessagesParent.GetComponent<SystemMessagePresenter>();
			var splash = sceneConponent.SplashPresenter;

			Container.BindInterfacesAndSelfTo(typeof(SceneComponents)).FromInstance(sceneConponent).AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo(typeof(SystemMessagePresenter)).FromInstance(systemMessageParent).AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo(typeof(SplashPresenter)).FromInstance(splash).AsSingle().NonLazy();
		}

		public void Awake()
		{
			Container.Resolve<IScreenParent>();
			Container.Resolve<ISystemMessagesParent>();
			Container.Resolve<ISystemMessageVisible>();
			Container.Resolve<ISettings>();
			Container.Resolve<ISplash>();
		}

	}

}
