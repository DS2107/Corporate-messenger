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
            clvm = new ChatListViewModel();
            BindingContext = clvm;
            MessagingCenter.Subscribe<ChatsListPage>(
                this, // кто подписывается на сообщения
                "ListClear",   // название сообщения
                (sender) => { clvm.ChatList.Clear(); });    // вызываемое действие

        }

        protected override void OnAppearing()
        {
           
          
            DependencyService.Get<IForegroundService>().chat_room_id = 0;
            bool flag = File.Exists(DependencyService.Get<IFileService>().GetPath("token.txt"));
            AuthorizationMainPage mainPage = new AuthorizationMainPage();
            if (!flag){
                DependencyService.Get<IFileService>().CreateFile(clvm.SpecDataUser.Token, clvm.SpecDataUser.Id, clvm.SpecDataUser.Name);
            }
              

           
            if (DependencyService.Get<IForegroundService>().SocketFlag == false)
            {
                DependencyService.Get<IForegroundService>().StartService();
              
                DependencyService.Get<IForegroundService>().LoginPosition = false;
            }
               

            clvm.ThreadChats = new Thread(new ThreadStart(clvm.ThreadFunc_GetMessage));
            clvm.ThreadChats.Start();
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
            _ = GoToPagaeFriend();
        }
        async Task GoToPagaeFriend()
        {
            await Navigation.PushAsync(new FriendPage());

        }
    }
}