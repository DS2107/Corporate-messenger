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

          
            clvm = new ChatListViewModel(Navigation);
            BindingContext = clvm;
        }


        
        protected override async void OnAppearing()
        {
           
            DependencyService.Get<IForegroundService>().chat_room_id = 0;
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

            var ItemSelect = (ChatListModel)e.Item;


            // await Navigation.PushAsync(new ChatPage(ItemSelect.Id, ItemSelect.Title),true);
            await Navigation.PushAsync(new CacheChatPage(new CacheChatViewModel(ItemSelect)));
        }

       

      

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new UserPage());
        }
    }
}