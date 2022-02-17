﻿using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Views;
using System.Threading.Tasks;
using Android.Net.Sip;
using System.Net;
using System.Net.Sockets;
using AndroidX.Core.Content;
using Android;
using AndroidX.Core.App;
using Corporate_messenger.Service;
using Xamarin.Forms;
using Java.Util;
using Android.Content;
using System.Linq;
using AndroidX.AppCompat.App;
using Corporate_messenger.Service.Notification;

namespace Corporate_messenger.Droid
{
    [Activity(Label = "Мессенджер", Icon = "@drawable/MyChat", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize,ScreenOrientation =ScreenOrientation.Portrait )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
    
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            bool flag = false;
            int noteId = Intent.GetIntExtra("title", 0);
          
            if (Intent.HasExtra("init_call"))
            {
                
                flag = true;
            }
  
            LoadApplication(new App(flag));
            //Intent.RemoveExtra()
            flag = false;
            /* TinyAccountManager.Droid.AccountManager.Initialize();
             if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
             {

                 Window.SetStatusBarColor(Android.Graphics.Color.Black);
             }
           */
           // SetContentView(Resource.Layout.NotificationLayoutCall);
           





        }

      

        private static Context context = global::Android.App.Application.Context;
  

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}