#if UNITY_IOS

using System.Text;
using System.Threading.Tasks;
using Unity.Notifications.iOS;

namespace Platforms.Notifications.Adapters.Unity.Processors
{
	public class TDIosProcessor : ITDUnityNotificatiosProcessor
	{
		public TDIosProcessor()
		{
			_ = InitiateAsync();
		}

		private async Task InitiateAsync()
		{
			var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge;
			using (var req = new AuthorizationRequest(authorizationOption, true))
			{
				while (!req.IsFinished)
					await Task.Delay(50);

				StringBuilder sb = new StringBuilder()
				.Append($"{TDNotificationsProvider.LogKey}:")
				.Append("\n finished: " + req.IsFinished)
				.Append("\n granted :  " + req.Granted)
				.Append("\n error:  " + req.Error)
				.Append("\n deviceToken:  " + req.DeviceToken);

				UnityEngine.Debug.Log(sb.ToString());
			}
		}

		public void CancelNotification(string notifictionId)
		{
			iOSNotificationCenter.RemoveScheduledNotification(notifictionId);
		}

		public string SendNotification(Base.TDNotification notification)
		{

			var notificationSend = new iOSNotification()
			{
				Title = notification.Title,
				//Body = "Scheduled at: " + DateTime.Now.ToShortDateString() + " triggered in 5 seconds",
				Subtitle = notification.Text,
				ShowInForeground = true,
				ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
				CategoryIdentifier = "category_a",
				ThreadIdentifier = "thread1",
				Trigger = new iOSNotificationCalendarTrigger()
				{
					Year = notification.FireTime.Year,
					Month = notification.FireTime.Month,
					Day = notification.FireTime.Day,
					Hour = notification.FireTime.Hour,
					Minute = notification.FireTime.Minute,
					Second = notification.FireTime.Second,
					Repeats = false
				}
			};

			iOSNotificationCenter.ScheduleNotification(notificationSend);

			return notificationSend.Identifier;
		}
	}
}
#endif