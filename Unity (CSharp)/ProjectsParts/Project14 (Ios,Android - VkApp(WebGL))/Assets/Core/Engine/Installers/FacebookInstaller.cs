using Zenject;

namespace Core.Engine.Installers
{
	public class FacebookInstaller : MonoInstaller
	{
#if FACEBOOK_SERVICE
		public override void InstallBindings()
		{
			Container.Bind<FacebookPermissions>().AsSingle().NonLazy();
			Container.Bind<FacebookShare>().AsSingle().NonLazy();
			Container.Bind<FacebookAppEvent>().AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<FacebookInit>().AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<FacebookLogin>().AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<FacebookService>().AsSingle().NonLazy();
		}

		private void Awake()
		{
			Container.Resolve<IFacebookPermissions>();
			Container.Resolve<IFacebookShare>();
			Container.Resolve<IFacebookAppEvent>();
			Container.Resolve<IFacebookInit>();
			Container.Resolve<IFacebookLogin>();
			Container.Resolve<IFacebookService>();
		}

#endif
	}
}
