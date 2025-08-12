using Core.Installers.Base;
using Core.Materials.Loading.AutoPreloader;
using Core.Materials.Loading.Loader;
using Core.Materials.Loading.Loader.Command.Factory;
using Core.Materials.Parsing;
using Core.Materials.Storage;

namespace Core.Installers
{
	public class MaterialsCoreInstaller : AutoResolvingMonoInstaller<MaterialsCoreInstaller>
	{
		public override void InstallBindings()
		{
			BindInterfacesTo<MemberInfoHelper>().AsSingle().NonLazy();
			
			BindInterfacesTo<MaterialDataStorage>().AsSingle().NonLazy();
			BindInterfacesTo<MaterialDataParsingHelper>().AsSingle().NonLazy();
			BindInterfacesTo<MaterialDataParser>().AsSingle().NonLazy();
			BindInterfacesTo<AutoMaterialDataPreloader>().AsSingle().NonLazy();
			
			BindInterfacesTo<MaterialDataLoadingCommandFactory>().AsSingle().NonLazy();
			BindInterfacesTo<MaterialDataLoader>().AsSingle().NonLazy();
			
			base.InstallBindings();
		}

		protected override void ResolveAll()
		{
			Resolve<IMemberInfoHelper>();
			
			Resolve<IMaterialDataStorage>();
			Resolve<IMaterialDataParsingHelper>();
			Resolve<IMaterialDataParser>();
			Resolve<IAutoMaterialDataPreloader>();
			
			Resolve<IMaterialDataLoadingCommandFactory>();
			Resolve<IMaterialDataLoader>();
			
			base.ResolveAll();
		}
	}
}