using Core.FileResources;
using Core.FileResources.Caching;
using Core.FileResources.Command.Factory;
using Core.FileResources.Customizing;
using Core.Installers.Base;

namespace Core.Installers
{
	public class FileResourcesCoreInstaller : AutoResolvingMonoInstaller<FileResourcesCoreInstaller>
	{
		public override void InstallBindings()
		{
			BindInterfacesTo<ResourceRequestCommandFactory>().AsSingle().NonLazy();
			BindInterfacesTo<ResourceCustomizer>().AsSingle().NonLazy();
			BindInterfacesTo<ResourceCachingService>().AsSingle().NonLazy();
			BindInterfacesTo<ResourceProvider>().AsSingle().NonLazy();
			
			base.InstallBindings();
		}
		
		protected override void ResolveAll()
		{
			Resolve<IResourceRequestCommandFactory>();
			Resolve<IResourceCustomizer>();
			Resolve<IResourceCachingService>();
			Resolve<IResourceProvider>();
			
			base.ResolveAll();
		}
	}
}