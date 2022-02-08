using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using AndroidX.Core.App;
using Corporate_messenger.Service.Notification;
using System;
using Xamarin.Forms;
using AndroidApp = Android.App.Application;

[assembly: Dependency(typeof(Corporate_messenger.Droid.NotificationManager.NotificationMessage))]
namespace Corporate_messenger.Droid.NotificationManager
{
    public class NotificationMessage : INotificationManager
    {
        const string channelId = "Message";
        const string channelName = "Notification_Message";
        const string channelDescription = "Сообщения";

        public static string TitleKey = "Message_title";
        public static string MessageKey = "Message_message";

        bool channelInitialized = false;
        int messageId = 0;
        int pendingIntentId = 0;

        Android.App.NotificationManager manager;

        public event EventHandler NotificationReceived;
        public static NotificationMessage Instance { get; private set; }

        public NotificationMessage() => Initialize();
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
            Intent intent = new Intent(AndroidApp.Context, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);


                intent.PutExtra(TitleKey, "message");
                PendingIntent pendingIntent = PendingIntent.GetActivity(AndroidApp.Context, pendingIntentId++, intent, PendingIntentFlags.UpdateCurrent);

                 notification = new NotificationCompat.Builder(AndroidApp.Context, channelId)
                    .SetAutoCancel(true)
                    .SetPriority((int)NotificationPriority.High)
                    .SetVisibility((int)NotificationVisibility.Public)
                    .SetContentIntent(pendingIntent)
                    .SetContentTitle(title)
                    .SetContentText(message)
                    .SetLargeIcon(BitmapFactory.DecodeResource(AndroidApp.Context.Resources, Resource.Drawable.kot))
                    .SetSmallIcon(Resource.Drawable.MyChat)
                    .SetDefaults((int)NotificationDefaults.Sound | (int)NotificationDefaults.Vibrate);
            
                    manager.Notify(messageId++, notification.Build());
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