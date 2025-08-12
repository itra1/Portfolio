using Core.Installers.Base;
using Elements.Common.Presenter.VlcEngine.Caching;
using Elements.Common.Presenter.VlcEngine.Factory;
using Elements.StatusColumn.Controller.Helper;
using Elements.StatusColumn.Controller.Playlist.Factory;
using Elements.StatusColumn.Presenter.Components.Factory;
using Elements.StatusTabItem.Controller.Buffer;
using Elements.Windows.Common.Presenter.Commands.Interpreter;
using Elements.Windows.Common.Presenter.Selection;
using Elements.Windows.Pdf.Presenter.Adapters.Components.Factory;
using Elements.Windows.Pdf.Presenter.Adapters.Factory;
using Elements.Windows.Video.Presenter.VideoPlayer.Factory;
using Elements.Windows.VideoFile.Presenter.VideoPlayer.Factory;
using Elements.Windows.VideoSplit.Presenter.VideoPlayer.Factory;
using Elements.Windows.WebView.Presenter.Authorization;
using Elements.Windows.WebView.Presenter.Preloading;
using Elements.Windows.WebView.Presenter.Zooming;

namespace Installers
{
	public class ElementAddonsInstaller : AutoResolvingMonoInstaller<ElementAddonsInstaller>
	{

		public override void InstallBindings()
		{
			_ = BindInterfacesTo<WindowSelectionController>().AsSingle().NonLazy();

			_ = BindInterfacesTo<WindowCommandInterpreter>().AsSingle().NonLazy();

			_ = BindInterfacesTo<HostAuthorizationInfo>().AsSingle().NonLazy();

			_ = BindInterfacesTo<WebViewZooming>().AsSingle().NonLazy();
			_ = BindInterfacesTo<WebViewPreloadCoordinator>().AsSingle().NonLazy();

			_ = BindInterfacesTo<PdfAsPicturePageFactory>().AsSingle().NonLazy();
			_ = BindInterfacesTo<PdfViewAdapterFactory>().AsSingle().NonLazy();

			_ = BindInterfacesTo<VideoPlayerFactory>().AsSingle().NonLazy();
			_ = BindInterfacesTo<VideoFilePlayerFactory>().AsSingle().NonLazy();
			_ = BindInterfacesTo<VideoSplitPlayerFactory>().AsSingle().NonLazy();

			_ = BindInterfacesTo<VlcLibraryFactory>().AsSingle().NonLazy();
			_ = BindInterfacesTo<VlcMediaPlayerFactory>().AsSingle().NonLazy();
			_ = BindInterfacesTo<VlcMediaFactory>().AsSingle().NonLazy();
			_ = BindInterfacesTo<VlcPlayerTexturesFactory>().AsSingle().NonLazy();
			_ = BindInterfacesTo<VlcPlayerFactory>().AsSingle().NonLazy();
			_ = BindInterfacesTo<VlcStreamCache>()
					.FromInstance(FindObjectOfType<VlcStreamCache>()).AsSingle().NonLazy();

			_ = BindInterfacesTo<StatusColumnPlaylistFactory>().AsSingle().NonLazy();
			_ = BindInterfacesTo<StatusColumnMaterialsHelper>().AsSingle().NonLazy();
			_ = BindInterfacesTo<StatusTabItemDragAndDropBuffer>().AsSingle().NonLazy();
			_ = BindInterfacesTo<StatusHeaderTabFactory>().AsSingle().NonLazy();

			base.InstallBindings();
		}

		protected override void ResolveAll()
		{
			_ = Resolve<IWindowSelectionController>();

			_ = Resolve<IWindowCommandInterpreter>();

			_ = Resolve<IHostAuthorizationInfo>();

			_ = Resolve<IWebViewZooming>();
			_ = Resolve<IWebViewPreloadCoordinator>();

			_ = Resolve<IPdfAsPicturePageFactory>();
			_ = Resolve<IPdfViewAdapterFactory>();

			_ = Resolve<IVideoPlayerFactory>();
			_ = Resolve<IVideoFilePlayerFactory>();
			_ = Resolve<IVideoSplitPlayerFactory>();

			_ = Resolve<IVlcLibraryFactory>();
			_ = Resolve<IVlcMediaPlayerFactory>();
			_ = Resolve<IVlcMediaFactory>();
			_ = Resolve<IVlcPlayerTexturesFactory>();
			_ = Resolve<IVlcPlayerFactory>();
			_ = Resolve<IVlcStreamCache>();

			_ = Resolve<IStatusColumnPlaylistFactory>();
			_ = Resolve<IStatusColumnMaterialsHelper>();
			_ = Resolve<IStatusTabItemDragAndDropBuffer>();
			_ = Resolve<IStatusHeaderTabFactory>();

			base.ResolveAll();
		}
	}
}