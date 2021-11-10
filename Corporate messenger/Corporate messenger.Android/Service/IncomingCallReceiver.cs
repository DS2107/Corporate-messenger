using Android.App;
using Android.Content;
using Android.Net.Sip;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Corporate_messenger.Droid.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corporate_messenger.Droid
{
    
    [BroadcastReceiver(Name = "com.companyname.corporate_messenger.IncomingCallReceiver", Enabled = true, Exported = true)]
    [IntentFilter(new[] { "com.companyname.corporate_messenger.INCOMING_CALL" })]
    public class IncomingCallReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            SipAudioCall incomingCall = null;

            var listener = new SipAudioCall.Listener();
            SipService service = (SipService)context;

            incomingCall = service.SipManager.TakeAudioCall(intent, listener);
            if (incomingCall.IsMuted) incomingCall.ToggleMute();
            service.AudioCall = incomingCall;

            var newIntent = new Intent(Android.App.Application.Context, typeof(AcceptanceActivity1));
            newIntent.AddFlags(ActivityFlags.NewTask);
            service.StartActivity(newIntent);

        }

       
    }
}