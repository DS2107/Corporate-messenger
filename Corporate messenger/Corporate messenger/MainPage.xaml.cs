using Corporate_messenger.Models;
using Corporate_messenger.Service;
using Corporate_messenger.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Corporate_messenger
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : Shell
    {
      
        public MainPage()
        {
            InitializeComponent();
          
            OnAppearing();
            Corporate_messenger.ViewModels.AuthorizationMainPageViewModel z = new AuthorizationMainPageViewModel();
             BindingContext =  z;

            // SetPhotoAsync();


        }



        protected override void OnAppearing()
        {
            var ImageFly = Preferences.Get("ImageFly", "");
            FlyoutBackgroundImage = ImageFly;
            base.OnAppearing();

        }

    }
}