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
            if (player == null)
            {
                player = new MediaPlayer();
            }
            player.Reset();
            player.SetDataSource(fileName);
            player.Prepare();
            player.Start();
            // var fd = global::Android.App.Application.Context.Assets.OpenFd(fileName);
            /* player.Prepared += (s, e) =>
             {
                 player.Start();
             };
             player.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);*/


        }

        MediaRecorder ClassMedia;
        public void StopSendMessageAudioCommandAsync()
        {

            if (ClassMedia != null)
            {
              
                ClassMedia.Stop();
                ClassMedia.Release();
                ClassMedia = null;
            }
        }




        FileService FileService = new FileService();
        public void SendMessageAudioCommand()
        {
            if (ClassMedia == null)
                ClassMedia = new MediaRecorder();
            else
                ClassMedia.Reset();

            ClassMedia.SetAudioSource(AudioSource.Mic);
            ClassMedia.SetOutputFormat(OutputFormat.AmrNb);
            ClassMedia.SetAudioEncoder(AudioEncoder.AmrNb);
            ClassMedia.SetOutputFile(FileService.CreateAudioFile());



            ClassMedia.Prepare();
            ClassMedia.Start();
        }

        public void PlayStop()
        {
            player.Stop();
        }
    }
}