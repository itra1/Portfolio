using Core.Engine.Services.WVService;
using Zenject;

namespace Core.Engine.Installers
{
	public class WebViewInstaller : MonoInstaller
	{

#if WEBVIEW_SERVICE
		public override void InstallBindings()
		{
			base.InstallBindings();
			Container.BindInterfacesAndSelfTo<WebViewService>().AsSingle().NonLazy();
		}

		private void Awake()
		{
			Container.Resolve<IWebViewService>();
		}

#endif

	}
}