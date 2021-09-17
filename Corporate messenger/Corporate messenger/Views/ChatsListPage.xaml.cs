using Corporate_messenger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Corporate_messenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatsListPage : ContentPage
    {
        private bool back = true;
      
        //Список сообщений 
        public List<ChatListModel> messages { get; set; }

        public ChatsListPage()
        {
            InitializeComponent();
           // var i = Preferences.Get("Image", Color.BlueViolet.ToString());
           // var color = ;
           // Shell.SetBackgroundColor(this, Color.FromHex(i as string));
            // Обновление списка
            MyListView.RefreshCommand = new Command(() =>
            {
                if (back)
                {

                    Title = "Обнови";
                    back = false;
                }
                else
                {
                    Title = "Сообщение";
                    back = true;
                }

                MyListView.IsRefreshing = false;
            });
            // Заполнение списка
            messages = new List<ChatListModel>
            {
                new ChatListModel {Image="logan.jpg", Avtor="Нач",Id=1 },
                new ChatListModel {Image="ge.jpg", Avtor="Таня",Id=2 },
                new ChatListModel {Image="women.jpg", Avtor="Ваня",Id=3},
                new ChatListModel {Image="teacher.png", Avtor="Даня",Id=4 },
                new ChatListModel {Image="child.png",Avtor="Саня",Id=5 },
                new ChatListModel {Image="anon.jpg", Avtor="Анонимус",Id=6 },
                new ChatListModel {Image="enot.jpg", Avtor="Енот",Id=7 },
                new ChatListModel {Image="kot.jpg", Avtor="Кот",Id=8 },
                new ChatListModel {Image="kot3.jpg", Avtor="Страшный кот",Id=9 },
                new ChatListModel {Image="chel.jpg", Avtor="Бухгалтер",Id=10 },
                new ChatListModel {Image="wolf.jpg", Avtor="Друган",Id=11 }
            };

            // Заполняем список
            MyListView.ItemsSource = messages;
        }

        // Нажатие по ячейке чата
         async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            var v = (ChatListModel)e.Item;
            // await DisplayAlert("Item Tapped", "An item was tapped.", "OK");
            var i = v.Id;
           await Navigation.PushAsync(new ChatPage(v.Avtor, v.Image));
            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}