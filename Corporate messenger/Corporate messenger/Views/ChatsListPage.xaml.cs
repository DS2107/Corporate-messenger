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
        public List<Message> messages { get; set; }

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
            messages = new List<Message>
            {
                new Message {Image="logan.jpg", Avtor="Нач",Id=1 },
                new Message {Image="ge.jpg", Avtor="Таня",Id=2 },
                new Message {Image="women.jpg", Avtor="Ваня",Id=3},
                new Message {Image="teacher.png", Avtor="Даня",Id=4 },
                new Message {Image="child.png",Avtor="Саня",Id=5 },
                new Message {Image="anon.jpg", Avtor="Анонимус",Id=6 },
                new Message {Image="enot.jpg", Avtor="Енот",Id=7 },
                new Message {Image="kot.jpg", Avtor="Кот",Id=8 },
                new Message {Image="kot3.jpg", Avtor="Страшный кот",Id=9 },
                new Message {Image="chel.jpg", Avtor="Бухгалтер",Id=10 },
                new Message {Image="wolf.jpg", Avtor="Друган",Id=11 }
            };

            // Заполняем список
            MyListView.ItemsSource = messages;
        }

        // Нажатие по ячейке чата
         async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            var v = (Message)e.Item;
            // await DisplayAlert("Item Tapped", "An item was tapped.", "OK");
            var i = v.Id;
           await Navigation.PushAsync(new ChatPage(v.Avtor, v.Image));
            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}