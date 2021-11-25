using Android.App;
using Android.Content;
using Android.Media;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Corporate_messenger.Droid.AndroidService;
using Corporate_messenger.Service;
using Java.Net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(AudioStreaming))]
namespace Corporate_messenger.Droid.AndroidService
{
    class dataRoom
    {
        [JsonProperty("type")]
        public string subs { get; set; }
        [JsonProperty("sender_id")]
        public int sendr_id { get; set; }
    }
    class MyAudio
    {
        [JsonProperty("voice_audio")]
        public byte[] audio { get; set; }
        [JsonProperty("type")]
        public string type { get; set; }
        [JsonProperty("sender_id")]
        public int sendr_id { get; set; }
      
        [JsonProperty("receiverId")]
        public int receiverId { get; set; }

    }
 
    class AudioStreaming: IAudioSocket
    {
        
        private bool StartStopAudioStream_Flag { get; set; }
        private int freq = 22050;
        private AudioRecord audioRecord = null;
        private Thread Rthread = null;
        private AudioManager audioManager = null;
        private AudioTrack audioTrack = null;
        int rec_id = 0;
        int userid = 0;
        int bufferSize = 0;
        public void Init(int sender_id,int receiverId)
        {

            userid = sender_id;
            rec_id = receiverId;
          
           // ws.OnOpen += WsOnOpen;
           // ws.Connect();

            bufferSize = AudioRecord.GetMinBufferSize(freq,
                   ChannelIn.Mono,
                   Android.Media.Encoding.Pcm16bit);


            audioRecord = new AudioRecord(AudioSource.Mic, freq,
                   ChannelIn.Mono,
                      Android.Media.Encoding.Pcm16bit, bufferSize);

            audioTrack = new AudioTrack(Android.Media.Stream.VoiceCall, freq,
                     ChannelConfiguration.Mono,
                     Android.Media.Encoding.Pcm16bit, bufferSize,
                    AudioTrackMode.Stream);
           

           
            
        }

        public async Task Start2(WebSocketSharp.WebSocket ws)
        {
            await Task.Run(() => Start(ws));
           
        }
        
       



      
       /* private void WsOnMEssage(object sender, MessageEventArgs e)
        {
            var myAudio = JsonConvert.DeserializeObject<MyAudio>(e.Data);
            byte[] audio_message = myAudio.audio;
            audioTrack.SetPlaybackRate(freq);
            audioTrack.Play();
            audioTrack.Write(audio_message, 0, audio_message.Length);
        }*/
        public void PlayVoiceChat(byte[] audio_message)
        {
           
            audioTrack.SetPlaybackRate(freq);
            audioTrack.Play();
            audioTrack.Write(audio_message, 0, audio_message.Length);
        }
        public AudioStreaming()
        {
          
        }

       public void FileCreate()
        {
            string filename = Path.Combine(Application.Context.GetExternalFilesDir(null).ToString(), "recording.wav");

            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
        }


        public void StopAudio()
        {
            audioRecord.Stop();
            audioTrack.Stop();
            StartStopAudioStream_Flag = false;
        }

        public void Start(WebSocketSharp.WebSocket ws)
        {
            // audioTrack.SetPlaybackRate(freq);
            StartStopAudioStream_Flag = true;
            byte[] buffer = new byte[bufferSize];
            audioRecord.StartRecording();
           
            while (StartStopAudioStream_Flag)
            {
                try
                {
                    audioRecord.Read(buffer, 0, bufferSize);
                    var new_message = new MyAudio { type = "call", audio = buffer,sendr_id=userid,receiverId=rec_id };
                    var message = JsonConvert.SerializeObject(new_message);
                    ws.Send(message);
                  
                    
                }
                catch (Exception t)
                {
                 
                }
            }
        
        } 
    }    
}