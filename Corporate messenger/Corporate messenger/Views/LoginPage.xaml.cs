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
    public partial class LoginPage : ContentPage
    {
       
        public LoginPage()
        {
            InitializeComponent();
           
        }

        private async void B_login_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//chats_list",true);
            string value = UsernameTxt.Text;
            UsernameTxt.Text = value;
            Preferences.Set("name", value);
        }

        protected override void OnAppearing()
        {
            string name = Preferences.Get("name", "Username");
            UsernameTxt.Text = name;
            base.OnAppearing();
           
        }
    }
}