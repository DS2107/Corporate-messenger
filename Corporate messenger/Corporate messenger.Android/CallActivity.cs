using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Corporate_messenger.DB;
using Corporate_messenger.Service;
using Corporate_messenger.Service.Notification;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Corporate_messenger.Droid
{
    [Activity(Label = "CallActivity",ScreenOrientation = ScreenOrientation.Portrait)]
    public class CallActivity : Activity, ISensorEventListener
    {
        public static bool MyFlag_Check_Position_User { get; set; }
        public CallActivity()
        {

        }
        public CallActivity(bool Flag_Check_Position_User)
        {
            MyFlag_Check_Position_User = Flag_Check_Position_User;
        }

        Android.Widget.Button BtnStartCall;
        Android.Widget.Button BtnEndCall;
        Android.Widget.Button BtnEndCallCenter;

        SensorManager sensorManager;
        Sensor proximitySensor;

  
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            

           
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.CallPage);
             BtnStartCall = FindViewById<Android.Widget.Button>(Resource.Id.btncall);
             BtnEndCall = FindViewById<Android.Widget.Button>(Resource.Id.btnendcall);
             BtnEndCallCenter = FindViewById<Android.Widget.Button>(Resource.Id.btnendcallCenter);

            BtnStartCall.Click += BtnStartCall_Click;
            BtnEndCall.Click += BtnEndCall_Click;
            BtnEndCallCenter.Click += BtnEndCallCenter_Click;

         
            // Если я ответил
            if (DependencyService.Get<IForegroundService>().CallPageFlag == true)
            {
               
                // calling sensor service.
                sensorManager = (SensorManager) GetSystemService(Context.SensorService);
                sensorManager.RegisterListener(this, sensorManager.GetDefaultSensor(SensorType.Proximity), SensorDelay.Ui);
                // from sensor service we are 
                // calling proximity sensor
                proximitySensor = sensorManager.GetDefaultSensor(SensorType.Proximity);
              
            
                // Скрываем кнопки
                BtnStartCall.Visibility = ViewStates.Invisible;
                BtnEndCallCenter.Visibility = ViewStates.Visible;
                BtnEndCall.Visibility = ViewStates.Invisible;

                // Запускаем udp
                DependencyService.Get<IAudioUDPSocketCall>().InitUDP();
                DependencyService.Get<IAudioUDPSocketCall>().StartReceive();
                DependencyService.Get<IAudioUDPSocketCall>().SendMessage();
              

            }
            // Если я перешел из уведомления 
            else
            {
                BtnStartCall.Visibility = ViewStates.Visible;
                BtnEndCallCenter.Visibility = ViewStates.Invisible;
                BtnEndCall.Visibility = ViewStates.Visible;
            }


        }

        private async void BtnEndCallCenter_Click(object sender, EventArgs e)
        {
            var MyUser = await UserDbService.GetUser();
            // Уведомление для собеседника о прекращении звонка 
            DependencyService.Get<ISocket>().MyWebSocket.Send(JsonConvert.SerializeObject(new { type = "init_call", status = "401", sender_id =  MyUser.Id }));

            // Выключить UDP 
            DependencyService.Get<IAudioUDPSocketCall>().StopAudioUDPCall();

            // Перейти в приложение
            Intent mainActivity = new Intent(Android.App.Application.Context, typeof(MainActivity));
            mainActivity.AddFlags(ActivityFlags.NewTask);
            Android.App.Application.Context.StartActivity(mainActivity);
        }

        private async void BtnEndCall_Click(object sender, EventArgs e)
        {
            var MyUser = await UserDbService.GetUser();
            // Убрать Уведомление 
            DependencyService.Get<IForegroundService>().manager.Cancel(0);

            // Бросить Уведомление о что я не хочу поднимать трубку 
            DependencyService.Get<ISocket>().MyWebSocket.Send(JsonConvert.SerializeObject(new { type = "init_call", status = "403", sender_id = MyUser.Id }));
            
            // Выключить музыку
            DependencyService.Get<IAudio>().StopAudioFile();

            // Перейти на главный экран
            Intent mainActivity = new Intent(Android.App.Application.Context, typeof(MainActivity));         
            mainActivity.AddFlags(ActivityFlags.NewTask);
            Android.App.Application.Context.StartActivity(mainActivity);
        }

        private async void BtnStartCall_Click(object sender, EventArgs e)
        {
            // Настройки сенсора 
            sensorManager = (SensorManager)GetSystemService(Context.SensorService);
            sensorManager.RegisterListener(this, sensorManager.GetDefaultSensor(SensorType.Proximity), SensorDelay.Ui);
            DependencyService.Get<IAudio>().StopAudioFile();
            // Скрываем кнопки
            BtnStartCall.Visibility = ViewStates.Invisible;
            BtnEndCallCenter.Visibility = ViewStates.Visible;
            BtnEndCall.Visibility = ViewStates.Invisible;

          
            var user = await UserDbService.GetUser();

            var vrem = DependencyService.Get<IForegroundService>().receiver_id;

            // Авторизуемся на UDP
            DependencyService.Get<IAudioUDPSocketCall>().ConnectionToServer();

            // Говорим Звонящему, что мы приняли звонок
            DependencyService.Get<ISocket>().MyWebSocket.Send(JsonConvert.SerializeObject(new
            {
                type = "init_call",
                sender_id = user.Id,
                status = "200",
                receiver_id = DependencyService.Get<IForegroundService>().receiver_id,
                call_address = DependencyService.Get<IAudioUDPSocketCall>().GetServerIp(),
                call_id = DependencyService.Get<IForegroundService>().call_id
            }));



            // Запускаем udp вещание 
            DependencyService.Get<IAudioUDPSocketCall>().InitUDP();
            DependencyService.Get<IAudioUDPSocketCall>().StartReceive();
            DependencyService.Get<IAudioUDPSocketCall>().SendMessage();
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            var s = sensor;
        }

        public void OnSensorChanged(SensorEvent e)
        {
            var s = 1;
        }
    }
}