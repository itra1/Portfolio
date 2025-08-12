#if UNITY_ANDROID

using Unity.Notifications.Android;

namespace Platforms.Notifications.Adapters.Unity.Processors
{
	public class TDAndroidProcessor : ITDUnityNotificatiosProcessor
	{
		private string _channelId = "channel_id";
		public TDAndroidProcessor()
		{
			var channel = new AndroidNotificationChannel()
			{
				Id = _channelId,
				Name = "Default Channel",
				Importance = Importance.High,
				Description = "Generic notifications",
			};
			AndroidNotificationCenter.RegisterNotificationChannel(channel);
		}

		public void CancelNotification(string notifictionId)
		{
			if (int.TryParse(notifictionId, out var notificationIdInt))
				AndroidNotificationCenter.CancelNotification(notificationIdInt);
		}

		public string SendNotification(Base.TDNotification notification)
		{
			var notificationSend = new AndroidNotification
			{
				Title = notification.Title,
				Text = notification.Text,
				FireTime = notification.FireTime
			};

			var identificationId = AndroidNotificationCenter.SendNotification(notificationSend, _channelId);

			return identificationId.ToString();
		}
	}
}
#endif