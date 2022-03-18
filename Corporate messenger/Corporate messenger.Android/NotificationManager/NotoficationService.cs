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
using System.Net.NetworkInformation;
using System.Net;

[assembly: Xamarin.Forms.Dependency(typeof(startServiceAndroid))]
[assembly: Xamarin.Forms.Dependency(typeof(GetSocket))]
namespace Corporate_messenger.Droid.NotificationManager
{


    public class GetSocket:ISocket, INotifyPropertyChanged
    {
      
       
        
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
        public bool CallPageFlag { get; set; }
        private static Context context = global::Android.App.Application.Context;
        public bool Flag_On_Off_Service { get; set; }
        public bool Flag_AudioCalls_Init { get; set; }
       public bool Flag_On_Off_Socket { get; set; }
        public int call_id { get; set; }
        public int chat_room_id { get; set; }
       
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
                Flag_On_Off_Service = true;
            }
            else
            {
                context.StartService(intent);
                Flag_On_Off_Service = true;
            }
        }

        public void StopService()
        {
            var intent = new Intent(context, typeof(NotoficationService));
            Flag_On_Off_Service = false;
            DependencyService.Get<ISocket>().MyWebSocket.CloseAsync();
            context.StopService(intent);
        }
    }
    [Service(Exported = true)]
    public class NotoficationService : Android.App.Service
    {
        // address
        private string address = "192.168.10.254:55201";
        // ID Уведомления о включении службы
        public const int ServiceRunningNotifID = 9001;
        // Сокет
        GetSocket socket = new GetSocket();
        // Локальные уведомления для сообщений 
        private INotificationManager NotificationMessageManager;
        // Локальные уведомления для Звонков
        private NotificationCall NotificationCalllManager = new NotificationCall();
        // Пользователь 
        private UserDataModel MyUser;
      
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            // Запуск сервиса 
            Notification notif = DependencyService.Get<IStaticNotification>().ReturnNotif();
            StartForeground(ServiceRunningNotifID, notif);
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

           

           
               
                socket.MyWebSocket = new WebSocketSharp.WebSocket("ws://"+ address);
                socket.MyWebSocket.OnMessage += Ws_OnMessage;
                socket.MyWebSocket.OnOpen += Ws_OnOpen;
                socket.MyWebSocket.OnClose += Ws_OnClose;
                socket.MyWebSocket.OnError += Ws_OnError;
                DependencyService.Get<IForegroundService>().Flag_On_Off_Socket = true;
                Task.Run(()=> socket.MyWebSocket.ConnectAsync());
                MyTimer();
             
            
            return StartCommandResult.Sticky;
        }
        Ping x = new Ping();
        private  void MyTimer()
        {
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                try
                {
                    var pingFlag = socket.MyWebSocket.Ping();

                    if (!pingFlag)
                    {
                        Task.Run(() => socket.MyWebSocket.CloseAsync());
                        Task.Run(() => socket.MyWebSocket.ConnectAsync()).Wait();                          
                    
                        
                    }
                }
                catch (Exception ex)
                {
                    var s = ex;
                    DependencyService.Get<IForegroundService>().Flag_On_Off_Socket = false;
                    Task.Run(() => socket.MyWebSocket.CloseAsync());
                    socket.MyWebSocket = new WebSocketSharp.WebSocket("ws://"+ address);
                    socket.MyWebSocket.OnMessage += Ws_OnMessage;
                    socket.MyWebSocket.OnOpen += Ws_OnOpen;
                    socket.MyWebSocket.OnClose += Ws_OnClose;
                    socket.MyWebSocket.OnError += Ws_OnError;
                    //  Task.Run(() => socket.MyWebSocket.ConnectAsync()).Wait();
                }
               
              
               
                return true; // return true to repeat counting, false to stop timer
            });
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
            MyUser = await UserDbService.GetUser();
            if (MyUser != null)
            {
                var message = JsonConvert.SerializeObject(new { type = "subscribe", sender_id = MyUser.Id, });
                socket.MyWebSocket.Send(message);
            }       
        }

        private  void Ws_OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
        {
            dynamic Json_obj = JObject.Parse(e.Data);         
            string type_status = (string)Json_obj.status != null? (string)Json_obj.status : (string)Json_obj.type;

            switch (type_status) {
                // Если приходит Уведомление о звонке
                case "100":
                    Status100(Json_obj);
                    break;
                // Если пользователь, которому звонили поднял трубку 
                case "200":
                    Status200();
                    break;
                 // Если сбросил Звонящий 
                case "400":
                    Status400();                 
                    break;
                // Если сбросил звонок отвечающий
                case "401":
                    Status401();                                  
                    break;
                // Если сбросил Звонящий до поднятия трубки 
                case "402":
                    Status402();
                    break;
                // Если сбросил Отвечающий до поднятия трубки 
                case "403":
                    Status403();                 
                    break;
             
                case "message":
                    if (DependencyService.Get<IForegroundService>().chat_room_id != (int)Json_obj.chat_room_id)
                        NotificationMessage(e);
                   
                    break;
            }
       
        }
       
        

        private void Status100(dynamic Json_obj)
        {
            // Узнать кто звонит
            DependencyService.Get<IForegroundService>().receiver_id = (int)Json_obj.sender_id;
            // Узнать какая комната 
            DependencyService.Get<IForegroundService>().call_id = (int)Json_obj.call_id;

            // Кинуть Уведомление о Звонке
            NotificationCalllManager.SendNotification("Звонок", (string)Json_obj.username);
            // Включить музыку
            DependencyService.Get<IAudio>().PlayAudioFile("zvonok.mp3", Android.Media.Stream.Ring);
        }

        private void Status200()
        {
            // Флаг Для отключения музыки 
            DependencyService.Get<IForegroundService>().Flag_AudioCalls_Init = false;

            // Полное выключение музыки
            DependencyService.Get<IAudio>().StopAudioFile();

            // Установить флаг который уведомляет Звонящего пользователя , что его звонок был принят 
            DependencyService.Get<IAudioUDPSocketCall>().FlagRaised = true;

            // Запустить протокол UDP
            DependencyService.Get<IAudioUDPSocketCall>().InitUDP();
            DependencyService.Get<IAudioUDPSocketCall>().SendMessage();
            DependencyService.Get<IAudioUDPSocketCall>().StartReceive();
        }

        private void Status400()
        {
            // Выключить UDP 
            DependencyService.Get<IAudioUDPSocketCall>().StopAudioUDPCall();

            // Перейти в приложение
            Intent mainActivity = new Intent(Android.App.Application.Context, typeof(MainActivity));
            mainActivity.AddFlags(ActivityFlags.NewTask);
            Android.App.Application.Context.StartActivity(mainActivity);
        }

        private void Status401()
        {
            DependencyService.Get<IAudioUDPSocketCall>().StopAudioUDPCall();
            Task.Run(() => DependencyService.Get<IAudioWebSocketCall>().callView.ClosePageAsync()).Wait();
            DependencyService.Get<IForegroundService>().Flag_AudioCalls_Init = false;
        }

        private void Status402()
        {
            // Полностью выключаем музыку
            DependencyService.Get<IForegroundService>().Flag_AudioCalls_Init = false;
            DependencyService.Get<IAudio>().StopAudioFile();
            // Убираем уведомление 
            if (DependencyService.Get<IForegroundService>().manager != null)
                DependencyService.Get<IForegroundService>().manager.Cancel(0);
        }

        private void Status403()
        { 
            // Полностью выключаем музыку
            DependencyService.Get<IForegroundService>().Flag_AudioCalls_Init = false;
            DependencyService.Get<IAudio>().StopAudioFile();

            // Выхожу со страницы
            Task.Run(() => DependencyService.Get<IAudioWebSocketCall>().callView.ClosePageAsync()).Wait();
        }

        
        
        private void NotificationMessage(WebSocketSharp.MessageEventArgs args)
        {
            dynamic Json_obj = JObject.Parse(args.Data);
            if ((int)Json_obj.sender_id != MyUser.Id)
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
            socket.MyWebSocket.CloseAsync();
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