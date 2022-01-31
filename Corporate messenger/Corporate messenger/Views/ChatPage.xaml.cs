﻿using Corporate_messenger.DB;
using Corporate_messenger.Models;
using Corporate_messenger.Service;
using Corporate_messenger.Service.Notification;
using Corporate_messenger.ViewModels;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Corporate_messenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {

        ChatViewModel chat;
        private int id_room ;
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
            var user = await UserDbService.GetUser();
            //await chat.GetSql(id_room);
           
            if (chat.Next_page_url == null)
            {
                chat.contentJobjects = await chat.GetInfo_HttpMethod_Get_Async("/api/chat/" + id_room + "/" + user.Id + "/dialog");
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
                Task.Run(() => this.GetUser()).Wait();
                chat.ws.Send(JsonConvert.SerializeObject(new { type = "init_call", status = "100", sender_id = User.Id, receiver_id = chat.SpecDataUser.receiverId }));
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
        private async void GetUser()
        {
            User = await UserDbService.GetUser();
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