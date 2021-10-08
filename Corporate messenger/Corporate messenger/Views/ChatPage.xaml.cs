using Corporate_messenger.Models;
using Corporate_messenger.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Corporate_messenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
     

        public ChatPage(int id,string title)
        {
            InitializeComponent();
            BindingContext = new ChatViewModel(id,title);

            MessagingCenter.Subscribe<ChatViewModel>(this, "Scrol", (sender) => {
                object d = 0;
                foreach (var s in MyListView.ItemsSource)
                {
                    d = s;

                }
                MyListView.ScrollTo(d, ScrollToPosition.End, true);


            });

        }

      
    }
    
}