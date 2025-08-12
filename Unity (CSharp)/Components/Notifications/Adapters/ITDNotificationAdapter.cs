namespace Platforms.Notifications.Adapters
{
	public interface ITDNotificationAdapter
	{
		string SendNotification(Base.TDNotification notification);
		void CancelNotification(string notifictionId);
	}
}
