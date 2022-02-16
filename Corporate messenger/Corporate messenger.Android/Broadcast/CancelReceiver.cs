using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Corporate_messenger.Service;
using Corporate_messenger.Service.Notification;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Corporate_messenger.Droid.Broadcast
{
    [BroadcastReceiver(Enabled = true, Label = "Cancel Click Receiver", Exported = true, Name = "com.companyname.corporate_messenger.CancelReceiver")]
    [IntentFilter(new[] { "com.companyname.corporate_messenger.Cancel_Receiver" })]
    public class CancelReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            DependencyService.Get<IForegroundService>().manager.Cancel(0);
       
            DependencyService.Get<IAudio>().StopAudioFile();
            DependencyService.Get<ISocket>().MyWebSocket.Send(JsonConvert.SerializeObject(new { type = "init_call", status = "400" }));
            Toast.MakeText(context, "Сбросил звонок", ToastLength.Short).Show();
        }
    }
}