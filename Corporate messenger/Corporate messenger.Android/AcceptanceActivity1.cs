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
    [Activity(Label = "AcceptanceActivity1")]
    public class AcceptanceActivity1 : Activity
    {
        private SipServiceConnection _serviceConnection;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ///stuff
            if (_serviceConnection == null)
                _serviceConnection = new SipServiceConnection(this);
            Intent serviceToStart = new Intent(this, typeof(SipService));
            BindService(serviceToStart, _serviceConnection, Bind.AutoCreate);
        }

        private void CloseOnClick(object sender, EventArgs e)
        {
            _serviceConnection?.StopCall();
            Finish();
        }

        private void AnswerOnClick(object sender, EventArgs e)
        {
            var root = new Intent(this, typeof(CallActivity1));
            StartActivity(root);
            Finish();
        }
    }
}