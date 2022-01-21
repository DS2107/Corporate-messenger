using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Corporate_messenger.Models;
using Corporate_messenger.ViewModels;
using Corporate_messenger.Service;
using System.Threading.Tasks;
using System.Linq;
using Corporate_messenger.Service.Notification;
using System;
using Corporate_messenger.DB;

namespace Corporate_messenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        const double SmallScreen = 720;
        const double BigScreen = 1280;

        public LoginPage()
        {
            OnAppearing();
            InitializeComponent();
         
            
            BindingContext = new LoginViewModel(Navigation);
          
            DependencyService.Get<IForegroundService>().LoginPosition = true;
            DependencyService.Get<IForegroundService>().StopService();
            DependencyService.Get<IForegroundService>().SocketFlag = false;
            if (DependencyService.Get<ISocket>().MyWebSocket != null)
            {
                DependencyService.Get<ISocket>().MyWebSocket.CloseAsync();
            }
           
            SpecialDataModel special = new SpecialDataModel();
            SizeChanged += LoginPage_SizeChanged;
        
            special.Id = 0;
            special.Token = null;
          
            if (UsernameTxt.Text == null || UsernameTxt.Text.Length == 0)
            {
                LabelUserName.IsVisible = false;
              
            }
            if (PasswordTxt.Text == null || PasswordTxt.Text.Length == 0)
            {
              
                LabelPassword.IsVisible = false;
            }


        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            Shell.SetTabBarIsVisible(this, false);

            //UsernameTxt.Text = "";
            //PasswordTxt.Text = "";

           
            UserDataModel user = await UserDbService.GetUser();
            if (user != null)
            {
               
                if (Shell.Current != null)
                    await Shell.Current.GoToAsync($"//chats_list");
            }
    
        }



        private void LoginPage_SizeChanged(object sender, System.EventArgs e)
        {
            if (Height > SmallScreen)
            {
                FirstContainer.Padding = new Thickness(0, 150, 0, 0);
            }
        }

        private void UsernameTxt_Focused(object sender, FocusEventArgs e)
        {
            LabelUserName.IsVisible = true;

            if (UsernameTxt.Text == null || UsernameTxt.Text.Length == 0)
            {
                UsernameTxt.Placeholder = "Логин";
                UsernameTxt.Placeholder = string.Empty;
            }
        }

        private void UsernameTxt_Unfocused(object sender, FocusEventArgs e)
        {
            if (UsernameTxt.Text == null || UsernameTxt.Text.Length == 0)
            {
                UsernameTxt.Placeholder = "Логин";
                LabelUserName.IsVisible = false;
            }
        }

        private void PasswordTxt_Focused(object sender, FocusEventArgs e)
        {
            LabelPassword.IsVisible = true;

            if (PasswordTxt.Text == null || PasswordTxt.Text.Length == 0)
            {
                PasswordTxt.Placeholder = "Пароль";
                PasswordTxt.Placeholder = string.Empty;
            }
        }

        private void PasswordTxt_Unfocused(object sender, FocusEventArgs e)
        {
            if (PasswordTxt.Text == null || PasswordTxt.Text.Length == 0)
            {
                PasswordTxt.Placeholder = "Пароль";
                LabelPassword.IsVisible = false;
            }
        }
       
      
       
    }
}