using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Corporate_messenger.Models;
using Corporate_messenger.ViewModels;
using Corporate_messenger.Service;
using System.Threading.Tasks;
using System.Linq;
using Corporate_messenger.Service.Notification;

namespace Corporate_messenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        const double SmallScreen = 720;
        const double BigScreen = 1280;

        public LoginPage()
        {
            InitializeComponent();
         
            
            BindingContext = new LoginViewModel(Navigation);
           // Shell.ItemsProperty.cl
            // отправляем сообщение
            MessagingCenter.Send<LoginPage>(this, "ListClear");
            DependencyService.Get<IFileService>().Delete();
            DependencyService.Get<IForegroundService>().StopService(); 
            SpecialDataModel special = new SpecialDataModel();
            SizeChanged += LoginPage_SizeChanged;
        
            special.Id = 0;
            special.Token = null;
            UsernameTxt.Text = "";
            PasswordTxt.Text = "";
            if (UsernameTxt.Text == null || UsernameTxt.Text.Length == 0)
            {
                LabelUserName.IsVisible = false;
              
            }
            if (PasswordTxt.Text == null || PasswordTxt.Text.Length == 0)
            {
              
                LabelPassword.IsVisible = false;
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