using Corporate_messenger.Models;
using Corporate_messenger.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using WebSocketSharp;
using System.Diagnostics;

namespace Corporate_messenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatsListPage : ContentPage
    {
        
      
        

        public ChatsListPage()
        {
            InitializeComponent();
           
            BindingContext = new ChatListViewModel();
           
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

        private void CallButton_Clicked(object sender, EventArgs e)
        {
            GoToPagaeFriend();
        }
        async Task GoToPagaeFriend()
        {
            await Navigation.PushAsync(new FriendPage());

        }
    }
}