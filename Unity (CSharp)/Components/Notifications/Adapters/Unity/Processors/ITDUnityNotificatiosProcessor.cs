
namespace Platforms.Notifications.Adapters.Unity.Processors
{
	public interface ITDUnityNotificatiosProcessor
	{
		void CancelNotification(string notifictionId);
		string SendNotification(Base.TDNotification notification);
	}
}
