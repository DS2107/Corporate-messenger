using Android.App;
using Android.Content;
using Android.OS;
using Corporate_messenger.Droid.NotificationManager;
using Corporate_messenger.Models.Chat;
using Corporate_messenger.Service;
using Corporate_messenger.Service.Notification;
using Newtonsoft.Json;
using System;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(startServiceAndroid))]
namespace Corporate_messenger.Droid.NotificationManager
{
    public class startServiceAndroid : IForegroundService
    {
    
        private static Context context = global::Android.App.Application.Context;
        public void StartService()
        {
           
            var intent = new Intent(context, typeof(NotoficationService));
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                context.StartForegroundService(intent);
            }
            else
            {
                context.StartService(intent);
            }
        }

        public void StopService()
        {
            var intent = new Intent(context, typeof(NotoficationService));
            context.StopService(intent);
        }
    }
    [Service(Exported = true)]
    public class NotoficationService : Android.App.Service
    {
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public const int ServiceRunningNotifID = 9000;
        WebSocketSharp.WebSocket ws;
        INotificationManager notificationManager;
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += (sender, eventArgs) =>
            {
                var evtData = (NotificationEventArgs)eventArgs;
                ShowNotification(evtData.Title, evtData.Message);
            };
            Notification notif = DependencyService.Get<IStaticNotification>().ReturnNotif();
            StartForeground(ServiceRunningNotifID, notif);
            if (ws == null)
            {
                ws = new WebSocketSharp.WebSocket("ws://192.168.0.105:6001");
                ws.OnMessage += Ws_OnMessage;
                ws.OnOpen += Ws_OnOpen;
                ws.ConnectAsync();
            }
           
           

            return StartCommandResult.Sticky;
        }
        class dataRoom
        {
            [JsonProperty("type")]
            public string subs { get; set; }
            [JsonProperty("sender_id")]
            public int sendr_id { get; set; }
            [JsonProperty("reciever_id")]
            public int reciever_id { get; set; }
        }

       
        private void Ws_OnOpen(object sender, EventArgs e)
        {

            var message = JsonConvert.SerializeObject(new dataRoom { subs = "subscribe", sendr_id = 2, reciever_id = 1 });
            ws.Send(message);
        }

        private void Ws_OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
        {
            var message = JsonConvert.DeserializeObject(e.Data).ToString();
            message = message.Substring(13, 4);
            if (message == "call")
            {
                var myAudio = JsonConvert.DeserializeObject<AndroidService.MyAudio>(e.Data);
                DependencyService.Get<IAudioWebSocketCall>().ListenerWebSocketCall(myAudio.audio);

            }

            else
            {
                ChatModel new_message = JsonConvert.DeserializeObject<ChatModel>(e.Data);
                if (new_message.Audio == null)
                {
                    new_message.MaximumSlider = 1;
                    new_message.IsAuidoVisible = false;
                    new_message.IsMessageVisible = true;
                    new_message.SourceImage = "play.png";
                    new_message.ValueSlider = 1;

                    notificationManager.SendNotification("Вам сообщение", new_message.Message);
                }
                else
                {
                    new_message.ValueSlider = 1;
                    new_message.SourceImage = "play.png";
                    new_message.IsAuidoVisible = true;
                    new_message.IsMessageVisible = false;
                    new_message.MaximumSlider = 1;
                    notificationManager.SendNotification("Вам сообщение", "Голосовое");

                }

               
            }
         
        }

        private void ShowNotification(string title, string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var msg = new Label()
                {
                    Text = $"Notification Received:\nTitle: {title}\nMessage: {message}"
                };
              //  stackLayout.Children.Add(msg);
            });
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override bool StopService(Intent name)
        {
            return base.StopService(name);
        }
    }

  
}