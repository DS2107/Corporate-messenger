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
            clvm = new ChatListViewModel(Navigation);
            BindingContext = clvm;
          

            New_message.Source = "AddStart.png";

        }

        protected override async void OnAppearing()
        {
            clvm.IsRefreshing = true;
          
            DependencyService.Get<IForegroundService>().chat_room_id = 0;
            bool flag = File.Exists(DependencyService.Get<IFileService>().GetPath("token.txt"));
            //   AuthorizationMainPage mainPage = new AuthorizationMainPage();
            if (!flag)
            {
                DependencyService.Get<IFileService>().CreateFile(clvm.SpecDataUser.Token, clvm.SpecDataUser.Id, clvm.SpecDataUser.Name);
            }
            clvm.Name = clvm.SpecDataUser.Name;



            if (DependencyService.Get<IForegroundService>().SocketFlag == false)
            {
                DependencyService.Get<IForegroundService>().StartService();
              
                DependencyService.Get<IForegroundService>().LoginPosition = false;
            }

            //****** РАСШИФРОВКА_ОТВЕТА ******
            clvm.contentJobjects = await clvm.GetInfo_HttpMethod_Get_Async("/api/user/" + clvm.SpecDataUser.Id + "/chatroom");

            if (clvm.contentJobjects == null)
            {

                DependencyService.Get<IFileService>().MyToast("Отсутствует соеденение с сервером, проверьте подключение к интернету и потворите попытку");
                clvm.IsRefreshing = false;
            }
            else
            {
                clvm.ThreadChats = new Thread(new ThreadStart(clvm.SendToken_GetChats));
                clvm.ThreadChats.Start();
            }

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
    }
}