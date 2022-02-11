using Corporate_messenger.Models;
using Corporate_messenger.Service;
using Corporate_messenger.Service.Notification;
using Corporate_messenger.ViewModels;
using System;
using System.Threading.Tasks;
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


            if(DependencyService.Get<IForegroundService>().Flag_On_Off_Service ==false)
                 DependencyService.Get<IForegroundService>().StartService();

        }
    }
}