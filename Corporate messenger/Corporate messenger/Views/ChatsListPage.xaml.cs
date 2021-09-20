using Corporate_messenger.Models;
using Corporate_messenger.ViewModels;
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
      
        

        public ChatsListPage()
        {
            InitializeComponent();
            BindingContext = new ChatListViewModel();
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
           
        }

        // Нажатие по ячейке чата
         async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

          //  var v = (ChatListModel)e.Item;
            // await DisplayAlert("Item Tapped", "An item was tapped.", "OK");
          //  var i = v.id;
           await Navigation.PushAsync(new ChatPage());
            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}