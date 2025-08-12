using Platforms.Notifications.Adapters.Unity.Processors;

namespace Platforms.Notifications.Adapters.Unity
{
	public class TDUnityNotificationAdapter : ITDNotificationAdapter
	{
		private ITDUnityNotificatiosProcessor _processor;

		public TDUnityNotificationAdapter()
		{
			_processor = CreateProcessor();
		}

		public void CancelNotification(string notifictionId)
		{
			_processor.CancelNotification(notifictionId);
		}

		public string SendNotification(Base.TDNotification notification)
		{
			return _processor.SendNotification(notification);
		}

		private ITDUnityNotificatiosProcessor CreateProcessor()
		{
#if UNITY_IOS
			return new TDIosProcessor();
#elif UNITY_ANDROID
			return new TDAndroidProcessor();
#endif
			throw new System.NotImplementedException("");
		}
	}
}
