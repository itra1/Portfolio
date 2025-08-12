using Core.Installers.Base;
using Core.Materials.Loading.AutoPreloader.Types;
using Materials.Loading.AutoPreloader.Types;

namespace Installers
{
	public class MaterialsInstaller : AutoResolvingMonoInstaller<MaterialsInstaller>
	{
		public override void InstallBindings()
		{
			BindInterfacesTo<AutoPreloadedMaterialDataTypes>().AsSingle().NonLazy();
			
			base.InstallBindings();
		}

		protected override void ResolveAll()
		{
			Resolve<IAutoPreloadedMaterialDataTypes>();
			
			base.ResolveAll();
		}
	}
}