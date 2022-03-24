
using System;
using System.Collections.Generic;
using System.Text;

namespace Corporate_messenger.Service.Notification
{
    public interface IForegroundService
    {
        public void StartService();
        public void StopService();

        public string NameUserCall { get; set; }
        public bool Flag_On_Off_Service { get; set; }
        public bool CallPageFlag { get; set; }
        public Android.App.NotificationManager manager { get; set; }
        public int receiver_id { get; set; }
        public void MyToast(string message);
        public bool Flag_AudioCalls_Init { get; set; }
        public int call_id { get; set; }
        public int chat_room_id { get; set; }
       public bool Flag_On_Off_Socket { get; set; }



    }
}
