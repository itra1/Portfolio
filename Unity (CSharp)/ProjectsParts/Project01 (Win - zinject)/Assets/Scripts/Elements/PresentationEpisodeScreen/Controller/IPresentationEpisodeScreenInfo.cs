using Core.Materials.Data;
using Elements.PresentationEpisodeScreen.Presenter;

namespace Elements.PresentationEpisodeScreen.Controller
{
    public interface IPresentationEpisodeScreenInfo
    {
        ContentAreaMaterialData AreaMaterial { get; }
        
        IPresentationEpisodeScreenPresenter Presenter { get; }
    }
}