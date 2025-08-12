using Providers.Biometric;
using Providers.User;
using Zenject;
using Providers.Network;
using Providers.Network.Common;
using Providers.SystemMessage;
using Providers.WebView;
using Providers.Splash;

namespace Installers
{
	public class ProvidersInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<SplashProvider>().AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<UserProvider>().AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<NetworkProvider>().AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<BiometricProvider>().AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<SystemMessageProvider>().AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<WebViewProvider>().AsSingle().NonLazy();
		}

		private void Awake()
		{
			Container.Resolve<ISplashProvider>();
			Container.Resolve<IUserProvider>();
			Container.Resolve<IUserAuth>();
			Container.Resolve<INetworkProvider>();
			Container.Resolve<INetworkApi>();
			Container.Resolve<IBiometricProvider>();
			Container.Resolve<ISystemMessage>();
			Container.Resolve<IWebViewProvider>();
			Container.Resolve<IUserProviderRequests>();
		}
	}
}
