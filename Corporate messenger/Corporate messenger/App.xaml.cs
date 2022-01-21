using Corporate_messenger.Service;
using Corporate_messenger.Views;
using System;
using System.IO;
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
                MainPage =  new AuthorizationMainPage();
                DependencyService.Get<IFileService>().MyProperty = MainPage;
            }
            else
            {
                MainPage = new CallPage(false);
            }
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
