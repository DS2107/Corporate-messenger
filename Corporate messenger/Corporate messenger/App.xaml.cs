using Corporate_messenger.DB.Repository;
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
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
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
