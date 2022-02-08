using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Corporate_messenger.Droid.NotificationManager;
using Corporate_messenger.Models;
using Corporate_messenger.DB;
using Corporate_messenger.Models.Chat;
using Corporate_messenger.Service;
using Corporate_messenger.Service.Notification;
using Corporate_messenger.ViewModels;
using Corporate_messenger.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Timers;
using Xamarin.Forms;
using System.Threading.Tasks;
using Plugin.LocalNotification;


[assembly: Xamarin.Forms.Dependency(typeof(startServiceAndroid))]
[assembly: Xamarin.Forms.Dependency(typeof(GetSocket))]
namespace Corporate_messenger.Droid.NotificationManager
{


    public class GetSocket:ISocket, INotifyPropertyChanged
    {
        SpecialDataModel user = new SpecialDataModel();
       
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
       
        public bool AudioCalls_Init { get; set; }
        public bool SocketFlag { get; set; }
        public int call_id { get; set; }
        public int chat_room_id { get; set; }
        public bool LoginPosition { get; set ; }
        public int receiver_id { get ; set ; }
        public Android.App.NotificationManager manager { get ; set; }

        public void MyToast(string message)
        {
            Toast.MakeText(Android.App.Application.Context, message, ToastLength.Short).Show();
        }
        public void StartService()
        {
             var intent = new Intent(context, typeof(NotoficationService));
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                context.StartForegroundService(intent);


                DependencyService.Get<IForegroundService>().SocketFlag = true;
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
        public const int ServiceRunningNotifID = 9001;
        WebSocketSharp.WebSocket ws;
        INotificationManager NotificationMessageManager;
        NotificationCall NotificationCalllManager = new NotificationCall();
        UserDataModel user;
      
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
          
            NotificationMessageManager = (NotificationMessage)DependencyService.Get<INotificationManager>();
            
            NotificationMessageManager.NotificationReceived += (sender, eventArgs) =>
            {
                var evtData = (Service.Notification.NotificationEventArgs)eventArgs;
                ShowNotification(evtData.Title, evtData.Message);
            };
            NotificationCalllManager.NotificationReceived += (sender, eventArgs) =>
            {
                var evtData = (Service.Notification.NotificationEventArgs)eventArgs;
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
               
                TimerStartService();

            }
            return StartCommandResult.Sticky;
        }

       
        private void Ws_OnError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            var s = e;
        }

        private void Ws_OnClose(object sender, WebSocketSharp.CloseEventArgs e)
        {
            throw new NotImplementedException();
        }

        
        private async void Ws_OnOpen(object sender, EventArgs e)
        {
            user = await UserDbService.GetUser();
            var message = JsonConvert.SerializeObject(new  { type = "subscribe", sender_id = user.Id, });
            ws.Send(message);
        }

        private void Ws_OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
        {
            dynamic Json_obj = JObject.Parse(e.Data);         
            string type_status = (string)Json_obj.status != null? (string)Json_obj.status : (string)Json_obj.type;

            switch (type_status) {

                case "200":
                   
                    DependencyService.Get<IForegroundService>().AudioCalls_Init = false;
                    DependencyService.Get<IAudio>().StopAudioFile();
                    DependencyService.Get<IAudioUDPSocketCall>().InitUDP();                 
                    DependencyService.Get<IAudioUDPSocketCall>().SendMessage();
                    DependencyService.Get<IAudioUDPSocketCall>().StartReceive();
                    break;
                case "400":
                    DependencyService.Get<IAudioUDPSocketCall>().StopAudioUDPCall();
                    _ = DependencyService.Get<IAudioWebSocketCall>().callView.ClosePageAsync();           
                    DependencyService.Get<IForegroundService>().AudioCalls_Init = false;
                    DependencyService.Get<IAudio>().StopAudioFile();
              
                    
                    break;
                case "450":
                    _ = DependencyService.Get<IAudioWebSocketCall>().callView.ClosePageAsync();
                    DependencyService.Get<IAudioWebSocketCall>().StopAudioWebSocketCall();
                    DependencyService.Get<IForegroundService>().AudioCalls_Init = false;
                    DependencyService.Get<IAudio>().StopAudioFile();
                    break;
                case "call":
                    DependencyService.Get<IAudioWebSocketCall>().ListenerWebSocketCall((byte[])Json_obj.voice_audio);
                    break;
                case "100": // 100
                    DependencyService.Get<IForegroundService>().receiver_id = (int)Json_obj.sender_id;
                    DependencyService.Get<IForegroundService>().call_id = (int)Json_obj.call_id;
                    NotificationCalllManager.SendNotification("Звонок", (string)Json_obj.username);
                   // LocalNotificationBuilder(e);
                    DependencyService.Get<IAudio>().PlayAudioFile("zvonok.mp3", Android.Media.Stream.Ring);
                    break;
                case "message":
                    if (DependencyService.Get<IForegroundService>().chat_room_id != (int)Json_obj.chat_room_id)
                        NotificationMessage(e);
                      //  LocalNotificationBuilder(e);
                    break;
            }
       
        }
       
        
        
