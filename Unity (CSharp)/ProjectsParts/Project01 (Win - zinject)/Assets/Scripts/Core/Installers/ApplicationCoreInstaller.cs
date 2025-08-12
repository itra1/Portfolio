using Core.App;
using Core.Installers.Base;
using Core.Logging;
using Core.Recovery;

namespace Core.Installers
{
    public class ApplicationCoreInstaller : AutoResolvingMonoInstaller<ApplicationCoreInstaller>
    {
        public override void InstallBindings()
        {
            BindInterfacesTo<ApplicationState>().AsSingle().NonLazy();
            
            BindInterfacesTo<PreviousSessionRecovery>().AsSingle().NonLazy();
            
            base.InstallBindings();
        }
        
        protected override void ResolveAll()
        {
            Resolve<IApplicationState>();
            
            Resolve<IPreviousSessionRecovery>();
            
            base.ResolveAll();
        }
        
        private void OnApplicationQuit()
        {
            Container?.UnbindAll();
            Debug.Dispose();
        }
    }
}