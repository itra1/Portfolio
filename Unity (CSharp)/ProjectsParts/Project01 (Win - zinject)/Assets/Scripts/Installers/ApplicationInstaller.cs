using App;
using App.Parsing;
using App.Parsing.Handlers.Factory;
using Core.Installers.Base;

namespace Installers
{
	public class ApplicationInstaller : AutoResolvingMonoInstaller<ApplicationInstaller>
	{
		public override void InstallBindings()
		{
			BindInterfacesTo<CommandLineArgumentHandlerFactory>().AsSingle().NonLazy();
			BindInterfacesTo<CommandLineArgumentsParser>().AsSingle().NonLazy();
			
			BindInterfacesTo<ApplicationLaunchController>().AsSingle().NonLazy();
			BindInterfacesTo<ApplicationPresenter>()
				.FromInstance(FindObjectOfType<ApplicationPresenter>()).AsSingle().NonLazy();
			
			BindInterfacesTo<ApplicationEnvironmentController>().AsSingle().NonLazy();
			
			base.InstallBindings();
		}

		protected override void ResolveAll()
		{
			Resolve<ICommandLineArgumentHandlerFactory>();
			Resolve<ICommandLineArgumentsParser>();
			
			Resolve<IApplicationLaunchController>();
			Resolve<IApplicationPresenter>();
			
			Resolve<IApplicationEnvironmentController>();
			
			base.ResolveAll();
		}
	}
}