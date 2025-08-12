using Base;
using Core.Options.Offsets;

namespace UI.ScreenLayers.Presenter
{
    public interface IScreenLayersPresenter : IRectTransformable, IVisualAsync, IUnloadable
    {
        void Initialize(IScreenOffsets screenOffsets);
    }
}