
using Services.Network;

using Zenject;

namespace Installers
{
	public class ServicesInstaller: MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<NetworkProvider>().AsSingle().NonLazy();
		}

		private void Awake()
		{
			Container.Resolve<INetworkProvider>();
		}
	}
}
