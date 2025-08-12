using UnityEngine;

namespace UI.Notifications.Presenter.Popups.Factory
{
    public interface INotificationPopupFactory
    {
        INotificationPopup Create(RectTransform parent, string text, Color color, float activityTime, float fadeDuration);
    }
}