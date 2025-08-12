namespace Core.UI.Notifications.Data
{
    public struct NotificationInfo
    {
        public NotificationType Type { get; }
        public string Text { get; }

        public NotificationInfo(NotificationType type, string text)
        {
            Type = type;
            Text = text;
        }
    }
}