using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Corporate_messenger.Droid.NotificationManager;

[assembly: Xamarin.Forms.Dependency(typeof(NotificationHelper))]
namespace Corporate_messenger.Droid.NotificationManager
{
    class NotificationHelper : IStaticNotification
    {
        private static string foregroundChannelId = "9001";
        private static Context context = global::Android.App.Application.Context;


        public Notification ReturnNotif()
        {
            // Building intent
            var intent = new Intent(context, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.FromBackground);
           // intent.PutExtra("Title", "Message");

            var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);

            var notifBuilder = new NotificationCompat.Builder(context, foregroundChannelId)
                .SetContentTitle("")
                .SetContentText("")
                .SetSmallIcon(Resource.Drawable.MyChat)
                .SetOngoing(true)
                .SetColor(1234)
                .SetPriority(1)
                
                .SetContentIntent(pendingIntent);


            // Building channel if API verion is 26 or above
            if (global::Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                NotificationChannel notificationChannel = new NotificationChannel(foregroundChannelId, "Title", NotificationImportance.Max);
                notificationChannel.Importance = NotificationImportance.Max;
                notificationChannel.EnableLights(true);
                notificationChannel.EnableVibration(true);
                notificationChannel.SetShowBadge(true);
                notificationChannel.LockscreenVisibility = NotificationVisibility.Public;
                
                notificationChannel.SetVibrationPattern(new long[] { 0L });

                var notifManager = context.GetSystemService(Context.NotificationService) as Android.App.NotificationManager;
                if (notifManager != null)
                {
                    notifBuilder.SetChannelId(foregroundChannelId);
                    notifManager.CreateNotificationChannel(notificationChannel);
                }
            }

            return notifBuilder.Build();
        }
    }
}