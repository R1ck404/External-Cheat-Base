using Client.ClientBase.Fonts;

namespace Client.ClientBase.UI.Notification
{
    public static class NotificationManager
    {
        public static Form overlay = null!;
        public static List<Notification> notifications = new List<Notification>();

        public static void Init(Form form)
        {
            overlay = form;
        }

        public static void AddNotification(string text, Color lineColor)
        {
            int x = notifications.Count > 0 ? notifications[0]._boxRect.X : 0;
            int y = notifications.Count > 0 ? notifications[0]._boxRect.Y - notifications[0]._boxRect.Height - 10 : overlay.Height - 10;
            var notification = new Notification(text, new Rectangle(x, y, 400, 50), new Font("Gadugi", 15, System.Drawing.FontStyle.Bold), new Font(FontRenderer.sigmaFamily, 13, System.Drawing.FontStyle.Regular), Color.FromArgb(150, Color.Black), lineColor, overlay);
            notifications.Add(notification);
        }

        public static void UpdateNotifications(Graphics graphics)
        {
            int yOffset = overlay.Height - 70;

            foreach (Notification notification in notifications)
            {
                if (!notification.isShowing)
                {
                    notification._boxRect.Y = yOffset;
                    notification.Show();
                }

                notification.OnDraw(graphics);

                notification.Expired += (sender, e) =>
                {
                    notifications.Remove(notification);
                };

                yOffset -= notification._boxRect.Height + 10;
            }
        }
    }
}