using Settings;
using UnityEngine;
using Debug = Core.Logging.Debug;

namespace UI.Notifications.Presenter.Popups.Factory
{
    public class NotificationPopupFactory : INotificationPopupFactory
    {
        private readonly IPrefabSettings _prefabs;
        
        public NotificationPopupFactory(IPrefabSettings prefabs) => _prefabs = prefabs;
        
        public INotificationPopup Create(RectTransform parent, string text, Color color, float activityTime, float fadeDuration)
        {
            if (parent == null)
            {
                Debug.LogError("An attempt was detected to assign a null parent to the NotificationPopup");
                return null;
            }
            
            if (!_prefabs.TryGetComponent<NotificationPopup>(out var original))
            {
                Debug.LogError("There is no prefab with the NotificationPopup component in the prefab settings");
                return null;
            }
            
            var gameObject = Object.Instantiate(original.gameObject);
            
            gameObject.SetActive(false);
            
            var popup = gameObject.GetComponent<NotificationPopup>();
            
            popup.SetName("NotificationPopup");
            popup.Initialize(text, color, activityTime, fadeDuration);
            popup.SetParentOnInitialize(parent);
            popup.AlignToParent();
            
            return popup;
        }
    }
}