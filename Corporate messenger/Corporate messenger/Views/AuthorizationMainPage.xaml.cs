using Corporate_messenger.Models;
using Corporate_messenger.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            InitializeComponent();
            
            //BindingContext = new SpecialDataModel();
            SpecialDataModel special = new SpecialDataModel();
            var s = special.Name;
     
        }
        public ICommand ExecuteLogout => new Command(async () => await GoToAsync("//login"));
    }
}