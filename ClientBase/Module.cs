using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Client.ClientBase
{
    public class Module
    {
        public readonly string category;
        public readonly string name;
        public readonly string desc;
        public bool enabled;
        public char keybind;
        private DateTime lastKeyPressTime = DateTime.MinValue;
        public bool tickable = true;

        protected Module(string name, char keybind, string category, string desc, bool tickable, bool enabled = false)
        {
            this.name = name;
            this.keybind = keybind;
            this.category = category;
            this.enabled = enabled;
            this.desc = desc;
            this.tickable = tickable;
        }

        public virtual void OnEnable()
        {
            Client.ClientBase.UI.Notification.NotificationManager.AddNotification(name + " has been enabled", Color.FromArgb(102, 255, 71));
            enabled = true;
        }

        public virtual void OnDisable()
        {
            Client.ClientBase.UI.Notification.NotificationManager.AddNotification(name + " has been disabled", Color.FromArgb(173, 28, 17));
            enabled = false;
        }

        public virtual void OnTick()
        {
            if (!tickable)
                return;
            Task.Delay(100);
        }

        public bool MatchesKey(char key)
        {
            if (DateTime.Now.Subtract(lastKeyPressTime).TotalMilliseconds > 500)
            {
                if (keybind == char.ToUpper(key))
                {
                    lastKeyPressTime = DateTime.Now;
                    return true;
                }
            }
            return false;
        }
    }
}