
using System;
using System.Collections.Generic;
using System.Text;

namespace Corporate_messenger.Service.Notification
{
    public interface IForegroundService
    {
        public void StartService();
        public void StopService();

        public Android.App.NotificationManager manager { get; set; }
        public int receiver_id { get; set; }
        public void MyToast(string message);
        public bool AudioCalls_Init { get; set; }
        public int call_id { get; set; }
        public int chat_room_id { get; set; }
        public bool SocketFlag { get; set; }

        public bool LoginPosition { get; set; }
    }
}
