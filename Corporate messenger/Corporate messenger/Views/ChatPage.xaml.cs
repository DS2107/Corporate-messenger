using Corporate_messenger.Models.Chat;
using Corporate_messenger.Service;
using Corporate_messenger.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Corporate_messenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {

        ChatViewModel chat;
        public ChatPage(int id, string title)
        {
            InitializeComponent();

            BindingContext = chat = new ChatViewModel(id, title);
            Title = title;
            MessagingCenter.Subscribe<ChatViewModel>(this, "Scrol", (sender) =>
            {
                object d = 0;
                foreach (var s in MyListView.ItemsSource)
                {
                    d = s;

                }
                MyListView.ScrollTo(d, ScrollToPosition.End, true);
                MessageEditor.Focus();
            });
            //DependencyService.Get<IAudioUDPSocketCall>().InitUDP();
        }

        private void CallButton_Clicked(object sender, EventArgs e)
        {
            _ = GoToPagaeCall();
        }

        async Task GoToPagaeCall()
        {
            await Navigation.PushAsync(new CallPage());

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
            if (!BackColor_Flag)
            {
                mic_message.IsVisible = false;
                VoiceRecord.IconImageSource = ImageSource.FromFile("rec.png");
                // DependencyService.Get<IAudioWebSocketCall>().InitAudioWebSocketCall(chat.user.Id, chat.user.receiverId);
                // DependencyService.Get<IAudioWebSocketCall>().StartAudioWebSocketCallAsync(chat.ws);
                DependencyService.Get<IAudioUDPSocketCall>().InitUDP();
                DependencyService.Get<IAudioUDPSocketCall>().StartAudioUDPCallAsync();
                BackColor_Flag = true;
            }
            else
            {
                mic_message.IsVisible = true;
                VoiceRecord.IconImageSource = ImageSource.FromFile("audioSocket.png");
                DependencyService.Get<IAudioUDPSocketCall>().StopAudioUDPCall();
               // DependencyService.Get<IAudioWebSocketCall>().StopAudioWebSocketCall();
                BackColor_Flag = false;
            }

         
        }
    }


}