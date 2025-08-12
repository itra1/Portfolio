using Core.Engine.Providers;
using Zenject;

namespace Core.Engine.Installers
{
	public class NetworkInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<NetworkProvider>().AsSingle().Lazy();
		}

		private void Awake()
		{
			Container.Resolve<INetworkProvider>();
		}
	}
}