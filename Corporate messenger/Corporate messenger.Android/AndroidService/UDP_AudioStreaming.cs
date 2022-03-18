using Android.Media;
using Corporate_messenger.DB;
using Corporate_messenger.Droid.AndroidService;
using Corporate_messenger.Models;
using Corporate_messenger.Service;
using Corporate_messenger.Service.Notification;
using Corporate_messenger.ViewModels;
using Sockets.Plugin;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(UDP_AudioStreaming))]
namespace Corporate_messenger.Droid.AndroidService
{

    class UDP_AudioStreaming : IAudioUDPSocketCall
    {
        public bool FlagRaised { get; set; } // Отметка что пользователь поднял трубку
        static UdpClient sender; // создаем UdpClient для отправки сообщений
        private bool StartStopAudioStream_Flag { get; set; } // флаг остановки передачи голоса
        private bool StartStopAudioReceiver_Flag { get; set; } // флаг отсановки прослушивания
        private int Frequency_Audio { get; set; } // частота звука
        private int Buffer_Size { get; set; } 
        public CallViewModel callView { get; set; }

        private AudioRecord AudioRecord = null;
        private AudioTrack AudioTrack = null;

        private string address = "192.168.10.254";
        private int port = 1234;

       
        public UDP_AudioStreaming()
        {
          
            
        }

        public void ConnectionToServer()
        {

            sender = new UdpClient();
            sender.Connect(address, port);
        }
        public string GetServerIp()
        {
            return sender.Client.LocalEndPoint.ToString(); 
        }
        Thread receiveThread;
        public void StartReceive()
        {
            receiveThread = new Thread(new ThreadStart(ReceiveMessage));
            receiveThread.Start();
        }
        Thread SendThread;
        public  void SendMessage()
        {
            SendThread = new Thread(new ThreadStart(SendVoice));
            SendThread.Start();
          
        }
        
        public void MicOff()
        {
            StartStopAudioStream_Flag = false;
            AudioRecord.Stop();
            SendThread.Abort();
        }

        public void MicOn()
        {
            StartStopAudioStream_Flag = true;
            SendThread = new Thread(new ThreadStart(SendVoice));
            SendThread.Start();
        }

        private void SendVoice()
        {
            StartStopAudioStream_Flag = true;
            byte[] buffer = new byte[Buffer_Size];

            Task.Run(() => this.GetUserAsync()).Wait();
           
            int id = User.Id;
            byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes("sender_id:" + id.ToString() +";" + " call_id:"+DependencyService.Get<IForegroundService>().call_id+";");
            AudioRecord.StartRecording();
            try
            {
                while (StartStopAudioStream_Flag)
                {

                    AudioRecord.Read(buffer, 0, Buffer_Size);
                    sender.Send(data, data.Length);
                    sender.Send(buffer, buffer.Length); // отправка


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                sender.Close();
                SendThread.Abort();
            }
          
        }
        UserDataModel User;
        private async Task GetUserAsync()
        {
             User = await UserDbService.GetUser();
        }

        private  void ReceiveMessage()
        {
            StartStopAudioReceiver_Flag = true;
             IPAddress ipserv = IPAddress.Parse(address);
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(ipserv, port);
            try
            {
                while (StartStopAudioReceiver_Flag)
                {
                    byte[] data = sender.Receive(ref RemoteIpEndPoint); // получаем данные
                    AudioTrack.SetPlaybackRate(Frequency_Audio);
                    AudioTrack.Play();
                    AudioTrack.Write(data, 0, data.Length);
                    
                }
              
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                sender.Close();
            }
            finally
            {
              //  sender.Close();
            }
        }

        public void Client_MessageReceived(object sender, Sockets.Plugin.Abstractions.UdpSocketMessageReceivedEventArgs e)
        {
            AudioTrack.SetPlaybackRate(Frequency_Audio);
            AudioTrack.Play();
            AudioTrack.Write(e.ByteData, 0, e.ByteData.Length);
        }

        public void InitUDP()
        {
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
       
        public void StopAudioUDPCall()
        {
            sender.Close();
                StartStopAudioReceiver_Flag = false;
                StartStopAudioStream_Flag = false;
                AudioRecord.Stop();
                AudioTrack.Stop();
            AudioRecord = null;
            AudioTrack = null;

              //  sender.Close();
              //   sender = null;

        }
    }
}