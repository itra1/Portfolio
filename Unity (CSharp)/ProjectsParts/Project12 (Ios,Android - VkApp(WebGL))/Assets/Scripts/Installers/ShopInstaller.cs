using Game.Scripts.Providers.Shop;
using Game.Scripts.Providers.Shop.Factorys;
using Zenject;

namespace Game.Scripts.Installers
{
	class ShopInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			_ = Container.BindInterfacesTo<ProductFactory>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<ShopProvider>().AsSingle().NonLazy();
		}
	}
}
