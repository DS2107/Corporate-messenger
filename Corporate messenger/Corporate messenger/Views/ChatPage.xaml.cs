using Corporate_messenger.DB;
using Corporate_messenger.Models;
using Corporate_messenger.Service;
using Corporate_messenger.Service.Notification;
using Corporate_messenger.ViewModels;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Corporate_messenger.Views
{
    public class State
    {
        private const int bufSize = 8 * 1024;
        public byte[] buffer = new byte[bufSize];
    }
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {

        ChatViewModel chat;
        private int id_room ;
        private const int bufSize = 8 * 1024;
        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
       
        private State state = new State();
        private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
        private AsyncCallback recv = null;
     
        private string Refport;
        public ChatPage(int id, string title)
        {
            InitializeComponent();
            id_room = id;
            DependencyService.Get<IForegroundService>().chat_room_id = id;
            BindingContext = chat = new ChatViewModel(id, title,Navigation);
            Title = title;
            send_message.IsVisible = false;
            mic_message.IsVisible = true;
            
            MessagingCenter.Subscribe<ChatViewModel>(this, "Scrol", (sender) =>
            {
                object d = 0;
                foreach (var s in MyListView.ItemsSource)
                {
                    d = s;

                }
                MyListView.ScrollTo(d, ScrollToPosition.End, true);
               // MessageEditor.Focus();
            });
            //DependencyService.Get<IAudioUDPSocketCall>().InitUDP();
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetTabBarIsVisible(this, false);
            User = await UserDbService.GetUser();
            //await chat.GetSql(id_room);
           
            if (chat.Next_page_url == null)
            {
                chat.contentJobjects = await chat.GetInfo_HttpMethod_Get_Async("/api/chat/" + id_room + "/" + User.Id + "/dialog");
                if (chat.contentJobjects != null)
                {
                    chat.ThreadMessage = new Thread(new ThreadStart(chat.SendToken_GetChats));
                    chat.ThreadMessage.Start();
                }
                else
                {
                    DependencyService.Get<IFileService>().MyToast("Отсутствует соеденение с сервером, проверьте подключение к интернету и потворите попытку");
                }
            }


        }

        private void CallButton_Clicked(object sender, EventArgs e)
        {
            _ = GoToPagaeCall();
        }

        async Task GoToPagaeCall()
        {
            await Navigation.PushAsync(new CallPage(true));

        }

        private void mic_message_Released(object sender, EventArgs e)
        {
            mic_message.Background = Brush.Transparent;
            send_message.IsEnabled = true;
            MessageEditor.IsEnabled = true;
            DependencyService.Get<IAudio>().StopSendMessageAudio();
            byte[] bytes = File.ReadAllBytes(DependencyService.Get<IFileService>().GetAudioFile());
            chat.SendMyMessage(bytes);
        }



        private void mic_message_Pressed(object sender, EventArgs e)
        {
            send_message.IsEnabled = false;
            MessageEditor.IsEnabled = false;
            mic_message.Background = Brush.Red;
            DependencyService.Get<IAudio>().SendMessageAudio();
        }
   
        public bool BackColor_Flag = false;
      

        private  void VoiceRecord_Clicked(object sender, EventArgs e)
        {
            //  DependencyService.Get<IAudioWebSocketCall>().InitAudioWebSocketCall(chat.user.Id, chat.user.receiverId);
            //  DependencyService.Get<IAudioWebSocketCall>().StartAudioWebSocketCallAsync(chat.ws);
          
            Navigation.PushAsync(new CallPage(true));
            try
            {

                //Client("192.168.0.105", 1234);

                DependencyService.Get<IAudioUDPSocketCall>().ConnectionToServer();
                DependencyService.Get<ISocket>().MyWebSocket.Send(JsonConvert.SerializeObject(new { 
                    type = "init_call",
                    status = "100", 
                    sender_id = User.Id,
                    receiver_id = chat.SpecDataUser.receiverId,
                    call_address = DependencyService.Get<IAudioUDPSocketCall>().GetServerIp()
            }));
                DependencyService.Get<IAudio>().PlayAudioFile("gudok.mp3", Android.Media.Stream.VoiceCall);
                DependencyService.Get<IForegroundService>().AudioCalls_Init = true;
                    

            }
            catch (Exception ex)
            {
                var b = ex;
            }

            /* if (!BackColor_Flag)
             {
                 mic_message.IsVisible = false;
                 VoiceRecord.IconImageSource = ImageSource.FromFile("rec.png");
                  DependencyService.Get<IAudioWebSocketCall>().InitAudioWebSocketCall(chat.user.Id, chat.user.receiverId);
                  DependencyService.Get<IAudioWebSocketCall>().StartAudioWebSocketCallAsync(chat.ws);
                // DependencyService.Get<IAudioUDPSocketCall>().InitUDP(chat.user.Id,chat.user.receiverId);
                // DependencyService.Get<IAudioUDPSocketCall>().StartAudioUDPCallAsync();
                 BackColor_Flag = true;
             }
             else
             {
                 mic_message.IsVisible = true;
                 VoiceRecord.IconImageSource = ImageSource.FromFile("audioSocket.png");
                 //DependencyService.Get<IAudioUDPSocketCall>().StopAudioUDPCall();
                 DependencyService.Get<IAudioWebSocketCall>().StopAudioWebSocketCall();
                 BackColor_Flag = false;
             }*/


        }

        UserDataModel User;


        public void Server(string address, int port)
        {
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.Bind(new IPEndPoint(IPAddress.Parse(address), port));
            Receive();
        }

        public void Client(string address, int port)
        {
            _socket.Connect(IPAddress.Parse(address), port);
            Refport = _socket.LocalEndPoint.ToString ();

            Receive();
        }

        public void Send(string text)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            _socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndSend(ar);
                Console.WriteLine("SEND: {0}, {1}", bytes, text);
            }, state);
        }

        private void Receive()
        {
            _socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndReceiveFrom(ar, ref epFrom);
                _socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);
                Console.WriteLine("RECV: {0}: {1}, {2}", epFrom.ToString(), bytes, Encoding.ASCII.GetString(so.buffer, 0, bytes));
            }, state);
        }

        private void MessageEditor_Completed(object sender, EventArgs e)
        {
            MessageEditor.Focus();
        }

        private void MessageEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MessageEditor.Text != "")
            {
                mic_message.IsVisible = false;
                send_message.IsVisible = true;
            }
            else
            {
                send_message.IsVisible = false;
                mic_message.IsVisible = true;
               
            }
        }
    }


}