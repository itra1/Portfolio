#if UNITY_IOS || UNITY_ANDROID
using System;
using Cysharp.Threading.Tasks;
using Unity.Notifications.iOS;
using UnityEngine;
#endif


namespace Game.Providers.PushNotifications {
	public class PushNotificationsProvider {

#if UNITY_IOS || UNITY_ANDROID
		public PushNotificationsProvider() {
			RequestAuthorization().Forget();
		}

		private async UniTask RequestAuthorization() {
			var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge;
			using var req = new AuthorizationRequest(authorizationOption, true);
			await UniTask.WaitUntil(() => req.IsFinished);
			await UniTask.Delay(500);

			iOSNotificationCenter.RemoveAllScheduledNotifications();

			AddNotification();
		}

		private void AddNotification() {
			var timeTrigger = new iOSNotificationTimeIntervalTrigger() {
				TimeInterval = new TimeSpan(12, 0, 0),
				Repeats = true
			};

			var notification = new iOSNotification() {
				// You can specify a custom identifier which can be used to manage the notification later.
				// If you don't provide one, a unique string will be generated automatically.
				Identifier = "_notification_01",
				Title = Application.productName,
				Subtitle = "Get your prize!",
				Body = "It's been a long time, check the tournament results and claim your winnings!",
				ShowInForeground = true,
				ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
				CategoryIdentifier = "category_a",
				ThreadIdentifier = "thread1",
				Trigger = timeTrigger,
			};

			iOSNotificationCenter.ScheduleNotification(notification);
		}
#endif
	}
}
