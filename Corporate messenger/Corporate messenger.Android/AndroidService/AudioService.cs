using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Corporate_messenger.Droid.AndroidService;
using Corporate_messenger.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

[assembly: Dependency(typeof(AudioService))]
namespace Corporate_messenger.Droid.AndroidService
{
    public class AudioService:IAudio
    {
       
        public AudioService()
        {

        }
        MediaPlayer player;
        public void PlayAudioFile(string fileName)
        {
             player = new MediaPlayer();
            var fd = global::Android.App.Application.Context.Assets.OpenFd(fileName);
            player.Prepared += (s, e) =>
            {
                player.Start();
            };
            player.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);
            player.Prepare();

            
        }

        public void PlayStop()
        {
            player.Stop();
        }
    }
}