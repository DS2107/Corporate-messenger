using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using Corporate_messenger.Service.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using AndroidApp = Android.App.Application;

[assembly: Dependency(typeof(Corporate_messenger.Droid.NotificationManager.NotificationCall))]
namespace Corporate_messenger.Droid.NotificationManager
{
    class NotificationCall
    {
        const string channelId = "Call";
        const string channelName = "Notification_Call";
        const string channelDescription = "Вызовы";

        public static string TitleKey = "Call_title";
        public static string MessageKey = "Calll_message";

        bool channelInitialized = false;
      
        int pendingIntentId = 0;

        Android.App.NotificationManager manager;

        public event EventHandler NotificationReceived;
        public static NotificationCall Instance { get; private set; }

        public NotificationCall() => Initialize();
        public void Initialize()
        {
            if (Instance == null)
            {
                CreateNotificationChannel();
                Instance = this;
            }
        }
        public void SendNotification(string title, string message)
        {
            if (!channelInitialized)
            {
                CreateNotificationChannel();
            }


            Show(title, message);

        }

        public void ReceiveNotification(string title, string message)
        {
            var args = new NotificationEventArgs()
            {
                Title = title,
                Message = message,
            };
            NotificationReceived?.Invoke(null, args);
        }
        NotificationCompat.Builder notification;
        public void Show(string title, string message)
        {
            Intent intent = new Intent(AndroidApp.Context, typeof(CallActivity));
            DependencyService.Get<IForegroundService>().CallPageFlag = false;
            Intent IntentAcceptCall = new Intent("com.companyname.corporate_messenger.Accept_Receiver");
            Intent intentCancelCall = new Intent("com.companyname.corporate_messenger.Cancel_Receiver");
            intent.AddFlags(ActivityFlags.ClearTop);
         
            TitleKey = "init_call";
                intent.PutExtra(TitleKey, "init_call");
                // intent.PutExtra(MessageKey, message);

                var view = new RemoteViews("com.companyname.corporate_messenger", Resource.Layout.NotificationLayoutCall);
                view.SetTextViewText(Resource.Id.title_user, "Звонок от "+ message );
                view.SetImageViewResource(Resource.Id.image, Resource.Drawable.kot);
                
                PendingIntent pendingIntent = PendingIntent.GetActivity(AndroidApp.Context, pendingIntentId++, intent, PendingIntentFlags.UpdateCurrent);
                PendingIntent pendingAcceptCall = PendingIntent.GetBroadcast(AndroidApp.Context, pendingIntentId++, IntentAcceptCall, PendingIntentFlags.UpdateCurrent);
                PendingIntent pendingCancelCall = PendingIntent.GetBroadcast(AndroidApp.Context, pendingIntentId++, intentCancelCall, PendingIntentFlags.UpdateCurrent);

            view.SetOnClickPendingIntent(Resource.Id.button_accept_call, pendingAcceptCall);
            view.SetOnClickPendingIntent(Resource.Id.button_stop_call, pendingCancelCall);

            notification = new NotificationCompat.Builder(AndroidApp.Context, channelId)
                   .SetSmallIcon(Resource.Drawable.MyChat)
                   .SetAutoCancel(true)
                   .SetContentIntent(pendingIntent)
                   .SetPriority((int)NotificationPriority.Max)
                   .SetContent(view)
                   .SetContentTitle("Incoming call")
                   .SetFullScreenIntent(pendingIntent, true)             
                   .SetOngoing(true)
                   .SetContentText("James Smith");

            Notification notif = notification.Build();
           
            manager.Notify(0, notif);
            DependencyService.Get<IForegroundService>().manager = manager;
        }

        void CreateNotificationChannel()
        {
            manager = (Android.App.NotificationManager)AndroidApp.Context.GetSystemService(AndroidApp.NotificationService);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelNameJava = new Java.Lang.String(channelName);
                var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.Max)
                {
                    LockscreenVisibility = NotificationVisibility.Public,
                    Importance = NotificationImportance.Max,
                    Description = channelDescription
                };
                manager.CreateNotificationChannel(channel);
            }

            channelInitialized = true;
        }

        long GetNotifyTime(DateTime notifyTime)
        {
            DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(notifyTime);
            double epochDiff = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;
            long utcAlarmTime = utcTime.AddSeconds(-epochDiff).Ticks / 10000;
            return utcAlarmTime; // milliseconds
        }
    }
}