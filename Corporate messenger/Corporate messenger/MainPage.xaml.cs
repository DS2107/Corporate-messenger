using Corporate_messenger.Service;
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