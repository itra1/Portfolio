using Core.Installers.Base;
using FileResources;

namespace Installers
{
    public class FileResourcesInstaller : AutoResolvingMonoInstaller<FileResourcesInstaller>
    {
        public override void InstallBindings()
        {
            BindInterfacesTo<TextureProvider>().AsSingle().NonLazy();
            
            base.InstallBindings();
        }
		
        protected override void ResolveAll()
        {
            Resolve<ITextureProvider>();
            
            base.ResolveAll();
        }
    }
}