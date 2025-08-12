using System;
using Core.Installers.Base;
using Preview;
using Preview.Stages;
using ScreenStreaming;
using ScreenStreaming.Parameters;
using ScreenStreaming.Sender.Factory;
using ScreenStreaming.Sender.Source.Factory;

namespace Installers
{
    public class ServicesInstaller : AutoResolvingMonoInstaller<ServicesInstaller>
    {
        public override void InstallBindings()
        {
#if UNITY_EDITOR
            BindInterfacesTo<ScreenStreamingConfig>().AsSingle().NonLazy();
#else
			BindInterfacesTo<ScreenStreamingOptions>().AsSingle().NonLazy();
#endif
            BindInterfacesTo<ApplicationVideoStreamSourceFactory>().AsSingle().NonLazy();
            BindInterfacesTo<ApplicationVideoStreamSenderFactory>().AsSingle().NonLazy();
            BindInterfacesTo<ScreenStreamingController>().AsSingle().NonLazy();
            
            BindInterfacesTo<PreviewImageEncoder>().AsSingle().NonLazy();
            Bind(typeof(IPreviewStages), typeof(IDisposable))
                .FromInstance(FindObjectOfType<PreviewStages>(true)).AsSingle().NonLazy();
            BindInterfacesTo<PreviewProvider>().AsSingle().NonLazy();
            
            base.InstallBindings();
        }
        
        protected override void ResolveAll()
        {
            Resolve<IScreenStreamingParameters>();
            Resolve<IApplicationVideoStreamSourceFactory>();
            Resolve<IApplicationVideoStreamSenderFactory>();
            Resolve<IScreenStreamingController>();
            
            Resolve<IPreviewImageEncoder>();
            Resolve<IPreviewStages>();
            Resolve<IPreviewProvider>();
            
            base.ResolveAll();
        }
    }
}