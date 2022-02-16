using Corporate_messenger.DB;
using Corporate_messenger.Models;
using Corporate_messenger.Service;
using Corporate_messenger.Views;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Corporate_messenger
{
    public partial class App : Application
    {
       
        public App(bool navigate)
        {
            InitializeComponent();


          
            if (!navigate)
            {
                //UsernameTxt.Text = "";
                //PasswordTxt.Text = "";

                Task.Run(() => this.GetUser()).Wait();
            
                if (MyUser != null)
                {
                    MainPage = new AuthorizationMainPage();
                    DependencyService.Get<IFileService>().MyMainPage = MainPage;
                    Task.Run(()=> Shell.Current.GoToAsync("//chats_list")).Wait(); 
                }
                else
                {
                    MainPage = new LoginPage();
                   
                }    
            }
            else
            {
                MainPage = new AuthorizationMainPage();
                DependencyService.Get<IFileService>().MyMainPage = MainPage;
                Task.Run(()=>MainPage.Navigation.PushModalAsync(new CallPage(false))).Wait();
            }
        }
        UserDataModel MyUser;
        private async Task GetUser()
        {
            MyUser = await UserDbService.GetUser();
        }


        protected override void OnStart()
        {
            

           
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
