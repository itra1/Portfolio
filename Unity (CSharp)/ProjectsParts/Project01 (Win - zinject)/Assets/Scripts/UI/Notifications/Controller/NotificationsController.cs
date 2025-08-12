using System;
using Core.Options.Offsets;
using UI.Canvas.Presenter;
using UI.Notifications.Presenter;
using UI.Notifications.Presenter.Popups.Factory;

namespace UI.Notifications.Controller
{
    public class NotificationsController : INotificationsController, IDisposable
    {
        private INotificationsPresenter _presenter;
        
        public NotificationsController(IScreenOffsets screenOffsets,
            ICanvasPresenter root,
            INotificationPopupFactory factory)
        {
            _presenter = root.Notifications;
            _presenter.Initialize(screenOffsets, factory);
        }
        
        public void Dispose()
        {
            if (_presenter == null)
                return;
            
            _presenter.Unload();
            _presenter = null;
        }
    }
}