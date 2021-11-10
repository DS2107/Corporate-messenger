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

namespace Corporate_messenger.Droid.Notifi
{
    interface INotification
    {
        Notification ReturnNotif();
        // Receiver1 receiver { get; set; }
    }
}