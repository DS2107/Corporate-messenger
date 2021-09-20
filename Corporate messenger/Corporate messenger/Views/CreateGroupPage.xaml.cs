using Corporate_messenger.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Corporate_messenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateGroupPage : ContentPage
    {
        public CreateGroupPage()
        {
            InitializeComponent();
            BindingContext = new CreateGroupPageViewModel();
          
            
        }

    }
}