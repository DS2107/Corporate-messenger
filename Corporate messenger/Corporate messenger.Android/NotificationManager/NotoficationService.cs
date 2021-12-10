using Android.App;
using Android.Content;
using Android.OS;
using Corporate_messenger.Droid.NotificationManager;
using Corporate_messenger.Models;
using Corporate_messenger.Models.Chat;
using Corporate_messenger.Service;
using Corporate_messenger.Service.Notification;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(startServiceAndroid))]
[assembly: Xamarin.Forms.Dependency(typeof(GetSocket))]
namespace Corporate_messenger.Droid.NotificationManager
{


    public class GetSocket:ISocket, INotifyPropertyChanged
    {
        public GetSocket()
        {

        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// Socket
        /// </summary>
        public WebSocketSharp.WebSocket MyWebSocket
        {
            get { return myWebSocket; }
            set
            {
                if (myWebSocket != value)
                {
                    myWebSocket = value;
                    OnPropertyChanged("MyWebSocket");
                }
            }
        }
        private static WebSocketSharp.WebSocket myWebSocket { get; set; }
    }

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
                GetSocket socket = new GetSocket();
                ws = new WebSocketSharp.WebSocket("ws://192.168.0.105:6001");
                ws.OnMessage += Ws_OnMessage;
                ws.OnOpen += Ws_OnOpen;
                ws.OnClose += Ws_OnClose;
                ws.OnError += Ws_OnError;            
                socket.MyWebSocket = ws;
                bool time;
                TimerStartService();

            }
           
           

            return StartCommandResult.Sticky;
        }

        private void Ws_OnError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Ws_OnClose(object sender, WebSocketSharp.CloseEventArgs e)
        {
            throw new NotImplementedException();
        }

        SpecialDataModel user = new SpecialDataModel();
        private void Ws_OnOpen(object sender, EventArgs e)
        {
           
            var message = JsonConvert.SerializeObject(new  { type = "subscribe", sender_id = user.Id, });
            ws.Send(message);
        }

        private void Ws_OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
        {
            dynamic Json_obj = JObject.Parse(e.Data);
            if((int)Json_obj.sender_id != user.Id)
            {
                if (Json_obj.type == "call")
                {
                    // var myAudio = JsonConvert.DeserializeObject<AndroidService.MyAudio>(e.Data);
                    // DependencyService.Get<IAudioWebSocketCall>().ListenerWebSocketCall(myAudio.audio);

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

                        notificationManager.SendNotification((string)Json_obj.username, new_message.Message);
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
           
         
        }

        private void TimerStartService()
        {
            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                if (ws.Ping() == true)
                {
                    if(ws.ReadyState == WebSocketSharp.WebSocketState.Closed)
                    {
                        ws.ConnectAsync();
                    }
                    return true;
                }
                   
                
                else
                {
                    ws.Close();
                    ws.ConnectAsync();
                    return true;
                }
                    
            });
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
            ws.CloseAsync();
            base.OnDestroy();
        }

        public override bool StopService(Intent name)
        {
            return base.StopService(name);
        }
    }

  
}