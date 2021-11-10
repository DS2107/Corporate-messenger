using Android.App;
using Android.Content;
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
    [Activity(Label = "CallActivity1")]
    public class CallActivity1 : Activity
    {
        SipServiceConnection _serviceConnection;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (_serviceConnection == null)
                _serviceConnection = new SipServiceConnection(this);
            Intent serviceToStart = new Intent(this, typeof(SipService));
            BindService(serviceToStart, _serviceConnection, Bind.AutoCreate);
            StartCall();
        }
        private void CloseOnClick(object sender, EventArgs e)
        {
            _serviceConnection?.StopCall();
        }

        private void StartCall()
        {
            _serviceConnection?.StartCall();
        }
        
    }
}