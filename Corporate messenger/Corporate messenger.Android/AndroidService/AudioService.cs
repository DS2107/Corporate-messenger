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
        MediaRecorder ClassMedia;
        FileService FileService = new FileService();

        public void PlayAudioFile(string fileName)
        {
            if (player == null)          
                player = new MediaPlayer();
           
            player.Reset();          
            player.SetDataSource(fileName);
            player.Prepare();
            player.Start();
  
        }

        public void ResumeAudioFile()
        {       
            player.Start();
        }

        public double GetPositionAudio()
        {
            if (player != null)
                return player.CurrentPosition/1000.0;
            else
                return 0;
        }
        public double GetFullTimeAudio()
        {
            double arr =0;
            if (player != null)
            {
                arr = player.Duration/1000.0;

                return arr;
            }
            else
                return 0;
            
        }

        public void StopAudioFile()
        {
            if (player != null)                       
                player.Pause();         
        }
   
        public void StopSendMessageAudio()
        {
            if (ClassMedia != null)
            {            
                ClassMedia.Stop();
                ClassMedia.Release();
                ClassMedia = null;
            }
        }

        public void SendMessageAudio()
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