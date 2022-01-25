using Corporate_messenger.Models;
using Corporate_messenger.ViewModels;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Corporate_messenger.Service.Notification;
using Corporate_messenger.Service;
using System.IO;
using Corporate_messenger.DB;
using System.Collections.Generic;

namespace Corporate_messenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatsListPage : ContentPage
    {



        ChatListViewModel clvm;
        public ChatsListPage()
        {
            InitializeComponent();
            DependencyService.Get<IFileService>().flag = true;
            clvm = new ChatListViewModel(Navigation);
            BindingContext = clvm;
          

            New_message.Source = "AddStart.png";

        }

        Thread ThreadSql;
        
        protected override async void OnAppearing()
        {
            
          
            DependencyService.Get<IForegroundService>().chat_room_id = 0;
          
         



            if (DependencyService.Get<IForegroundService>().SocketFlag == false)
            {
                DependencyService.Get<IForegroundService>().StartService();
              
                DependencyService.Get<IForegroundService>().LoginPosition = false;
            }

            await clvm.GetSqlChats();

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            clvm.ChatList = null;
        }


        // Нажатие по ячейке чата
        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
           

            if (e.Item == null)
                return;

            var v = (ChatListModel)e.Item;

            var i = v.Id;
            //ChatPage s = new ChatPage(v.Id, v.Title);
            await Navigation.PushAsync(new ChatPage(v.Id, v.Title));

        }

       

        private void New_message_Released(object sender, EventArgs e)
        {
            New_message.Source = "AddStart.png";
        }

        private void New_message_Pressed(object sender, EventArgs e)
        {
            New_message.Source = "AddEnd.png";
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new UserPage());
        }
    }
}