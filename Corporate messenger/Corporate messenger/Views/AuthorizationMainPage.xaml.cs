using Corporate_messenger.Models;
using Corporate_messenger.Service;
using Corporate_messenger.ViewModels;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Corporate_messenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthorizationMainPage : Shell
    {
      
        public AuthorizationMainPage()
        {
            OnAppearing();
            InitializeComponent();
            Routing.RegisterRoute(nameof(LoginPage),
               typeof(LoginPage));

            Routing.RegisterRoute(nameof(ChatsListPage),
                typeof(ChatsListPage));

            Routing.RegisterRoute(nameof(ChatPage),
                typeof(ChatPage));

            BindingContext = main;
        }
        MainPageViewModel main = new MainPageViewModel();
        public ICommand ExecuteLogout => new Command(async () => await GoToAsync("//login"));

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var token = DependencyService.Get<IFileService>().ReadFile(DependencyService.Get<IFileService>().GetRootPath());
            var data = token.Split('/');

            if (data[0] != "")
            {
                SpecialDataModel special = new SpecialDataModel();
                special.Token = data[0];
                special.Id = Int32.Parse(data[1]);
                main.Name = data[2];
             
            }

        }
    }
}