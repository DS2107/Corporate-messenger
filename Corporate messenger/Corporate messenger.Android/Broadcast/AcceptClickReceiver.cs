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
        public override async void OnReceive(Context context, Intent intent)
        {
           await DependencyService.Get<IFileService>().MyMainPage.Navigation.PushModalAsync(new CallPage(true));
     
            var user = await UserDbService.GetUser();
            var vrem = DependencyService.Get<IForegroundService>().receiver_id;

            DependencyService.Get<IAudioUDPSocketCall>().ConnectionToServer();
            DependencyService.Get<IAudioUDPSocketCall>().GetServerIp();


             DependencyService.Get<ISocket>().MyWebSocket.Send(JsonConvert.SerializeObject(new
            {
                type = "init_call",
                sender_id = user.Id,
                status = "200",
                receiver_id = DependencyService.Get<IForegroundService>().receiver_id,
                call_address = DependencyService.Get<IAudioUDPSocketCall>().GetServerIp(),
                call_id = DependencyService.Get<IForegroundService>().call_id
            }));
            DependencyService.Get<IForegroundService>().manager.Cancel(0);
            DependencyService.Get<IAudio>().StopAudioFile();
            DependencyService.Get<IAudioUDPSocketCall>().InitUDP();
            DependencyService.Get<IAudioUDPSocketCall>().SendMessage();
            DependencyService.Get<IAudioUDPSocketCall>().StartReceive();
            Toast.MakeText(context, "Приятного общения", ToastLength.Short).Show();
        }
    }
}