        private void NotificationMessage(WebSocketSharp.MessageEventArgs args)
        {
            dynamic Json_obj = JObject.Parse(args.Data);
            if ((int)Json_obj.sender_id != user.Id)
            {

                ChatModel new_message = JsonConvert.DeserializeObject<ChatModel>(args.Data);
                if (new_message.Audio == null)
                {
                    new_message.MaximumSlider = 1;
                    new_message.IsAuidoVisible = false;
                    new_message.IsMessageVisible = true;
                    new_message.SourceImage = "play.png";
                    new_message.ValueSlider = 1;

                    NotificationMessageManager.SendNotification((string)Json_obj.username, new_message.Message);
                }
                else
                {
                    new_message.ValueSlider = 1;
                    new_message.SourceImage = "play.png";
                    new_message.IsAuidoVisible = true;
                    new_message.IsMessageVisible = false;
                    new_message.MaximumSlider = 1;
                    NotificationMessageManager.SendNotification("Вам сообщение", "Голосовое");

                }
            }
        }
        int Message_id = 0;
        private void LocalNotificationBuilder(WebSocketSharp.MessageEventArgs args)
        {

            dynamic Json_obj = JObject.Parse(args.Data);
            long[] pattern = { 0, 100, 500, 100, 500, 100, 500, 100, 500, 100, 500 };
            if ((string)Json_obj.type == "message")
            {
                if ((int)Json_obj.sender_id != user.Id)
                {
                    var notification = new NotificationRequest
                    {
                        BadgeNumber = 1,
                        Description = (string)Json_obj.message,
                        Title = (string)Json_obj.username,
                        NotificationId = Message_id,

                        CategoryType = NotificationCategoryType.Alarm,
                        Silent = true,
                        Android = new Plugin.LocalNotification.AndroidOption.AndroidOptions {
                            AlarmType = Plugin.LocalNotification.AndroidOption.AndroidAlarmType.ElapsedRealtimeWakeup,
                            Priority = Plugin.LocalNotification.NotificationPriority.Max,
                            AutoCancel = false,
                            
                            IsProgressBarIndeterminate = true,
                            VibrationPattern = pattern,
                            Group = "Message",
                            IsGroupSummary = true,
                            ChannelId = "Message"
                            

                        }

                    };
                    NotificationCenter.Current.Show(notification);
                }
            }
            if((string)Json_obj.type == "init_call")
            {
                if ((int)Json_obj.sender_id != user.Id)
                {
                    var notification = new NotificationRequest
                    {
                        BadgeNumber = 1,
                        Description = "Звонок",
                        Title = (string)Json_obj.username,
                        NotificationId = 1337,
                        Android = new Plugin.LocalNotification.AndroidOption.AndroidOptions
                        {
                            AlarmType = Plugin.LocalNotification.AndroidOption.AndroidAlarmType.Rtc,

                        }

                    };
                    NotificationCenter.Current.Show(notification);
                }
            }

           
           
               
        }

        bool flag = false;
        private void TimerStartService()
        {
            Device.StartTimer(TimeSpan.FromSeconds(2), () =>
            {
               if(ws!=null)
                     flag = ws.Ping();
               
                    if (!flag)
                    {
                        try {
                            if(ws!=null)
                                ws.ConnectAsync();
                          
                         }
                        catch (Exception ex)
                        {
                        if (ex.Message == "A series of reconnecting has failed.")
                        {// refusal of ws object to reconnect; create new ws-object

                            ws.Close();

                            ws = new WebSocketSharp.WebSocket("ws://192.168.0.105:6001");
                            ws.OnOpen += Ws_OnOpen;
                            ws.OnMessage += Ws_OnMessage;
                            ws.OnError += Ws_OnError;
                            ws.OnClose += Ws_OnClose;
                            DependencyService.Get<ISocket>().MyWebSocket = ws;
                        }
                    }
                        
                    }
                
              
                    return true;
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

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnTaskRemoved(Intent rootIntent)
        {
            base.OnTaskRemoved(rootIntent);
        }
    }

  
}