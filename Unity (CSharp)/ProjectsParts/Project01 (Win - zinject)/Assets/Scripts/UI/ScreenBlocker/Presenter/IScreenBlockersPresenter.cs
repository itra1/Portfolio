using Base;
using Core.Options.Offsets;
using UI.ScreenBlocker.Presenter.Popups.Common.Factory;

namespace UI.ScreenBlocker.Presenter
{
    public interface IScreenBlockersPresenter : IUnloadable
    {
        void Initialize(IScreenOffsets screenOffsets, IScreenBlockerPopupFactory factory);
    }
}