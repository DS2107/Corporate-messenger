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
    public partial class FriendPage : ContentPage
    {
        FriendPageViewModel friendPage;
        public FriendPage()
        {
            InitializeComponent();
            friendPage = new FriendPageViewModel(Navigation);
            BindingContext = friendPage;



        }

        private void Editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            friendPage.SearchFriendAsync(e.NewTextValue);
        }
    }
}