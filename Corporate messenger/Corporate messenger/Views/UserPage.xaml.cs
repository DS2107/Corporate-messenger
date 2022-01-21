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
    public partial class UserPage : ContentPage
    {
        UserViewModel user;
        public UserPage()
        {
            InitializeComponent();
            user = new UserViewModel(Navigation);
            BindingContext = user;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await user.GetInfoUser();


        }
    }
}