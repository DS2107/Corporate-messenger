using Android.App;
using Android.Content;
using Android.Net.Sip;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Corporate_messenger.Droid.Notifi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corporate_messenger.Droid.Service
{
    class SipService:Android.App.Service, ISipRegistrationListener
    {
        public IBinder Binder { get; private set; }
        public SipManager SipManager;
        public SipProfile SipProfile;
        public IncomingCallReceiver CallReceiver;
        public SipAudioCall AudioCall;
        private static Context context = global::Android.App.Application.Context;
        Notification notification = new Notification();
        NotofocationHolper help = new NotofocationHolper();
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {

            Toast.MakeText(context, "Служба запущена", ToastLength.Long).Show();
            notification = help.ReturnNotif();
            NotofocationHolper not = new NotofocationHolper();
            //BroadcastReceiver
            // alarm mamager android
            StartForeground(8000, not.ReturnNotif());

            RegisterReceiver();
            CreateSipProfile("1002", "1234", "192.168.0.105");
            return StartCommandResult.Sticky;
        }
    

        public void RegisterReceiver()
        {
            var filter = new IntentFilter();
            filter.AddAction("com.companyname.corporate_messenger.IncomingCallReceiver");
            CallReceiver = new IncomingCallReceiver();
            RegisterReceiver(CallReceiver, filter);
        }

        public override IBinder OnBind(Intent intent)
        {
            Binder = new SipBinder(this);
            return new SipBinder(this);
        }

        public void StartCall()
        {
            if (AudioCall == null) return;
            AudioCall.AnswerCall(30);
            AudioCall.StartAudio();
            if (AudioCall.IsMuted) AudioCall.ToggleMute();
        }

        public void StopCall()
        {
            AudioCall?.Close();
        }

        public override void OnDestroy()
        {
            Binder = null;
            CloseLocalSipProfile();
            base.OnDestroy();
        }

        public void OnRegistering(string localProfileUri) { }

        public void OnRegistrationDone(string localProfileUri, long expiryTime) { }

        public void OnRegistrationFailed(string localProfileUri, SipErrorCodes errorCode, string errorMessage)
        {
            CloseLocalSipProfile();
        }


        private void CreateSipProfile(string username, string password, string domain)
        {
            if (SipManager == null)
                SipManager = SipManager.NewInstance(this);
            var builder = new SipProfile.Builder(username, domain);
            builder.SetPassword(password);
            SipProfile = builder.Build();
            RegisterSipIncom();
            
        }

        private void RegisterSipIncom()
        {
            var intent = new Intent();
            intent.SetAction("com.companyname.corporate_messenger.INCOMING_CALL");
            var pendingIntent = PendingIntent.GetBroadcast(this, 0, intent,
                     (PendingIntentFlags)FillInFlags.Data);
            SipManager?.Open(SipProfile, pendingIntent, null);
            SipManager?.SetRegistrationListener(SipProfile.UriString, this);
        }

        public void CloseLocalSipProfile()
        {
            if (SipManager == null) return;
            if (SipProfile != null)
                SipManager.Close(SipProfile.UriString);
            if (CallReceiver != null)
                UnregisterReceiver(CallReceiver);
        }
    }
}