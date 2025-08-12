using Core.Installers.Base;
using Core.Workers.Material.Coordinator;
using Core.Workers.Material.Factory;

namespace Core.Installers
{
	public class WorkersCoreInstaller : AutoResolvingMonoInstaller<WorkersCoreInstaller>
	{
		public override void InstallBindings()
		{
			BindInterfacesTo<MaterialWorkerFactory>().AsSingle().NonLazy();
			BindInterfacesTo<MaterialWorkerCoordinator>().AsSingle().NonLazy();
			
			base.InstallBindings();
		}

		protected override void ResolveAll()
		{
			Resolve<IMaterialWorkerFactory>();
			Resolve<IMaterialWorkerCoordinator>();
			
			base.ResolveAll();
		}
	}
}