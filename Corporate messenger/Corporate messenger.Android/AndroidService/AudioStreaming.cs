using Android.Media;
using Corporate_messenger.Droid.AndroidService;
using Corporate_messenger.Service;
using Corporate_messenger.ViewModels;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using WebSocketSharp;
using Xamarin.Forms;

[assembly: Dependency(typeof(AudioStreaming))]
namespace Corporate_messenger.Droid.AndroidService
{

    public class AudioStreaming: IAudioWebSocketCall
    {
        public CallViewModel callView { get; set; }
        private bool StartStopAudioStream_Flag { get; set; }      
        private int Frequency_Audio { get; set; }
        private int Receiver_id { get; set; }
        private int User_id { get; set; }
        private int Buffer_Size { get; set; }
        public bool FlagRaised { get ; set; }

        private AudioRecord AudioRecord = null;
        private AudioTrack AudioTrack = null;
        public AudioStreaming()
        {
           
        }
        public void InitAudioWebSocketCall(int sender_id)
        {
            Frequency_Audio = 22050;
            User_id = sender_id;
            StartStopAudioStream_Flag = true;


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
        public void ListenerWebSocketCall(byte[] audio_message){
           
            AudioTrack.SetPlaybackRate(Frequency_Audio);
            AudioTrack.Play();
            AudioTrack.Write(audio_message, 0, audio_message.Length);
        }    
        public void StopAudioWebSocketCall(){
            AudioRecord.Stop();
            AudioTrack.Stop();
            
            StartStopAudioStream_Flag = false;
        }
        public async Task StartAudioWebSocketCallAsync(WebSocketSharp.WebSocket ws)
        {
            await Task.Run(() => StartAudioWebSocketCall(ws));
        } 

        private void StartAudioWebSocketCall(WebSocket ws){
           
            byte[] buffer = new byte[Buffer_Size];
            try
            {
                AudioRecord.StartRecording();
            }
            catch(Exception ex)
            {

            }
             
            
            while (StartStopAudioStream_Flag){
                try{
                    AudioRecord.Read(buffer, 0, Buffer_Size);    
                    ws.Send(JsonConvert.SerializeObject(new { type = "init_call", voice_audio = buffer, sender_id = User_id,status="201"}));
                }
                catch (Exception t){
                    var s = t;
                }
            }
        }
    }    
}