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
    public partial class Settings : ContentPage
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void UpdateImageFly_Clicked(object sender, EventArgs e)
        {

            _ = SetPhotoAsync();
        }

        public async Task SetPhotoAsync()
        {
            // выбираем фото
            var photo = await MediaPicker.PickPhotoAsync();
            // загружаем в ImageView
            Preferences.Set("ImageFly", photo.FullPath);
              
        }
    }
}