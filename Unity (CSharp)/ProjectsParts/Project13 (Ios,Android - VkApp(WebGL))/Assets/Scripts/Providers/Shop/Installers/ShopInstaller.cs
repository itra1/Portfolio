using Zenject;

namespace Game.Providers.Shop.Installers
{
	public class ShopInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			//_ = Container.BindInterfacesTo<ShopProductHelper>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<ShopProvider>().AsSingle().NonLazy();
		}
	}
}
