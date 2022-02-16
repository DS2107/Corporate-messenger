using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Corporate_messenger.DB;
using Corporate_messenger.Service;
using Corporate_messenger.Service.Notification;
using Corporate_messenger.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Corporate_messenger.Droid.Broadcast
{
    [BroadcastReceiver(Enabled = true,Label ="Accept Click Receiver",Exported =true, Name = "com.companyname.corporate_messenger.AcceptReceiver")]
    [IntentFilter(new[] { "com.companyname.corporate_messenger.Accept_Receiver" })]
    public class AcceptClickReceiver : BroadcastReceiver
    {
        public static string TitleKey = "Call_title";
        public override async void OnReceive(Context context, Intent intent)
        {
           Intent mycallIntent = new Intent(context, typeof(CallActivity));
            DependencyService.Get<IForegroundService>().CallPageFlag = true;
            mycallIntent.AddFlags(ActivityFlags.NewTask);
            Android.App.Application.Context.StartActivity(mycallIntent);

            /*Intent intent2 = new Intent(context, typeof(MainActivity));
             intent2.AddFlags(ActivityFlags.ClearTop);         
             intent2.AddFlags(ActivityFlags.NewTask);

             Android.App.Application.Context.StartActivity(intent2);*/
            DependencyService.Get<IForegroundService>().manager.Cancel(0);
            DependencyService.Get<IAudio>().StopAudioFile();


            Toast.MakeText(context, "Приятного общения", ToastLength.Short).Show();
            var user = await UserDbService.GetUser();

            var vrem = DependencyService.Get<IForegroundService>().receiver_id;

            DependencyService.Get<IAudioUDPSocketCall>().ConnectionToServer();

             DependencyService.Get<ISocket>().MyWebSocket.Send(JsonConvert.SerializeObject(new
            {
                type = "init_call",
                sender_id = user.Id,
                status = "200",
                receiver_id = DependencyService.Get<IForegroundService>().receiver_id,
                call_address = DependencyService.Get<IAudioUDPSocketCall>().GetServerIp(),
                call_id = DependencyService.Get<IForegroundService>().call_id
            }));
            
        }
    }
}