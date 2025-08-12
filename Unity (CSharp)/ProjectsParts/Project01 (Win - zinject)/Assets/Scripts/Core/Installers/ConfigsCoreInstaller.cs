using Core.Configs;
using Core.Installers.Base;

namespace Core.Installers
{
	public class ConfigsCoreInstaller : AutoResolvingMonoInstaller<ConfigsCoreInstaller>
	{
		public override void InstallBindings()
		{
			BindInterfacesTo<ConfigProvider>().AsSingle().NonLazy();
			
			BindInterfacesTo<VlcConfigProvider>().AsSingle().NonLazy();
			
			base.InstallBindings();
		}
		
		protected override void ResolveAll()
		{
			Resolve<IConfig>();
			
			Resolve<IVlcConfig>();
			
			base.ResolveAll();
		}
	}
}