using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Corporate_messenger.Models;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Corporate_messenger.ViewModels;
using TinyAccountManager;
using TinyAccountManager.Abstraction;
using Corporate_messenger.Service;

namespace Corporate_messenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
      

        public LoginPage()
        {
            InitializeComponent();
            BindingContext = new LoginViewModel();
            DependencyService.Get<IFileService>().Delete();
        }    
    }
}