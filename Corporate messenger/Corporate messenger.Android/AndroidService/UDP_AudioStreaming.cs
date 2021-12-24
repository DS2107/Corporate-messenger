using Android.Media;
using Corporate_messenger.Droid.AndroidService;
using Corporate_messenger.Service;
using Corporate_messenger.ViewModels;
using Sockets.Plugin;
using System;
using System.Net;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(UDP_AudioStreaming))]
namespace Corporate_messenger.Droid.AndroidService
{

    class UDP_AudioStreaming : IAudioUDPSocketCall
    {

        private bool StartStopAudioStream_Flag { get; set; }
        private int Frequency_Audio { get; set; }
        private int Receiver_id { get; set; }
        private int User_id { get; set; }
        private int Buffer_Size { get; set; }
        public CallViewModel callView { get; set; }

        private AudioRecord AudioRecord = null;
        private AudioTrack AudioTrack = null;
        int port = 1234;
        string address = "192.168.0.105";

        UdpSocketClient client  = new UdpSocketClient();
        public UDP_AudioStreaming()
        {
          
            
        }

        public void Client_MessageReceived(object sender, Sockets.Plugin.Abstractions.UdpSocketMessageReceivedEventArgs e)
        {
            AudioTrack.SetPlaybackRate(Frequency_Audio);
            AudioTrack.Play();
            AudioTrack.Write(e.ByteData, 0, e.ByteData.Length);
        }

        public void InitUDP(int user_id,int rec_id)
        {
           
            client.ConnectAsync(address, port);
          
            client.MessageReceived += Client_MessageReceived;

            Frequency_Audio = 22050;
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

        
        private void StartAudioUDPCall()
        {
           
            StartStopAudioStream_Flag = true;
            byte[] buffer = new byte[Buffer_Size];
            AudioRecord.StartRecording();

            while (StartStopAudioStream_Flag)
            {
                try
                {
                    AudioRecord.Read(buffer, 0, Buffer_Size);
                    client.SendAsync(buffer);
                }
                catch (Exception t)
                {
                    var s = t;
                }
            }
        }
        public async Task StartAudioUDPCallAsync()
        {
            await Task.Run(() => StartAudioUDPCall());
        }

        public void StopAudioUDPCall()
        {
            AudioRecord.Stop();
            AudioTrack.Stop();
            StartStopAudioStream_Flag = false;
            client.DisconnectAsync();
        }
    }
}