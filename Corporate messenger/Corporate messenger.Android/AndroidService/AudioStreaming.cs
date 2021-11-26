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
 
    class AudioStreaming: IAudioWebSocketCall
    {
        
        private bool StartStopAudioStream_Flag { get; set; }      
        private int Frequency_Audio { get; set; }
        private int Receiver_id { get; set; }
        private int User_id { get; set; }
        private int Buffer_Size { get; set; }
        private AudioRecord AudioRecord = null;
        private AudioTrack AudioTrack = null;
        public AudioStreaming()
        {

        }
        public void InitAudioWebSocketCall(int sender_id,int receiverId)
        {
            Frequency_Audio = 22050;
            User_id = sender_id;
            Receiver_id = receiverId;

            Buffer_Size = AudioRecord.GetMinBufferSize
                (
                     Frequency_Audio,
                     ChannelIn.Mono,
                     Android.Media.Encoding.Pcm16bit
                );
            AudioRecord = new AudioRecord
                (
                    AudioSource.Mic, 
                    Frequency_Audio,
                    ChannelIn.Mono,
                    Android.Media.Encoding.Pcm16bit,
                    Buffer_Size
                );

            AudioTrack = new AudioTrack
                (
                    Android.Media.Stream.VoiceCall, 
                    Frequency_Audio,
                    ChannelConfiguration.Mono,
                    Android.Media.Encoding.Pcm16bit, 
                    Buffer_Size,
                    AudioTrackMode.Stream
                );   
        }    
        public void ListenerWebSocketCall(byte[] audio_message)
        {
           
            AudioTrack.SetPlaybackRate(Frequency_Audio);
            AudioTrack.Play();
            AudioTrack.Write(audio_message, 0, audio_message.Length);
        }    
        public void StopAudioWebSocketCall()
        {
            AudioRecord.Stop();
            AudioTrack.Stop();
            StartStopAudioStream_Flag = false;
        }
        public async Task StartAudioWebSocketCallAsync(WebSocketSharp.WebSocket ws)
        {
            await Task.Run(() => StartAudioWebSocketCall(ws));
        } 

        private void StartAudioWebSocketCall(WebSocket ws)
        {
            StartStopAudioStream_Flag = true;
            byte[] buffer = new byte[Buffer_Size];
            AudioRecord.StartRecording();

            while (StartStopAudioStream_Flag)
            {
                try
                {
                    AudioRecord.Read(buffer, 0, Buffer_Size);
                    var new_message = new MyAudio { type = "call", audio = buffer, sendr_id = User_id, receiverId = Receiver_id };
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