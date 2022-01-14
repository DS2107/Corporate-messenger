using Corporate_messenger.DB.Repository;
using Corporate_messenger.Models;
using Corporate_messenger.Service;
using Corporate_messenger.Views;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Corporate_messenger
{
    public partial class App : Application
    {
        public const string DATABASE_NAME = "Messanger.db";
        public static ChatListRepository database;
        public static ChatListRepository Database
        {
            get
            {
                if (database == null)
                {

                    database = new ChatListRepository(
                        Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DATABASE_NAME));
                }
                return database;
            }
        }
        public App(bool navigate)
        {
            InitializeComponent();


          
            if (!navigate)
            {
               
                    MainPage =  new AuthorizationMainPage();
                DependencyService.Get<IFileService>().MyProperty = MainPage;
                    // MainPage = new NavigationPage(new AuthorizationMainPage());

            }
            else
            {
                MainPage = new CallPage(false);
            }
             

          // MainPage = new AuthorizationPage();



        }

        protected override void OnStart()
        {
            var file = DependencyService.Get<IFileService>().CreateFile();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
