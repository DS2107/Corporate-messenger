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
            var s = player.CurrentPosition;
             var time  = player.Duration/1000.0;
            var doubletime = TimeSpan.FromSeconds(time);
            var sd = GetInfo();
            

        }

        public void Resume()
        {
           // player.SeekTo( GetPosition());
            player.Start();
        }

        public double GetPosition()
        {
            if (player != null)
                return player.CurrentPosition/1000.0;
            else
                return 0;
        }
        public double GetInfo()
        {
            double arr =0;
            if (player != null)
            {
                arr = player.Duration/1000.0;

                return arr;
            }
            else
            {
                return 0;
            }
        }

        public void StopAudioFile()
        {
            if (player != null)
            {
             
                player.Pause();
            }

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
            ClassMedia.SetOutputFormat(OutputFormat.AmrWb);
            ClassMedia.SetAudioEncoder(AudioEncoder.AmrWb);
            ClassMedia.SetOutputFile(FileService.CreateAudioFile());
           


            ClassMedia.Prepare();
            ClassMedia.Start();
        }

    }
}