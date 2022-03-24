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
 
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
      
        private ChatViewModel chat;
        private int id_room ;
        private UserDataModel User;
        public ChatPage(int id, string title)
        {
            OnAppearing();
            this.InitializeComponent();
          
            DependencyService.Get<IForegroundService>().chat_room_id = id_room = id;
            if(User !=null)
                BindingContext = chat = new ChatViewModel(id, title, User.Id);

            Title = title;
       

            MessagingCenter.Subscribe<ChatViewModel>(this, "Scrol", (sender) =>
            {
                object d = 0;
                foreach (var s in MyListView.ItemsSource)
                {
                    d = s;

                }
                MyListView.ScrollTo(d, ScrollToPosition.End, true);
             
            });



        }

        private async void GetUser()
        {
            User = await UserDbService.GetUser();
        }

        protected override  void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetTabBarIsVisible(this, false);
            Task.Run(() => GetUser()).Wait();

        }

        private async void VoiceRecord_Clicked(object sender, EventArgs e)
        {
            CallPage callPage = new CallPage(true);
            callPage.SetName(Title);
          await  Navigation.PushAsync(callPage);
            try
            {
                DependencyService.Get<IAudioUDPSocketCall>().ConnectionToServer();
                var s = DependencyService.Get<IAudioUDPSocketCall>().GetServerIp();
                chat.ws.Send(JsonConvert.SerializeObject(new { 
                    type = "init_call",
                    status = "100", 
                    sender_id = User.Id,
                    receiver_id = chat.Rec_id,
                    call_address = DependencyService.Get<IAudioUDPSocketCall>().GetServerIp()
            }));
                DependencyService.Get<IAudio>().PlayAudioFile("gudok.mp3", Android.Media.Stream.VoiceCall);
                DependencyService.Get<IForegroundService>().Flag_AudioCalls_Init = true;
                    

            }
            catch (Exception ex)
            {
                DependencyService.Get<IForegroundService>().MyToast("Не удается позвонить, возможно потеряно соединение с сервором: "+ex.Message);
            }
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