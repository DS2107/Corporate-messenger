using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corporate_messenger.Droid.Service
{
    class SipServiceConnection: Java.Lang.Object, IServiceConnection
    {
        private CallActivity1 _activityCall;
        private AcceptanceActivity1 _activityAcceptance;
        public bool IsConnected { get; private set; }
        public SipBinder Binder { get; private set; }

        public SipServiceConnection(CallActivity1 activity)
        {
            _activityCall = activity;
            IsConnected = false;
            Binder = null;
        }

        public SipServiceConnection(AcceptanceActivity1 activity)
        {
            _activityAcceptance = activity;
            IsConnected = false;
            Binder = null;
        }


        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            Binder = service as SipBinder;
            IsConnected = Binder != null;
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            IsConnected = false;
            Binder = null;
        }

        public void StartCall()
        {
            if (IsConnected) Binder?.StartCall();
        }

        public void StopCall()
        {
            if (IsConnected) Binder?.StopCall();
        }
    }
}