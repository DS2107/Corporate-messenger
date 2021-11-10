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
    class SipBinder:Binder
    {
        public SipService Service { get; private set; }
        public SipBinder(SipService service)
        {
            Service = service;
        }

        public void StartCall()
        {
            Service.StartCall();
        }

        public void StopCall()
        {
            Service.StopCall();
        }
    }
}