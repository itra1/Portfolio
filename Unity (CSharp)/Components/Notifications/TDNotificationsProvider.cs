#define UNITY_NOTIFICATIONS

using System;
using Platforms.Notifications.Adapters;
using NotificationInstance = Platforms.Notifications.Base.TDNotification;

namespace Platforms.Notifications
{
	public class TDNotificationsProvider
	{
		public const string LogKey = "[Notifications]";

		private ITDNotificationAdapter _adapter;

		public TDNotificationsProvider()
		{
			_adapter = CreateAdapter();
		}

		private ITDNotificationAdapter CreateAdapter()
		{
#if UNITY_EDITOR
			return new Adapters.Editors.TDEditorNotificationAdapter();
#elif UNITY_NOTIFICATIONS
			return new Adapters.Unity.TDUnityNotificationAdapter();
#endif

			throw new NotImplementedException($"{TDNotificationsProvider.LogKey} There is no adapter");
		}

		public void CancelNotification(string notificationId)
		{
			_adapter.CancelNotification(notificationId);
		}

		public string SendNotification(NotificationInstance notification)
		{
			return _adapter.SendNotification(notification);
		}
	}
}
