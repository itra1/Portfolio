using Core.Installers.Base;
using Core.User;
using Core.User.Installation;
using Core.User.Installation.Loading;
using Core.User.Installation.Parsing;

namespace Core.Installers
{
	public class UserCoreInstaller : AutoResolvingMonoInstaller<UserCoreInstaller>
	{
		public override void InstallBindings()
		{
			BindInterfacesTo<UserProfile>().AsSingle().NonLazy();
			BindInterfacesTo<UserInstallation>().AsSingle().NonLazy();
			BindInterfacesTo<UserInstallationParser>().AsSingle().NonLazy();
			BindInterfacesTo<UserInstallationPreloader>().AsSingle().NonLazy();
			
			base.InstallBindings();
		}

		protected override void ResolveAll()
		{
			Resolve<IUserProfile>();
			Resolve<IUserInstallation>();
			Resolve<IUserInstallationSetter>();
			Resolve<IUserInstallationParser>();
			Resolve<IUserInstallationPreloader>();
			
			base.ResolveAll();
		}
	}
}