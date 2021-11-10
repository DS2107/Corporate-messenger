using Corporate_messenger.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Corporate_messenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {

        ChatViewModel chat;
        public ChatPage(int id,string title)
        {
            InitializeComponent();
            BindingContext = chat= new ChatViewModel(id,title);
            Title = title;
            MessagingCenter.Subscribe<ChatViewModel>(this, "Scrol", (sender) => {
                object d = 0;
                foreach (var s in MyListView.ItemsSource)
                {
                    d = s;

                }
                MyListView.ScrollTo(d, ScrollToPosition.End, true);
                MessageEditor.Focus();
            });
            
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
            chat.StopSendMessageAudioCommandAsync();
        }

     

        private void mic_message_Pressed(object sender, EventArgs e)
        {
            mic_message.Background = Brush.Red;
            chat.SendMessageAudioCommand();
        }
    }
    
}