using Base.Controller;
using Elements.Windows.Base;

namespace Elements.PresentationEpisodeScreen.Controller
{
	public interface IPresentationEpisodeScreenController : IPresentationEpisodeScreenInfo, IController, IPreloadingAsync, 
		IWindowMaterialActionPerformer, IWindowStateProvider
	{
		
	}
}