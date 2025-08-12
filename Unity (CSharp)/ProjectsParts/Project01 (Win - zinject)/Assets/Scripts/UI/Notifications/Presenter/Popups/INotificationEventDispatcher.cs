using System;

namespace UI.Notifications.Presenter.Popups
{
    public interface INotificationEventDispatcher
    {
        public event EventHandler Shown;
        public event EventHandler Hidden;
    }
}