using Core.Installers.Base;
using Core.Options;
using Core.Options.Offsets;

namespace Core.Installers
{
	public class OptionsCoreInstaller : AutoResolvingMonoInstaller<OptionsCoreInstaller>
	{
		public override void InstallBindings()
		{
			BindInterfacesTo<ApplicationOptions>().AsSingle().NonLazy();
			BindInterfacesTo<ScreenOffsets>().AsSingle().NonLazy();
			
			base.InstallBindings();
		}

		protected override void ResolveAll()
		{
			Resolve<IApplicationOptions>();
			Resolve<IApplicationOptionsInfo>();
			Resolve<IApplicationOptionsSetter>();
			Resolve<IScreenOffsets>();
			Resolve<IScreenOffsetsSetter>();
			
			base.ResolveAll();
		}
	}
}