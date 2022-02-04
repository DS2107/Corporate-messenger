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

[assembly: Dependency(typeof(Corporate_messenger.Droid.NotificationManager.AndroidNotificationManager))]
namespace Corporate_messenger.Droid.NotificationManager
{
    public class AndroidNotificationManager : INotificationManager
    {
        const string channelId = "Message";
        const string channelName = "MessageNotification";
        const string channelDescription = "The default channel for notifications.";

        public static  string TitleKey = "title";
        public  static string MessageKey = "message";

        bool channelInitialized = false;
        int messageId = 0;
        int pendingIntentId = 0;

        Android.App.NotificationManager manager;

        public event EventHandler NotificationReceived;
        public static AndroidNotificationManager Instance { get; private set; }

        public AndroidNotificationManager() => Initialize();
        public void Initialize()
        {
            if (Instance == null)
            {
                CreateNotificationChannel();
                Instance = this;
            }
        }
        public void SendNotification(string title, string message, DateTime? notifyTime = null)
        {
            if (!channelInitialized)
            {
                CreateNotificationChannel();
            }

            if (notifyTime != null)
            {
                Intent intent = new Intent(AndroidApp.Context, typeof(AlarmHandler));
                intent.PutExtra(TitleKey, title);
                intent.PutExtra(MessageKey, message);
          
                PendingIntent pendingIntent = PendingIntent.GetBroadcast(AndroidApp.Context, pendingIntentId++, intent, PendingIntentFlags.CancelCurrent);
                long triggerTime = GetNotifyTime(notifyTime.Value);
                AlarmManager alarmManager = AndroidApp.Context.GetSystemService(Context.AlarmService) as AlarmManager;
                alarmManager.Set(AlarmType.RtcWakeup, triggerTime, pendingIntent);
            }
            else
            {
                Show(title, message);
            }
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

        public void Show(string title, string message)
        {
            Intent intent = new Intent(AndroidApp.Context, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);

            if (message == "Звонок")
            {
                TitleKey = "init_call";
                intent.PutExtra(TitleKey, "init_call");
            }

            else
            {
                intent.PutExtra(TitleKey, "message");
            }

            // intent.PutExtra(MessageKey, message);

            var view = new RemoteViews("com.companyname.corporate_messenger", Resource.Layout.NotificationLayout);
            view.SetTextViewText(Resource.Id.title, "Title Text");
            view.SetImageViewResource(Resource.Id.image, Resource.Drawable.kot);

            PendingIntent pendingIntent = PendingIntent.GetActivity(AndroidApp.Context, pendingIntentId++, intent, PendingIntentFlags.UpdateCurrent);
            Notification s = new Notification();

            var notification = new NotificationCompat.Builder(AndroidApp.Context, channelId)
                .SetSmallIcon(Resource.Drawable.MyChat)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent)
                .SetPriority((int)NotificationPriority.Max)
               .SetContent(view)
                .SetContentTitle("Incoming call")
                .SetFullScreenIntent(pendingIntent, true)
            // Notification text, usually the caller’s name
            .SetOngoing(true)
            .SetContentText("James Smith");







           // Notification notification = builder.Build();


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