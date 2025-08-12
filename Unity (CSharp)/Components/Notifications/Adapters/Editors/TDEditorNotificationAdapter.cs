
namespace Platforms.Notifications.Adapters.Editors
{
	public class TDEditorNotificationAdapter : ITDNotificationAdapter
	{
		public void CancelNotification(string notifictionId)
		{

		}

		public string SendNotification(Base.TDNotification notification)
		{
			UnityEngine.Debug.Log($"{TDNotificationsProvider.LogKey} Make Notification {Newtonsoft.Json.JsonConvert.SerializeObject(notification)}");

			return System.Guid.NewGuid().ToString();
		}
	}
}
