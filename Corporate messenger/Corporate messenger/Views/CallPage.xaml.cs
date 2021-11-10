using Android.Media;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Net.Sip;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Corporate_messenger.Service;
using Corporate_messenger.ViewModels;

namespace Corporate_messenger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CallPage : ContentPage
    {
       
        public CallPage()
        {
            InitializeComponent();
            BindingContext = new CallViewModel();
          //  CallClass call = new CallClass();
          //  call.LessPort();

        }


    }
}