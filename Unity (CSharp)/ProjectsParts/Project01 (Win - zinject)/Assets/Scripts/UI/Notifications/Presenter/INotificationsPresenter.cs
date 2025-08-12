using Base;
using Core.Options.Offsets;
using UI.Notifications.Presenter.Popups.Factory;

namespace UI.Notifications.Presenter
{
    public interface INotificationsPresenter : IUnloadable
    {
        void Initialize(IScreenOffsets screenOffsets, INotificationPopupFactory factory);
    }
}