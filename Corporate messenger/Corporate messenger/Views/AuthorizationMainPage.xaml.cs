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
            InitializeComponent();
            Routing.RegisterRoute(nameof(LoginPage),
               typeof(LoginPage));

            Routing.RegisterRoute(nameof(ChatsListPage),
                typeof(ChatsListPage));

            Routing.RegisterRoute(nameof(ChatPage),
                typeof(ChatPage));
        }
        public ICommand ExecuteLogout => new Command(async () => await GoToAsync("//login"));
        public async void GoChat()
        {
            await Shell.Current.GoToAsync($"//chats_list");
        }
    }
}